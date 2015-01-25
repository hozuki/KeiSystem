#region GPLv2

/*
KeiSystem
Copyright (C) 2015 MIC Studio
Developer homepage: https://github.com/GridSciense

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation; either version 2 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License along
with this program; if not, write to the Free Software Foundation, Inc.,
51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA.
*/

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kei.KTracker;
using MonoTorrent.BEncoding;

namespace Kei.KNetwork
{
    public partial class KClient
    {

        /// <summary>
        /// 是否将所有普通 tracker 通信视为带有 event=started 的 tracker 通信。
        /// </summary>
        private bool _regardAsStarted = false;

        /// <summary>
        /// 是否在收到普通（即不带 event 参数）的 tracker 通信时也广播用户进入消息。
        /// <para>此属性设置为 true 可以解决启动 μTorrent 和客户端-关闭并重启客户端 这个操作带来的 tracker 通信无 event 参数，导致无法正常寻找用户的问题。</para>
        /// <para>但是如果设置为 true，可能会发送大量冗余消息。应该指定一个超时，过了这段时间后此属性自动复位为 false。</para>
        /// </summary>
        public bool RegardNormalTrackerCommAsStarted
        {
            set
            {
                _regardAsStarted = value;
            }
            get
            {
                return _regardAsStarted;
            }
        }

        private void TrackerServer_TrackerComm(object sender, TrackerCommEventArgs e)
        {
            string tmpLog;
            tmpLog = "收到 tracker 服务器消息。";
            tmpLog += Environment.NewLine + "Infohash: " + e.InfoHash.ToHexString();
            tmpLog += Environment.NewLine + "状态: " + e.Status.ToString();
            Logger.Log(tmpLog);
            switch (e.Status)
            {
                case TaskStatus.Stopped:
                case TaskStatus.Paused:
                    BroadcastMyselfRemoveAsPeer(e.InfoHash);
                    break;
                case TaskStatus.Started:
                    BroadcastMyselfAddAsPeer(e.InfoHash);
                    break;
                default:
                    if (RegardNormalTrackerCommAsStarted)
                    {
                        BroadcastMyselfAddAsPeer(e.InfoHash);
                    }
                    else
                    {
                        lock (TrackerServer.Seeds)
                        {
                            if (!TrackerServer.Seeds.ContainsKey(e.InfoHash))
                            {
                                var peerList = new List<Peer>(8);
                                peerList.Add(TrackerServer.Myself);
                                TrackerServer.Seeds.Add(e.InfoHash, peerList); ;
                            }
                        }
                    }
                    break;
            }
        }

        /// <summary>
        /// 将当前的连接列表和用户列表编码进字节数组，以供之后的传输。
        /// </summary>
        /// <returns>一个 B-编码形式的字节数组，包含了当前的连接列表和用户列表。</returns>
        private byte[] EncodeFromConnectionListAndSeedDictionary()
        {
            // 返回当前所有的连接信息
            // 结构：
            // d
            // {
            //     "connections" : l
            //         {
            //             d
            //                 {
            //                     "ip" : ip(4)
            //                     "port" : port(2)
            //                 }
            //         }
            //     "peers" : d
            //         {
            //             infohash_1 : l
            //                 {
            //                     d
            //                         {
            //                             "ip" : ip(4)
            //                             "port" : port(2)
            //                         }
            //                 }
            //             ... infohash_n
            //         }
            // }
            BEncodedDictionary dictionary = new BEncodedDictionary();
            BEncodedList connList = new BEncodedList(ConnectionList.Count);
            lock (ConnectionList)
            {
                foreach (var item in ConnectionList)
                {
                    var d = new BEncodedDictionary();
                    d.Add("ip", item.ClientLocation.GetAddressBytes());
                    d.Add("port", item.ClientLocation.GetPortBytes());
                    connList.Add(d);
                }
            }
            dictionary.Add("connections", connList);
            BEncodedDictionary peersList = new BEncodedDictionary();
            lock (TrackerServer.Seeds)
            {
                foreach (var item in TrackerServer.Seeds)
                {
                    var list = new BEncodedList(item.Value.Count);
                    foreach (var t in item.Value)
                    {
                        var d = new BEncodedDictionary();
                        d.Add("ip", t.EndPoint.GetAddressBytes());
                        d.Add("port", t.EndPoint.GetPortBytes());
                        list.Add(d);
                    }
                    peersList.Add(item.Key.ToByteArray(), list);
                }
            }
            dictionary.Add("peers", peersList);
            byte[] data = dictionary.Encode();
            return data;
        }

        /// <summary>
        /// 解码收到的字节数组，并将其包含的连接列表和用户列表添加到本客户端的相应列表中。
        /// </summary>
        /// <param name="data">收到的数据。</param>
        ///// <param name="sendPeerEnter">解码过程中是否应该广播 PeerEnterNetwork 消息。</param>
        /// <remarks>
        /// 需要发送 PeerEnterNetwork 消息的情况会发生于：A开启客户端和μT，B开启客户端和μT，A（用户列表非空）再尝试连接B，此时如果B并没有保存全网的用户列表，那么A就要广播 PeerEnterNetwork。
        /// </remarks>
        private void DecodeToConnectionListAndSeedDicionary(byte[] data)
        {
            // 如果对方发过来的是空，那么就肯定不会有数据啦
            if (data.Length > 0)
            {
                BEncodedDictionary dictionary = BEncodedDictionary.Decode(data) as BEncodedDictionary;
                if (dictionary == null)
                {
                    throw new Exception();
                }
                BEncodedList connList = dictionary["connections"] as BEncodedList;
                BEncodedDictionary peersDict = dictionary["peers"] as BEncodedDictionary;
                // ...
                lock (ConnectionList)
                {
                    foreach (var item in connList)
                    {
                        var d = item as BEncodedDictionary;
                        KEndPoint kep = KEndPoint.Empty;
                        kep.SetAddress((d["ip"] as BEncodedString).TextBytes);
                        kep.SetPort((int)BitConverter.ToInt16((d["port"] as BEncodedString).TextBytes, 0));
                        ConnectionList.Add(new ConnectionListItem(kep));
                    }
                }

                // 如果已经有用户登记了，那么应该广播
                if (TrackerServer.Seeds.Count > 0)
                {
                    lock (TrackerServer.Seeds)
                    {
                        foreach (var kv in TrackerServer.Seeds)
                        {
                            // 广播消息
                            BroadcastMyselfAddAsPeer(kv.Key);
                        }
                    }
                }
                lock (TrackerServer.Seeds)
                {
                    foreach (var kv in peersDict)
                    {
                        InfoHash infoHash = InfoHash.FromByteArray(kv.Key.TextBytes);
                        List<Peer> peers = new List<Peer>((kv.Value as BEncodedList).Count);
                        foreach (var item in (kv.Value as BEncodedList))
                        {
                            var d = item as BEncodedDictionary;
                            KEndPoint kep = KEndPoint.Empty;
                            kep.SetAddress((d["ip"] as BEncodedString).TextBytes);
                            kep.SetPort((int)BitConverter.ToInt16((d["port"] as BEncodedString).TextBytes, 0));
                            Peer peer = Peer.Create(kep);
                            peers.Add(peer);
                        }
                        TrackerServer.Seeds.Add(infoHash, peers);
                    }
                }
            }
            else
            {
                Logger.Log("待解码的数据为空，这意味着对方客户端目前持有的连接列表和用户列表为空。");
            }
        }

    }
}
