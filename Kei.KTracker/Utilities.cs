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
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace Kei.KTracker
{
    /// <summary>
    /// 实用函数类。
    /// </summary>
    internal static class Utilities
    {

        /// <summary>
        /// 将 HTTP GET 请求参数分解，并返回结果。
        /// </summary>
        /// <param name="parameters">带有参数和值的字符串。例如：param1=value1&param2=&param3=value3。</param>
        /// <returns>一个 <see cref="System.Collections.Spcialized.StringDictionary"/>，表示分解的结果。</returns>
        public static StringDictionary DecomposeParameterString(string parameters)
        {
            StringDictionary ret = new StringDictionary();
            var argsKVPairs = parameters.Split(true, '&');
            for (var i = 0; i < argsKVPairs.Length; i++)
            {
                var args = argsKVPairs[i].Split(true, '=');
                ret.Add(args[0], args.Length > 1 ? args[1] : string.Empty);
            }
            return ret;
        }

        /// <summary>
        /// 返回一个 0 到 15 之间的数字的十六进制字符（大写）。
        /// </summary>
        /// <param name="v">一个 0 到 15 之间的数字。</param>
        /// <returns>0 到 9，或 A 到 F，表示该数字对应的十六进制字符。</returns>
        private static char GetHex(byte v)
        {
            if (v > 0xf)
            {
                v %= 0x10;
            }
            if (v < 10)
            {
                return (char)((byte)'0' + v);
            }
            else
            {
                return (char)(((byte)'A' - 10) + v);
            }
        }

        /// <summary>
        /// 返回大写字母。
        /// </summary>
        /// <param name="c">要测试的字符。</param>
        /// <returns>如果该字符是小写字母，则返回对应的大写字母；否则返回其自身。</returns>
        private static char EnsureUpper(char c)
        {
            if (c >= 'a' && c <= 'z')
            {
                return (char)((byte)c - 32);
            }
            return c;
        }

        /// <summary>
        /// 反转义 BitTorrent 客户端所使用的部分 URI 编码字符串。
        /// </summary>
        /// <param name="enc">BitTorrent 客户端给出的部分 URI 编码字符串。</param>
        /// <returns>一个十六进制字符串（所有字符由 0 到 9 或 A 到 F 组成），表示原字符串直接反转义得到的字符串每个字符的 ASCII 码值经十六进制编码后的结果。</returns>
        /// <exception cref="ArgumentException">输入的字符串格式错误时发生。</exception>
        public static string UnescapePartialUriEncodedString(string enc)
        {
            // 这里假设输入是完全正确的
            if (string.IsNullOrEmpty(enc))
            {
                return string.Empty;
            }
            // 20位的 SHA-1，十六进制编码，需要40个字符
            var sb = new StringBuilder(40);
            int i = 0;
            while (i < enc.Length)
            {
                if (enc[i] == '%')
                {
                    // 转义过的
                    if (i + 2 >= enc.Length)
                    {
                        throw new ArgumentException("部分转义格式错误。");
                    }
                    i++;
                    sb.Append(EnsureUpper(enc[i++]));
                    sb.Append(EnsureUpper(enc[i++]));
                }
                else
                {
                    // 手工转义
                    byte v = (byte)enc[i];
                    char c1, c2;
                    c1 = GetHex((byte)(v >> 4));
                    c2 = GetHex((byte)((v << 4) >> 4));
                    sb.Append(c1);
                    sb.Append(c2);
                    i++;
                }
            }
            return sb.ToString();
        }

    }
}
