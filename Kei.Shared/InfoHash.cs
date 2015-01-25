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

namespace Kei
{
    /// <summary>
    /// 对 BitTorrent 客户端使用的 InfoHash 的封装。
    /// </summary>
    public class InfoHash : IEquatable<InfoHash>
    {

        /// <summary>
        /// 空的 InfoHash，数据全为零。
        /// </summary>
        public static readonly InfoHash Empty = new InfoHash();

        /// <summary>
        /// 此 InfoHash 对应的字节数据。
        /// </summary>
        private byte[] _dataBytes;

        /// <summary>
        /// 生成的十六进制字符串的缓存。
        /// </summary>
        private string _hexStringCache = null;

        /// <summary>
        /// 创建一个空的 InfoHash。
        /// </summary>
        private InfoHash()
        {
            _dataBytes = new byte[20];
        }

        /// <summary>
        /// 从字节数组构造一个 <see cref="Kei.InfoHash"/>。
        /// </summary>
        /// <param name="dataBytes">包含 InfoHash 数据的字节数组。</param>
        /// <returns>构造的 <see cref="Kei.InfoHash"/>。</returns>
        /// <exception cref="System.ArgumentNullException">dataBytes 为 null 时发生。</exception>
        /// <exception cref="System.ArgumentException">dataBytes 长度不为 20 时发生。</exception>
        public static InfoHash FromByteArray(byte[] dataBytes)
        {
            if (dataBytes == null)
            {
                throw new ArgumentNullException("dataBytes");
            }
            if (dataBytes.Length != 20)
            {
                throw new ArgumentException("长度必须为 20。");
            }
            var infoHash = new InfoHash();
            Buffer.BlockCopy(dataBytes, 0, infoHash._dataBytes, 0, 20);
            return infoHash;
        }

        /// <summary>
        /// 从一个十六进制字符串构造一个 <see cref="Kei.InfoHash"/>。
        /// </summary>
        /// <param name="dataString">包含数据的十六进制字符串。</param>
        /// <returns>构造的 <see cref="Kei.InfoHash"/>。</returns>
        /// <exception cref="System.ArgumentNullException">dataString 为 null 时发生。</exception>
        /// <exception cref="System.ArgumentException">dataString 长度不为 40 时发生。</exception>
        /// <exception cref="System.ArgumentException">dataString 格式错误时发生。</exception>
        /// <exception cref="System.ArgumentException">dataString 不是一个十六进制字符串时发生。</exception>
        public static InfoHash FromHexString(string dataString)
        {
            if (dataString == null)
            {
                throw new ArgumentNullException("dataBytes");
            }
            if (dataString.Length != 40)
            {
                throw new ArgumentException("长度必须为 40。");
            }
            byte[] buf = new byte[20];
            for (var i = 0; i < 20; i++)
            {
                if (!(Uri.IsHexDigit(dataString[i * 2]) && Uri.IsHexDigit(dataString[i * 2 + 1])))
                {
                    throw new ArgumentException("dataString 格式错误。");
                }
                buf[i] = (byte)((Uri.FromHex(dataString[i * 2]) << 4) + Uri.FromHex(dataString[i * 2 + 1]));
            }
            var infoHash = new InfoHash();
            Buffer.BlockCopy(buf, 0, infoHash._dataBytes, 0, 20);
            return infoHash;
        }

        /// <summary>
        /// 返回该 <see cref="Kei.InfoHash"/> 的十六进制字符串表达形式。其中，字母全部为大写。
        /// </summary>
        /// <returns>一个表示该 <see cref="Kei.InfoHash"/> 的十六进制字符串。</returns>
        public string ToHexString()
        {
            if (_hexStringCache == null)
            {
                var sb = new StringBuilder(40);
                byte b1, b2;
                char c1, c2;
                for (var i = 0; i < _dataBytes.Length; i++)
                {
                    b1 = (byte)((_dataBytes[i] >> 4) & 0xf);
                    b2 = (byte)(_dataBytes[i] & 0xf);
                    c1 = b1 > 9 ? (char)(b1 + (byte)'A' - 10) : (char)(b1 + (byte)'0');
                    c2 = b2 > 9 ? (char)(b2 + (byte)'A' - 10) : (char)(b2 + (byte)'0');
                    sb.Append(c1);
                    sb.Append(c2);
                }
                _hexStringCache = sb.ToString();
            }
            return _hexStringCache;
        }

        /// <summary>
        /// 返回包含该 <see cref="Kei.InfoHash"/> 信息的字节数组的副本。
        /// </summary>
        /// <returns>一个 <see cref="System.Byte"/>[]，为包含该 <see cref="Kei.InfoHash"/> 信息的字节数组的副本。</returns>
        public byte[] ToByteArray()
        {
            return (byte[])_dataBytes.Clone();
        }

        /// <summary>
        /// 返回该 <see cref="Kei.InfoHash"/> 的字符串表示形式。
        /// </summary>
        /// <returns>该 <see cref="Kei.InfoHash"/> 的字符串表示形式。</returns>
        public override string ToString()
        {
            return ToHexString();
        }

        /// <summary>
        /// 返回该 <see cref="Kei.InfoHash"/> 的散列值。
        /// </summary>
        /// <returns>该 <see cref="Kei.InfoHash"/> 的散列值。</returns>
        public override int GetHashCode()
        {
            return ToHexString().GetHashCode();
        }

        /// <summary>
        /// 判断该 <see cref="Kei.InfoHash"/> 实例与一个 <see cref="System.Object"/> 等价。
        /// </summary>
        /// <param name="obj">要比较的 <see cref="System.Object"/>。</param>
        /// <returns>返回 true 表示等价，返回 false 表示不等价。</returns>
        public override bool Equals(object obj)
        {
            if (obj is InfoHash)
            {
                return Equals((InfoHash)obj);
            }
            else
            {
                return base.Equals(obj);
            }
        }

        /// <summary>
        /// 判断该 <see cref="Kei.InfoHash"/> 实例与另一个 <see cref="Kei.InfoHash"/> 等价。
        /// </summary>
        /// <param name="other">另一个 <see cref="Kei.InfoHash"/>。</param>
        /// <returns>返回 true 表示等价，返回 false 表示不等价。</returns>
        public bool Equals(InfoHash other)
        {
            if (other == null)
            {
                // 假设某些变态使用了 ((InfoHash)null) == null 来玩
                return this == null;
            }
            else
            {
                return GetHashCode() == other.GetHashCode();
            }
        }

    }
}
