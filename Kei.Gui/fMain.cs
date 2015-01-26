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

            txtLocalKClientPort.Text = KeiGuiOptions.Current.LocalKClientPort.ToString();
            txtLocalTrackerServerPort.Text = KeiGuiOptions.Current.LocalTrackerServerPort.ToString();
        }

        private void TrySaveOptions()
        {
            if (KGState >= KeiGuiState.ServersStarted)
            {
                KeiGuiOptions.Current.LocalIntranetAddress = cboPossibleAddresses.Text;
                KeiGuiOptions.Current.LocalKClientPort = _kClient.LocalListenEndPoint.Port;
                KeiGuiOptions.Current.LocalTrackerServerPort = _kTracker.LocalEndPoint.Port;
            }
            if (KGState >= KeiGuiState.Connected)
            {
                if (_kClient.ConnectionList.Count > 0)
                {
                    KeiGuiOptions.Current.TargetEndPoints.Clear();
                    lock (_kClient.ConnectionList)
                    {
                        foreach (var item in _kClient.ConnectionList)
                        {
                            KeiGuiOptions.Current.TargetEndPoints.Add(item.ClientLocation.ToString());
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
                            KeiGuiOptions.Current.Save(xmlwriter);
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
                IPEndPoint ipep = IPUtil.ParseIPEndPoint(cboTargetKClientEndPoint.Text);
                Thread kcWorkerThread;
                var wh = _kClient.EnterNetwork(ipep, out kcWorkerThread);
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
            if (_kClient == null && _kTracker == null)
            {
                if (CheckServerFields())
                {
                    var ipa = IPAddress.Parse(cboPossibleAddresses.Text);
                    _kTracker = new TrackerServer(new IPEndPoint(ipa, Convert.ToInt32(txtLocalTrackerServerPort.Text)));
                    _kClient = new KClient(_kTracker, new IPEndPoint(ipa, Convert.ToInt32(txtLocalKClientPort.Text)));
                    _kClient.ConnectionListChanged += _kClient_ConnectionListChanged;

                    if (KeiGuiOptions.Current.EnableLogging)
                    {
                        _kTracker.Logger = Program.Logger;
                        _kClient.Logger = Program.Logger;
                    }

                    _kTracker.Listen();
                    _kClient.Listen();

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
                var ipa = IPAddress.Parse(cboPossibleAddresses.Text);
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

            var possibleAddresses = Dns.GetHostEntry(IPAddress.Loopback).AddressList;
            foreach (var address in possibleAddresses)
            {
                if (address.AddressFamily == AddressFamily.InterNetwork && IPUtil.IsAddressIntranet(address))
                {
                    cboPossibleAddresses.Items.Add(address.ToString());
                }
            }
            if (cboPossibleAddresses.Items.Count > 0)
            {
                if (cboPossibleAddresses.Items.Contains(KeiGuiOptions.Current.LocalIntranetAddress))
                {
                    cboPossibleAddresses.Text = KeiGuiOptions.Current.LocalIntranetAddress;
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

            foreach (var item in KeiGuiOptions.Current.TargetEndPoints)
            {
                cboTargetKClientEndPoint.Items.Add(item);
            }
            // 暂时不要自动切换到第0项，要不然我的机子就爆炸了……

            groupBox2.Enabled = false;
            mnuOpForceBroadcast.Enabled = false;
            ctxForceBroadcast.Enabled = false;
            SetStatusText("就绪");

            notifier.Icon = this.Icon;
            notifier.ContextMenu = ctxMenu;
            notifier.Text = Application.ProductName;
            notifier.Visible = true;
        }
    }
}
