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
using System.Threading;

namespace Kei
{
    /// <summary>
    /// 事件触发的实用类。
    /// </summary>
    public static class EventHelper
    {

        /// <summary>
        /// 同步触发指定事件。
        /// </summary>
        /// <typeparam name="T">继承自 <see cref="System.EventArgs"/>。</typeparam>
        /// <param name="eventHandler">要调用的事件委托。</param>
        /// <param name="sender">事件委托的 sender 参数。</param>
        /// <param name="e">事件委托的 e 参数。</param>
        public static void RaiseEvent<T>(EventHandler<T> eventHandler, object sender, T e) where T : EventArgs
        {
            if (eventHandler != null)
            {
                eventHandler.Invoke(sender, e);
            }
        }

        /// <summary>
        /// 同步触发指定事件。
        /// </summary>
        /// <param name="eventHandler">要调用的事件委托。</param>
        /// <param name="sender">事件委托的 sender 参数。</param>
        /// <param name="e">事件委托的 e 参数。</param>
        public static void RaiseEvent(EventHandler eventHandler, object sender, EventArgs e)
        {
            if (eventHandler != null)
            {
                eventHandler.Invoke(sender, e);
            }
        }

        /// <summary>
        /// 异步触发指定事件。
        /// </summary>
        /// <typeparam name="T">继承自 <see cref="System.EventArgs"/>。</typeparam>
        /// <param name="eventHandler">要调用的事件委托。</param>
        /// <param name="sender">事件委托的 sender 参数。</param>
        /// <param name="e">事件委托的 e 参数。</param>
        public static void RaiseEventAsync<T>(EventHandler<T> eventHandler, object sender, T e) where T : EventArgs
        {
            if (eventHandler != null)
            {
                Delegate[] delegates = eventHandler.GetInvocationList();
                for (var i = 0; i < delegates.Length; i++)
                {
                    var sink = (EventHandler<T>)delegates[i];
                    sink.BeginInvoke(sender, e, null, null);
                }
            }
        }

        /// <summary>
        /// 异步触发指定事件。
        /// </summary>
        /// <param name="eventHandler">要调用的事件委托。</param>
        /// <param name="sender">事件委托的 sender 参数。</param>
        /// <param name="e">事件委托的 e 参数。</param>
        public static void RaiseEventAsync(EventHandler eventHandler, object sender, EventArgs e)
        {
            if (eventHandler != null)
            {
                Delegate[] delegates = eventHandler.GetInvocationList();
                for (var i = 0; i < delegates.Length; i++)
                {
                    var sink = (EventHandler)delegates[i];
                    sink.BeginInvoke(sender, e, null, null);
                }
            }
        }

    }
}
