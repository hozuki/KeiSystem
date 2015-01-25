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
using System.Collections.Specialized;

namespace Kei.KTracker
{
    /// <summary>
    /// <see cref="System.Collections.Specialized.StringDictionary"/> 相关的扩展类。
    /// </summary>
    public static class StringDictionaryExtensions
    {

        /// <summary>
        /// 以给定的 <see cref="System.String"/> 为键，尝试从 <see cref="System.Collections.Specialized.StringDictionary"/> 中获取一项，并返回获取结果。
        /// </summary>
        /// <param name="dictionary">要从中获取项的 <see cref="System.Collections.Specialized.StringDictionary"/>。</param>
        /// <param name="key">一个 <see cref="System.String"/>，表示目标项的键。</param>
        /// <param name="value">一个 <see cref="System.String"/>，表示获取的值。如果函数返回 false，则该值无效。</param>
        /// <returns>返回 true 表示找到项，返回 false 表示未找到项。</returns>
        public static bool TryGetValue(this StringDictionary dictionary, string key, out string value)
        {
            if (dictionary == null)
            {
                throw new NullReferenceException("dictionary is null");
            }
            if (dictionary.ContainsKey(key))
            {
                value = dictionary[key];
                return true;
            }
            else
            {
                value = null;
                return false;
            }
        }

    }
}
