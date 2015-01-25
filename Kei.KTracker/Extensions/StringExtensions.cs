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

namespace Kei.KTracker
{
    /// <summary>
    /// <see cref="System.String"/> 相关的扩展类。
    /// </summary>
    public static class StringExtensions
    {

        /// <summary>
        /// 将一个 <see cref="System.String"/> 拆分成多个子 <see cref="System.String"/>。
        /// </summary>
        /// <param name="str">待拆分的 <see cref="System.String"/>。</param>
        /// <param name="removeEmptyEntries">是否要删除拆分结果中的空项。</param>
        /// <param name="delimeters">一个 <see cref="System.Char"/>[]，表示用来分隔的字符集合。</param>
        /// <returns>一个 <see cref="System.String"/>[]，表示拆分结果。</returns>
        public static string[] Split(this string str, bool removeEmptyEntries, params char[] delimeters)
        {
            if (str == null)
            {
                throw new NullReferenceException("str is null");
            }
            if (removeEmptyEntries)
            {
                return str.Split(delimeters);
            }
            else
            {
                return str.Split(delimeters, StringSplitOptions.RemoveEmptyEntries);
            }
        }

        /// <summary>
        /// 将一个 <see cref="System.String"/> 拆分成多个子 <see cref="System.String"/>。
        /// </summary>
        /// <param name="str">待拆分的 <see cref="System.String"/>。</param>
        /// <param name="removeEmptyEntries">是否要删除拆分结果中的空项。</param>
        /// <param name="delimeters">一个 <see cref="System.String"/>[]，表示用来分隔的字符串集合。</param>
        /// <returns>一个 <see cref="System.String"/>[]，表示拆分结果。</returns>
        public static string[] Split(this string str, bool removeEmptyEntries, params string[] delimeters)
        {
            if (str == null)
            {
                throw new NullReferenceException("str is null");
            }
            return str.Split(delimeters, removeEmptyEntries ? StringSplitOptions.RemoveEmptyEntries : StringSplitOptions.None);
        }

    }
}
