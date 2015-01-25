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
using System.Diagnostics;

namespace Kei
{
    /// <summary>
    /// 对 <see cref="Kei.ILogger"/> 的一个初步实现。此类必须被继承。
    /// </summary>
    public abstract class Logger : ILogger
    {

        /// <summary>
        /// 空的 <see cref="Kei.Logger"/> 类。
        /// </summary>
        public static readonly Logger Null = NullLogger.Instance;

        /// <summary>
        /// 将一个日志项记录入日志。
        /// </summary>
        /// <param name="log">要记录的项内容。</param>
        public virtual void Log(string log)
        {
        }

        /// <summary>
        /// <see cref="Kei.Logger"/> 自带的空记录类，其 Log() 方法不执行任何操作。
        /// </summary>
        private class NullLogger : Logger
        {

            /// <summary>
            /// 全局唯一的 <see cref="Kei.Logger.NullLogger"/> 实例。
            /// </summary>
            public static readonly NullLogger Instance = new NullLogger();

            /// <summary>
            /// 初始化一个新的 <see cref="Kei.Logger.NullLogger"/>。
            /// </summary>
            private NullLogger()
            {
            }

            /// <summary>
            /// 将一个日志项记录入日志。该方法不执行任何操作。
            /// </summary>
            /// <param name="log">要记录的项内容。</param>
            public override void Log(string log)
            {
            }

        }

    }
}
