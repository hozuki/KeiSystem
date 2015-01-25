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
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Kei.KNetwork;
using Kei.KTracker;

namespace Kei.Tests
{
    public partial class Form1 : Form, ILogger
    {

        private TrackerServer _kTracker = null;
        private KClient _kClient = null;
        private StreamLogger _logger = null;
        private object _logger_lock = new object();

        private static readonly string Rev = "rev2";

        public Form1()
        {
            InitializeComponent();
            InitializeEventHandlers();
            CheckForIllegalCrossThreadCalls = false;
        }

        private void InitializeEventHandlers()
        {
            cmdStartServers.Click += cmdStartServers_Click;
            this.Load += Form1_Load;
            this.FormClosing += Form1_FormClosing;
            cmdConnectTargetKClient.Click += cmdConnectTargetKClient_Click;
            cmdStartRegardAsStart.Click += cmdStartRegardAsStart_Click;
            tmrRegardAsStart.Tick += tmrRegardAsStart_Tick;
        }

        void tmrRegardAsStart_Tick(object sender, EventArgs e)
        {
            tmrRegardAsStart.Enabled = false;
            _kClient.RegardNormalTrackerCommAsStarted = false;
            groupBox4.Enabled = true;
        }

        void cmdStartRegardAsStart_Click(object sender, EventArgs e)
        {
            TimeSpan ts;
            switch (cboRegardAsStartTime.SelectedIndex)
            {
                case 0:
                    ts = TimeSpan.FromMinutes(1.5);
                    break;
                default:
                    ts = TimeSpan.FromMinutes(2.5);
                    break;
            }
            tmrRegardAsStart.Interval = (int)ts.TotalMilliseconds;
            groupBox4.Enabled = false;
            _kClient.RegardNormalTrackerCommAsStarted = true;
            tmrRegardAsStart.Enabled = true;
        }

        void cmdConnectTargetKClient_Click(object sender, EventArgs e)
        {
            try
            {
                IPEndPoint ipep = new IPEndPoint(IPAddress.Parse(cboTargetKClientAddress.Text), Convert.ToInt32(txtTargetKClientPort.Text));
                Thread kcWorkerThread;
                var wh = _kClient.EnterNetwork(ipep, out kcWorkerThread);
                Thread thread = new Thread(delegate()
                {
                    Invoke(new Action(() =>
                    {
                        cmdConnectTargetKClient.Enabled = false;
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
                            cmdConnectTargetKClient.Enabled = true;
                            groupBox2.Enabled = false;
                        }));
                    }
                    else
                    {
                        Invoke(new Action(() =>
                        {
                            cmdConnectTargetKClient.Enabled = true;
                        }));
                    }
                });
                thread.IsBackground = true;
                thread.Start();
            }
            catch (Exception ex)
            {
                Log(ex.Message);
                Log(ex.StackTrace);
            }
        }

        void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
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

        void Form1_Load(object sender, EventArgs e)
        {
            _logger = StreamLogger.Create(new FileStream("log.log", FileMode.Append, FileAccess.Write));
            var possibleAddresses = Dns.GetHostEntry(IPAddress.Loopback).AddressList;
            foreach (var address in possibleAddresses)
            {
                if (address.AddressFamily == AddressFamily.InterNetwork)
                {
                    cboPossibleAddresses.Items.Add(address.ToString());
                }
            }
            if (cboPossibleAddresses.Items.Count > 0)
            {
                cboPossibleAddresses.SelectedIndex = 0;
            }

            cboRegardAsStartTime.SelectedIndex = 0;

            Log("KeiSystem 测试程序及组件，" + Rev + "。");
            groupBox2.Enabled = false;
            groupBox4.Enabled = false;
        }

        void cmdStartServers_Click(object sender, EventArgs e)
        {
            if (CheckServerFields() && _kClient == null && _kTracker == null)
            {
                var ipa = IPAddress.Parse(cboPossibleAddresses.Text);
                int trackerPort, kcPort;
                trackerPort = Convert.ToInt32(txtTrackerServerPort.Text);
                kcPort = Convert.ToInt32(txtKClientPort.Text);
                _kTracker = new TrackerServer(new IPEndPoint(ipa, trackerPort));
                _kClient = new KClient(_kTracker, new IPEndPoint(ipa, kcPort));

                _kTracker.Logger = this;
                _kClient.Logger = this;

                _kTracker.Listen();
                _kClient.Listen();

                Log("启动完成。本地参数: KClient=" + cboPossibleAddresses.Text + ":" + kcPort.ToString() + ", KTracker=" + cboPossibleAddresses.Text + ":" + trackerPort.ToString());

                txtTrackerURL.Text = "http://localhost:" + txtTrackerServerPort.Text + _kTracker.AnnouceUrl.TrimEnd('?');
                txtLocalKClientAddrAndPort.Text = cboPossibleAddresses.Text + ":" + txtKClientPort.Text;

                groupBox1.Enabled = false;
                groupBox2.Enabled = true;
                groupBox4.Enabled = true;
            }
            else
            {
                Log("本地服务器设置错误。");
            }
        }

        private bool CheckServerFields()
        {
            try
            {
                var ipa = IPAddress.Parse(cboPossibleAddresses.Text);
                int trackerPort, kcPort;
                trackerPort = Convert.ToInt32(txtTrackerServerPort.Text);
                kcPort = Convert.ToInt32(txtKClientPort.Text);
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

        public TrackerServer KTracker
        {
            get
            {
                return _kTracker;
            }
        }

        public KClient KClient
        {
            get
            {
                return _kClient;
            }
        }

        public void Log(string log)
        {
            lock (_logger_lock)
            {
                string slog = Thread.CurrentThread.ManagedThreadId.ToString() + " => " + log;
                if (_logger != null)
                {
                    _logger.Log(slog);
                }
                if (IsHandleCreated && !IsDisposed)
                {
                    if (false)
                    {
                        Invoke(new Action(() => Log2(slog)));
                    }
                    else
                    {
                        Log2(slog);
                    }
                }
            }
        }

        private void Log2(string log)
        {
            try
            {
                if (txtMessage.IsHandleCreated && !txtMessage.IsDisposed)
                {
                    txtMessage.AppendText(DateTime.Now.ToString() + Environment.NewLine);
                    txtMessage.AppendText(log + Environment.NewLine);
                }
            }
            catch (Exception)
            {
            }
        }
    }
}
