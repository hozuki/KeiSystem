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

/*
This file is based on HttpProcessor in SimpleHttpServer developed by David Jeske.
*/

#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;

namespace Kei.KTracker
{

    /// <summary>
    /// 负责处理 HTTP 请求的类。此类不能被继承。
    /// <para>支持的请求有：GET、POST。</para>
    /// </summary>
    internal sealed class HttpProcessor
    {

        /// <summary>
        /// 负责通信的 <see cref="System.Net.TcpClient"/>。
        /// </summary>
        protected TcpClient _tcpClient;

        /// <summary>
        /// 发起处理的 <see cref="Kei.KTracker.HttpServer"/> 对象。
        /// </summary>
        protected HttpServer _server;

        /// <summary>
        /// 对网络输入/输出流进行读写的 <see cref="System.IO.Stream"/>。
        /// </summary>
        protected Stream _ioStream;

        /// <summary>
        /// 当前请求的请求方法。
        /// </summary>
        protected string _httpMethod;

        /// <summary>
        /// 当前请求的目标 URL。
        /// <para>例如，请求 http://some.domain.com/page?param1=value1 的目标 URL 是 /page?param1=value1。</para>
        /// </summary>
        protected string _httpUrl;

        /// <summary>
        /// HTTP 请求的版本字符串。
        /// </summary>
        protected string _httpProtocolVersionString;

        /// <summary>
        /// 收到的 HTTP 请求头。
        /// </summary>
        protected StringDictionary _httpHeaders = new StringDictionary();

        /// <summary>
        /// 读 POST 请求体时所用缓冲区的大小（单位：字节）。
        /// </summary>
        protected const int BUF_SIZE = 4096;

        /// <summary>
        /// POST 请求体的最大发送信息大小（单位：字节）。
        /// </summary>
        protected const int MAX_POST_SIZE = 10 * 1024 * 1024; // 10MB

        /// <summary>
        /// 使用指定的 <see cref="System.Net.TcpClient"/> 和 <see cref="Kei.KTracker.HttpServer"/> 构造一个新的 <see cref="Kei.KTracker.HttpProcessor"/> 实例。
        /// </summary>
        /// <param name="client">指定的 <see cref="System.Net.TcpClient"/>。</param>
        /// <param name="server">指定的 HttpServer。</param>
        public HttpProcessor(TcpClient client, HttpServer server)
        {
            _tcpClient = client;
            _server = server;
        }

        /// <summary>
        /// 获取对网络输入/输出流进行读写的 <see cref="System.IO.Stream"/>。此属性为只读。
        /// </summary>
        private Stream IOStream
        {
            get
            {
                return _ioStream;
            }
        }

        /// <summary>
        /// 获取发起请求的客户端所使用的 <see cref="System.Net.IPEndPoint"/>。此属性为只读。
        /// </summary>
        public IPEndPoint SourceEndPoint
        {
            get
            {
                return (IPEndPoint)_tcpClient.Client.RemoteEndPoint;
            }
        }

        /// <summary>
        /// 获取当前请求的目标 URL。此属性为只读。
        /// <para>例如，请求 http://some.domain.com/page?param1=value1 的目标 URL 是 /page?param1=value1。</para>
        /// </summary>
        public string RequestUrl
        {
            get
            {
                return _httpUrl;
            }
        }

        /// <summary>
        /// 获取当前请求的请求方法。此属性为只读。
        /// </summary>
        public string MethodString
        {
            get
            {
                return _httpMethod;
            }
        }

        /// <summary>
        /// 获取 HTTP 请求的版本字符串。此属性为只读。
        /// </summary>
        public string ProtocolVersionString
        {
            get
            {
                return _httpProtocolVersionString;
            }
        }

        /// <summary>
        /// 在流中读取一行，并返回这一行的字符串内容。当前只支持读取 ASCII 字符集内字符构成的字符串。
        /// </summary>
        /// <param name="inputStream">要从中读取的流。</param>
        /// <returns>一个 <see cref="System.String"/>，表示读取的这一行（不包括换行和回车）。</returns>
        protected string StreamReadLine(Stream inputStream)
        {
            int next_char;
            StringBuilder sb = new StringBuilder();
            while (true)
            {
                next_char = inputStream.ReadByte();
                if (next_char == '\n') { break; }
                if (next_char == '\r') { continue; }
                if (next_char == -1) { Thread.Sleep(1); continue; };
                sb.Append(Convert.ToChar(next_char));
            }
            return sb.ToString();
        }

        /// <summary>
        /// 处理请求的总方法。
        /// </summary>
        internal void Process()
        {
            // we can't use a StreamReader for input, because it buffers up extra data on us inside it's
            // "processed" view of the world, and we want the data raw after the headers
            try
            {
                _ioStream = _tcpClient.GetStream();
                {
                    try
                    {
                        ParseRequest();
                        ReadHeaders();

                        if (_httpMethod == "get")
                        {
                            HandleGetRequest();
                        }
                        //else if (_httpMethod == "post")
                        //{
                        //    HandlePostRequest();
                        //}
                        else
                        {
                            // ...
                        }
                    }
                    catch (Exception)
                    {
                        //Console.WriteLine("Exception: " + e.ToString());
                        WriteFailure();
                    }

                    _server.Logger.Log("[HP]准备关闭连接。");
                    _ioStream.Flush();
                }
                _ioStream = null;
            }
            finally
            {
                _server.Logger.Log("[HP]关闭连接。");
                _tcpClient.Close();
            }
        }

        /// <summary>
        /// 解析 HTTP 协议头。
        /// </summary>
        protected void ParseRequest()
        {
            string request = StreamReadLine(_ioStream);
            string[] tokens = request.Split(' ');
            if (tokens.Length != 3)
            {
                throw new ArgumentException("无效 HTTP 请求。");
            }
            _httpMethod = tokens[0].ToLower();
            _httpUrl = tokens[1];
            _httpProtocolVersionString = tokens[2];

            //Console.WriteLine("starting: " + request);
        }

        /// <summary>
        /// 解析 HTTP 请求头。
        /// </summary>
        protected void ReadHeaders()
        {
            //Console.WriteLine("readHeaders()");
            string line;
            while ((line = StreamReadLine(_ioStream)) != null)
            {
                if (line.Equals(string.Empty))
                {
                    //Console.WriteLine("got headers");
                    return;
                }
                line = line.ToLower();

                int separator = line.IndexOf(':');
                if (separator == -1)
                {
                    throw new ArgumentException("无效 HTTP 头: " + line);
                }
                string name = line.Substring(0, separator);
                int pos = separator + 1;
                while ((pos < line.Length) && (line[pos] == ' '))
                {
                    pos++; // strip any spaces
                }

                string value = line.Substring(pos, line.Length - pos);
                //Console.WriteLine("header: {0}:{1}", name, value);
                _httpHeaders[name] = value;
            }
        }

        /// <summary>
        /// 处理 GET 请求。
        /// </summary>
        protected void HandleGetRequest()
        {
            _server.HandleGetRequest(this, IOStream);
        }

        /// <summary>
        /// 处理 POST 请求。
        /// </summary>
        protected void HandlePostRequest()
        {
            // this post data processing just reads everything into a memory stream.
            // this is fine for smallish things, but for large stuff we should really
            // hand an input stream to the request processor. However, the input stream 
            // we hand him needs to let him see the "end of the stream" at this content 
            // length, because otherwise he won't know when he's seen it all! 

            //Console.WriteLine("get post data start");
            int contentLength = 0;
            using (var ms = new MemoryStream(1024))
            {
                if (_httpHeaders.ContainsKey("content-length"))
                {
                    contentLength = Convert.ToInt32(_httpHeaders["content-length"]);
                    if (contentLength > MAX_POST_SIZE)
                    {
                        throw new ArgumentException(string.Format("POST 请求的 Content-Length 太长，本服务器无法处理。", contentLength));
                    }
                    byte[] buf = new byte[BUF_SIZE];
                    int to_read = contentLength;
                    while (to_read > 0)
                    {
                        //Console.WriteLine("starting Read, to_read={0}", to_read);

                        int numread = this._ioStream.Read(buf, 0, Math.Min(BUF_SIZE, to_read));
                        //Console.WriteLine("read finished, numread={0}", numread);
                        if (numread == 0)
                        {
                            if (to_read == 0)
                            {
                                break;
                            }
                            else
                            {
                                throw new InvalidOperationException("客户端在 POST 请求时断开。");
                            }
                        }
                        to_read -= numread;
                        ms.Write(buf, 0, numread);
                    }
                    ms.Seek(0, SeekOrigin.Begin);
                }
                //Console.WriteLine("get post data end");
                _server.HandlePostRequest(this, ms);
            }
        }

        /// <summary>
        /// 向此 HttpProcessor 自身的默认输入输出流中写入表示成功的头信息。同时可以指定一些可选的头。
        /// <para>标准响应为：</para>
        /// <para>HTTP/1.0 200 OK</para>
        /// <para>Content-Type: text/html</para>
        /// <para>Connection: Keep-Alive</para>
        /// <para>...</para>
        /// </summary>
        /// <param name="optionalHeaders">可选的头，为 null 表示没有可选的头。注意，该头内不能包含 Content-Type 和 Connection。</param>
        /// <param name="contentType">指定返回的内容类型。默认为 text/html。</param>
        public void WriteSuccess(IDictionary<string, string> optionalHeaders = null, string contentType = "text/html")
        {
            // this is the successful HTTP response line
            IOStream.WriteLine("HTTP/1.0 200 OK");
            // these are the HTTP headers...          
            IOStream.WriteLine("Content-Type: " + contentType);
            IOStream.WriteLine("Connection: Keep-Alive");
            // ..add your own headers here if you like
            if (optionalHeaders != null && optionalHeaders.Count > 0)
            {
                foreach (var kv in optionalHeaders)
                {
                    IOStream.WriteLine(kv.Key + ": " + kv.Value);
                }
            }

            IOStream.WriteLine(); // this terminates the HTTP headers.. everything after this is HTTP body..
        }

        /// <summary>
        /// 向此 HttpProcessor 自身的默认输入输出流中写入表示失败的头信息。同时可以指定一些可选的头。
        /// <para>标准响应为：</para>
        /// <para>HTTP/1.0 500 Internal server error</para>
        /// <para>Connection: Close</para>
        /// <para>...</para>
        /// </summary>
        /// <param name="optionalHeaders">可选的头，为 null 表示没有可选的头。注意，该头内不能包含 Connection。</param>
        /// <param name="errorCode">指定返回的错误码。默认为 500（内部服务器错误）。</param>
        /// <param name="errorDescription">指定返回的错误信息。默认为“Internal server error”。</param>
        public void WriteFailure(IDictionary<string, string> optionalHeaders = null, int errorCode = 500, string errorDescription = "Internal server error")
        {
            // this is an http 404 failure response
            IOStream.WriteLine("HTTP/1.0 " + errorCode.ToString() + " " + errorDescription);
            // these are the HTTP headers
            IOStream.WriteLine("Connection: Close");
            // ..add your own headers here
            if (optionalHeaders != null && optionalHeaders.Count > 0)
            {
                foreach (var kv in optionalHeaders)
                {
                    IOStream.WriteLine(kv.Key + ": " + kv.Value);
                }
            }

            IOStream.WriteLine(); // this terminates the HTTP headers.
        }
    }

}