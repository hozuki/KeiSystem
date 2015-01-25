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

#if false
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Kei
{
    /// <summary>
    /// 引用类对象的扩展类。
    /// </summary>
    public static class ReferenceTypeExtensions
    {

        /// <summary>
        /// 通过 Lock() 函数添加的锁。
        /// </summary>
        private static readonly HashSet<object> Locks;

        /// <summary>
        /// 锁定一个对象，让该实例在同一个时刻只能被一个线程所访问。在 <see cref="System.Collections.IEnumerable"/> 上使用时，可以避免修改时迭代器失效的问题。
        /// </summary>
        /// <param name="obj">要锁定的对象。</param>
        /// <typeparam name="T">要锁定的对象的类型。因值类型默认克隆而导致分配锁后无法释放，这里只能采用引用类型。</typeparam>
        /// <returns>返回 true 表示锁定成功或者已经锁定，返回 false 表示锁定失败。</returns>
        public static bool LockX<T>(this T obj) where T : class
        {

            if (Locks.Contains(obj))
            {
                return true;
            }
            bool locked = true;
            if (obj != null)
            {
                Monitor.Enter(obj, ref locked);
                if (locked)
                {
                    Locks.Add(obj);
                }
                return locked;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 解除对象的锁定。
        /// </summary>
        /// <param name="obj">要解锁的对象。</param>
        /// <typeparam name="T">要锁定的对象的类型。因值类型默认克隆而导致分配锁后无法释放，这里只能采用引用类型。</typeparam>
        /// <returns>返回 true 表示解锁成功，返回 false 表示解锁失败。</returns>
        public static bool UnlockX<T>(this T obj) where T : class
        {
            if (Locks.Contains(obj))
            {
                Monitor.Exit(obj);
                Locks.Remove(obj);
                return true;
            }
            else
            {
                return false;
            }
        }

    }
}
#endif