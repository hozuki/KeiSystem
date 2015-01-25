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
using System.IO;
using System.Net;
using System.Net.Sockets;
using MonoTorrent.BEncoding;

namespace Kei.KTracker
{
    public sealed partial class TrackerServer : HttpServer
    {

        private string _announceUrl;

        public TrackerServer(IPEndPoint localEndPoint, string announceUrl = DefaultAnnounceUrl)
            : base(localEndPoint)
        {
            _announceUrl = announceUrl;
        }

        public string AnnouceUrl
        {
            get
            {
                return _announceUrl;
            }
        }

        internal override void HandleGetRequest(HttpProcessor processor, Stream ioStream)
        {
            Logger.Log("[Tracker]GET 请求。请求 URL: " + processor.RequestUrl);
            if (processor.RequestUrl.StartsWith(AnnouceUrl))
            {
                Logger.Log("[Tracker]是 announce。");
                var parameters = processor.RequestUrl.Substring(AnnouceUrl.Length);
                HandleTrackerRequest(processor, ioStream, parameters);
            }
            else
            {
                //processor.WriteFailure();
            }
        }

        internal override void HandlePostRequest(HttpProcessor processor, Stream ioStream)
        {
            processor.WriteFailure();
        }

        private void HandleTrackerRequest(HttpProcessor processor, Stream ioStream, string parameters)
        {
            // parameters 传入类似 info_hash=XXXX&port=XXXX&ipv6=XXXX 的形式
            var args = Utilities.DecomposeParameterString(parameters);
            Logger.Log("[Tracker]请求: " + processor.RequestUrl);

            try
            {
                Logger.Log("[Tracker]解码收到的参数。");
                // 先解码处理各种参数（注意可能会引发异常）
                string infoHashString = Utilities.UnescapePartialUriEncodedString(args["info_hash"]);
                InfoHash infoHash = InfoHash.FromHexString(infoHashString);
                int portNo = Convert.ToInt32(args["port"]);
                TaskStatus taskStatus;
                if (args.ContainsKey("event"))
                {
                    switch (args["event"])
                    {
                        case "started":
                            taskStatus = TaskStatus.Started;
                            break;
                        case "stopped":
                            taskStatus = TaskStatus.Stopped;
                            break;
                        case "paused":
                            taskStatus = TaskStatus.Paused;
                            break;
                        default:
                            taskStatus = TaskStatus.None;
                            break;
                    }
                }
                else
                {
                    taskStatus = TaskStatus.None;
                }
                bool compact;
                if (args.ContainsKey("compact"))
                {
                    var n = Convert.ToInt32(args["compact"]);
                    compact = n != 0;
                }
                else
                {
                    compact = false;
                }
                bool noPeerID;
                if (args.ContainsKey("no_peer_id"))
                {
                    var n = Convert.ToInt32(args["no_peer_id"]);
                    noPeerID = n != 0;
                }
                else
                {
                    noPeerID = false;
                }
                var peerIDString = Utilities.UnescapePartialUriEncodedString(args["peer_id"]);

                // 别忘了重新确认自己
                IPEndPoint newMyself = new IPEndPoint(LocalEndPoint.Address, portNo);
                Logger.Log("[Tracker]新的自己(BitTorrent 客户端): " + newMyself.ToString());
                SetMyself(newMyself, peerIDString);

                List<Peer> peers;
                TrackerCommEventArgs eventArgs;

                // 触发事件，得到新的可用用户列表
                if (Seeds.TryGetValue(infoHash, out peers))
                {
                    peers = Seeds[infoHash];
                }
                else
                {
                    peers = new List<Peer>();
                    // TODO: 添加 myself
                    peers.Add(_myself);
                    Seeds.Add(infoHash, peers);
                }
                eventArgs = new TrackerCommEventArgs(infoHash, peers, portNo, peerIDString, compact, noPeerID, taskStatus);
                EventHelper.RaiseEventAsync(TrackerComm, this, eventArgs);
                if (eventArgs.Peers == null)
                {
                    throw new NullReferenceException("Peers list is null");
                }

                // 生成返回给 μTorrent 的信息
                Logger.Log("[Tracker]反馈给 BitTorrent 客户端。");
                BEncodedDictionary data = new BEncodedDictionary();
                data.Add("interval", 60);
                // 为了防止其他线程修改这个 list 导致迭代器失效，先锁定这个对象
                lock (peers)
                {
                    if (compact)
                    {
                        // 生成字节数组
                        byte[] peersArray = new byte[peers.Count * 6];
                        byte[] tmp;
                        int i = 0;
                        foreach (var peer in peers)
                        {
                            tmp = peer.ToByteArray();
                            Array.Copy(tmp, 0, peersArray, i * 6, 6);
                            i++;
                        }
                        data.Add("peers", peersArray);
                    }
                    else
                    {
                        // 生成列表
                        BEncodedList peersList = new BEncodedList(peers.Count);
                        foreach (var peer in peers)
                        {
                            BEncodedDictionary peerDict = new BEncodedDictionary();
                            peerDict.Add("id", peer.ID);
                            peerDict.Add("ip", peer.EndPoint.GetAddressString());
                            peerDict.Add("port", peer.EndPoint.GetPortNumber());
                            peersList.Add(peerDict);
                        }
                        data.Add("peers", peersList);
                    }
                }
                data.Add("complete", peers.Count);

                // 输出
                Logger.Log("[Tracker]写入“成功”头。");
                processor.WriteSuccess(null, "text/plain");
                Logger.Log("[Tracker]编码返回数据。");
                var dataBytes = data.Encode();
                Logger.Log("[Tracker]写入返回数据。");
                ioStream.Write(dataBytes, 0, dataBytes.Length);
                Logger.Log("[Tracker]向 BitTorrent 客户端返回了: " + Encoding.ASCII.GetString(dataBytes));
                ioStream.Flush();
            }
            catch (Exception ex)
            {
                Logger.Log(ex.Message + Environment.NewLine + ex.StackTrace);
                processor.WriteFailure();
                ioStream.WriteLine(ex.Message);
                ioStream.WriteLine(ex.StackTrace);
            }
        }

    }
}
