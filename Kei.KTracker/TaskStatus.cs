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
    /// BitTorrent 客户端报告的任务状态（event 参数）。
    /// </summary>
    public enum TaskStatus
    {

        /// <summary>
        /// 普通的通信。
        /// </summary>
        None,
        /// <summary>
        /// 任务切换到停止状态（event=stopped）。
        /// </summary>
        Stopped,
        /// <summary>
        /// 任务切换到开始状态（event=started）。
        /// </summary>
        Started,
        /// <summary>
        /// 任务切换到暂停状态（event=paused）。
        /// </summary>
        Paused,

    }
}
