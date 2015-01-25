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
    /// 表示一个 <see cref="Kei.KNetwork.KMessage"/> 的附带内容。
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct KMessageContent
    {

        /// <summary>
        /// 空内容，数据长度为零，数据为一个零长度的字节数组。
        /// </summary>
        public static readonly KMessageContent Empty = CreateEmptyInstance();

        /// <summary>
        /// 数据长度。
        /// </summary>
        public uint DataLength;

        /// <summary>
        /// 数据。
        /// </summary>
        public byte[] Data;

        #region Constructors
        /// <summary>
        /// 创建一个新的空实例。该方法应该仅用于初始化 <see cref="Kei.KNetwork.KMessageContent.Empty"/>。
        /// </summary>
        /// <returns></returns>
        private static KMessageContent CreateEmptyInstance()
        {
            return new KMessageContent()
            {
                DataLength = 0,
                Data = new byte[0]
            };
        }

        /// <summary>
        /// 由给定的数据构造一个包装的 <see cref="Kei.KNetwork.KMessageContent"/>。
        /// </summary>
        /// <param name="data">要装入 <see cref="Kei.KNetwork.KMessageContent"/> 的数据。如果为 null，表示创建空数据。</param>
        /// <returns>一个 <see cref="Kei.KNetwork.KMessageContent"/>，数据长度为 data 的长度，数据为 data。</returns>
        public static KMessageContent FromByteArray(byte[] data)
        {
            KMessageContent kmc;
            if (data != null)
            {
                kmc.DataLength = (uint)data.Length;
                kmc.Data = (byte[])data.Clone();
            }
            else
            {
                kmc = Empty;
            }
            return kmc;
        }
        #endregion

        /// <summary>
        /// 将本 <see cref="Kei.KNetwork.KMessageContent"/> 编码到字节数组中。
        /// </summary>
        /// <returns>一个包含本 <see cref="Kei.KNetwork.KMessageContent"/> 所有信息的字节数组。</returns>
        public byte[] ToByteArray()
        {
            var ret = new byte[4 + Data.Length];
            byte[] buffer;

            buffer = BitConverter.GetBytes(DataLength);
            Array.Copy(buffer, 0, ret, 0, 4);
            if (DataLength > 0)
            {
                Array.Copy(Data, 0, ret, 4, DataLength);
            }

            return ret;
        }

    }
}
