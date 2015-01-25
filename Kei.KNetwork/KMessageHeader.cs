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
using System.Runtime.InteropServices;

namespace Kei.KNetwork
{
    /// <summary>
    /// 表示一个 <see cref="Kei.KNetwork.KMessage"/> 的消息头。
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct KMessageHeader
    {

        /// <summary>
        /// 消息头的长度。
        /// <para>0.1 版本为 24 字节。</para>
        /// </summary>
        public uint HeaderLength;

        /// <summary>
        /// 一个 2 字节无符号整数，表示消息头的版本。格式为 0xaabb，表示 aa.bb 版本。
        /// </summary>
        public ushort HeaderVersion;

        /// <summary>
        /// 本消息的消息 ID。
        /// </summary>
        public ulong MessageID;

        /// <summary>
        /// 本消息的消息代码。
        /// </summary>
        public KMessageCode Code;

        /// <summary>
        /// 本消息的信息源的端点。
        /// </summary>
        public KEndPoint SourceEndPoint;

        /// <summary>
        /// 创建一个新的 <see cref="Kei.KNetwork.KMessageHeader"/>。
        /// </summary>
        /// <returns>一个标准的 <see cref="Kei.KNetwork.KMessageHeader"/>。</returns>
        public static KMessageHeader Create()
        {
            KMessageHeader header = new KMessageHeader();
            header.HeaderLength = (uint)Marshal.SizeOf(typeof(KMessageHeader));
            header.HeaderVersion = 0x0001;
            return header;
        }

        /// <summary>
        /// 从一个字节数组中读取 <see cref="Kei.KNetwork.KMessageHeader"/> 所需的信息，并创建相应的 <see cref="Kei.KNetwork.KMessageHeader"/>。
        /// </summary>
        /// <param name="data">包含所需信息的字节数组。注意长度必须等于某个版本的 <see cref="Kei.KNetwork.KMessageHeader"/> 的长度。</param>
        /// <returns>创建的 <see cref="Kei.KNetwork.KMessageHeader"/>。</returns>
        public static KMessageHeader FromByteArray(byte[] data)
        {
            KMessageHeader header;
            // +4
            header.HeaderLength = BitConverter.ToUInt32(data, 0);
            // +2
            header.HeaderVersion = BitConverter.ToUInt16(data, 4);
            // +8
            header.MessageID = BitConverter.ToUInt64(data, 6);
            // +4
            header.Code = (KMessageCode)BitConverter.ToInt32(data, 14);
            // +?
            header.SourceEndPoint = KEndPoint.FromByteArray(data.Skip(18).ToArray());

            return header;
        }

        /// <summary>
        /// 将本 <see cref="Kei.KNetwork.KMessageHeader"/> 的所有信息编码进一个字节数组。
        /// </summary>
        /// <returns>含有本 <see cref="Kei.KNetwork.KMessageHeader"/> 所有信息的字节数组。</returns>
        public byte[] ToByteArray()
        {
            var ret = new byte[Marshal.SizeOf(typeof(KMessageHeader))];
            int n = 0;
            byte[] buffer;

            buffer = BitConverter.GetBytes(HeaderLength);
            Array.Copy(buffer, 0, ret, n, buffer.Length);
            n += buffer.Length;
            buffer = BitConverter.GetBytes(HeaderVersion);
            Array.Copy(buffer, 0, ret, n, buffer.Length);
            n += buffer.Length;
            buffer = BitConverter.GetBytes(MessageID);
            Array.Copy(buffer, 0, ret, n, buffer.Length);
            n += buffer.Length;
            buffer = BitConverter.GetBytes((int)Code);
            Array.Copy(buffer, 0, ret, n, buffer.Length);
            n += buffer.Length;
            buffer = SourceEndPoint.ToByteArray();
            Array.Copy(buffer, 0, ret, n, buffer.Length);
            n += buffer.Length;

            return ret;
        }

    }
}
