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
    /// 表示客户端间通信的单元，即一条消息。
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct KMessage : IEquatable<KMessage>
    {

        // 字节顺序错误
        //private static readonly byte[] MagicBytes = new byte[]
        //{
        //    0x00, 0x75, 0x69, 0x68,
        //    0x61, 0x72, 0x75, 0x00,
        //};

        /// <summary>
        /// 校验码对应的字节数组。
        /// </summary>
        private static readonly byte[] MagicBytes = BitConverter.GetBytes(Magic);

        /// <summary>
        /// 消息头的校验码（0x0075696861727500，"uiharu"）。
        /// </summary>
        public const ulong Magic = 0x0075696861727500;

        /// <summary>
        /// 本消息的校验码。
        /// </summary>
        public ulong MagicNumber;

        /// <summary>
        /// 本消息的消息头。
        /// </summary>
        public KMessageHeader Header;

        /// <summary>
        /// 本消息的附带内容。
        /// </summary>
        public KMessageContent Content;

        /// <summary>
        /// 使用默认参数创建一个 <see cref="Kei.KNetwork.KMessage"/>。
        /// <para>注意，直接通过该方法创建的消息是没有任何意义的。要创建有意义的消息，请使用 <see cref="Kei.KNetwork.MessageFactory"/> 类。</para>
        /// </summary>
        /// <returns></returns>
        internal static KMessage CreateEmptyMessage()
        {
            KMessage message = new KMessage();
            message.MagicNumber = Magic;
            message.Header = KMessageHeader.Create();
            message.Content = KMessageContent.Empty;
            return message;
        }

        /// <summary>
        /// 判断本 <see cref="Kei.KNetwork.KMessage"/> 与另一个 <see cref="Kei.KNetwork.KMessage"/> 是否等价。
        /// </summary>
        /// <param name="other">另一个 <see cref="Kei.KNetwork.KMessage"/>。</param>
        /// <returns>返回 true 表示等价，返回 false 表示不等价。</returns>
        public bool Equals(KMessage other)
        {
            return Header.SourceEndPoint.Equals(other.Header.SourceEndPoint) && Header.MessageID == other.Header.MessageID;
        }

        /// <summary>
        /// 将本 <see cref="Kei.KNetwork.KMessage"/> 编码为字节数组。
        /// </summary>
        /// <returns>一个 <see cref="System.Byte"/>[]，内容为本 <see cref="Kei.KNetwork.KMessage"/> 的所有信息。</returns>
        public byte[] ToByteArray()
        {
            byte[] buf1 = Header.ToByteArray(), buf2 = Content.ToByteArray();
            byte[] ret = new byte[MagicBytes.Length + buf1.Length + buf2.Length];
            Array.Copy(MagicBytes, 0, ret, 0, MagicBytes.Length);
            Array.Copy(buf1, 0, ret, MagicBytes.Length, buf1.Length);
            Array.Copy(buf2, 0, ret, MagicBytes.Length + buf1.Length, buf2.Length);
            return ret;
        }
    }
}
