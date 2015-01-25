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
    /// 使用流直接读写消息时的读写错误码。
    /// </summary>
    public enum MessageIOErrorCode
    {

        /// <summary>
        /// 无错误，正常完成。
        /// </summary>
        NoError,
        /// <summary>
        /// 校验码错误。
        /// </summary>
        WrongMagicNumber,
        /// <summary>
        /// 读取校验码时发生了错误。
        /// </summary>
        ReadMagicNumberFailed,
        /// <summary>
        /// 读取消息头的时候发生了错误。
        /// </summary>
        ReadHeaderFailed,
        /// <summary>
        /// 读取消息附带内容的时候发生了错误。
        /// </summary>
        ReadContentFailed,
        /// <summary>
        /// 写消息的时候发生了错误。
        /// </summary>
        WriteFailed,

    }
}
