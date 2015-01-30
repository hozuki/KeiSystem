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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ManagedUPnP;
using ManagedUPnP.Descriptions;

namespace Kei
{
    /// <summary>
    /// 提供 UPnP（通用即插即用）的支持。
    /// </summary>
    public static class UPnPSupport
    {

        /// <summary>
        /// 可用的 UPnP 设备列表。注意该列表是静态的，程序中设置了就难以随着网络状况而改变。
        /// </summary>
        private static Devices _upnpDevices = null;

        /// <summary>
        /// 添加端口映射的方法。
        /// </summary>
        private static Func<string, object[], object[]> _addPortMapping = null;

        /// <summary>
        /// 删除端口映射的方法。
        /// </summary>
        private static Func<string, object[], object[]> _deletePortMapping = null;

        /// <summary>
        /// 内部端口。
        /// </summary>
        private static ushort _inPort;

        /// <summary>
        /// 外部端口。
        /// </summary>
        private static ushort _exPort;

        /// <summary>
        /// 获取或设置程序是否使用 UPnP 功能。如果不使用则所有 UPnP 操作无效。
        /// </summary>
        public static bool UseUPnP
        {
            get;
            set;
        }

        /// <summary>
        /// 获取目前能否使用端口映射。此属性为只读。
        /// </summary>
        public static bool CanUsePortMapping
        {
            get
            {
                return UseUPnP && (_addPortMapping != null && _deletePortMapping != null);
            }
        }

        /// <summary>
        /// 获取目前是否已经添加了端口映射。此属性为只读。
        /// </summary>
        public static bool AddedPortMapping
        {
            get
            {
                return _inPort != 0 && _exPort != 0;
            }
        }

        /// <summary>
        /// 添加一个端口映射。
        /// </summary>
        /// <param name="internalPort">要映射的端口（内部端口）。</param>
        /// <param name="externalPort">要映射为的端口（外部端口）。</param>
        /// <param name="clientAddress">使用这两个端口的客户端的 IP 地址。</param>
        /// <exception cref="System.InvalidOperationException">已经添加了端口映射并调用此方法时发生。</exception>
        public static void AddPortMapping(ushort internalPort, ushort externalPort, string clientAddress)
        {
            if (!CanUsePortMapping)
            {
                //throw new InvalidOperationException("Cannot use port mapping.");
                return;
            }
            if (_inPort != 0 && _exPort != 0)
            {
                throw new InvalidOperationException("You have added port mapping.");
            }
            System.Diagnostics.Debug.Print("Adding port mapping...");
            System.Diagnostics.Debug.Indent();
            System.Diagnostics.Debug.Print("External port: " + externalPort.ToString());
            System.Diagnostics.Debug.Print("Internal port: " + internalPort.ToString());
            System.Diagnostics.Debug.Print("Client address: " + clientAddress);
            System.Diagnostics.Debug.Unindent();
            // 这个 Sleep() 是用于放置 add port mapping 报错的
            // 貌似直接执行的话，如果 in 和 ex 是相同的，那么就报错
            // 但是在调试器下，暂停一下再执行，就没问题
            // 认为是 socket 上次通讯即使调用了 Close() 还未释放？（测试结果好像不是）
            System.Threading.Thread.Sleep(TimeSpan.FromSeconds(0.8));
            var returns = _addPortMapping("AddPortMapping", new object[]
            {
                string.Empty,
                externalPort,
                "TCP",
                internalPort,
                clientAddress, // internal client address
                true,  // true
                "KeiSystem",
                (uint)0,
            });
            _inPort = internalPort;
            _exPort = externalPort;
        }

        /// <summary>
        /// 删除现有的端口映射。
        /// </summary>
        /// <exception cref="System.InvalidOperationException">未添加端口映射就调用此方法时发生。</exception>
        public static void DeletePortMapping()
        {
            if (!CanUsePortMapping)
            {
                return;
            }
            if (_inPort != 0 && _exPort != 0)
            {
                _deletePortMapping("DeletePortMapping", new object[]
                {
                    string.Empty,
                    _exPort,
                    "TCP",
                });
                _inPort = _exPort = 0;
            }
            else
            {
                throw new InvalidOperationException("You haven't add any port mapping.");
            }
        }

        /// <summary>
        /// 初始化 UPnP 设备信息列表。
        /// </summary>
        /// <param name="discoveryTimeout">用于发现设备所用的超时时间。</param>
        public static void InitUPnP(TimeSpan discoveryTimeout)
        {
            if (!UseUPnP)
            {
                return;
            }
            //WindowsFirewall.CheckUPnPFirewallRules(form);
            bool searchCompleted;
            _upnpDevices = Discovery.FindDevices(string.Empty, (int)discoveryTimeout.TotalMilliseconds, 0, out searchCompleted, AddressFamilyFlags.IPv4);
            //_upnpDevices = Discovery.FindDevices("WANConnectionDevice:1", AddressFamilyFlags.IPv4); 
            //bool found = false;
            //FindPortMapping(_upnpDevices, ref found);
            Services services = _upnpDevices.FindServices(string.Empty, true);
            bool afound = false, dfound = false;
            foreach (var serv in services)
            {
                if (serv.FriendlyServiceTypeIdentifier == "WANIPConnection:1")
                {
                    var sd = serv.Description();
                    var ac = sd.Actions;
                    afound = dfound = false;
                    _addPortMapping = _deletePortMapping = null;
                    if (ac.Count > 0)
                    {
                        foreach (var item in ac)
                        {
                            if (item.Key == "AddPortMapping")
                            {
                                _addPortMapping = serv.InvokeAction;
                                //System.Diagnostics.Debug.Print("Name of action: " + item.Value.Name);
                                //foreach (var x in item.Value.Arguments)
                                //{
                                //    System.Diagnostics.Debug.Print(x.Key + " is " + x.Value.RelatedStateVariableDescription.DataTypeValue.BaseType().ToString());
                                //}
                                afound = true;
                                if (dfound)
                                {
                                    break;
                                }
                            }
                            else if (item.Key == "DeletePortMapping")
                            {
                                _deletePortMapping = serv.InvokeAction;
                                //System.Diagnostics.Debug.Print("Name of action: " + item.Value.Name);
                                //foreach (var x in item.Value.Arguments)
                                //{
                                //    System.Diagnostics.Debug.Print(x.Key + " is " + x.Value.RelatedStateVariableDescription.DataTypeValue.BaseType().ToString());
                                //}
                                //dfound = true;
                                if (afound)
                                {
                                    break;
                                }
                            }
                        }
                    }
                    if (afound && dfound)
                    {
                        break;
                    }
                }
            }
        }

    }
}
#endif