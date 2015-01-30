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
using System.Threading;
using MonoTorrent.BEncoding;

namespace Kei.KNetwork
{
    public partial class KClient
    {

        /// <summary>
        /// 连接列表发生变化时发生。
        /// <para>该事件通常不是发生在 UI 线程上的。</para>
        /// </summary>
        public event EventHandler<EventArgs> ConnectionListChanged;

        /// <summary>
        /// 发送一条消息的超时。若发送消息所用时间超过这个时间，则认为正在连接的端点无效。默认值为 500 毫秒，对于内网来说足够。
        /// </summary>
        private static readonly TimeSpan SendTimeout = TimeSpan.FromMilliseconds(500);

        /// <summary>
        /// 本客户端的连接列表。
        /// </summary>
        private List<ConnectionListItem> _connectionList = new List<ConnectionListItem>(40);

        /// <summary>
        /// 获取本客户端的连接列表。此属性为只读。
        /// </summary>
        public List<ConnectionListItem> ConnectionList
        {
            get
            {
                return _connectionList;
            }
        }

        /// <summary>
        /// 将一个端点（对应一个客户端）添加至连接列表。如果列表中已有此端点，则返回对应的连接列表项。
        /// <para>注意，本方法执行中会为 ConnectionList 加锁。</para>
        /// </summary>
        /// <param name="endPoint">要添加的客户端的端点。</param>
        /// <returns>一个 <see cref="Kei.KNetwork.ConnectionListItem"/>，表示添加或者找到的连接列表项。</returns>
        private ConnectionListItem AddToConnectionList(KEndPoint endPoint)
        {
            if (endPoint.Equals(LocalKEndPoint))
            {
                Logger.Log("KClient::AddToConnectionList(KEndPoint): 不能加入自己");
                return null;
            }
            Logger.Log("KClient::AddToConnectionList(KEndPoint)" + Environment.NewLine + "待加入的端点: " + endPoint.ToString());
            int index;
            var connItem = ConnectionList.FindConnectionListItem(endPoint, out index);
            if (connItem == null)
            {
                Logger.Log("在列表中未发现，即将加入。");
                connItem = new ConnectionListItem(endPoint);
                lock (ConnectionList)
                {
                    ConnectionList.Add(connItem);
                    EventHelper.RaiseEvent(ConnectionListChanged, this, EventArgs.Empty);
                }
            }
            return connItem;
        }

        /// <summary>
        /// 清扫连接列表，删除列表中所有状态为 <see cref="Kei.KNetwork.ConnectionState.RemovePending"/> 的列表项。
        /// <para>注意，本方法执行中会为 ConnectionList 加锁。</para>
        /// </summary>
        private void SweepConnectionList()
        {
            Logger.Log("清扫连接列表。");
            var i = 0;
            var removed = 0;
            lock (ConnectionList)
            {
                while (i < ConnectionList.Count)
                {
                    if (ConnectionList[i].State == ConnectionState.RemovePending)
                    {
                        ConnectionList.RemoveAt(i);
                        removed++;
                    }
                    else
                    {
                        i++;
                    }
                }
            }
            if (removed > 0)
            {
                Logger.Log("清除了 " + removed.ToString() + " 个连接项。");
                EventHelper.RaiseEvent(ConnectionListChanged, this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// 尝试将一个端点（对应一个客户端）加入连接列表，并向该端点发送一条消息。
        /// </summary>
        /// <param name="endPoint">目标端点。</param>
        /// <param name="message">要发送的消息。</param>
        /// <returns>一个 <see cref="System.Threading.WaitHandle"/>，通过此对象可以对发送操作进行等待。</returns>
        private WaitHandle SendMessage(KEndPoint endPoint, KMessage message)
        {
            string tmpLog;
            tmpLog = "KClient::SendMessage(KEndPoint, KMesage)";
            tmpLog += Environment.NewLine + "消息代码: " + message.Header.Code.ToString();
            tmpLog += Environment.NewLine + "消息唯一编码: " + message.Header.MessageID.ToString();
            tmpLog += Environment.NewLine + "消息来源: " + message.Header.SourceEndPoint.ToString();
            Logger.Log(tmpLog);
            AutoResetEvent ev = new AutoResetEvent(false);
            Thread thread = new Thread(delegate()
            {
                try
                {
                    TcpClient tcpClient = new TcpClient();
                    var remoteEndPoint = endPoint.GetEndPoint();
                    Logger.Log("KClient::SendMessage(KEndPoint, KMessage) 工作线程。");
                    var connListItem = AddToConnectionList(endPoint);
                    if (connListItem == null)
                    {
                        return;
                    }
                    Logger.Log("尝试连接端点: " + remoteEndPoint.ToString());
                    var iar = tcpClient.BeginConnect(remoteEndPoint.Address, remoteEndPoint.Port, null, null);
                    if (!iar.IsCompleted)
                    {
                        iar.AsyncWaitHandle.WaitOne(SendTimeout);
                    }
                    if (iar.IsCompleted)
                    {
                        Logger.Log("连接成功。");
                        // 未超时
                        tcpClient.EndConnect(iar);
                        connListItem.ResetTimesTried();
                        var bs = tcpClient.GetStream();
                        {
                            Logger.Log("发送消息。");
                            bs.WriteMessage(message);
                        }
                        tcpClient.Close();
                    }
                    else
                    {
                        Logger.Log("连接失败。");
                        // 设置/移除出列表
                        connListItem.IncreaseTimesTriedAndCheck();
                    }
                    ev.Set();
                    ev.Dispose();
                }
                catch (Exception ex)
                {
                    Logger.Log(ex.Message + Environment.NewLine + ex.StackTrace);
                }
            });
            thread.IsBackground = true;
            thread.Start();
            return ev;
        }

        /// <summary>
        /// 向连接列表中的所有端点发送消息（即广播）。
        /// <para>注意，本方法执行中会为 ConnectionList 加锁。</para>
        /// </summary>
        /// <param name="message">要广播的消息。</param>
        /// <returns>一个 <see cref="System.Threading.WaitHandle"/>，通过此对象可以对发送操作进行等待。</returns>
        private WaitHandle BroadcastMessage(KMessage message)
        {
            string tmpLog;
            tmpLog = "KClient::BroadcastMessage(KMesage)";
            tmpLog += Environment.NewLine + "消息代码: " + message.Header.Code.ToString();
            tmpLog += Environment.NewLine + "消息唯一编码: " + message.Header.MessageID.ToString();
            tmpLog += Environment.NewLine + "消息来源: " + message.Header.SourceEndPoint.ToString();
            Logger.Log(tmpLog);
            AutoResetEvent ev = new AutoResetEvent(false);
            Thread thread = new Thread(delegate()
            {
                try
                {
                    lock (ConnectionList)
                    {
                        foreach (var item in ConnectionList)
                        {
                            if (item.State != ConnectionState.RemovePending)
                            {
                                TcpClient tcpClient = new TcpClient();
                                var remoteEndPoint = item.ClientLocation.GetEndPoint();
                                Logger.Log("尝试连接端点: " + remoteEndPoint.ToString());
                                var iar = tcpClient.BeginConnect(remoteEndPoint.Address, remoteEndPoint.Port, null, null);
                                if (!iar.IsCompleted)
                                {
                                    iar.AsyncWaitHandle.WaitOne(SendTimeout);
                                }
                                if (iar.IsCompleted)
                                {
                                    Logger.Log("连接成功。");
                                    // 未超时
                                    tcpClient.EndConnect(iar);
                                    item.ResetTimesTried();
                                    var bs = tcpClient.GetStream();
                                    {
                                        Logger.Log("广播消息。");
                                        // 本次调用是因为收到了 TrackerComm 事件的消息
                                        // 发生事件前 TrackerServer 的 Myself 就已经设置完成了，所以这里不用担心
                                        bs.WriteMessage(message);
                                    }
                                    tcpClient.Close();
                                }
                                else
                                {
                                    Logger.Log("连接超时。");
                                    // 设置/移除出列表
                                    item.IncreaseTimesTriedAndCheck();
                                }
                            }
                        }
                    }
                    Logger.Log("清扫连接列表并重设等待事件。");
                    SweepConnectionList();
                    ev.Set();
                    ev.Dispose();
                }
                catch (Exception ex)
                {
                    Logger.Log(ex.Message + Environment.NewLine + ex.StackTrace);
                }
            });
            thread.IsBackground = true;
            thread.Start();
            return ev;
        }

        /// <summary>
        /// 连接到指定的接入端点，并广播“加入分布网络”消息。
        /// <para>注意，本方法执行中会为 ConnectionList 加锁。</para>
        /// </summary>
        /// <param name="pointInsertion">要连接的端点。</param>
        /// <param name="realPort">非零表示这是接入点要连接接入点，该端口是本机正在监听的端口；零表示只是普通用户连接接入点。</param>
        /// <param name="workerThread">返回发送用的 <see cref="System.Threading.Thread"/>。</param>
        /// <returns>一个 <see cref="System.Threading.WaitHandle"/>，通过此对象可以对发送操作进行等待。</returns>
        public WaitHandle EnterNetwork(IPEndPoint pointInsertion, ushort realPort, out Thread workerThread)
        {
            Logger.Log("KClient::EnterNetwork(IPEndPoint)" + Environment.NewLine + "接入点: " + pointInsertion.ToString());
            AutoResetEvent ev = new AutoResetEvent(false);
            Thread thread = new Thread(delegate()
            {
                try
                {
                    // 第一次发送信息，要使用以后用来接收信息的端口，这样才能让接入点知道你映射后的接收端口
                    //TcpClient client = new TcpClient();
                    // 那么就不能采取常规的方式，而应该用 TcpListener 的 socket 发出去
                    //TcpClient client = new TcpClient(new IPEndPoint(IPAddress.Loopback, LocalEndPoint.Port));
                    Socket enterSocket;
                    if (realPort == 0)
                    {
                        // 普通用户
                        enterSocket = _listener.Server;
                        if (!enterSocket.IsBound)
                        {
                            enterSocket.Bind(new IPEndPoint(IPAddress.Any, ((IPEndPoint)_listener.LocalEndpoint).Port));
                        }
                    }
                    else
                    {
                        enterSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    }
                    //Socket enterSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    //Socket enterSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
                    //enterSocket.Bind(new IPEndPoint(IPAddress.Loopback, LocalEndPoint.Port));
                    Logger.Log("尝试连接端点: " + pointInsertion.ToString());
                    //var iar = client.BeginConnect(pointInsertion.Address, pointInsertion.Port, null, null);
                    var iar = enterSocket.BeginConnect(pointInsertion, null, null);
                    if (!iar.IsCompleted)
                    {
                        iar.AsyncWaitHandle.WaitOne(SendTimeout);
                    }
                    if (!iar.IsCompleted)
                    {
                        Logger.Log("连接超时。");
                        // 未成功连接的话，直接返回
                        //return false;
                        ev.Set();
                        ev.Dispose();
                        return;
                        // 这里好像无法直接返回连接状态
                        // 但是应该可以判断连接列表是否为空来确定是否成功，非空则成功，否则失败
                    }
                    //client.EndConnect(iar);
                    enterSocket.EndConnect(iar);
                    Logger.Log("socket 本地端点: " + enterSocket.LocalEndPoint.ToString() + " 远程端点: " + enterSocket.RemoteEndPoint.ToString());
                    // 为了不干扰连接列表的报告，先发送信息
                    Logger.Log("连接成功，发送自己进入的消息。");

                    int origPortNumber;
                    string origAddress;

                    //var bs = client.GetStream();
                    {
                        // 写入
                        //bs.WriteMessage(MessageFactory.ClientEnterNetwork(LocalKEndPoint));
                        enterSocket.WriteMessage(MessageFactory.ClientEnterNetwork(LocalKEndPoint, realPort));

                        // 接下来接收接入点返回的当前连接信息
                        // 先是读取列表数据大小
                        Logger.Log("读取返回数据: 数据长度");
                        //int dataLength = bs.ReadInt32();
                        int dataLength = enterSocket.ReadInt32();
                        Logger.Log("长度: " + dataLength.ToString());
                        // 然后读列表
                        var dataBuffer = new byte[1024];
                        var data = new byte[dataLength];
                        int readLength = 0;
                        int totalReadLength = 0;
                        // 确保全部读取
                        while (totalReadLength < dataLength)
                        {
                            //readLength = bs.Read(dataBuffer, 0, dataBuffer.Length);
                            readLength = enterSocket.Receive(dataBuffer);
                            if (readLength > 0)
                            {
                                Array.Copy(dataBuffer, 0, data, totalReadLength, readLength);
                                totalReadLength += readLength;
                            }
                        }
                        Logger.Log("数据读取完毕。准备解码。");

                        origPortNumber = LocalEndPoint.Port;
                        origAddress = LocalEndPoint.Address.ToString();

                        // 解码并将自己的连接列表和用户列表同步为接入点的数据
                        DecodeTargetInformation(data);
                        Logger.Log("他人眼中的我: " + LocalEndPoint.ToString());

                        StringBuilder sb = new StringBuilder(100);
                        sb.AppendLine("刚刚收到的连接列表如下。");
                        lock (ConnectionList)
                        {
                            foreach (var item in ConnectionList)
                            {
                                sb.AppendLine(item.ToString());
                            }
                        }
                        sb.AppendLine("刚刚收到的用户列表如下。");
                        lock (TrackerServer.Seeds)
                        {
                            foreach (var item in TrackerServer.Seeds)
                            {
                                sb.AppendLine("[" + item.Key.ToHexString() + "]");
                                foreach (var peer in item.Value)
                                {
                                    sb.AppendLine(peer.EndPoint.ToString());
                                }
                            }
                        }
                        Logger.Log(sb.ToString());
                    }

                    Logger.Log("添加对方持有的连接信息结束，将对方加入自己的连接列表。");
                    // 然后将对方加入自己的连接列表
                    try
                    {
                        AddToConnectionList(KEndPoint.FromEndPoint(pointInsertion));
                    }
                    catch (Exception)
                    {
                    }

                    // 不需要
                    //if (ConnectionList.Count > 0)
                    //{
                    //    Logger.Log("尝试广播消息，让连接列表中的客户端报告他们存活。");
                    //    // 要其他人报告他们存活
                    //    // 能发送等价于存活，所以并不期望回音，只是测试能连接而已
                    //    BroadcastMessage(MessageFactory.ReportAlive(LocalKEndPoint));
                    //}

                    //client.Close();
                    enterSocket.Disconnect(false);
                    enterSocket.Close();
                    _listener.Stop();

                    // 说到底，要 EnterNetwork()，肯定是普通客户端，那么就请求端口映射吧
                    //if (origPortNumber != LocalEndPoint.Port)
                    {
                        if (PortMapping.CanUsePortMapping)
                        {
                            try
                            {
                                if (origPortNumber == LocalEndPoint.Port)
                                {
                                    Thread.Sleep(TimeSpan.FromSeconds(0.8));
                                }
                                //PortMapping.AddPortMapping((ushort)origPortNumber, (ushort)LocalEndPoint.Port, origAddress);
                                PortMapping.AddPortMapping((ushort)origPortNumber, (ushort)LocalEndPoint.Port);
                            }
                            catch (Exception ex)
                            {
                                System.Diagnostics.Debug.Print("AddPortMapping: " + ex.Message);
                                Logger.Log("AddPortMapping: " + ex.Message);
                            }
                        }
                    }

                    ev.Set();
                    ev.Dispose();
                    //return true;
                }
                catch (Exception ex)
                {
                    Logger.Log(ex.Message + Environment.NewLine + ex.StackTrace);
                }
            });
            thread.IsBackground = true;
            thread.Start();
            workerThread = thread;
            return ev;
        }

        /// <summary>
        /// 向连接列表中的所有端点广播本客户端“退出分布网络”消息。
        /// </summary>
        /// <param name="workerThread">返回发送用的 <see cref="System.Threading.Thread"/>。</param>
        /// <returns>一个 <see cref="System.Threading.WaitHandle"/>，通过此对象可以对发送操作进行等待。</returns>
        public WaitHandle ExitNetwork(out Thread workerThread)
        {
            Logger.Log("KClient::ExitNetwork()");
            AutoResetEvent ev = new AutoResetEvent(false);
            Thread thread = new Thread(delegate()
            {
                try
                {
                    if (PortMapping.AddedPortMapping)
                    {
                        PortMapping.DeletePortMapping();
                    }

                    foreach (var item in ConnectionList)
                    {
                        if (item.State != ConnectionState.RemovePending)
                        {
                            TcpClient tcpClient = new TcpClient();
                            var remoteEndPoint = item.ClientLocation.GetEndPoint();
                            Logger.Log("尝试连接端点: " + remoteEndPoint.ToString());
                            var iar = tcpClient.BeginConnect(remoteEndPoint.Address, remoteEndPoint.Port, null, null);
                            if (!iar.IsCompleted)
                            {
                                iar.AsyncWaitHandle.WaitOne(SendTimeout);
                            }
                            if (iar.IsCompleted)
                            {
                                // 未超时
                                tcpClient.EndConnect(iar);
                                Logger.Log("连接成功。发送自己退出的消息。");
                                var bs = tcpClient.GetStream();
                                {
                                    bs.WriteMessage(MessageFactory.ClientExitNetwork(LocalKEndPoint));
                                }
                                tcpClient.Close();
                            }
                            else
                            {
                                Logger.Log("连接失败。无任何其他操作。");
                            }
                        }
                    }
                    ev.Set();
                    ev.Dispose();
                }
                catch (Exception ex)
                {
                    Logger.Log(ex.Message + Environment.NewLine + ex.StackTrace);
                }
            });
            thread.IsBackground = true;
            thread.Start();
            workerThread = thread;
            return ev;
        }

        //public WaitHandle BroadcastMyselfAddAsPeer(InfoHash infoHash)
        //{
        //    AutoResetEvent ev = new AutoResetEvent(false);
        //    Thread thread = new Thread(delegate()
        //    {
        //        foreach (var item in ConnectionList)
        //        {
        //            if (item.State != ConnectionState.RemovePending)
        //            {
        //                TcpClient tcpClient = new TcpClient();
        //                var remoteEndPoint = item.ClientLocation.GetEndPoint();
        //                var iar = tcpClient.BeginConnect(remoteEndPoint.Address, remoteEndPoint.Port, null, null);
        //                if (!iar.IsCompleted)
        //                {
        //                    iar.AsyncWaitHandle.WaitOne(SendTimeout);
        //                }
        //                if (iar.IsCompleted)
        //                {
        //                    // 未超时
        //                    tcpClient.EndConnect(iar);
        //                    item.ResetTimesTried();
        //                    using (var bs = new BufferedStream(tcpClient.GetStream()))
        //                    {
        //                        // 本次调用是因为收到了 TrackerComm 事件的消息
        //                        // 发生事件前 TrackerServer 的 Myself 就已经设置完成了，所以这里不用担心
        //                        bs.WriteMessage(MessageFactory.PeerEnterNetwork(LocalListenEndPoint, infoHash, TrackerServer.Myself.EndPoint.GetPortNumber()));
        //                    }
        //                    tcpClient.Close();
        //                }
        //                else
        //                {
        //                    // 设置/移除出列表
        //                    item.IncreaseTimesTriedAndCheck();
        //                }
        //            }
        //        }
        //        SweepConnectionList();
        //        ev.Set();
        //        ev.Dispose();
        //    });
        //    thread.IsBackground = true;
        //    thread.Start();
        //    return ev;
        //}

        //public WaitHandle BroadcastMyselfRemoveAsPeer(InfoHash infoHash)
        //{
        //    AutoResetEvent ev = new AutoResetEvent(false);
        //    Thread thread = new Thread(delegate()
        //    {
        //        foreach (var item in ConnectionList)
        //        {
        //            if (item.State != ConnectionState.RemovePending)
        //            {
        //                TcpClient tcpClient = new TcpClient();
        //                var remoteEndPoint = item.ClientLocation.GetEndPoint();
        //                var iar = tcpClient.BeginConnect(remoteEndPoint.Address, remoteEndPoint.Port, null, null);
        //                if (!iar.IsCompleted)
        //                {
        //                    iar.AsyncWaitHandle.WaitOne(SendTimeout);
        //                }
        //                if (iar.IsCompleted)
        //                {
        //                    // 未超时
        //                    tcpClient.EndConnect(iar);
        //                    item.ResetTimesTried();
        //                    using (var bs = new BufferedStream(tcpClient.GetStream()))
        //                    {
        //                        // 本次调用是因为收到了 TrackerComm 事件的消息
        //                        // 发生事件前 TrackerServer 的 Myself 就已经设置完成了，所以这里不用担心
        //                        bs.WriteMessage(MessageFactory.PeerExitNetwork(LocalListenEndPoint, infoHash, TrackerServer.Myself.EndPoint.GetPortNumber()));
        //                    }
        //                    tcpClient.Close();
        //                }
        //                else
        //                {
        //                    // 设置/移除出列表
        //                    item.IncreaseTimesTriedAndCheck();
        //                }
        //            }
        //        }
        //        SweepConnectionList();
        //        ev.Set();
        //        ev.Dispose();
        //    });
        //    thread.IsBackground = true;
        //    thread.Start();
        //    return ev;
        //}

        /// <summary>
        /// 向连接列表中的所有端点广播本客户端的、具有指定 InfoHash 的“用户进入”消息。
        /// <para>注意，本方法执行中会为 ConnectionList 加锁。</para>
        /// </summary>
        /// <param name="infoHash">加入的用户所持有的 InfoHash。</param>
        /// <returns>一个 <see cref="System.Threading.WaitHandle"/>，通过此对象可以对发送操作进行等待。</returns>
        private WaitHandle BroadcastMyselfAddAsPeer(InfoHash infoHash)
        {
            Logger.Log("KClient::BroadcastMyselfAddAsPeer(InfoHash)" + Environment.NewLine + "Infohash: " + infoHash.ToString());
            return BroadcastMessage(MessageFactory.PeerEnterNetwork(LocalKEndPoint, infoHash, (ushort)TrackerServer.Myself.EndPoint.GetPortNumber()));
        }

        /// <summary>
        /// 向连接列表中的所有端点广播本客户端的、具有指定 InfoHash 的“用户退出”消息。
        /// <para>注意，本方法执行中会为 ConnectionList 加锁。</para>
        /// </summary>
        /// <param name="infoHash">加入的用户所持有的 InfoHash。</param>
        /// <returns>一个 <see cref="System.Threading.WaitHandle"/>，通过此对象可以对发送操作进行等待。</returns>
        private WaitHandle BroadcastMyselfRemoveAsPeer(InfoHash infoHash)
        {
            Logger.Log("KClient::BroadcastMyselfRemoveAsPeer(InfoHash)" + Environment.NewLine + "Infohash: " + infoHash.ToString());
            return BroadcastMessage(MessageFactory.PeerExitNetwork(LocalKEndPoint, infoHash, (ushort)TrackerServer.Myself.EndPoint.GetPortNumber()));
        }

    }
}
