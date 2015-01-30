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

namespace Kei.KTracker
{
    /// <summary>
    /// 用于传递一次 tracker 通信信息的事件参数类。此类不能被继承。
    /// </summary>
    public sealed class TrackerCommEventArgs : EventArgs
    {

        /// <summary>
        /// 通信中 BitTorrent 客户端报告的 <see cref="Kei.InfoHash"/>。
        /// </summary>
        protected InfoHash _infoHash;

        /// <summary>
        /// 通信中 BitTorrent 客户端报告的端口。
        /// </summary>
        protected int _port;

        /// <summary>
        /// 通信中 BitTorrent 客户端报告的用户 ID。
        /// </summary>
        protected string _peerID;

        /// <summary>
        /// 通信中 BitTorrent 客户端报告的是否希望返回压缩的列表形式。
        /// </summary>
        protected bool _compact;

        /// <summary>
        /// 通信中 BitTorrent 客户端报告的是否希望列表中不返回用户 ID。
        /// <para>no_peer_id 参数优先级低于 compact 参数。</para>
        /// </summary>
        protected bool _noPeerID;

        /// <summary>
        /// 通信中 BitTorrent 客户端报告的任务状态（普通、停止、开始、暂停）。
        /// </summary>
        protected TaskStatus _status;

        /// <summary>
        /// 创建一个 <see cref="Kei.KTracker.TrackerCommEventArgs"/> 实例。
        /// </summary>
        /// <param name="infoHash">指定的 <see cref="Kei.InfoHash"/>。</param>
        /// <param name="peers">与指定的 <see cref="Kei.InfoHash"/> 关联的用户列表。</param>
        /// <param name="port">BitTorrent 客户端报告的端口。</param>
        /// <param name="peerID">BitTorrent 客户端报告的用户 ID。</param>
        /// <param name="compact">是否应返回压缩的列表。</param>
        /// <param name="noPeerID">是否应不返回用户 ID。</param>
        /// <param name="status">任务状态。</param>
        public TrackerCommEventArgs(InfoHash infoHash, List<Peer> peers, int port, string peerID = null, bool compact = true, bool noPeerID = true, TaskStatus status = TaskStatus.None)
        {
            _infoHash = infoHash;
            Peers = peers;
            _port = port;
            _peerID = peerID;
            _compact = compact;
            _noPeerID = noPeerID;
            _status = status;
        }

        /// <summary>
        /// 获取通信中 BitTorrent 客户端报告的 <see cref="Kei.InfoHash"/>。此属性为只读。
        /// </summary>
        public InfoHash InfoHash
        {
            get
            {
                return _infoHash;
            }
        }

        /// <summary>
        /// 获取或设置与当前的 <see cref="Kei.InfoHash"/> 相关联的用户列表。可能为 null，表示不可用。
        /// </summary>
        public List<Peer> Peers
        {
            get;
            set;
        }

        /// <summary>
        /// 获取通信中 BitTorrent 客户端报告的端口。此属性为只读。
        /// </summary>
        public int Port
        {
            get
            {
                return _port;
            }
        }

        /// <summary>
        /// 获取通信中 BitTorrent 客户端报告的用户 ID。此属性为只读。
        /// </summary>
        public string PeerID
        {
            get
            {
                return _peerID;
            }
        }

        /// <summary>
        /// 获取通信中 BitTorrent 客户端报告的是否希望返回压缩的列表形式。此属性为只读。
        /// </summary>
        public bool Compact
        {
            get
            {
                return _compact;
            }
        }

        /// <summary>
        /// 获取通信中 BitTorrent 客户端报告的是否希望列表中不返回用户 ID。此属性为只读。
        /// <para>no_peer_id 参数优先级低于 compact 参数。</para>
        /// </summary>
        public bool NoPeerID
        {
            get
            {
                return _noPeerID;
            }
        }

        /// <summary>
        /// 获取通信中 BitTorrent 客户端报告的任务状态（普通、停止、开始、暂停）。此属性为只读。
        /// </summary>
        public TaskStatus Status
        {
            get
            {
                return _status;
            }
        }

    }
}
