using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;

namespace Kei.KNetwork
{

    /// <summary>
    /// 表示用于组织 <see cref="Kei.KNetwork.KClient"/> 的消息处理过程需要用到的信息。此类不能被继承。
    /// </summary>
    internal sealed class HandleMessageArgs
    {
        
        /// <summary>
        /// 传输用的 <see cref="System.IO.Stream"/>。
        /// </summary>
        private Stream _stream;

        /// <summary>
        /// 收到的消息。
        /// </summary>
        private KMessage _message;

        /// <summary>
        /// 通信中的源端点。
        /// </summary>
        private IPEndPoint _endPoint;

        /// <summary>
        /// 使用指定的参数创建一个新的 <see cref="Kei.KNetwork.HandleMessageArgs"/> 实例。
        /// </summary>
        /// <param name="stream">网络传输使用的 <see cref="System.IO.Stream"/>。</param>
        /// <param name="message">收到的消息。</param>
        /// <param name="endPoint">通信中的源端点。</param>
        internal HandleMessageArgs(Stream stream, KMessage message, IPEndPoint endPoint, ushort realPort)
        {
            _stream = stream;
            _message = message;
            _endPoint = endPoint;
            RealPort = realPort;
        }

        /// <summary>
        /// 获取传输用的 <see cref="System.IO.Stream"/>。此属性为只读。
        /// </summary>
        public Stream Stream
        {
            get
            {
                return _stream;
            }
        }

        /// <summary>
        /// 获取收到的消息。此属性为只读。
        /// </summary>
        public KMessage Message
        {
            get
            {
                return _message;
            }
        }

        /// <summary>
        /// 获取通信中的源端点。此属性为只读。
        /// </summary>
        public IPEndPoint EndPoint
        {
            get
            {
                return _endPoint;
            }
        }

        /// <summary>
        /// 发起请求的端点的端口。零表示忽略该值，非零表示发起请求的是一个接入点，应该添加这个接入点的端口。
        /// </summary>
        public ushort RealPort
        {
            get;
            set;
        }

    }
}
