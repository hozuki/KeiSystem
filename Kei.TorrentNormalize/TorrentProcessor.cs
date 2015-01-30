using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Windows.Forms;
using System.Security.Cryptography;
using System.Xml;
using MonoTorrent.BEncoding;
using Kei.Gui;
using Kei.KTracker;

namespace Kei.TorrentNormalize
{
    public static class TorrentProcessor
    {

        private static KeiGuiOptions kgOptions;

        private static void TryReadKeiGuiConfig()
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

        public static void Process(string inputFilename, BEncodedDictionary torrent)
        {
            bool successful = false;
            bool isFgbtTorrent = false;
            BEncodedDictionary newTorrent = new BEncodedDictionary();

            TryReadKeiGuiConfig();

            if (true && !string.IsNullOrEmpty(inputFilename) && torrent != null)
            {
                FileInfo fi = new FileInfo(inputFilename);
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
                var fi = new FileInfo(inputFilename);
                var newName = Path.Combine(fi.DirectoryName, "[KS]." + fi.Name);
                // 命令行状态下不提示
                //if (File.Exists(newName))
                //{
                //    var dresult = MessageBox.Show("已存在 " + newName + "，是否覆盖？", Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
                //    if (dresult == DialogResult.No)
                //    {
                //        return;
                //    }
                //}
                try
                {
                    using (var fs = new FileStream(newName, FileMode.OpenOrCreate, FileAccess.Write))
                    {
                        var buf = newTorrent.Encode();
                        fs.Write(buf, 0, buf.Length);
                    }
                    Console.WriteLine("成功保存到 " + newName + "。");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("无法写入 " + newName + ": " + ex.Message);
                }
            }
            else
            {
                Console.WriteLine("尝试生成并保存种子失败。");
            }
        }

    }
}
