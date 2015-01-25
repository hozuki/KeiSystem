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

namespace Kei.KNetwork
{
    public partial class KClient
    {

        /// <summary>
        /// 消息处理的核心分发过程。
        /// </summary>
        /// <param name="stream">一个 <see cref="System.IO.Stream"/>，表示处理该消息时所用的通信流。</param>
        /// <param name="message">一个 <see cref="Kei.KNetwork.KMessage"/>，表示收到的消息。</param>
        private void HandleMessage(Stream stream, KMessage message)
        {
            Logger.Log("KClient::HandleMessage() from " + message.Header.SourceEndPoint.ToString() + " with code " + message.Header.Code.ToString());
            // 确保不是自己发出的消息
            if (!message.Header.SourceEndPoint.Equals(_localKEndPoint))
            {
                KHandledMessage handledMessage;
                int handledMessageIndex;
                handledMessage = HandledMessages.FindHandledMessage(message, out handledMessageIndex);
                string tmpLog;
                tmpLog = "消息代码: " + message.Header.Code.ToString();
                tmpLog += Environment.NewLine + "消息唯一编码: " + message.Header.MessageID.ToString();
                tmpLog += Environment.NewLine + "消息来源: " + message.Header.SourceEndPoint.ToString();
                Logger.Log(tmpLog);
                // 若未处理过该消息
                if (handledMessage == null)
                {
                    Logger.Log("未处理过该消息。");
                    switch (message.Header.Code)
                    {
                        case KMessageCode.ReportAlive:
                            HandleReportAlive(stream, message);
                            AddToConnectionList(message.Header.SourceEndPoint);
                            break;
                        case KMessageCode.ClientEnterNetwork:
                            HandleClientEnterNetwork(stream, message);
                            AddToConnectionList(message.Header.SourceEndPoint);
                            break;
                        case KMessageCode.ClientExitNetwork:
                            HandleClientExitNetwork(stream, message);
                            AddToConnectionList(message.Header.SourceEndPoint);
                            break;
                        case KMessageCode.PeerEnterNetwork:
                            HandlePeerEnterNetwork(stream, message);
                            AddToConnectionList(message.Header.SourceEndPoint);
                            break;
                        case KMessageCode.PeerExitNetwork:
                            HandlePeerExitNetwork(stream, message);
                            AddToConnectionList(message.Header.SourceEndPoint);
                            break;
                        case KMessageCode.GotPeer:
                            HandleGotPeer(stream, message);
                            AddToConnectionList(message.Header.SourceEndPoint);
                            break;
                        default:
                            return;
                    }

                    Logger.Log("将消息加入“已处理”列表。");
                    // 对于收到的信息，都加入“已处理消息”列表
                    lock (HandledMessages)
                    {
                        HandledMessages.Add(new KHandledMessage(message));
                    }
                }
                else
                {
                    Logger.Log("更新消息生命周期。");
                    // 更新生命周期，并将其移动到列表最后（因为是“刚刚加入”的）
                    handledMessage.LifeStart = DateTime.Now;
                    lock (HandledMessages)
                    {
                        // 访问这个过程的只有一个线程，所以不用担心说前面某一项先被移动到后面去了导致索引错误
                        HandledMessages.RemoveAt(handledMessageIndex);
                        HandledMessages.Add(handledMessage);
                    }
                    Logger.Log("清理“已处理”列表。");
                    SweepHandledMessages();
                }
            }
        }

        /// <summary>
        /// 处理 ReportAlive 消息。
        /// </summary>
        /// <param name="stream">一个 <see cref="System.IO.Stream"/>，表示处理该消息时所用的通信流。</param>
        /// <param name="message">一个 <see cref="Kei.KNetwork.KMessage"/>，表示收到的消息。</param>
        private void HandleReportAlive(Stream stream, KMessage message)
        {
            // 这是一个报告存活的消息
            Logger.Log("收到消息: 报告存活。");
            // 不采取任何行动；后面会自动将对方加入连接列表
        }

        /// <summary>
        /// 处理 ClientEnterNetwork 消息。
        /// </summary>
        /// <param name="stream">一个 <see cref="System.IO.Stream"/>，表示处理该消息时所用的通信流。</param>
        /// <param name="message">一个 <see cref="Kei.KNetwork.KMessage"/>，表示收到的消息。</param>
        private void HandleClientEnterNetwork(Stream stream, KMessage message)
        {
            Logger.Log("收到消息: 客户端加入分布网络。");
            KEndPoint remoteEndPoint = message.Header.SourceEndPoint;
            int isMessageHandled = BitConverter.ToInt32(message.Content.Data, 0);
            if (!ConnectionList.Contains(remoteEndPoint))
            {
                if (isMessageHandled == 0)
                {
                    Logger.Log("本客户端是第一个收到这条进入消息的客户端。" + Environment.NewLine + "本机的连接列表如下。");
                    StringBuilder sb = new StringBuilder(100);
                    lock (ConnectionList)
                    {
                        foreach (var item in ConnectionList)
                        {
                            sb.AppendLine(item.ToString());
                        }
                    }
                    Logger.Log(sb.ToString());

                    Logger.Log("将当前连接信息编码并发送。");
                    // 将自己的连接列表和用户列表编码，准备发送到连接来的客户端
                    var data = EncodeFromConnectionListAndSeedDictionary();
                    try
                    {
                        // 先返回接下来的字节大小
                        stream.WriteInt32(data.Length);
                        // 然后一次性发送
                        stream.Write(data, 0, data.Length);
                        stream.Flush();
                    }
                    catch (Exception ex)
                    {
                        // 首次通信就失败了……
                        Logger.Log(ex.Message);
                        Logger.Log(ex.StackTrace);
                    }
                }
            }

            if (isMessageHandled == 0)
            {
                // 注意：虽然 KMessage 是一个值类型，但是其中含有引用类型（数组），这里修改了这个引用类型
                Logger.Log("设置该消息为处理过。");
                message.Content.Data = BitConverter.GetBytes((int)1);
            }
            else
            {
                // 本客户端不是第一个处理的，那就报告存活吧
                SendMessage(remoteEndPoint, MessageFactory.ReportAlive(LocalKEndPoint));
            }
            Logger.Log("转发消息。");
            // 转发
            BroadcastMessage(message);
        }

        /// <summary>
        /// 处理 ClientExitNetwork 消息。
        /// </summary>
        /// <param name="stream">一个 <see cref="System.IO.Stream"/>，表示处理该消息时所用的通信流。</param>
        /// <param name="message">一个 <see cref="Kei.KNetwork.KMessage"/>，表示收到的消息。</param>
        private void HandleClientExitNetwork(Stream stream, KMessage message)
        {
            Logger.Log("收到消息: 客户端离开分布网络。");
            KEndPoint remoteEndPoint = message.Header.SourceEndPoint;
            // 只是简单的移除
            int clItemIndex;
            ConnectionListItem item = ConnectionList.FindConnectionListItem(remoteEndPoint, out clItemIndex);
            if (item != null)
            {
                Logger.Log("找到项目并移除。");
                lock (ConnectionList)
                {
                    // 一样，应该不用担心前面的项被移动到后面去导致索引错误的事情吧
                    ConnectionList.RemoveAt(clItemIndex);
                }
            }

            Logger.Log("转发消息。");
            // 转发
            BroadcastMessage(message);
        }

        /// <summary>
        /// 处理 PeerEnterNetwork 消息。
        /// </summary>
        /// <param name="stream">一个 <see cref="System.IO.Stream"/>，表示处理该消息时所用的通信流。</param>
        /// <param name="message">一个 <see cref="Kei.KNetwork.KMessage"/>，表示收到的消息。</param>
        private void HandlePeerEnterNetwork(Stream stream, KMessage message)
        {
            Logger.Log("收到消息: 用户加入。");
            int bitTorrentClientPort;
            InfoHash infoHash;
            Logger.Log("解码用户及 infohash 数据。");
            MessageFactory.GetPeerMessageContent(message, out infoHash, out bitTorrentClientPort);
            Logger.Log("Infohash: " + infoHash.ToHexString() + Environment.NewLine + "用户端口: " + bitTorrentClientPort.ToString());
            List<Peer> peerList;
            if (TrackerServer.Seeds.TryGetValue(infoHash, out peerList))
            {
                Logger.Log("在本机发现该种子。加入列表。");
                KEndPoint remoteClientEP = message.Header.SourceEndPoint;
                KEndPoint remoteBTEP = remoteClientEP;
                remoteBTEP.SetPort(bitTorrentClientPort);
                Peer peer = Peer.Create(remoteBTEP);
                if (!peerList.Contains(peer))
                {
                    lock (peerList)
                    {
                        peerList.Add(peer);
                    }
                }

                // 同时报告信息源，我这里有种子
                Logger.Log("报告消息源在这里找到种子。");
                // 此时 ep 的 Port 已经被修改，所以不能直接向 ep 所表示的端点发送，之前写错了……
                // 所以后面索性改了名称，remoteClientEP 和 remoteBTEP
                SendMessage(remoteClientEP, MessageFactory.GotPeer(LocalKEndPoint, infoHash, TrackerServer.Myself.EndPoint.GetPortNumber()));
            }

            Logger.Log("转发消息。");
            // 转发
            BroadcastMessage(message);
        }

        /// <summary>
        /// 处理 PeerExitNetwork 消息。
        /// </summary>
        /// <param name="stream">一个 <see cref="System.IO.Stream"/>，表示处理该消息时所用的通信流。</param>
        /// <param name="message">一个 <see cref="Kei.KNetwork.KMessage"/>，表示收到的消息。</param>
        private void HandlePeerExitNetwork(Stream stream, KMessage message)
        {
            Logger.Log("收到消息: 用户退出。");
            int bitTorrentClientPort;
            InfoHash infoHash;
            Logger.Log("解码用户及 infohash 数据。");
            MessageFactory.GetPeerMessageContent(message, out infoHash, out bitTorrentClientPort);
            Logger.Log("Infohash: " + infoHash.ToHexString() + Environment.NewLine + "用户端口: " + bitTorrentClientPort.ToString());
            List<Peer> peerList;
            if (TrackerServer.Seeds.TryGetValue(infoHash, out peerList))
            {
                Logger.Log("在本机发现该种子。移出列表。");
                KEndPoint ep = message.Header.SourceEndPoint;
                ep.SetPort(bitTorrentClientPort);
                Peer peer = Peer.Create(ep);
                lock (peerList)
                {
                    peerList.Remove(peer);
                }
            }

            Logger.Log("转发消息。");
            // 转发
            BroadcastMessage(message);
        }

        /// <summary>
        /// 处理 GotPeer 消息。
        /// </summary>
        /// <param name="stream">一个 <see cref="System.IO.Stream"/>，表示处理该消息时所用的通信流。</param>
        /// <param name="message">一个 <see cref="Kei.KNetwork.KMessage"/>，表示收到的消息。</param>
        private void HandleGotPeer(Stream stream, KMessage message)
        {
            Logger.Log("收到消息: 在其他客户端上找到种子。");
            int bitTorrentClientPort;
            InfoHash infoHash;
            Logger.Log("解码用户及 infohash 数据。");
            MessageFactory.GetPeerMessageContent(message, out infoHash, out bitTorrentClientPort);
            Logger.Log("Infohash: " + infoHash.ToHexString() + Environment.NewLine + "用户端口: " + bitTorrentClientPort.ToString());
            List<Peer> peerList;
            if (!TrackerServer.Seeds.TryGetValue(infoHash, out peerList))
            {
                // 这是本客户端发出的信息，那么本客户端就要接收并处理
                peerList = new List<Peer>(8);
                lock (TrackerServer.Seeds)
                {
                    TrackerServer.Seeds.Add(infoHash, peerList);
                }
            }
            {
                Logger.Log("处理GOTPEER。添加对方地址。");
                KEndPoint ep = message.Header.SourceEndPoint;
                ep.SetPort(bitTorrentClientPort);
                Peer peer = Peer.Create(ep);
                if (!peerList.Contains(peer))
                {
                    lock (peerList)
                    {
                        peerList.Add(peer);
                    }
                }
            }
        }

    }
}
