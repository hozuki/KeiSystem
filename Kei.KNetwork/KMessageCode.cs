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

namespace Kei.KNetwork
{
    /// <summary>
    /// <see cref="Kei.KNetwork.KMessage"/> 的消息代码。
    /// </summary>
    public enum KMessageCode
    {
        /// <summary>
        /// 空消息。请不要使用这个常数。
        /// </summary>
        EmptyMessage = 0,
        /// <summary>
        /// ReportAlive 消息。用于连接检测，收到的客户端不需要回复。
        /// </summary>
        ReportAlive = 1,
        /// <summary>
        /// ClientEnterNetwork 消息。用于通告客户端接入分布网络。
        /// </summary>
        ClientEnterNetwork = 2,
        /// <summary>
        /// ClientExitNetwork 消息。用于通过客户端正常退出分布网络。
        /// </summary>
        ClientExitNetwork = 3,
        /// <summary>
        /// PeerEnterNetwork 消息。用于通告本机添加了一个具有指定 InfoHash 的用户。
        /// </summary>
        PeerEnterNetwork = 4,
        /// <summary>
        /// PeerExitNetwork 消息。用于通告本机一个具有指定 InfoHash 的用户已经停止工作或被移除。
        /// </summary>
        PeerExitNetwork = 5,
        /// <summary>
        /// GotPeer 消息。用于不与消息源直接连接的客户端向发送 PeerEnterNetwork 的客户端报告本地找到了所需的用户。
        /// </summary>
        GotPeer = 6,
    }
}
