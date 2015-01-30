using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using MonoTorrent.BEncoding;

namespace Kei.TorrentNormalize
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
#if WINDOWS
            string[] commandLineArgs = null;
            BEncodedDictionary torrent = null;
            string filename = null;

            List<Tuple<string, BEncodedDictionary>> torrentList = new List<Tuple<string, BEncodedDictionary>>();

            //MessageBox.Show(Environment.CommandLine);

            // 对于 Windows 系统，cmdline[0] => appname; cmdline[1] => ...
            // 对于 Linux……好像是 cmdline[0] => ... 吧……

            try
            {
                commandLineArgs = Environment.GetCommandLineArgs();
            }
            catch (Exception)
            {
            }
            bool shouldRunConsole = false;
            if (commandLineArgs != null && commandLineArgs.Length >= 2)
            {
                for (var i = 1; i < commandLineArgs.Length; i++)
                {
                    filename = commandLineArgs[i];
                    if (File.Exists(filename))
                    {
                        try
                        {
                            using (var fs = new FileStream(filename, FileMode.Open, FileAccess.Read))
                            {
                                torrent = BEncodedDictionary.DecodeTorrent(fs);
                                torrentList.Add(new Tuple<string, BEncodedDictionary>(filename, torrent));
                                shouldRunConsole = true;
                            }
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
            }

            if (shouldRunConsole)
            {
                RunConsole(torrentList);
            }
            else
            {
#endif
                RunGui();
#if WINDOWS
            }
#endif
        }

        static void RunGui()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new fMain());
        }

        static void RunConsole(List<Tuple<string, BEncodedDictionary>> torrentList)
        {
            // 此时 filename 和 torrent 不会为 null
            foreach (var x in torrentList)
            {
                TorrentProcessor.Process(x.Item1, x.Item2);
            }
        }
    }
}
