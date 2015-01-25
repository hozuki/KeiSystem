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
using System.Threading;
using System.Net;
using System.Net.Sockets;

namespace Kei.KNetwork
{
    public partial class KClient
    {

        /// <summary>
        /// 本客户端监听使用的 <see cref="System.Net.TcpListener"/>。
        /// </summary>
        private TcpListener _listener;

        /// <summary>
        /// 是否应该继续工作。
        /// </summary>
        private bool _isActive;

        /// <summary>
        /// 监听所用的工作线程。
        /// </summary>
        private Thread _workerThread;

        /// <summary>
        /// 启动监听过程。
        /// </summary>
        public void Listen()
        {
            if (_workerThread == null)
            {
                IsActive = true;
                _listener = new TcpListener(IPAddress.Any, LocalKEndPoint.GetPortNumber());
                _listener.Start();
                Logger.Log("启动 KClient。");
                _workerThread = new Thread(WorkProc);
                _workerThread.IsBackground = true;
                _workerThread.Start();
            }
        }

        /// <summary>
        /// 停止监听过程。
        /// <para>注意，从执行该方法到实际停止可能略有延时。</para>
        /// </summary>
        public void Shutdown()
        {
            if (_workerThread != null)
            {
                _listener.Stop();
                Logger.Log("尝试停止 KClient。");
                IsActive = false;
            }
        }

        /// <summary>
        /// 主工作过程。
        /// </summary>
        private void WorkProc()
        {
            try
            {
                IAsyncResult iar;
                while (IsActive)
                {
                    iar = _listener.BeginAcceptTcpClient(null, null);
                    while (!iar.IsCompleted && IsActive)
                    {
                        iar.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(10));
                    }
                    if (iar.IsCompleted && IsActive)
                    {
                        Logger.Log("收到连接。");
                        var tcpClient = _listener.EndAcceptTcpClient(iar);
                        Thread thread = new Thread(CommProc);
                        thread.IsBackground = true;
                        thread.Start(tcpClient);
                    }
                }
                _workerThread = null;
            }
            catch (Exception ex)
            {
                Logger.Log(ex.Message + Environment.NewLine + ex.StackTrace);
            }
        }

        /// <summary>
        /// 收到通信时的处理过程。
        /// </summary>
        /// <param name="clientObj">一个 <see cref="System.Object"/>，实际是 <see cref="System.Net.TcpClient"/>，表示此次通信所使用的 <see cref="System.Net.TcpClient"/>。</param>
        private void CommProc(object clientObj)
        {
            Logger.Log("KClient::CommProc(object)");
            var tcpClient = clientObj as TcpClient;
            var bs = tcpClient.GetStream();
            {
                MessageIOErrorCode err;
                KMessage message;
                Logger.Log("读取消息。");
                err = bs.ReadMessage(out message);
                Logger.Log("处理消息。");
                if (err == MessageIOErrorCode.NoError)
                {
                    HandleMessage(bs, message);
                }
                else
                {
                    Logger.Log("读取发生错误，错误信息: " + err.ToString());
                }
            }
            tcpClient.Close();
        }

        /// <summary>
        /// 获取或设置是否工作。
        /// </summary>
        private bool IsActive
        {
            get
            {
                lock (this)
                {
                    return _isActive;
                }
            }
            set
            {
                lock (this)
                {
                    _isActive = value;
                }
            }
        }

    }
}
