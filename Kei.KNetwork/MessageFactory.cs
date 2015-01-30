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
using MonoTorrent.BEncoding;

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
        /// <param name="realPort">非零表示这是接入点要连接接入点，该端口是本机正在监听的端口；零表示只是普通用户连接接入点。</param>
        /// <returns>创建的 ClientEnterNetwork 消息。</returns>
        public static KMessage ClientEnterNetwork(KEndPoint localEP, ushort realPort)
        {
            var message = KMessage.CreateEmptyMessage();
            message.Header.Code = KMessageCode.ClientEnterNetwork;
            message.Header.SourceEndPoint = localEP;
            message.Header.MessageID = GetMessageHash();

            BEncodedDictionary data = new BEncodedDictionary();
            data.Add("message handled", 0);
            data.Add("real port", realPort);
            message.Content = KMessageContent.FromByteArray(data.Encode());
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
        public static KMessage PeerEnterNetwork(KEndPoint localEP, InfoHash infoHash, ushort bitTorrentClientPort)
        {
            var message = KMessage.CreateEmptyMessage();
            message.Header.Code = KMessageCode.PeerEnterNetwork;
            message.Header.SourceEndPoint = localEP;
            message.Header.MessageID = GetMessageHash();

            BEncodedDictionary data = new BEncodedDictionary();
            data.Add("infohash", infoHash.ToByteArray());
            data.Add("bt client port", bitTorrentClientPort);
            message.Content = KMessageContent.FromByteArray(data.Encode());
            return message;
        }

        /// <summary>
        /// 生成一个 PeerExitNetwork 消息。
        /// </summary>
        /// <param name="localEP">消息源的端点。</param>
        /// <param name="infoHash">用户所持有的 InfoHash。</param>
        /// <param name="bitTorrentClientPort">用户所监听的端口（即 BitTorrent 客户端的监听端口）。</param>
        /// <returns>创建的 PeerExitNetwork 消息。</returns>
        public static KMessage PeerExitNetwork(KEndPoint localEP, InfoHash infoHash, ushort bitTorrentClientPort)
        {
            var message = KMessage.CreateEmptyMessage();
            message.Header.Code = KMessageCode.PeerExitNetwork;
            message.Header.SourceEndPoint = localEP;
            message.Header.MessageID = GetMessageHash();

            BEncodedDictionary data = new BEncodedDictionary();
            data.Add("infohash", infoHash.ToByteArray());
            data.Add("bt client port", bitTorrentClientPort);
            message.Content = KMessageContent.FromByteArray(data.Encode());
            return message;
        }

        /// <summary>
        /// 生成一个 GotPeer 消息。
        /// </summary>
        /// <param name="localEP">消息源的端点。</param>
        /// <param name="infoHash">用户所持有的 InfoHash。</param>
        /// <param name="bitTorrentClientPort">用户所监听的端口（即 BitTorrent 客户端的监听端口）。</param>
        /// <returns>创建的 GotPeer 消息。</returns>
        public static KMessage GotPeer(KEndPoint localEP, InfoHash infoHash, ushort bitTorrentClientPort)
        {
            var message = KMessage.CreateEmptyMessage();
            message.Header.Code = KMessageCode.GotPeer;
            message.Header.SourceEndPoint = localEP;
            message.Header.MessageID = GetMessageHash();

            BEncodedDictionary data = new BEncodedDictionary();
            data.Add("infohash", infoHash.ToByteArray());
            data.Add("bt client port", bitTorrentClientPort);
            message.Content = KMessageContent.FromByteArray(data.Encode());
            return message;
        }

        /// <summary>
        /// 解读一条用户连接消息。
        /// </summary>
        /// <param name="message">待解读的消息。</param>
        /// <param name="infoHash">解读出的 InfoHash。</param>
        /// <param name="bitTorrentClientPort">解读出的用户监听端口。</param>
        /// <exception cref="System.ArgumentException">解读的不是用户连接消息时发生。</exception>
        public static void GetPeerMessageContent(KMessage message, out InfoHash infoHash, out ushort bitTorrentClientPort)
        {
            //if (message.Content.Data.Length != 22)
            //{
            //    throw new ArgumentException("要解读的不是一条用户连接消息。");
            //}
            //infoHash = InfoHash.FromByteArray(message.Content.Data.Take(20).ToArray());
            //bitTorrentClientPort = BitConverter.ToUInt16(message.Content.Data, 20);
            BEncodedDictionary dictionary;
            try
            {
                dictionary = BEncodedValue.Decode<BEncodedDictionary>(message.Content.Data);
                infoHash = InfoHash.FromByteArray((dictionary["infohash"] as BEncodedString).TextBytes);
                bitTorrentClientPort = (ushort)((dictionary["bt client port"] as BEncodedNumber).Number);
            }
            catch (Exception)
            {
                throw new ArgumentException("附带数据不是用户连接消息数据。");
            }
            // 这里遇到了一个奇怪的问题，就是编解码的时候
#warning 系统的编解码和网络的编解码字节序可能不同！
            // BitConverter 的编解码和网络的编解码顺序可能不同
            // 但是奇怪的是，即使这样，KMessage 还能正常解码，地址大多数是正常的，但是端口大多数是不正常的
            // 即使是端口，也只是低2字节被颠倒了，高2字节都为零所以无法验证
            // 所以目前的临时方法是直接将低2字节颠倒回来，而且只保留低2字节
            // 再次测试后发现似乎是 μTorrent 自己的诈和……（以前）第一次收到的是错误的（后来在调试的时候，第一次好像也对了），接下来收到的都是正确的……
            //var b1 = (byte)((bitTorrentClientPort & 0x0000ff00) >> 8);
            //var b2 = (byte)(bitTorrentClientPort & 0x000000ff);
            //bitTorrentClientPort = (int)(((int)b2 << 8) + b1);
        }

    }
}
