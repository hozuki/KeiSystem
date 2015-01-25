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

namespace Kei.KNetwork
{

    /// <summary>
    /// 表示一条已处理的消息。
    /// </summary>
    public sealed class KHandledMessage : IEquatable<KHandledMessage>
    {

        /// <summary>
        /// 默认的生存时间。默认为 3 分钟。
        /// </summary>
        public static readonly TimeSpan DefaultLifeLength = TimeSpan.FromMinutes(3);

        /// <summary>
        /// 处理的消息。
        /// </summary>
        private KMessage _message;

        /// <summary>
        /// 该消息的处理时刻。可以对此值延时。
        /// </summary>
        private DateTime _lifeStart;

        /// <summary>
        /// 该消息的生存时间。
        /// </summary>
        private TimeSpan _lifeLength;

        /// <summary>
        /// 从一个 <see cref="Kei.KNetwork.KMessage"/> 创建一个新的实例，并指定为此时为处理时刻，使用默认生存时间。
        /// </summary>
        /// <param name="message">处理的基础 <see cref="Kei.KNetwork.KMessage"/></param>
        public KHandledMessage(KMessage message)
            : this(message, DateTime.Now)
        {
        }

        /// <summary>
        /// 从一个 <see cref="Kei.KNetwork.KMessage"/> 创建一个新的实例，设置处理时刻，并使用默认生存时间。
        /// </summary>
        /// <param name="message">处理的基础 <see cref="Kei.KNetwork.KMessage"/>。</param>
        /// <param name="lifeStart">消息的处理时刻。</param>
        public KHandledMessage(KMessage message, DateTime lifeStart)
            : this(message, lifeStart, DefaultLifeLength)
        {
        }

        /// <summary>
        /// 从一个 <see cref="Kei.KNetwork.KMessage"/> 创建一个新的实例，并设置处理时刻与生存时间。
        /// </summary>
        /// <param name="message">处理的基础 <see cref="Kei.KNetwork.KMessage"/>。</param>
        /// <param name="lifeStart">消息的处理时刻。</param>
        /// <param name="lifeLength">消息的生存时间。</param>
        public KHandledMessage(KMessage message, DateTime lifeStart, TimeSpan lifeLength)
        {
            _message = message;
            _lifeStart = lifeStart;
            _lifeLength = lifeLength;
        }

        /// <summary>
        /// 获取基础 <see cref="Kei.KNetwork.KMessage"/>。此属性为只读。
        /// </summary>
        public KMessage Message
        {
            get
            {
                return _message;
            }
        }

        /// <summary>
        /// 获取或设置本消息的处理时刻。
        /// </summary>
        public DateTime LifeStart
        {
            get
            {
                return _lifeStart;
            }
            set
            {
                _lifeStart = value;
            }
        }

        /// <summary>
        /// 获取本消息的生存时间。此属性为只读。
        /// </summary>
        public TimeSpan LifeLength
        {
            get
            {
                return _lifeLength;
            }
        }

        /// <summary>
        /// 判断本消息在指定时刻是否该被清理。
        /// </summary>
        /// <param name="time">作为判断标准的时刻。</param>
        /// <returns>返回 true 表示该被清理，返回 false 表示该保留。</returns>
        public bool ShouldBeDisposedAt(DateTime time)
        {
            return time > LifeStart + LifeLength;
        }

        /// <summary>
        /// 判断本消息与另一个 <see cref="Kei.KNetwork.KHandledMessage"/> 是否等价。
        /// </summary>
        /// <param name="other">另一个 <see cref="Kei.KNetwork.KHandledMessage"/>。</param>
        /// <returns>返回 true 表示等价，返回 false 表示不等价。</returns>
        public bool Equals(KHandledMessage other)
        {
            if (other == null)
            {
                return this == null;
            }
            else
            {
                return Message.Equals(other.Message);
            }
        }
    }
}
