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
using System.Net.Sockets;

namespace Kei
{
    /// <summary>
    /// IP 地址相关实用类。
    /// </summary>
    public static class IPUtil
    {

        /// <summary>
        /// 判断指定的 IP 地址是否是内网地址。
        /// </summary>
        /// <param name="address">要判断的 <see cref="System.Net.IPAddress"/>。</param>
        /// <returns>返回 true 表示是内网地址，返回 false 表示不是内网地址。</returns>
        /// <exception cref="System.ArgumentNullException">address 为 null 时发生。</exception>
        /// <exception cref="System.ArgumentException">address 所表示的地址非 IPv4 或 IPv6 地址时发生。</exception>
        public static bool IsAddressIntranet(IPAddress address)
        {
            byte[] addr;
            if (address == null)
            {
                throw new ArgumentNullException("address");
            }
            switch (address.AddressFamily)
            {
                case AddressFamily.InterNetwork:
                    addr = address.GetAddressBytes();
                    break;
                case AddressFamily.InterNetworkV6:
                    var adb = address.GetAddressBytes();
                    addr = adb.Skip(adb.Length - 4).ToArray();
                    break;
                default:
                    throw new ArgumentException("无效的 IP 地址。");
            }

            if (addr[0] == 10)
            {
                return true;
            }
            else
            {
                if (addr[0] == 172 && (addr[1] >= 16 && addr[1] < 32))
                {
                    return true;
                }
                else
                {
                    if (addr[0] == 192 && addr[1] == 168)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

    }
}
