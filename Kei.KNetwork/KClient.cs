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
using System.Net;
using Kei.KTracker;

namespace Kei.KNetwork
{

    /// <summary>
    /// 表示一个客户端的实例。此类不能被继承。
    /// </summary>
    public sealed partial class KClient
    {

        /// <summary>
        /// 与本客户端关联的 <see cref="Kei.KTracker.TrackerServer"/>。
        /// </summary>
        private TrackerServer _trackerServer;

        /// <summary>
        /// 本客户端监听的本地端点。注意，地址应采用局域网地址。
        /// </summary>
        private IPEndPoint _localListenEndPoint;

        /// <summary>
        /// 本客户端记录日志所使用的 <see cref="Kei.ILogger"/>。
        /// </summary>
        private ILogger _logger;

        /// <summary>
        /// 本地端点的 <see cref="Kei.KEndPoint"/> 表示形式，作为缓存。
        /// </summary>
        private KEndPoint _localKEndPoint;

        /// <summary>
        /// 使用指定的 <see cref="Kei.KTracker.TrackerServer"/> 和 <see cref="System.Net.IPEndPoint"/> 创建一个新的 <see cref="Kei.KNetwork.KClient"/> 实例。
        /// </summary>
        /// <param name="trackerServer">要用来处理 tracker 通信的 <see cref="Kei.KTracker.TrackerServer"/>。</param>
        /// <param name="localListenEndPoint">一个 <see cref="System.Net.IPEndPoint"/>，用来指示本地监听端点。注意应该使用本地局域网地址。</param>
        public KClient(TrackerServer trackerServer, IPEndPoint localListenEndPoint)
        {
            _trackerServer = trackerServer;
            _localListenEndPoint = localListenEndPoint;
            _trackerServer.TrackerComm += TrackerServer_TrackerComm;
            _localKEndPoint = KEndPoint.FromEndPoint(localListenEndPoint);
        }

        /// <summary>
        /// 获取或设置本客户端记录日志用的 <see cref="Kei.ILogger"/>。
        /// </summary>
        public ILogger Logger
        {
            get
            {
                if (_logger == null)
                {
                    _logger = Kei.Logger.Null;
                }
                return _logger;
            }
            set
            {
                _logger = value == null ? Kei.Logger.Null : value;
            }
        }

        /// <summary>
        /// 获取本客户端用来处理 tracker 通信的 <see cref="Kei.KTracker.TrackerServer"/>。本属性为只读。
        /// </summary>
        public TrackerServer TrackerServer
        {
            get
            {
                return _trackerServer;
            }
        }

        /// <summary>
        /// 获取本客户端的局域网地址及其监听端口。本属性为只读。
        /// </summary>
        public IPEndPoint LocalListenEndPoint
        {
            get
            {
                return _localListenEndPoint;
            }
        }

        /// <summary>
        /// 获取本客户端监听端点的的 <see cref="Kei.KEndPoint"/> 表示形式。本属性为只读。
        /// </summary>
        public KEndPoint LocalKEndPoint
        {
            get
            {
                return _localKEndPoint;
            }
        }

    }
}
