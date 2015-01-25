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
    /// <see cref="System.Collections.Generic.List<T>"/> 相关的扩展类。
    /// </summary>
    public static class ListExtensions
    {

        /// <summary>
        /// 判断一个 List&lt;ConnectionListItem&gt; 内是否含有以指定 <see cref="Kei.KEndPoint"/> 区分的 <see cref="Kei.KNetwork.ConnectionListItem"/>。
        /// </summary>
        /// <param name="list">要判断的 List&lt;ConnectionListItem&gt;。</param>
        /// <param name="endPoint">作为指定键的 <see cref="Kei.KEndPoint"/>。</param>
        /// <returns>一个 <see cref="System.Boolean"/>，指示是否找到了符合条件的 <see cref="Kei.KNetwork.ConnectionListItem"/>。</returns>
        public static bool Contains(this List<ConnectionListItem> list, KEndPoint endPoint)
        {
            if (list == null)
            {
                return false;
            }
            bool contains = false;
            lock (list)
            {
                foreach (var item in list)
                {
                    if (item.ClientLocation.Equals(endPoint))
                    {
                        contains = true;
                        break;
                    }
                }
            }
            return contains;
        }

        /// <summary>
        /// 判断一个 List&lt;KHandledMessage&gt; 内是否含有以指定 <see cref="Kei.KNetwork.KMessages"/> 区分的 <see cref="Kei.KNetwork.KHandledMessage"/>。
        /// </summary>
        /// <param name="list">要判断的 List&lt;KHandledMessage&gt;。</param>
        /// <param name="message">作为指定键的 <see cref="Kei.KNetwork.KMessage"/>。</param>
        /// <returns>一个 <see cref="System.Boolean"/>，指示是否找到了符合条件的 <see cref="Kei.KNetwork.KHandledMessage"/>。</returns>
        public static bool Contains(this List<KHandledMessage> list, KMessage message)
        {
            if (list == null)
            {
                return false;
            }
            bool contains = false;
            lock (list)
            {
                foreach (var item in list)
                {
                    if (item.Message.Equals(message))
                    {
                        contains = true;
                        break;
                    }
                }
            }
            return contains;
        }

        /// <summary>
        /// 在指定的 List&lt;KHandledMessage&gt; 中寻找以指定 <see cref="Kei.KNetwork.KMessage"/> 区分的 <see cref="Kei.KNetwork.KHandledMessage"/>，并返回查找结果。
        /// </summary>
        /// <param name="list">要在其中查找的 List&lt;KHandledMessage&gt;。</param>
        /// <param name="message">作为指定键的 <see cref="Kei.KNetwork.KMessage"/>。</param>
        /// <param name="index">输出一个 <see cref="System.Int32"/>。如果找到了，这个值是找到的项的索引；如果未找到，则该值无效。</param>
        /// <returns>一个 <see cref="Kei.KNetwork.KHandledMessage"/>，指示找到的项；或者 null，表示没找到。</returns>
        public static KHandledMessage FindHandledMessage(this List<KHandledMessage> list, KMessage message, out int index)
        {
            if (list == null)
            {
                index = 0;
                return null;
            }
            KHandledMessage khm = null;
            int i = 0;
            index = 0;
            lock (list)
            {
                foreach (var item in list)
                {
                    if (item.Message.Equals(message))
                    {
                        index = i;
                        khm = item;
                        break;
                    }
                    i++;
                }
            }
            return khm;
        }

        /// <summary>
        /// 在指定的 List&lt;ConnectionListItem&gt; 中寻找以指定 <see cref="Kei.KEndPoint"/> 区分的 <see cref="Kei.KNetwork.ConnectionListItem"/>，并返回查找结果。
        /// </summary>
        /// <param name="list">要在其中查找的 List&lt;ConnectionListItem&gt;。</param>
        /// <param name="endPoint">作为指定键的 <see cref="Kei.KEndPoint"/>。</param>
        /// <param name="index">输出一个 <see cref="System.Int32"/>。如果找到了，这个值是找到的项的索引；如果未找到，则该值无效。</param>
        /// <returns>一个 <see cref="Kei.KNetwork.ConnectionListItem"/>，指示找到的项；或者 null，表示没找到。</returns>
        public static ConnectionListItem FindConnectionListItem(this List<ConnectionListItem> list, KEndPoint endPoint, out int index)
        {
            if (list == null)
            {
                index = 0;
                return null;
            }
            ConnectionListItem cli = null;
            int i = 0;
            index = 0;
            lock (list)
            {
                foreach (var item in list)
                {
                    if (item.ClientLocation.Equals(endPoint))
                    {
                        index = i;
                        cli = item;
                        break;
                    }
                    i++;
                }
            }
            return cli;
        }

    }
}
