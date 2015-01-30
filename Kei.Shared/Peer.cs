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
using System.Text;
using System.Net;
using System.Runtime.InteropServices;

namespace Kei
{
    /// <summary>
    /// 一个 BitTorrent 客户端的用户表示形式。
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct Peer : IEquatable<Peer>
    {

        /// <summary>
        /// BitTorrent 客户端的 IP（本地 IP）地址和其监听端口。
        /// </summary>
        public KEndPoint EndPoint;

        /// <summary>
        /// PeerID（可选）。
        /// </summary>
        public string ID;

        /// <summary>
        /// 使用指定的 <see cref="Kei.KEndPoint"/> 和空用户 ID 创建一个新的用户。
        /// </summary>
        /// <param name="endPoint">指定的 <see cref="Kei.KEndPoint"/>。</param>
        /// <returns>创建的 <see cref="Kei.Peer"/>。</returns>
        public static Peer Create(KEndPoint endPoint)
        {
            return Create(endPoint, null);
        }

        /// <summary>
        /// 使用指定的 <see cref="Kei.KEndPoint"/> 和指定的用户 ID 创建一个新的用户。
        /// </summary>
        /// <param name="endPoint">指定的 <see cref="Kei.KEndPoint"/>。</param>
        /// <param name="peerID">指定的用户 ID。如果为 null，表示使用空用户 ID。</param>
        /// <returns>创建的 <see cref="Kei.Peer"/>。</returns>
        public static Peer Create(KEndPoint endPoint, string peerID)
        {
            Peer peer = new Peer();
            peer.EndPoint = endPoint;
            peer.ID = peerID == null ? string.Empty : peerID;
            return peer;
        }

        /// <summary>
        /// 返回该 <see cref="Kei.Peer"/> 的字节数组表示形式。注意是 big endian。
        /// </summary>
        /// <returns>该 <see cref="Kei.Peer"/> 的字节数组表示形式。</returns>
        public byte[] ToByteArray()
        {
            return EndPoint.ToByteArray(false);
        }

        /// <summary>
        /// 判断该 <see cref="Kei.Peer"/> 与另一个 <see cref="Kei.Peer"/> 是否等价。
        /// </summary>
        /// <param name="other">另一个 <see cref="Kei.Peer"/>。</param>
        /// <returns>返回 true 表示等价，返回 false 表示不等价。</returns>
        public bool Equals(Peer other)
        {
            return EndPoint.Equals(other.EndPoint);
        }
    }
}
