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

namespace Kei.KTracker
{
    public partial class TrackerServer
    {

        /// <summary>
        /// BitTorrent 客户端眼中的“自己”，使用本地局域网地址和其监听端口。
        /// </summary>
        private Peer _myself = Peer.Create(KEndPoint.Empty);

        /// <summary>
        /// 所有的种子列表。以 <see cref="Kei.InfoHash"/> 区分，每个 <see cref="Kei.InfoHash"/> 对应一个 List&lt;<see cref="Kei.Peer"/>&gt;。
        /// </summary>
        private Dictionary<InfoHash, List<Peer>> _seeds = new Dictionary<InfoHash, List<Peer>>();

        /// <summary>
        /// 收到一次 tracker 通信时发生。
        /// </summary>
        public event EventHandler<TrackerCommEventArgs> TrackerComm;

        /// <summary>
        /// 获取所有的种子列表。此属性为只读。
        /// </summary>
        public Dictionary<InfoHash, List<Peer>> Seeds
        {
            get
            {
                return _seeds;
            }
        }

        /// <summary>
        /// 设定程序认为的 BitTorrent 客户端眼中的“自己”是什么。
        /// </summary>
        /// <param name="endPoint">本地的 BitTorrent 客户端使用的端点，包括本地局域网地址（不是本地环回地址）及其监听端口。</param>
        public void SetMyself(IPEndPoint endPoint, string peerID = null)
        {
            _myself = Peer.Create(KEndPoint.FromEndPoint(endPoint), peerID);
        }

        /// <summary>
        /// 获取 BitTorrent 客户端眼中的“自己”。此属性为只读。
        /// </summary>
        public Peer Myself
        {
            get
            {
                return _myself;
            }
        }

    }
}
