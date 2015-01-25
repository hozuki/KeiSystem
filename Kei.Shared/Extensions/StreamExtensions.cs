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
using System.IO;

namespace Kei
{
    /// <summary>
    /// <see cref="System.IO.Stream"/> 的扩展类。
    /// </summary>
    public static class StreamExtensions
    {

        /// <summary>
        /// 向 <see cref="System.IO.Stream"/> 中写入一段 UTF-8 编码的文本。
        /// </summary>
        /// <param name="stream">要写入的 <see cref="System.IO.Stream"/>。</param>
        /// <param name="text">要写入的文本。</param>
        public static void Write(this Stream stream, string text)
        {
            Write(stream, text, Encoding.UTF8);
        }

        /// <summary>
        /// 向 <see cref="System.IO.Stream"/> 中写入一段指定编码的文本。
        /// </summary>
        /// <param name="stream">要写入的 <see cref="System.IO.Stream"/>。</param>
        /// <param name="text">要写入的文本。</param>
        /// <param name="encoding">要使用的文本编码。</param>
        public static void Write(this Stream stream, string text, Encoding encoding)
        {
            var bytes = encoding.GetBytes(text);
            stream.Write(bytes, 0, bytes.Length);
        }

        /// <summary>
        /// 向 <see cref="System.IO.Stream"/> 中写入一段 UTF-8 编码的文本并换行。
        /// </summary>
        /// <param name="stream">要写入的 <see cref="System.IO.Stream"/>。</param>
        /// <param name="text">要写入的文本。</param>
        /// <param name="newLine">要使用的换行符。默认为 CR-LF。</param>
        public static void WriteLine(this Stream stream, string text, string newLine = "\r\n")
        {
            WriteLine(stream, text, Encoding.UTF8, newLine);
        }

        /// <summary>
        /// 向 <see cref="System.IO.Stream"/> 中写入一段指定编码的文本并换行。
        /// </summary>
        /// <param name="stream">要写入的 <see cref="System.IO.Stream"/>。</param>
        /// <param name="text">要写入的文本。</param>
        /// <param name="encoding">要使用的文本编码。</param>
        /// <param name="newLine">要使用的换行符。默认为 CR-LF。</param>
        public static void WriteLine(this Stream stream, string text, Encoding encoding, string newLine = "\r\n")
        {
            var bytes = encoding.GetBytes(text + newLine);
            stream.Write(bytes, 0, bytes.Length);
        }

        /// <summary>
        /// 向 <see cref="System.IO.Stream"/> 中写入一个 UTF-8 编码的空行。
        /// </summary>
        /// <param name="stream">要写入的 <see cref="System.IO.Stream"/>。</param>
        public static void WriteLine(this Stream stream)
        {
            WriteLine(stream, string.Empty);
        }

    }
}
