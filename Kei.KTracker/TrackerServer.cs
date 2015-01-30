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
    /// <summary>
    /// 表示提供 Tracker 服务的 HTTP 服务器实例。
    /// </summary>
    public sealed partial class TrackerServer : HttpServer
    {

        /// <summary>
        /// 本服务器使用的 announce URL。
        /// </summary>
        private string _announceUrl;

        /// <summary>
        /// 使用指定的端点建立一个新的 Tracker 服务器。
        /// </summary>
        /// <param name="localEndPoint">要绑定的本地端点。注意地址为本机的内网地址而非环回地址。</param>
        public TrackerServer(IPEndPoint localEndPoint)
            : this(localEndPoint, DefaultAnnounceUrl)
        {
        }

        /// <summary>
        /// 使用指定的端点和指定的 announce URL 建立一个新的 Tracker 服务器。
        /// </summary>
        /// <param name="localEndPoint">要绑定的本地端点。注意地址为本机的内网地址而非环回地址。</param>
        /// <param name="announceUrl">指定的 announce URL。默认为 <see cref="Kei.KTracker.TrackerServer.DefaultAnnounceUrl"/>。</param>
        private TrackerServer(IPEndPoint localEndPoint, string announceUrl)
            : base(localEndPoint)
        {
            _announceUrl = announceUrl;
            FreeToGo = false;
        }

        /// <summary>
        /// 获取本 Tracker 服务器的 announce URL（以“?”结尾）。此属性为只读。
        /// </summary>
        public string AnnouceUrl
        {
            get
            {
                return _announceUrl;
            }
        }

        /// <summary>
        /// 获取或设置是否已完成启动。
        /// </summary>
        public bool FreeToGo
        {
            get;
            set;
        }

        /// <summary>
        /// 覆盖父类的 HandleGetRequest 方法。
        /// </summary>
        /// <param name="processor">为此请求生成的 <see cref="Kei.KTracker.HttpProcessor"/> 对象。</param>
        /// <param name="ioStream"><see cref="Kei.KTracker.HttpProcessor"/> 为此请求开启的 <see cref="System.IO.Stream"/>。</param>
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

        /// <summary>
        /// 覆盖父类的 HandlePostRequest 方法。
        /// </summary>
        /// <param name="processor">为此请求生成的 <see cref="Kei.KTracker.HttpProcessor"/> 对象。</param>
        /// <param name="ioStream"><see cref="Kei.KTracker.HttpProcessor"/> 为此请求开启的 <see cref="System.IO.Stream"/>。</param>
        internal override void HandlePostRequest(HttpProcessor processor, Stream ioStream)
        {
            processor.WriteFailure();
        }

        /// <summary>
        /// 处理 Tracker 请求的方法。
        /// </summary>
        /// <param name="processor">为此请求生成的 <see cref="Kei.KTracker.HttpProcessor"/> 对象。</param>
        /// <param name="ioStream"><see cref="Kei.KTracker.HttpProcessor"/> 为此请求开启的 <see cref="System.IO.Stream"/>。</param>
        /// <param name="parameters">URL 请求参数。</param>
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

                List<Peer> peers = null;
                TrackerCommEventArgs eventArgs;

                // 如果接入分布网络完成
                if (FreeToGo)
                {
                    // 触发事件，得到新的可用用户列表
                    if (Seeds.TryGetValue(infoHash, out peers))
                    {
                        peers = Seeds[infoHash];
                        if (peers.IndexOf(_myself) < 0)
                        {
                            // 如果之前我不在种子列表中
                            // TODO: 添加 myself
                            peers.Add(_myself);
                            // 而且此时确认为自己加入网络，强制设定 event=started
                            taskStatus = TaskStatus.Started;
                        }
                    }
                    else
                    {
                        // 如果之前没有相关种子
                        peers = new List<Peer>();
                        // TODO: 添加 myself
                        peers.Add(_myself);
                        Seeds.Add(infoHash, peers);
                        // 而且此时确认为自己加入网络，强制设定 event=started
                        taskStatus = TaskStatus.Started;
                    }
                }
                eventArgs = new TrackerCommEventArgs(infoHash, peers, portNo, peerIDString, compact, noPeerID, taskStatus);
                EventHelper.RaiseEventAsync(TrackerComm, this, eventArgs);
                //if (eventArgs.Peers == null)
                //{
                //    throw new NullReferenceException("Peers list is null");
                //}
                if (eventArgs.Peers != null)
                {
                    peers = eventArgs.Peers;
                }

                // 生成返回给 μTorrent 的信息
                Logger.Log("[Tracker]反馈给 BitTorrent 客户端。");
                if (peers != null)
                {
                    string peersListString = "[Tracker]种子 " + infoHash.ToString() + " 的用户列表如下:";
                    foreach (var p in peers)
                    {
                        peersListString += Environment.NewLine + p.EndPoint.ToString();
                    }
                    Logger.Log(peersListString);
                }
                else
                {
                    Logger.Log("[Tracker]种子 " + infoHash.ToString() + " 的用户列表为空。");
                }
                BEncodedDictionary data = new BEncodedDictionary();
                data.Add("interval", 60);
                if (peers != null)
                {
                    // 如果种子列表里有种子
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
                }
                else
                {
                    // 如果没有种子，就返回空列表
                    data.Add("peers", string.Empty);
                }

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
