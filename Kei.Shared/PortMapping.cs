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
using System.Threading;
using Mono.Nat;

namespace Kei
{
    // 先设计成只适用于单端口映射的样子，以后扩展为自动化、多端口
    public static class PortMapping
    {

        public static readonly string DefaultDescription = "KeiSystem";

        private static List<INatDevice> _devices = new List<INatDevice>();

        private static bool _delegatesAdded = false;

        private static Mapping _mapping = null;

        public static bool UsePortMapping
        {
            get;
            set;
        }

        public static bool CanUsePortMapping
        {
            get
            {
                return _devices.Count > 0;
            }
        }

        public static bool AddedPortMapping
        {
            get
            {
                return _mapping != null;
            }
        }

        private static void DeviceFoundHandler(object sender, DeviceEventArgs e)
        {
            _devices.Add(e.Device);
        }

        private static void DeviceLostHandler(object sender, DeviceEventArgs e)
        {
            _devices.Remove(e.Device);
        }

        public static void InitializePortMappingEnvironment(TimeSpan timeout)
        {
            if (!_delegatesAdded)
            {
                NatUtility.DeviceFound += DeviceFoundHandler;
                NatUtility.DeviceLost += DeviceLostHandler;
                _delegatesAdded = true;
            }
            NatUtility.StartDiscovery();
            Thread.Sleep(timeout);
            NatUtility.StopDiscovery();
        }

        public static void AddPortMapping(ushort privatePort, ushort publicPort, TransmissionProtocol protocol = TransmissionProtocol.Tcp)
        {
            AddPortMapping(privatePort, publicPort, DefaultDescription, protocol);
        }

        public static void AddPortMapping(ushort privatePort, ushort publicPort, string description, TransmissionProtocol protocol = TransmissionProtocol.Tcp)
        {
            if (!UsePortMapping || !CanUsePortMapping)
            {
                return;
            }
            if (AddedPortMapping)
            {
                throw new InvalidOperationException("Port mapping already added.");
            }
            switch (protocol)
            {
                case TransmissionProtocol.Tcp:
                    _mapping = new Mapping(Protocol.Tcp, privatePort, publicPort);
                    _mapping.Description = description;
                    foreach (var dev in _devices)
                    {
                        dev.CreatePortMap(_mapping);
                    }
                    break;
                case TransmissionProtocol.Udp:
                    _mapping = new Mapping(Protocol.Udp, privatePort, publicPort);
                    _mapping.Description = description;
                    foreach (var dev in _devices)
                    {
                        dev.CreatePortMap(_mapping);
                    }
                    break;
                default:
                    break;
            }
        }

        public static void DeletePortMapping()
        {
            if (!UsePortMapping || !CanUsePortMapping)
            {
                return;
            }
            if (!AddedPortMapping)
            {
                return;
            }
            foreach (var dev in _devices)
            {
                dev.DeletePortMap(_mapping);
            }
            _mapping = null;
        }

        public enum TransmissionProtocol
        {
            Tcp = 0,
            Udp = 1,
        }

    }
}
