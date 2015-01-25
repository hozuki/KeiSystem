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
    public partial class KClient
    {

        /// <summary>
        /// 本客户端的“已处理消息”列表。
        /// </summary>
        private List<KHandledMessage> _handledMessages = new List<KHandledMessage>(100);

        /// <summary>
        /// 获取本客户端的“已处理消息”列表。此属性为只读。
        /// </summary>
        public List<KHandledMessage> HandledMessages
        {
            get
            {
                return _handledMessages;
            }
        }

        /// <summary>
        /// 清理“已处理消息”列表，删除生存周期超过当前时刻的项。
        /// </summary>
        private void SweepHandledMessages()
        {
            Logger.Log("KClient::SweepHandledMessages()");
            // 由于已处理消息列表是有先来后到的顺序的，因此只需要判断、删除第一个即可
            DateTime now = DateTime.Now;
            int n = 0;
            lock (HandledMessages)
            {
                while (HandledMessages.Count > 0 && HandledMessages[0].ShouldBeDisposedAt(now))
                {
                    HandledMessages.RemoveAt(0);
                    n++;
                }
            }
            Logger.Log("移除了 " + n.ToString() + " 项。");
        }

    }
}
