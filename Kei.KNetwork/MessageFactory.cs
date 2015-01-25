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

namespace Kei.KNetwork
{

    /// <summary>
    /// <see cref="Kei.KNetwork.KMessage"/> 创建与解读的工厂类。
    /// </summary>
    public static class MessageFactory
    {

        /// <summary>
        /// 获取本机所用的消息 ID。
        /// </summary>
        /// <returns>一个 64 位无符号整数，表示生成的消息 ID。</returns>
        private static ulong GetMessageHash()
        {
            unchecked
            {
                return (ulong)DateTime.Now.ToBinary();
            }
        }

        /// <summary>
        /// 生成一个 ReportAlive 消息。
        /// </summary>
        /// <param name="localEP">消息源的端点。</param>
        /// <returns>创建的 ReportAlive 消息。</returns>
        public static KMessage ReportAlive(KEndPoint localEP)
        {
            var message = KMessage.CreateEmptyMessage();
            message.Header.Code = KMessageCode.ReportAlive;
            message.Header.SourceEndPoint = localEP;
            message.Header.MessageID = GetMessageHash();
            return message;
        }

        /// <summary>
        /// 生成一个 ClientEnterNetwork 消息。
        /// </summary>
        /// <param name="localEP">消息源的端点。</param>
        /// <returns>创建的 ClientEnterNetwork 消息。</returns>
        public static KMessage ClientEnterNetwork(KEndPoint localEP)
        {
            var message = KMessage.CreateEmptyMessage();
            message.Header.Code = KMessageCode.ClientEnterNetwork;
            message.Header.SourceEndPoint = localEP;
            message.Header.MessageID = GetMessageHash();
            // 生成一个表示未处理过的进入网络消息
            message.Content.DataLength = 4;
            message.Content.Data = new byte[4];
            return message;
        }

        /// <summary>
        /// 生成一个 ClientExitNetwork 消息。
        /// </summary>
        /// <param name="localEP">消息源的端点。</param>
        /// <returns>创建的 ClientExitNetwork 消息。</returns>
        public static KMessage ClientExitNetwork(KEndPoint localEP)
        {
            var message = KMessage.CreateEmptyMessage();
            message.Header.Code = KMessageCode.ClientExitNetwork;
            message.Header.SourceEndPoint = localEP;
            message.Header.MessageID = GetMessageHash();
            return message;
        }

        /// <summary>
        /// 生成一个 PeerEnterNetwork 消息。
        /// </summary>
        /// <param name="localEP">消息源的端点。</param>
        /// <param name="infoHash">用户所持有的 InfoHash。</param>
        /// <param name="bitTorrentClientPort">用户所监听的端口（即 BitTorrent 客户端的监听端口）。</param>
        /// <returns>创建的 PeerEnterNetwork 消息。</returns>
        public static KMessage PeerEnterNetwork(KEndPoint localEP, InfoHash infoHash, int bitTorrentClientPort)
        {
            var message = KMessage.CreateEmptyMessage();
            message.Header.Code = KMessageCode.PeerEnterNetwork;
            message.Header.SourceEndPoint = localEP;
            message.Header.MessageID = GetMessageHash();

            var infoHashData = infoHash.ToByteArray();
            var data = new byte[infoHashData.Length + 4];
            var buffer = BitConverter.GetBytes(bitTorrentClientPort);
            Array.Copy(infoHashData, 0, data, 0, infoHashData.Length);
            Array.Copy(buffer, 0, data, infoHashData.Length, buffer.Length);
            message.Content = KMessageContent.FromByteArray(data);
            return message;
        }

        /// <summary>
        /// 生成一个 PeerExitNetwork 消息。
        /// </summary>
        /// <param name="localEP">消息源的端点。</param>
        /// <param name="infoHash">用户所持有的 InfoHash。</param>
        /// <param name="bitTorrentClientPort">用户所监听的端口（即 BitTorrent 客户端的监听端口）。</param>
        /// <returns>创建的 PeerExitNetwork 消息。</returns>
        public static KMessage PeerExitNetwork(KEndPoint localEP, InfoHash infoHash, int bitTorrentClientPort)
        {
            var message = KMessage.CreateEmptyMessage();
            message.Header.Code = KMessageCode.PeerExitNetwork;
            message.Header.SourceEndPoint = localEP;
            message.Header.MessageID = GetMessageHash();

            var infoHashData = infoHash.ToByteArray();
            var data = new byte[infoHashData.Length + 4];
            var buffer = BitConverter.GetBytes(bitTorrentClientPort);
            Array.Copy(infoHashData, 0, data, 0, infoHashData.Length);
            Array.Copy(buffer, 0, data, infoHashData.Length, buffer.Length);
            message.Content = KMessageContent.FromByteArray(data);
            return message;
        }

        /// <summary>
        /// 生成一个 GotPeer 消息。
        /// </summary>
        /// <param name="localEP">消息源的端点。</param>
        /// <param name="infoHash">用户所持有的 InfoHash。</param>
        /// <param name="bitTorrentClientPort">用户所监听的端口（即 BitTorrent 客户端的监听端口）。</param>
        /// <returns>创建的 GotPeer 消息。</returns>
        public static KMessage GotPeer(KEndPoint localEP, InfoHash infoHash, int bitTorrentClientPort)
        {
            var message = KMessage.CreateEmptyMessage();
            message.Header.Code = KMessageCode.GotPeer;
            message.Header.SourceEndPoint = localEP;
            message.Header.MessageID = GetMessageHash();

            var infoHashData = infoHash.ToByteArray();
            var data = new byte[infoHashData.Length + 4];
            var buffer = BitConverter.GetBytes(bitTorrentClientPort);
            Array.Copy(infoHashData, 0, data, 0, infoHashData.Length);
            Array.Copy(buffer, 0, data, infoHashData.Length, buffer.Length);
            message.Content = KMessageContent.FromByteArray(data);
            return message;
        }

        /// <summary>
        /// 解读一条用户连接消息。
        /// </summary>
        /// <param name="message">待解读的消息。</param>
        /// <param name="infoHash">解读出的 InfoHash。</param>
        /// <param name="bitTorrentClientPort">解读出的用户监听端口。</param>
        /// <exception cref="System.ArgumentException">解读的不是用户连接消息时发生。</exception>
        public static void GetPeerMessageContent(KMessage message, out InfoHash infoHash, out int bitTorrentClientPort)
        {
            if (message.Content.Data.Length != 24)
            {
                throw new ArgumentException("要解读的不是一条用户连接消息。");
            }
            infoHash = InfoHash.FromByteArray(message.Content.Data.Take(20).ToArray());
            bitTorrentClientPort = BitConverter.ToInt32(message.Content.Data, 20);
        }

    }
}
