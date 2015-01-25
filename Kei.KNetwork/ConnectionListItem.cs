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
    /// 表示连接列表中的一项。
    /// </summary>
    public class ConnectionListItem : IEquatable<ConnectionListItem>
    {

        /// <summary>
        /// 最多重试次数。若超过该次数，则该连接列表项将进入 ConnectionState.RemovePending 状态，等待移除。
        /// </summary>
        public static readonly uint MaximumTimesToTry = 3;

        /// <summary>
        /// 目标客户端的端点。
        /// </summary>
        private KEndPoint _clientLocation;

        /// <summary>
        /// 当前的项状态。
        /// </summary>
        private ConnectionState _state;

        /// <summary>
        /// 当前的已尝试次数。
        /// </summary>
        private uint _timesTried;

        /// <summary>
        /// 使用指定的目标端点、项状态和已尝试次数创建一个新的 ConnectionListItem。
        /// </summary>
        /// <param name="clientLocation">指定的目标端点。</param>
        /// <param name="state">指定的项状态。</param>
        /// <param name="timesTried">指定的已尝试次数。</param>
        private ConnectionListItem(KEndPoint clientLocation, ConnectionState state, uint timesTried)
        {
            _clientLocation = clientLocation;
            _state = state;
            _timesTried = timesTried;
        }

        /// <summary>
        /// 使用指定的目标端点创建一个新的 ConnectionListItem。
        /// </summary>
        /// <param name="clientLocation">指定的目标端点。</param>
        public ConnectionListItem(KEndPoint clientLocation)
            : this(clientLocation, ConnectionState.Active, 0)
        {
        }

        /// <summary>
        /// 获取目标客户端的端点。此属性为只读。
        /// </summary>
        public KEndPoint ClientLocation
        {
            get
            {
                return _clientLocation;
            }
        }

        /// <summary>
        /// 获取该连接列表项的当前状态。此属性为只读。
        /// </summary>
        public ConnectionState State
        {
            get
            {
                return _state;
            }
        }

        /// <summary>
        /// 获取该连接列表的已尝试次数。此属性为只读。
        /// </summary>
        public uint TimesTried
        {
            get
            {
                return _timesTried;
            }
        }

        /// <summary>
        /// 判断一个 ConnectionListItem 是否等价于另一个 ConnectionListItem。
        /// </summary>
        /// <param name="other">要与之比较的 ConnectionListItem。</param>
        /// <returns>一个 <see cref="System.Boolean"/>，指示二者是否等价。</returns>
        public bool Equals(ConnectionListItem other)
        {
            if (other == null)
            {
                return this == null;
            }
            else
            {
                return ClientLocation.Equals(other.ClientLocation);
            }
        }

        /// <summary>
        /// 将该连接列表项的已尝试次数增加 1。
        /// </summary>
        /// <returns>新的已尝试次数。</returns>
        private uint IncreaseTimesTried()
        {
            _state = ConnectionState.Inactive;
            return ++_timesTried;
        }

        /// <summary>
        /// 将该连接列表项的已尝试次数增加 1，并更新项状态。
        /// </summary>
        /// <returns>一个 <see cref="System.Boolean"/>，指示是否该在下一次列表清理中移除该项。</returns>
        public bool IncreaseTimesTriedAndCheck()
        {
            if ((++_timesTried) > MaximumTimesToTry)
            {
                _state = ConnectionState.RemovePending;
                return true;
            }
            else
            {
                _state = ConnectionState.Inactive;
                return false;
            }
        }

        /// <summary>
        /// 将该连接列表项重置为正常状态，已尝试次数为零。
        /// </summary>
        public void ResetTimesTried()
        {
            _state = ConnectionState.Active;
            _timesTried = 0;
        }

        /// <summary>
        /// 返回该连接列表项的字符串表示形式。
        /// </summary>
        /// <returns>该连接列表项的字符串表示形式</returns>
        public override string ToString()
        {
            return "端点 [" + ClientLocation.ToString() + "], 已尝试次数 " + TimesTried.ToString();
        }
    }
}
