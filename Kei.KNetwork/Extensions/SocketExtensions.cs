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
using System.Net;
using System.Net.Sockets;

namespace Kei.KNetwork
{
    /// <summary>
    /// <see cref="System.Net.Sockets.Socket"/> 相关的扩展类。
    /// </summary>
    public static class SocketExtensions
    {

        /// <summary>
        /// 向指定的 <see cref="System.Net.Sockets.Socket"/> 中写入一条消息。
        /// </summary>
        /// <param name="socket">要进行写入的 <see cref="System.Net.Sockets.Socket"/>。</param>
        /// <param name="message">要写入的消息。</param>
        /// <returns>一个 <see cref="Kei.KNetwork.MessageIOErrorCode"/> 值，表示写入的结果。</returns>
        public static MessageIOErrorCode WriteMessage(this Socket socket, KMessage message)
        {
            var data = message.ToByteArray();
            try
            {
                socket.Send(data);
                return MessageIOErrorCode.NoError;
            }
            catch (Exception)
            {
                return MessageIOErrorCode.WriteFailed;
            }
        }

        /// <summary>
        /// 从指定的 <see cref="System.Net.Sockets.Socket"/> 中读取一个 <see cref="System.Int32"/>。
        /// </summary>
        /// <param name="socket">要读取的 <see cref="System.Net.Sockets.Socket"/>。</param>
        /// <returns>读出的 <see cref="System.Int32"/>。</returns>
        /// <exception cref="System.ArgumentNullException">socket 为 null 时发生。</exception>
        public static int ReadInt32(this Socket socket)
        {
            if (socket == null)
            {
                throw new ArgumentNullException("socket");
            }
            var buffer = new byte[Marshal.SizeOf(typeof(int))];
            socket.Receive(buffer);
            return BitConverter.ToInt32(buffer, 0);
        }

    }
}
