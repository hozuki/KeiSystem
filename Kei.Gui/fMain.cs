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
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Xml;
using Kei.KNetwork;
using Kei.KTracker;

namespace Kei.Gui
{
    public sealed partial class fMain : Form
    {

        private TrackerServer _kTracker = null;
        private KClient _kClient = null;
        private bool? _wasStartedAsPI = new Nullable<bool>();

        public KeiGuiState KGState
        {
            get;
            set;
        }

        public fMain()
        {
            InitializeComponent();
            InitializeEventHandlers();
            ExtraInit();
            CheckForIllegalCrossThreadCalls = false;
            KGState = KeiGuiState.Initialized;
        }

        private void ExtraInit()
        {
            mnuOpSeparator1.Visible = false;
        }

        private void InitializeEventHandlers()
        {
            this.Load += fMain_Load;
            this.FormClosing += fMain_FormClosing;
            this.SizeChanged += fMain_SizeChanged;
            mnuFileExit.Click += mnuFileExit_Click;
            cmdStartServers.Click += cmdStartServers_Click;
            notifier.MouseClick += notifier_MouseClick;
            notifier.DoubleClick += notifier_DoubleClick;
            cmdConnectToTargetKClient.Click += cmdConnectToTargetKClient_Click;
            ctxExit.Click += ctxExit_Click;
            ctxShowMainForm.Click += ctxShowMainForm_Click;
            mnuOpForceBroadcast.Click += forceBroadcast_Handler;
            ctxForceBroadcast.Click += forceBroadcast_Handler;
            tmrForceBroadcast.Tick += tmrForceBroadcast_Tick;
            mnuHelpAbout.Click += mnuHelpAbout_Click;
            mnuOpOptions.Click += mnuOpOptions_Click;
            mnuHelpContent.Click += mnuHelpContent_Click;
        }

        void _kClient_ConnectionListChanged(object sender, EventArgs e)
        {
            if (KGState >= KeiGuiState.Connected)
            {
                SetStatusText("已经连接到 " + cboTargetKClientEndPoint.Text + "，开始工作 [" + _kClient.ConnectionList.Count.ToString() + "]");
            }
            else if (KGState >= KeiGuiState.ServersStarted)
            {
                SetStatusText("服务器已启动 [" + _kClient.ConnectionList.Count.ToString() + "]");
            }
        }

        void mnuHelpContent_Click(object sender, EventArgs e)
        {
            var fileName = Path.Combine(Application.StartupPath, "help/index.htm");
            if (File.Exists(fileName))
            {
                Process process = new Process();
                process.StartInfo = new ProcessStartInfo(fileName);
                process.Start();
            }
            else
            {
                MessageBox.Show((new FileInfo(fileName)).FullName + " 未找到。", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        void mnuOpOptions_Click(object sender, EventArgs e)
        {
            using (var f = new fOptions())
            {
                f.ShowDialog(this);
            }
        }

        void mnuHelpAbout_Click(object sender, EventArgs e)
        {
            using (var f = new fAbout())
            {
                f.ShowDialog(this);
            }
        }

        void tmrForceBroadcast_Tick(object sender, EventArgs e)
        {
            _kClient.RegardNormalTrackerCommAsStarted = false;
            tmrForceBroadcast.Enabled = false;
            mnuOpForceBroadcast.Checked = false;
            ctxForceBroadcast.Checked = false;
            mnuOpForceBroadcast.Enabled = true;
            ctxForceBroadcast.Enabled = true;
        }

        void forceBroadcast_Handler(object sender, EventArgs e)
        {
            mnuOpForceBroadcast.Checked = true;
            ctxForceBroadcast.Checked = true;
            mnuOpForceBroadcast.Enabled = false;
            ctxForceBroadcast.Enabled = false;
            tmrForceBroadcast.Interval = (int)KeiGuiOptions.Current.ForceBroadcastTime.TotalMilliseconds;
            tmrForceBroadcast.Enabled = true;
            _kClient.RegardNormalTrackerCommAsStarted = true;
        }

        void ctxShowMainForm_Click(object sender, EventArgs e)
        {
            Show();
            WindowState = FormWindowState.Normal;
        }

        void ctxExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void TryReadOptions()
        {
            var configFileName = Path.Combine(Application.StartupPath, KeiGuiOptions.DefaultConfigurationFileName);
            if (File.Exists(configFileName))
            {
                try
                {
                    using (var fs = new FileStream(configFileName, FileMode.Open, FileAccess.Read))
                    {
                        using (var xmlreader = new XmlTextReader(fs))
                        {
                            try
                            {
                                KeiGuiOptions.Current = KeiGuiOptions.Read(xmlreader);
                            }
                            catch (Exception)
                            {
                                KeiGuiOptions.Current = (KeiGuiOptions)KeiGuiOptions.Default.Clone();
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    KeiGuiOptions.Current = (KeiGuiOptions)KeiGuiOptions.Default.Clone();
                }
            }
            else
            {
                KeiGuiOptions.Current = (KeiGuiOptions)KeiGuiOptions.Default.Clone();
            }
            KeiGuiOptions.Modified = (KeiGuiOptions)KeiGuiOptions.Current.Clone();

            txtLocalKClientPort.Text = KeiGuiOptions.Current.LocalKClientPort.ToString();
            txtLocalTrackerServerPort.Text = KeiGuiOptions.Current.LocalTrackerServerPort.ToString();
        }

        private void TrySaveOptions()
        {
            if (KGState >= KeiGuiState.ServersStarted)
            {
                // 只有在作为接入点启动的时候以下两项才是不变的
                if (!(_wasStartedAsPI.HasValue && _wasStartedAsPI.Value))
                {
                    KeiGuiOptions.Modified.LocalIntranetAddress = cboPossibleAddresses.Text;
                    KeiGuiOptions.Modified.LocalKClientPort = _kClient.LocalEndPoint.Port;
                }
                KeiGuiOptions.Modified.LocalTrackerServerPort = _kTracker.LocalEndPoint.Port;
            }
            if (KGState >= KeiGuiState.Connected)
            {
                if (_kClient.ConnectionList.Count > 0)
                {
                    KeiGuiOptions.Modified.TargetEndPoints.Clear();
                    lock (_kClient.ConnectionList)
                    {
                        foreach (var item in _kClient.ConnectionList)
                        {
                            KeiGuiOptions.Modified.TargetEndPoints.Add(item.ClientLocation.ToString());
                        }
                    }
                }
            }

            try
            {
                using (var fs = new FileStream(Path.Combine(Application.StartupPath, KeiGuiOptions.DefaultConfigurationFileName), FileMode.Create, FileAccess.Write))
                {
                    using (var xmlwriter = new XmlTextWriter(fs, Encoding.UTF8))
                    {
                        try
                        {
                            KeiGuiOptions.Modified.Save(xmlwriter);
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        void cmdConnectToTargetKClient_Click(object sender, EventArgs e)
        {
            try
            {
                IPEndPoint ipep = KeiUtil.ParseIPEndPoint(cboTargetKClientEndPoint.Text);
                Thread kcWorkerThread;
                for (var i = 0; i < 1; i++)
                {
                    //Thread.Sleep(1500);
                    WaitHandle wh;
                    if (KeiGuiOptions.Current.IsPointInsertion)
                    {
                        wh = _kClient.EnterNetwork(ipep, Convert.ToUInt16(txtLocalKClientPort.Text), out kcWorkerThread);
                    }
                    else
                    {
                        // 普通用户
                        wh = _kClient.EnterNetwork(ipep, 0, out kcWorkerThread);
                    }
                    Thread thread = new Thread(delegate()
                    {
                        Invoke(new Action(() =>
                        {
                            cmdConnectToTargetKClient.Enabled = false;
                        }));
                        TimeSpan passedTime = TimeSpan.Zero;
                        while (kcWorkerThread.IsAlive && !wh.SafeWaitHandle.IsClosed && passedTime < TimeSpan.FromSeconds(3))
                        {
                            wh.WaitOne(TimeSpan.FromMilliseconds(10));
                            passedTime += TimeSpan.FromMilliseconds(10);
                        }
                        if (_kClient.ConnectionList.Count > 0)
                        {
                            Invoke(new Action(() =>
                            {
                                if (!_kClient.IsActive)
                                {
                                    // 普通用户此时启动
                                    // 不过确实，想想，对于普通用户，如果是端口在从 TestBind() 到现在（即，点下“启动”按钮到点下“连接”按钮并成功）
                                    // 这段时间内，该K客户端端口被占用了怎么办？这里就会直接异常，线程崩溃了……
                                    _kClient.Listen();
                                }
                                cmdConnectToTargetKClient.Enabled = false;
                                groupBox2.Enabled = false;
                                KGState = KeiGuiState.Connected;
                                if (!cboTargetKClientEndPoint.Items.Contains(cboTargetKClientEndPoint.Text))
                                {
                                    cboTargetKClientEndPoint.Items.Add(cboTargetKClientEndPoint.Text);
                                }
                                SetStatusText("已经连接到 " + cboTargetKClientEndPoint.Text + "，开始工作 [" + _kClient.ConnectionList.Count.ToString() + "]");
                                mnuOpForceBroadcast.Enabled = true;
                                ctxForceBroadcast.Enabled = true;
                            }));
                        }
                        else
                        {
                            Invoke(new Action(() =>
                            {
                                cmdConnectToTargetKClient.Enabled = true;
                                SetStatusText("未能连接到 " + cboTargetKClientEndPoint.Text);
                            }));
                        }
                    });
                    thread.IsBackground = true;
                    thread.Start();
                }
            }
            catch (Exception ex)
            {
                Program.Logger.Log(ex.Message);
            }
        }

        void notifier_DoubleClick(object sender, EventArgs e)
        {
            Show();
            WindowState = FormWindowState.Normal;
        }

        void notifier_MouseClick(object sender, MouseEventArgs e)
        {
        }

        void fMain_SizeChanged(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                Hide();
            }
        }

        void cmdStartServers_Click(object sender, EventArgs e)
        {
            if (_kClient == null || _kTracker == null)
            {
                if (CheckServerFields())
                {
                    IPAddress ipa;
                    _wasStartedAsPI = KeiGuiOptions.Current.IsPointInsertion;
                    if (KeiGuiOptions.Current.IsPointInsertion || (false && KeiGuiOptions.Current.UsePortMapping))
                    {
                        // 作为接入点或者启用了 UPnP 的普通用户启动
                        ipa = IPAddress.Parse(cboPossibleAddresses.Text.Split(' ')[0]);
                    }
                    else
                    {
                        // 作为普通用户启动
                        ipa = IPAddress.Loopback;
                    }
                    var nTracker = Convert.ToInt32(txtLocalTrackerServerPort.Text);
                    var nKClient = Convert.ToInt32(txtLocalKClientPort.Text);
                    var trackerEndPoint = new IPEndPoint(ipa, nTracker);
                    var kcEndPoint = new IPEndPoint(ipa, nKClient);

                    if (_kTracker == null)
                    {
                        _kTracker = new TrackerServer(trackerEndPoint);
                    }
                    if (_kClient == null)
                    {
                        _kClient = new KClient(_kTracker, kcEndPoint);
                        _kClient.ConnectionListChanged += _kClient_ConnectionListChanged;
                    }

                    //if (!_kTracker.IsBound)
                    //{
                    //    if (!_kTracker.TestBind(trackerEndPoint))
                    //    {
                    //        MessageBox.Show("Tracker 服务器端口不可用，请设置一个新的端口。", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    //        _kTracker = null;
                    //        return;
                    //    }
                    //}
                    //txtLocalTrackerServerPort.Enabled = false;
                    //if (!_kClient.IsBound)
                    //{
                    //    if (!_kClient.TestBind(kcEndPoint))
                    //    {
                    //        MessageBox.Show("K客户端服务器端口不可用，请设置一个新的端口。", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    //        _kClient = null;
                    //        return;
                    //    }
                    //}
                    //txtLocalKClientPort.Enabled = false;

                    if (KeiGuiOptions.Current.EnableLogging)
                    {
                        _kTracker.Logger = Program.Logger;
                        _kClient.Logger = Program.Logger;
                    }

                    if (KeiGuiOptions.Current.IsPointInsertion)
                    {
                        _kClient.FreeToGo = true;
                        _kTracker.FreeToGo = true;
                    }

                    _kTracker.Listen();
                    if (KeiGuiOptions.Current.IsPointInsertion)
                    {
                        // 接入点立即启动，普通用户要连接到接入点后才能启动
                        _kClient.Listen();
                    }

                    groupBox1.Enabled = false;
                    groupBox2.Enabled = true;

                    txtLocalKClientEndPoint.Text = _kClient.LocalKEndPoint.ToString();
                    txtLocalTrackerAddress.Text = "http://localhost:" + _kTracker.LocalEndPoint.Port.ToString() + _kTracker.AnnouceUrl.TrimEnd('?');

                    SetStatusText("服务器已启动");
                    KGState = KeiGuiState.ServersStarted;
                }
            }
        }

        private void SetStatusText(string text)
        {
            statusBar.Panels[0].Text = "状态: " + text;
        }

        private bool CheckServerFields()
        {
            try
            {
                if (KeiGuiOptions.Current.IsPointInsertion)
                {
                    var ipa = IPAddress.Parse(cboPossibleAddresses.Text.Split(' ')[0]);
                }
                int trackerPort, kcPort;
                trackerPort = Convert.ToInt32(txtLocalTrackerServerPort.Text);
                kcPort = Convert.ToInt32(txtLocalKClientPort.Text);
                if (trackerPort > IPEndPoint.MaxPort || trackerPort < IPEndPoint.MinPort)
                {
                    return false;
                }
                if (kcPort > IPEndPoint.MaxPort || trackerPort < IPEndPoint.MinPort)
                {
                    return false;
                }
                if (trackerPort == kcPort)
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        void fMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            TrySaveOptions();

            if (_kClient != null)
            {
                Thread kcWorkerThread;
                var wh = _kClient.ExitNetwork(out kcWorkerThread);
                // 最多等3秒，然后就全部销毁
                //wh.WaitOne(TimeSpan.FromSeconds(3));
                TimeSpan passedTime = TimeSpan.Zero;
                while (kcWorkerThread.IsAlive && !wh.SafeWaitHandle.IsClosed && passedTime < TimeSpan.FromSeconds(10))
                {
                    wh.WaitOne(TimeSpan.FromMilliseconds(10));
                    passedTime += TimeSpan.FromMilliseconds(10);
                }
            }
            if (_kClient != null)
            {
                try
                {
                    _kClient.Shutdown();
                }
                catch (Exception)
                {
                }
            }
            if (_kTracker != null)
            {
                try
                {
                    _kTracker.Shutdown();
                }
                catch (Exception)
                {
                }
            }
        }

        void mnuFileExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        void fMain_Load(object sender, EventArgs e)
        {
            TryReadOptions();

            // 只有普通用户才能选择启用 UPnP
            KeiGuiOptions.Current.UsePortMapping = KeiGuiOptions.Current.UsePortMapping && !KeiGuiOptions.Current.IsPointInsertion;
            PortMapping.UsePortMapping = KeiGuiOptions.Current.UsePortMapping;
            //if (KeiGuiOptions.Current.UseUPnP)
            //{
            //    ManagedUPnP.WindowsFirewall.CheckUPnPFirewallRules(this);
            //}

            if (KeiGuiOptions.Current.IsPointInsertion)
            {
                Text = Text + " (接入点)";
            }

            // 接入点是禁止通过路由的
            if (!KeiGuiOptions.Current.IsPointInsertion && KeiGuiOptions.Current.UsePortMapping)
            {
                cmdStartServers.Enabled = false;
                SetStatusText("初始化端口映射...");
                Thread t = new Thread(delegate()
                {
                    //PortMapping.InitUPnP(TimeSpan.FromSeconds(20));
                    PortMapping.InitializePortMappingEnvironment(TimeSpan.FromSeconds(5));
                    KeiGuiOptions.Current.UsePortMapping = KeiGuiOptions.Current.UsePortMapping && PortMapping.CanUsePortMapping;
                    label1.Visible = KeiGuiOptions.Current.UsePortMapping;
                    cboPossibleAddresses.Visible = KeiGuiOptions.Current.UsePortMapping;
                    // HACK: HACK
                    label1.Visible = cboPossibleAddresses.Visible = false;
                    if (KeiGuiOptions.Current.UsePortMapping)
                    {
                        SetStatusText("就绪");
                    }
                    else
                    {
                        SetStatusText("就绪 (端口映射不可用)");
                    }
                    cmdStartServers.Enabled = true;
                });
                t.IsBackground = true;
                t.Start();
            }
            else
            {
                SetStatusText("就绪");
            }

            // 如果这只小白鼠（例如我）愿意做接入点的话……那就给ta选吧
            // 设置界面项：如果是接入点（高级用户）或者是要使用 UPnP 的用户，则应该显示本地地址
            if (KeiGuiOptions.Current.IsPointInsertion || (false && KeiGuiOptions.Current.UsePortMapping))
            {
                //var possibleAddresses = Dns.GetHostEntry(IPAddress.Loopback).AddressList;
                //foreach (var address in possibleAddresses)
                //{
                //    if (address.AddressFamily == AddressFamily.InterNetwork && IPUtil.IsAddressIntranet(address))
                //    {
                //        cboPossibleAddresses.Items.Add(address.ToString());
                //    }
                //}

                var ifs = System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces();
                foreach (var face in ifs)
                {
                    switch (face.NetworkInterfaceType)
                    {
                        case System.Net.NetworkInformation.NetworkInterfaceType.Ethernet:
                        case System.Net.NetworkInformation.NetworkInterfaceType.Ethernet3Megabit:
                        case System.Net.NetworkInformation.NetworkInterfaceType.FastEthernetFx:
                        case System.Net.NetworkInformation.NetworkInterfaceType.FastEthernetT:
                        case System.Net.NetworkInformation.NetworkInterfaceType.Fddi:
                        case System.Net.NetworkInformation.NetworkInterfaceType.GigabitEthernet:
                        case System.Net.NetworkInformation.NetworkInterfaceType.Wireless80211:
                            if (face.OperationalStatus == System.Net.NetworkInformation.OperationalStatus.Up)
                            {
                                var p = face.GetIPProperties();
                                if (p.UnicastAddresses.Count > 0)
                                {
                                    foreach (var a in p.UnicastAddresses)
                                    {
                                        if (a.Address.AddressFamily == AddressFamily.InterNetwork && KeiUtil.IsAddressIntranet(a.Address))
                                        {
                                            cboPossibleAddresses.Items.Add(a.Address.ToString() + " [" + face.Name + "]");
                                        }
                                    }
                                }
                            }
                            break;
                        default:
                            break;
                    }
                }

                // 调整 combobox 的下拉宽度
                if (cboPossibleAddresses.Items.Count > 0)
                {
                    SizeF maxSizeF = SizeF.Empty;
                    using (var g = cboPossibleAddresses.CreateGraphics())
                    {
                        foreach (var item in cboPossibleAddresses.Items)
                        {
                            var s = g.MeasureString((string)item, cboPossibleAddresses.Font);
                            if (s.Width > maxSizeF.Width)
                            {
                                maxSizeF = s;
                            }
                        }
                    }
                    cboPossibleAddresses.DropDownWidth = (int)Math.Ceiling(maxSizeF.Width);
                }

                if (cboPossibleAddresses.Items.Count > 0)
                {
                    int foundIndex = -1;
                    int i = 0;
                    bool found = false;
                    foreach (var item in cboPossibleAddresses.Items)
                    {
                        if (((string)item).StartsWith(KeiGuiOptions.Current.LocalIntranetAddress))
                        {
                            found = true;
                            foundIndex = i;
                            break;
                        }
                        i++;
                    }
                    if (found)
                    {
                        cboPossibleAddresses.SelectedIndex = foundIndex;
                    }
                    else
                    {
                        cboPossibleAddresses.SelectedIndex = 0;
                    }
                }
                else
                {
                    MessageBox.Show("在您的计算机上没有找到合适的内网地址，将无法正常启动。", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                label1.Visible = false;
                cboPossibleAddresses.Visible = false;
            }
            // 设置界面项：如果是普通用户，就不需要知道本地的客户端端点了
            if (!KeiGuiOptions.Current.IsPointInsertion)
            {
                label5.Visible = false;
                txtLocalKClientEndPoint.Visible = false;
            }
            // 设置界面项：如果是接入点，则“连接”框都可以免去了
            if (false && KeiGuiOptions.Current.IsPointInsertion)
            {
                groupBox2.Visible = false;
            }

            foreach (var item in KeiGuiOptions.Current.TargetEndPoints)
            {
                cboTargetKClientEndPoint.Items.Add(item);
            }
            // 暂时不要自动切换到第0项，要不然我的机子就爆炸了……

            groupBox2.Enabled = false;
            mnuOpForceBroadcast.Enabled = false;
            ctxForceBroadcast.Enabled = false;

            notifier.Icon = this.Icon;
            notifier.ContextMenu = ctxMenu;
            notifier.Text = Application.ProductName;
            notifier.Visible = true;
        }
    }
}
