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
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Security.Cryptography;
using MonoTorrent.BEncoding;
using Kei.Gui;
using Kei.KTracker;

namespace Kei.TorrentNormalize
{
    public sealed partial class fMain : Form
    {

        private BEncodedDictionary torrent = null;
        /// <summary>
        /// 如果已经拖放了种子文件，而且用户可能会继续拖放操作，那么这里就该缓存文本。
        /// </summary>
        private string displayText = null;
        private bool isHandlingTorrent = false;
        private string currentTorrentFilename = null;

        private const string DROP_IN_HERE = "将一个种子文件拖放到这里";
        private const string RELEASE_YOUR_MOUSE = "松开鼠标";

        private KeiGuiOptions kgOptions = null;

        public fMain()
        {
            InitializeComponent();
            InitializeEventHandlers();
        }

        private void InitializeEventHandlers()
        {
            this.Load += fMain_Load;
            txtInfo.QueryContinueDrag += txtInfo_QueryContinueDrag;
            txtInfo.DragDrop += txtInfo_DragDrop;
            txtInfo.DragEnter += txtInfo_DragEnter;
            txtInfo.DragLeave += txtInfo_DragLeave;
            cmdGenerateNewTorrentFile.Click += cmdGenerateNewTorrentFile_Click;
        }

        void cmdGenerateNewTorrentFile_Click(object sender, EventArgs e)
        {
            bool successful = false;
            bool isFgbtTorrent = false;
            BEncodedDictionary newTorrent = new BEncodedDictionary();

            if (isHandlingTorrent && !string.IsNullOrEmpty(currentTorrentFilename) && torrent != null)
            {
                FileInfo fi = new FileInfo(currentTorrentFilename);
                // 先判断是否是未来花园的种子
                if (fi.Name.StartsWith("[FGBT]."))
                {
                    try
                    {
                        var sourceString = ((torrent["info"] as BEncodedDictionary)["source"] as BEncodedString).Text;
                        if (sourceString.StartsWith("FGBT-"))
                        {
                            // 是原来的未来花园的种子
                            isFgbtTorrent = true;
                        }
                    }
                    catch (Exception)
                    {
                    }
                }

                var kt = new TrackerServer(new IPEndPoint(IPAddress.Loopback, 10000));
                var announceString = "http://localhost:" + kgOptions.LocalTrackerServerPort.ToString() + kt.AnnouceUrl.TrimEnd('?');

                try
                {
                    newTorrent.Add("announce", announceString);
                    newTorrent.Add("announce-list", new BEncodedList()
                    {
                        new BEncodedList()
                        {
                            announceString,
                        }
                    });
                    newTorrent.Add("created by", (torrent["created by"] as BEncodedString).Text);
                    newTorrent.Add("creation date", (torrent["creation date"] as BEncodedNumber).Number);
                    // 遵循花园的原则，不转码，直接设置编码为 UTF-8
                    newTorrent.Add("encoding", "UTF-8");

                    var info = new BEncodedDictionary();
                    if (isFgbtTorrent)
                    {
                        foreach (var item in (torrent["info"] as BEncodedDictionary))
                        {
                            info.Add(item);
                        }
                    }
                    else
                    {
                        if ((torrent["info"] as BEncodedDictionary).ContainsKey("files"))
                        {
                            // 单文件的种子是没有 files 项的
                            info.Add("files", (torrent["info"] as BEncodedDictionary)["files"]);
                        }
                        else
                        {
                            // 单文件的种子有 length 项
                            info.Add("length", (torrent["info"] as BEncodedDictionary)["length"]);
                        }
                        info.Add("name", (torrent["info"] as BEncodedDictionary)["name"]);
                        info.Add("piece length", (torrent["info"] as BEncodedDictionary)["piece length"]);
                        info.Add("pieces", (torrent["info"] as BEncodedDictionary)["pieces"]);
                        info.Add("private", 1);

                        // 至于 source 呢，要经过编码……
                        var origInfo = torrent["info"].Encode();
                        var sha1 = SHA1.Create();
                        var origInfoHash = sha1.ComputeHash(origInfo);
                        sha1.Dispose();
                        var origInfoHashString = BitConverter.ToString(origInfoHash);
                        origInfoHashString = new string(origInfoHashString.Where((c) => c != '-').ToArray());
                        info.Add("source", "KS-" + origInfoHashString);
                    }

                    newTorrent.Add("info", info);
                    successful = true;
                }
                catch (Exception)
                {
                    successful = false;
                }
                kt = null;
            }

            if (successful)
            {
                var fi = new FileInfo(currentTorrentFilename);
                var newName = Path.Combine(fi.DirectoryName, "[KS]." + fi.Name);
                if (File.Exists(newName))
                {
                    var dresult = MessageBox.Show("已存在 " + newName + "，是否覆盖？", Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
                    if (dresult == DialogResult.No)
                    {
                        return;
                    }
                }
                try
                {
                    using (var fs = new FileStream(newName, FileMode.OpenOrCreate, FileAccess.Write))
                    {
                        var buf = newTorrent.Encode();
                        fs.Write(buf, 0, buf.Length);
                    }
                    MessageBox.Show("成功保存到 " + newName + "。", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("无法写入 " + newName + ":" + Environment.NewLine + ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
            else
            {
                MessageBox.Show("尝试生成并保存种子失败。", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        void txtInfo_DragLeave(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(displayText))
            {
                txtInfo.Text = displayText;
                displayText = null;
            }
            else
            {
                txtInfo.Text = DROP_IN_HERE;
            }
        }

        void txtInfo_DragEnter(object sender, DragEventArgs e)
        {
            var dataObject = e.Data as DataObject;
            if (dataObject != null)
            {
                if (dataObject.ContainsFileDropList())
                {
                    var list = dataObject.GetFileDropList();
                    bool hasTorrent = false;
                    foreach (var item in list)
                    {
                        if (item.ToLower().EndsWith(".torrent"))
                        {
                            hasTorrent = true;
                            break;
                        }
                    }
                    if (hasTorrent)
                    {
                        if (isHandlingTorrent)
                        {
                            displayText = txtInfo.Text;
                        }
                        txtInfo.Text = RELEASE_YOUR_MOUSE;
                        e.Effect = DragDropEffects.Copy;
                        return;
                    }
                }
            }
            e.Effect = DragDropEffects.None;
        }

        void txtInfo_DragDrop(object sender, DragEventArgs e)
        {
            var dataObject = e.Data as DataObject;
            if (dataObject != null)
            {
                if (dataObject.ContainsFileDropList())
                {
                    var list = dataObject.GetFileDropList();
                    bool hasTorrent = false;
                    foreach (var item in list)
                    {
                        if (item.ToLower().EndsWith(".torrent"))
                        {
                            hasTorrent = true;
                            currentTorrentFilename = item;
                            break;
                        }
                    }
                    if (hasTorrent)
                    {
                        ReadTorrent(currentTorrentFilename);
                    }
                }
            }
        }

        void txtInfo_QueryContinueDrag(object sender, QueryContinueDragEventArgs e)
        {
            e.Action = e.EscapePressed ? DragAction.Cancel : DragAction.Continue;
        }

        void fMain_Load(object sender, EventArgs e)
        {
            txtInfo.Text = DROP_IN_HERE;
            txtInfo.AllowDrop = true;
            cmdGenerateNewTorrentFile.Left = (ClientSize.Width - cmdGenerateNewTorrentFile.Width) / 2;
            cmdGenerateNewTorrentFile.Enabled = false;

            TryReadKeiGuiConfig();
        }

        private void ReadTorrent(string filename)
        {
            using (var fs = new FileStream(filename, FileMode.Open))
            {
                try
                {
                    torrent = BEncodedDictionary.DecodeTorrent(fs);
                    isHandlingTorrent = true;
                    cmdGenerateNewTorrentFile.Enabled = true;
                    ShowTorrentInformation();
                }
                catch (Exception ex)
                {
                    isHandlingTorrent = false;
                    cmdGenerateNewTorrentFile.Enabled = false;
                    txtInfo.Text = ex.Message + Environment.NewLine + DROP_IN_HERE;
                }
            }
        }

        private void ShowTorrentInformation()
        {
            try
            {
                BEncodedValue value;
                BEncodedString str;
                bool b;

                txtInfo.Clear();

                txtInfo.AppendText("种子文件: " + currentTorrentFilename + Environment.NewLine);

                b = torrent.TryGetValue("created by", out value);
                if (b)
                {
                    str = value as BEncodedString;
                    if (str != null)
                    {
                        txtInfo.AppendText("创建程序: " + str.ToString() + Environment.NewLine);
                    }
                }

                b = torrent.TryGetValue("creation date", out value);
                if (b)
                {
                    str = value as BEncodedString;
                    if (str != null)
                    {
                        string creationDateString = str.ToString();
                        try
                        {
                            long longDate;
                            longDate = Convert.ToInt64(creationDateString);
                            DateTime creationDate = DateTime.FromBinary(longDate);
                            txtInfo.AppendText("创建日期: " + creationDate.ToString() + Environment.NewLine);
                        }
                        catch (Exception)
                        {
                            txtInfo.AppendText("创建日期: (无效)" + Environment.NewLine);
                        }
                    }
                }

                b = torrent.TryGetValue("encoding", out value);
                if (b)
                {
                    str = value as BEncodedString;
                    if (str != null)
                    {
                        txtInfo.AppendText("编码: " + str.ToString() + Environment.NewLine);
                    }
                }

                // 如果不存在，那肯定不是种子文件
                value = torrent["info"];
                BEncodedDictionary dict = (BEncodedDictionary)value;
                BEncodedString torrentName = (BEncodedString)dict["name"];
                txtInfo.AppendText("种子名称: " + torrentName.ToString() + Environment.NewLine);
                BEncodedNumber pieceLength = (BEncodedNumber)dict["piece length"];
                txtInfo.AppendText("分块大小: ");
                string postfix;
                double val = 0;
                var num = pieceLength.Number;
                if (num < 1024)
                {
                    postfix = " B";
                    val = num;
                }
                else if (num < 1024 * 1024)
                {
                    postfix = " KB";
                    val = num / (double)1024;
                }
                else if (num < 1024 * 1024 * 1024)
                {
                    postfix = " MB";
                    val = num / (double)(1024 * 1024);
                }
                else
                {
                    postfix = " GB";
                    val = num / (double)(1024 * 1024 * 1024);
                }
                val = Math.Round(val, 2);
                txtInfo.AppendText(val.ToString() + postfix + Environment.NewLine);
                BEncodedString pieces = (BEncodedString)dict["pieces"];
                txtInfo.AppendText("分块数量: " + (pieces.TextBytes.Length / 20).ToString() + Environment.NewLine);
            }
            catch (Exception)
            {
                isHandlingTorrent = false;
                txtInfo.AppendText("解析种子时出现错误。");
            }
        }

        private void TryReadKeiGuiConfig()
        {
            var filename = Path.Combine(Application.StartupPath, KeiGuiOptions.DefaultConfigurationFileName);
            if (File.Exists(filename))
            {
                try
                {
                    using (var fs = new FileStream(filename, FileMode.Open, FileAccess.Read))
                    {
                        using (var xmlreader = new XmlTextReader(fs))
                        {
                            kgOptions = KeiGuiOptions.Read(xmlreader);
                        }
                    }
                }
                catch (Exception)
                {
                    kgOptions = (KeiGuiOptions)KeiGuiOptions.Default.Clone();
                }
            }
            else
            {
                kgOptions = (KeiGuiOptions)KeiGuiOptions.Default.Clone();
            }
        }
    }
}
