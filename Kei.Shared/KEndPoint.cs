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
using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace Kei
{

    /// <summary>
    /// 一个端点的结构表示形式。
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public unsafe struct KEndPoint : IEquatable<KEndPoint>
    {

        /// <summary>
        /// 表示一个空的端点，即地址为 0.0.0.0，端口为 0 的端点。
        /// </summary>
        public static readonly KEndPoint Empty = CreateEmptyInstance();

        /// <summary>
        /// 该端点的地址。
        /// </summary>
        public fixed byte Address[4];

        /// <summary>
        /// 该端点的端口。
        /// </summary>
        public fixed byte Port[2];

        #region Methods

        /// <summary>
        /// 设置该端点的地址。
        /// </summary>
        /// <param name="address">一个 <see cref="System.Net.IPAddress"/>，表示要设置为的地址。</param>
        /// <exception cref="System.ArgumentException">设置的地址不是 IPv4 或 IPv6 地址时发生。</exception>
        public void SetAddress(IPAddress address)
        {
            unsafe
            {
                if (address == null)
                {
                    fixed (byte* p = Address)
                    {
                        p[0] = p[1] = p[2] = p[3] = 0;
                    }
                }
                else
                {
                    fixed (byte* p = Address)
                    {
                        if (address.AddressFamily == AddressFamily.InterNetwork)
                        {
                            var addressBytes = address.GetAddressBytes();
                            p[0] = addressBytes[0];
                            p[1] = addressBytes[1];
                            p[2] = addressBytes[2];
                            p[3] = addressBytes[3];
                        }
                        else if (address.AddressFamily == AddressFamily.InterNetworkV6)
                        {
                            var bytes = address.GetAddressBytes();
                            var len = bytes.Length;
                            p[0] = bytes[len - 4];
                            p[1] = bytes[len - 3];
                            p[2] = bytes[len - 2];
                            p[3] = bytes[len - 1];
                        }
                        else
                        {
                            throw new ArgumentException("意外的地址格式。");
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 设置该端点的地址。
        /// </summary>
        /// <param name="address">一个 <see cref="System.String"/>，指定要设置的地址。</param>
        public void SetAddress(string address)
        {
            unsafe
            {
                IPAddress ipAddress;
                var b = IPAddress.TryParse(address, out ipAddress);
                if (b)
                {
                    SetAddress(ipAddress);
                }
                else
                {
                    fixed (byte* p = Address)
                    {
                        p[0] = p[1] = p[2] = p[3] = 0;
                    }
                }
            }
        }

        /// <summary>
        /// 设置该端点的地址。
        /// </summary>
        /// <param name="address">一个 <see cref="System.Byte"/>[]，指定要设置的地址。</param>
        /// <exception cref="System.ArgumentException">address 长度不为 4 时发生。</exception>
        public void SetAddress(byte[] address)
        {
            unsafe
            {
                if (address == null)
                {
                    fixed (byte* p = Address)
                    {
                        p[0] = p[1] = p[2] = p[3] = 0;
                    }
                }
                else
                {
                    if (address.Length != 4)
                    {
                        throw new ArgumentException("address 长度必须为 4。");
                    }
                    fixed (byte* p = Address)
                    {
                        p[0] = address[0];
                        p[1] = address[1];
                        p[2] = address[2];
                        p[3] = address[3];
                    }
                }
            }
        }

        /// <summary>
        /// 设置该端点的端口。
        /// </summary>
        /// <param name="port">要设置为的端口。</param>
        /// <exception cref="System.ArgumentOutOfRangeException">端口超出可用端口范围时发生。</exception>
        public void SetPort(int port)
        {
            unsafe
            {
                if (port > IPEndPoint.MaxPort || port < IPEndPoint.MinPort)
                {
                    throw new ArgumentOutOfRangeException("port");
                }
                byte b1, b2;
                b1 = (byte)((port & 0x0000ff00) >> 8);
                b2 = (byte)(port & 0x000000ff);
                fixed (byte* p = Port)
                {
                    p[0] = b1;
                    p[1] = b2;
                }
            }
        }

        /// <summary>
        /// 设置该端点的端口。
        /// </summary>
        /// <param name="port">要设置的端口的字符串表示形式。若无法解析，则认为是端口 0。</param>
        public void SetPort(string port)
        {
            int portNo = 0;
            try
            {
                portNo = Convert.ToInt32(port);
            }
            catch (Exception)
            {
            }
            SetPort(portNo);
        }

        /// <summary>
        /// 获取该端点地址的字符串表示形式。
        /// </summary>
        /// <returns>一个 <see cref="System.String"/>，表示该端点的地址。</returns>
        public string GetAddressString()
        {
            unsafe
            {
                fixed (byte* p = Address)
                {
                    return string.Concat(p[0].ToString(), ".", p[1].ToString(), ".", p[2].ToString(), ".", p[3].ToString());
                }
            }
        }

        /// <summary>
        /// 获取该端点地址的字节数组表示形式。
        /// </summary>
        /// <returns>一个长度为 4 的 <see cref="System.Byte"/>[]，表示该端点的地址。</returns>
        public byte[] GetAddressBytes()
        {
            unsafe
            {
                fixed (byte* p = Address)
                {
                    byte[] b = new byte[4];
                    b[0] = p[0];
                    b[1] = p[1];
                    b[2] = p[2];
                    b[3] = p[3];
                    return b;
                }
            }
        }

        /// <summary>
        /// 获取该端点的端口号。
        /// </summary>
        /// <returns>该端点的端口号。</returns>
        public int GetPortNumber()
        {
            unsafe
            {
                fixed (byte* p = Port)
                {
                    return (int)((p[0] << 8) + p[1]);
                }
            }
        }

        /// <summary>
        /// 获取该端点端口的字节数组表示形式。
        /// </summary>
        /// <returns>该端点端口的字节数组表示形式。</returns>
        public byte[] GetPortBytes()
        {
            unsafe
            {
                fixed (byte* p = Port)
                {
                    byte[] b = new byte[2];
                    b[0] = p[0];
                    b[1] = p[1];
                    return b;
                }
            }
        }

        /// <summary>
        /// 获取该端点的 <see cref="System.Net.IPEndPoint"/> 表示形式。
        /// </summary>
        /// <returns>该端点的 <see cref="System.Net.IPEndPoint"/> 表示形式。</returns>
        public IPEndPoint GetEndPoint()
        {
            unsafe
            {
                byte[] addressBytes = new byte[4];
                fixed (byte* p = Address)
                {
                    addressBytes[0] = p[0];
                    addressBytes[1] = p[1];
                    addressBytes[2] = p[2];
                    addressBytes[3] = p[3];
                }
                return new IPEndPoint(new IPAddress(addressBytes), GetPortNumber());
            }
        }

        /// <summary>
        /// 获取该端点的字节数组表示形式。
        /// </summary>
        /// <returns>一个长度为 6 的 <see cref="System.Byte"/>[]，其中前 4 个元素为地址，后 2 个元素为端口。</returns>
        public byte[] ToByteArray()
        {
            unsafe
            {
                fixed (byte* p = Address)
                {
                    fixed (byte* q = Port)
                    {
                        return new[] { p[0], p[1], p[2], p[3], q[0], q[1] };
                    }
                }
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// 创建一个空的 <see cref="Kei.KEndPoint"/> 实例。该方法应该仅用于初始化 <see cref="Kei.KEndPoint.Empty"/>。
        /// </summary>
        /// <returns>一个空的 <see cref="Kei.KEndPoint"/> 实例。</returns>
        private static KEndPoint CreateEmptyInstance()
        {
            var kep = new KEndPoint();
            kep.SetAddress((byte[])null);
            kep.SetPort(0);
            return kep;
        }

        /// <summary>
        /// 从一个 <see cref="System.Net.IPEndPoint"/> 创建表示其的 <see cref="Kei.KEndPoint"/>。
        /// </summary>
        /// <param name="endPoint">源 <see cref="System.Net.IPEndPoint"/>。</param>
        /// <returns>创建的 <see cref="Kei.KEndPoint"/>。</returns>
        public static KEndPoint FromEndPoint(IPEndPoint endPoint)
        {
            KEndPoint kep = new KEndPoint();
            if (endPoint != null)
            {
                kep.SetAddress(endPoint.Address);
                kep.SetPort(endPoint.Port);
            }
            return kep;
        }

        /// <summary>
        /// 从字节数组创建一个 <see cref="Kei.KEndPoint"/>。
        /// </summary>
        /// <param name="bytes">带有数据的字节数组。长度必须为 6，前 4 个元素为地址，后 2 个元素为端口。</param>
        /// <returns>创建的 <see cref="Kei.KEndPoint"/>。</returns>
        /// <exception cref="System.ArgumentException">bytes 长度不为 6 时发生。</exception>
        public static unsafe KEndPoint FromByteArray(byte[] bytes)
        {
            if (bytes == null || bytes.Length != 6)
            {
                throw new ArgumentException("无效的用于创建 KEndPoint 的字节数组。");
            }
            KEndPoint kep;
            kep.Address[0] = bytes[0];
            kep.Address[1] = bytes[1];
            kep.Address[2] = bytes[2];
            kep.Address[3] = bytes[3];
            kep.Port[0] = bytes[4];
            kep.Port[1] = bytes[5];
            return kep;
        }
        #endregion

        /// <summary>
        /// 判断本 <see cref="Kei.KEndPoint"/> 与另一个 <see cref="Kei.KEndPoint"/> 是否等价。
        /// </summary>
        /// <param name="other">另一个 <see cref="Kei.KEndPoint"/>。</param>
        /// <returns>返回 true 表示等价，返回 false 表示不等价。</returns>
        public unsafe bool Equals(KEndPoint other)
        {
            fixed (byte* p = Address)
            {
                fixed (byte* q = Port)
                {
                    bool b = p[0] == other.Address[0] &&
                        p[1] == other.Address[1] &&
                        p[2] == other.Address[2] &&
                        p[3] == other.Address[3] &&
                        q[0] == other.Port[0] &&
                        q[1] == other.Port[1];
                    return b;
                }
            }
        }

        /// <summary>
        /// 返回该 <see cref="Kei.KEndPoint"/> 的字符串表示形式。
        /// </summary>
        /// <returns>该 <see cref="Kei.KEndPoint"/> 的字符串表示形式。</returns>
        public override string ToString()
        {
            return GetAddressString() + ":" + GetPortNumber().ToString();
        }
    }
}
