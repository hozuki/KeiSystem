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
    /// 表示一个 ConnectionListItem 可能的状态。
    /// </summary>
    public enum ConnectionState
    {

        /// <summary>
        /// 该 ConnectionListItem 处于活动状态，上一次通信是成功的。
        /// </summary>
        Active,
        /// <summary>
        /// 该 ConnectionListItem 处于不活动状态，上一次通信失败。
        /// </summary>
        Inactive,
        /// <summary>
        /// 该 ConnectionListItem 处于即将移除状态，尝试次数已经超过阈值。
        /// </summary>
        RemovePending,

    }
}
