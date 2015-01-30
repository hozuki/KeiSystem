using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using System.Threading;

namespace Kei.Gui
{
    internal static class Program
    {

        private static StreamLogger _logger;

        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        private static void Main()
        {
            bool isOnlyInstance;
            Mutex mutex;
            mutex = new Mutex(true, "kei-gui", out isOnlyInstance);
            if (isOnlyInstance)
            {
                using (var fs = new FileStream(Path.Combine(Application.StartupPath, "KeiSystem.Debugging.log"), FileMode.Append, FileAccess.Write))
                {
                    using (_logger = StreamLogger.Create(fs))
                    {
                        Application.EnableVisualStyles();
                        Application.SetCompatibleTextRenderingDefault(false);
                        using (var fM = new fMain())
                        {
                            Application.Run(fM);
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("KeiGUI 已经启动。", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            mutex.Dispose();
        }

        internal static StreamLogger Logger
        {
            get
            {
                return _logger;
            }
        }
    }
}
