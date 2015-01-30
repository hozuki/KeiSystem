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
    /// KeiSystem 公用实用类。
    /// </summary>
    public static class KeiUtil
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

        /// <summary>
        /// 解析指定的字符串，得到一个 <see cref="System.Net.IPEndPoint"/>。
        /// </summary>
        /// <param name="str">指定的字符串，含有要解析的端点信息。</param>
        /// <returns>解析出的 <see cref="System.Net.IPEndPoint"/></returns>
        /// <exception cref="System.ArgumentNullException">str 为 null 时发生。</exception>
        /// <exception cref="System.FormatException">输入的字符串不是 IP 端点时发生。</exception>
        public static IPEndPoint ParseIPEndPoint(string str)
        {
            // 格式为: a.a.a.a:p
            if (str == null)
            {
                throw new ArgumentNullException("str");
            }
            var s = str.Split(':');
            if (s.Length != 2)
            {
                throw new FormatException("应该类似 a.a.a.a:p 的形式。");
            }
            var ipa = IPAddress.Parse(s[0]);
            var port = Convert.ToInt32(s[1]);
            return new IPEndPoint(ipa, port);
        }

        /// <summary>
        /// 获取一个指定端口的环回端点。
        /// </summary>
        /// <param name="port">指定的端口。</param>
        /// <returns>获取的 <see cref="System.Net.IPEndPoint"/>。</returns>
        public static IPEndPoint GetLoopbackEndPoint(int port)
        {
            return new IPEndPoint(IPAddress.Loopback, port);
        }

        /// <summary>
        /// 翻转字节序。常用于网络传输字节序不匹配时。
        /// </summary>
        /// <param name="source">要翻转的源字节数组。</param>
        /// <returns>翻转后的字节数组。</returns>
        /// <exception cref="System.ArgumentException">source 为 null 时发生。</exception>
        public static byte[] ReverseByteOrder(byte[] source)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            return source.Reverse().ToArray();
        }

    }
}
