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
using System.IO;
using System.Runtime.InteropServices;

namespace Kei.KNetwork
{
    /// <summary>
    /// <see cref="System.IO.Stream"/> 相关的扩展类。
    /// </summary>
    public static class StreamExtensions
    {

        //public static bool TryRead(this Stream stream, byte[] buffer, int offset, int count, out int bytesRead)
        //{
        //    bool successful = true;
        //    int read = 0;
        //    try
        //    {
        //        read = stream.Read(buffer, offset, count);
        //    }
        //    catch (Exception)
        //    {
        //        successful = false;
        //    }
        //    bytesRead = read;
        //    return successful;
        //}

        //public static bool TryWrite(this Stream stream, byte[] buffer, int offset, int count)
        //{
        //    bool successful = true;
        //    try
        //    {
        //        stream.Write(buffer, offset, count);
        //    }
        //    catch (Exception)
        //    {
        //        successful = false;
        //    }
        //    return successful;
        //}

        /// <summary>
        /// 从指定的 <see cref="System.IO.Stream"/> 中读取一条消息。
        /// </summary>
        /// <param name="stream">要读取的 <see cref="System.IO.Stream"/>。</param>
        /// <param name="message">读取出的消息。如果返回值不是 <see cref="Kei.KNetwork.MessageIOErrorCode.NoError"/>，则该值无效。</param>
        /// <returns>一个 <see cref="Kei.KNetwork.MessageIOCode"/> 值，表示读取的结果。</returns>
        public static MessageIOErrorCode ReadMessage(this Stream stream, out KMessage message)
        {
            byte[] buffer;
            KMessage km = new KMessage();

            // Check magic
            try
            {
                buffer = new byte[8];
                stream.Read(buffer, 0, 8);
                km.MagicNumber = BitConverter.ToUInt64(buffer, 0);
                if (km.MagicNumber != KMessage.Magic)
                {
                    message = km;
                    return MessageIOErrorCode.WrongMagicNumber;
                }
            }
            catch (Exception)
            {
                message = km;
                return MessageIOErrorCode.ReadMagicNumberFailed;
            }

            // Read header
            try
            {
                var headerSize = Marshal.SizeOf(typeof(KMessageHeader));
                buffer = new byte[headerSize];
                stream.Read(buffer, 0, headerSize);
                km.Header = KMessageHeader.FromByteArray(buffer);
            }
            catch (Exception)
            {
                message = km;
                return MessageIOErrorCode.ReadHeaderFailed;
            }

            // Read content
            try
            {
                KMessageContent content = new KMessageContent();
                buffer = new byte[4];
                stream.Read(buffer, 0, 4);
                content.DataLength = BitConverter.ToUInt32(buffer, 0);
                content.Data = new byte[content.DataLength];
                if (content.DataLength > 0)
                {
                    stream.Read(content.Data, 0, (int)content.DataLength);
                }
                km.Content = content;
            }
            catch (Exception)
            {
                message = km;
                return MessageIOErrorCode.ReadContentFailed;
            }

            message = km;
            return MessageIOErrorCode.NoError;
        }

        /// <summary>
        /// 向指定的 <see cref="System.IO.Stream"/> 中写入一条消息。
        /// </summary>
        /// <param name="stream">要进行写入的 <see cref="System.IO.Stream"/>。</param>
        /// <param name="message">要写入的消息。</param>
        /// <returns>一个 <see cref="Kei.KNetwork.MessageIOErrorCode"/> 值，表示写入的结果。</returns>
        public static MessageIOErrorCode WriteMessage(this Stream stream, KMessage message)
        {
            var data = message.ToByteArray();
            try
            {
                stream.Write(data, 0, data.Length);
                stream.Flush();
                return MessageIOErrorCode.NoError;
            }
            catch (Exception)
            {
                return MessageIOErrorCode.WriteFailed;
            }
        }

        /// <summary>
        /// 从指定的 <see cref="System.IO.Stream"/> 中读取一个 <see cref="System.Int16"/>。
        /// </summary>
        /// <param name="stream">要读取的 <see cref="System.IO.Stream"/>。</param>
        /// <returns>读出的 <see cref="System.Int16"/>。</returns>
        public static short ReadInt16(this Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }
            var buffer = new byte[Marshal.SizeOf(typeof(short))];
            stream.Read(buffer, 0, buffer.Length);
            return BitConverter.ToInt16(buffer, 0);
        }

        /// <summary>
        /// 从指定的 <see cref="System.IO.Stream"/> 中读取一个 <see cref="System.UInt16"/>。
        /// </summary>
        /// <param name="stream">要读取的 <see cref="System.IO.Stream"/>。</param>
        /// <returns>读出的 <see cref="System.UInt16"/>。</returns>
        public static ushort ReadUInt16(this Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }
            var buffer = new byte[Marshal.SizeOf(typeof(ushort))];
            stream.Read(buffer, 0, buffer.Length);
            return BitConverter.ToUInt16(buffer, 0);
        }

        /// <summary>
        /// 从指定的 <see cref="System.IO.Stream"/> 中读取一个 <see cref="System.Int32"/>。
        /// </summary>
        /// <param name="stream">要读取的 <see cref="System.IO.Stream"/>。</param>
        /// <returns>读出的 <see cref="System.Int32"/>。</returns>
        public static int ReadInt32(this Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }
            var buffer = new byte[Marshal.SizeOf(typeof(int))];
            stream.Read(buffer, 0, buffer.Length);
            return BitConverter.ToInt32(buffer, 0);
        }

        /// <summary>
        /// 从指定的 <see cref="System.IO.Stream"/> 中读取一个 <see cref="System.UInt32"/>。
        /// </summary>
        /// <param name="stream">要读取的 <see cref="System.IO.Stream"/>。</param>
        /// <returns>读出的 <see cref="System.UInt32"/>。</returns>
        public static uint ReadUInt32(this Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }
            var buffer = new byte[Marshal.SizeOf(typeof(uint))];
            stream.Read(buffer, 0, buffer.Length);
            return BitConverter.ToUInt32(buffer, 0);
        }

        /// <summary>
        /// 从指定的 <see cref="System.IO.Stream"/> 中读取一个 <see cref="System.Int64"/>。
        /// </summary>
        /// <param name="stream">要读取的 <see cref="System.IO.Stream"/>。</param>
        /// <returns>读出的 <see cref="System.Int64"/>。</returns>
        public static long ReadInt64(this Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }
            var buffer = new byte[Marshal.SizeOf(typeof(long))];
            stream.Read(buffer, 0, buffer.Length);
            return BitConverter.ToInt64(buffer, 0);
        }

        /// <summary>
        /// 从指定的 <see cref="System.IO.Stream"/> 中读取一个 <see cref="System.UInt64"/>。
        /// </summary>
        /// <param name="stream">要读取的 <see cref="System.IO.Stream"/>。</param>
        /// <returns>读出的 <see cref="System.UInt64"/>。</returns>
        public static ulong ReadUInt64(this Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }
            var buffer = new byte[Marshal.SizeOf(typeof(ulong))];
            stream.Read(buffer, 0, buffer.Length);
            return BitConverter.ToUInt64(buffer, 0);
        }

        /// <summary>
        /// 向指定的 <see cref="System.IO.Stream"/> 中写入一个 <see cref="System.Int16"/>。
        /// </summary>
        /// <param name="stream">要进行写入的 <see cref="System.IO.Stream"/>。</param>
        /// <param name="value">要写入的 <see cref="System.Int16"/>。</param>
        public static void WriteInt16(this Stream stream, short value)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }
            var buffer = BitConverter.GetBytes(value);
            stream.Write(buffer, 0, buffer.Length);
        }

        /// <summary>
        /// 向指定的 <see cref="System.IO.Stream"/> 中写入一个 <see cref="System.UInt16"/>。
        /// </summary>
        /// <param name="stream">要进行写入的 <see cref="System.IO.Stream"/>。</param>
        /// <param name="value">要写入的 <see cref="System.UInt16"/>。</param>
        public static void WriteUInt16(this Stream stream, ushort value)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }
            var buffer = BitConverter.GetBytes(value);
            stream.Write(buffer, 0, buffer.Length);
        }

        /// <summary>
        /// 向指定的 <see cref="System.IO.Stream"/> 中写入一个 <see cref="System.Int32"/>。
        /// </summary>
        /// <param name="stream">要进行写入的 <see cref="System.IO.Stream"/>。</param>
        /// <param name="value">要写入的 <see cref="System.Int32"/>。</param>
        public static void WriteInt32(this Stream stream, int value)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }
            var buffer = BitConverter.GetBytes(value);
            stream.Write(buffer, 0, buffer.Length);
        }

        /// <summary>
        /// 向指定的 <see cref="System.IO.Stream"/> 中写入一个 <see cref="System.UInt32"/>。
        /// </summary>
        /// <param name="stream">要进行写入的 <see cref="System.IO.Stream"/>。</param>
        /// <param name="value">要写入的 <see cref="System.UInt32"/>。</param>
        public static void WriteUInt32(this Stream stream, uint value)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }
            var buffer = BitConverter.GetBytes(value);
            stream.Write(buffer, 0, buffer.Length);
        }

        /// <summary>
        /// 向指定的 <see cref="System.IO.Stream"/> 中写入一个 <see cref="System.Int64"/>。
        /// </summary>
        /// <param name="stream">要进行写入的 <see cref="System.IO.Stream"/>。</param>
        /// <param name="value">要写入的 <see cref="System.Int64"/>。</param>
        public static void WriteInt64(this Stream stream, long value)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }
            var buffer = BitConverter.GetBytes(value);
            stream.Write(buffer, 0, buffer.Length);
        }

        /// <summary>
        /// 向指定的 <see cref="System.IO.Stream"/> 中写入一个 <see cref="System.UInt64"/>。
        /// </summary>
        /// <param name="stream">要进行写入的 <see cref="System.IO.Stream"/>。</param>
        /// <param name="value">要写入的 <see cref="System.UInt64"/>。</param>
        public static void WriteUInt64(this Stream stream, ulong value)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }
            var buffer = BitConverter.GetBytes(value);
            stream.Write(buffer, 0, buffer.Length);
        }

    }
}
