namespace Kei.Gui
{
    partial class fMain
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            try
            {
                base.Dispose(disposing);
            }
            catch (System.Exception)
            {
            }
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(fMain));
            this.notifier = new System.Windows.Forms.NotifyIcon(this.components);
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cmdStartServers = new System.Windows.Forms.Button();
            this.txtLocalTrackerServerPort = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtLocalKClientPort = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cboPossibleAddresses = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.cmdConnectToTargetKClient = new System.Windows.Forms.Button();
            this.cboTargetKClientEndPoint = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.mainMenu = new System.Windows.Forms.MainMenu(this.components);
            this.mnuFile = new System.Windows.Forms.MenuItem();
            this.mnuFileExit = new System.Windows.Forms.MenuItem();
            this.mnuOp = new System.Windows.Forms.MenuItem();
            this.mnuOpForceBroadcast = new System.Windows.Forms.MenuItem();
            this.mnuOpSeparator1 = new System.Windows.Forms.MenuItem();
            this.mnuOpOptions = new System.Windows.Forms.MenuItem();
            this.mnuHelp = new System.Windows.Forms.MenuItem();
            this.mnuHelpContent = new System.Windows.Forms.MenuItem();
            this.mnuHelpAbout = new System.Windows.Forms.MenuItem();
            this.statusBar = new System.Windows.Forms.StatusBar();
            this.statusBarPanel1 = new System.Windows.Forms.StatusBarPanel();
            this.ctxMenu = new System.Windows.Forms.ContextMenu();
            this.ctxShowMainForm = new System.Windows.Forms.MenuItem();
            this.ctxSeparator1 = new System.Windows.Forms.MenuItem();
            this.ctxForceBroadcast = new System.Windows.Forms.MenuItem();
            this.ctxSeparator2 = new System.Windows.Forms.MenuItem();
            this.ctxExit = new System.Windows.Forms.MenuItem();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.txtLocalTrackerAddress = new System.Windows.Forms.TextBox();
            this.txtLocalKClientEndPoint = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.tmrForceBroadcast = new System.Windows.Forms.Timer(this.components);
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.lstConnectionList = new System.Windows.Forms.ListBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.statusBarPanel1)).BeginInit();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // notifier
            // 
            this.notifier.Visible = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cmdStartServers);
            this.groupBox1.Controls.Add(this.txtLocalTrackerServerPort);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.txtLocalKClientPort);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.cboPossibleAddresses);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(222, 137);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "服务器";
            // 
            // cmdStartServers
            // 
            this.cmdStartServers.Location = new System.Drawing.Point(122, 100);
            this.cmdStartServers.Name = "cmdStartServers";
            this.cmdStartServers.Size = new System.Drawing.Size(92, 28);
            this.cmdStartServers.TabIndex = 3;
            this.cmdStartServers.Text = "启动(&S)";
            this.cmdStartServers.UseVisualStyleBackColor = true;
            // 
            // txtLocalTrackerServerPort
            // 
            this.txtLocalTrackerServerPort.Location = new System.Drawing.Point(155, 73);
            this.txtLocalTrackerServerPort.Name = "txtLocalTrackerServerPort";
            this.txtLocalTrackerServerPort.Size = new System.Drawing.Size(59, 21);
            this.txtLocalTrackerServerPort.TabIndex = 2;
            this.txtLocalTrackerServerPort.Text = "9057";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 76);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(143, 12);
            this.label3.TabIndex = 4;
            this.label3.Text = "Tracker 服务器监听端口:";
            // 
            // txtLocalKClientPort
            // 
            this.txtLocalKClientPort.Location = new System.Drawing.Point(155, 46);
            this.txtLocalKClientPort.Name = "txtLocalKClientPort";
            this.txtLocalKClientPort.Size = new System.Drawing.Size(59, 21);
            this.txtLocalKClientPort.TabIndex = 1;
            this.txtLocalKClientPort.Text = "9029";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 49);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(101, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "K客户端监听端口:";
            // 
            // cboPossibleAddresses
            // 
            this.cboPossibleAddresses.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboPossibleAddresses.FormattingEnabled = true;
            this.cboPossibleAddresses.Location = new System.Drawing.Point(71, 20);
            this.cboPossibleAddresses.Name = "cboPossibleAddresses";
            this.cboPossibleAddresses.Size = new System.Drawing.Size(143, 20);
            this.cboPossibleAddresses.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(59, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "内网地址:";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.cmdConnectToTargetKClient);
            this.groupBox2.Controls.Add(this.cboTargetKClientEndPoint);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Location = new System.Drawing.Point(12, 155);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(222, 82);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "接入分布网络";
            // 
            // cmdConnectToTargetKClient
            // 
            this.cmdConnectToTargetKClient.Location = new System.Drawing.Point(122, 46);
            this.cmdConnectToTargetKClient.Name = "cmdConnectToTargetKClient";
            this.cmdConnectToTargetKClient.Size = new System.Drawing.Size(92, 28);
            this.cmdConnectToTargetKClient.TabIndex = 5;
            this.cmdConnectToTargetKClient.Text = "连接(&N)";
            this.cmdConnectToTargetKClient.UseVisualStyleBackColor = true;
            // 
            // cboTargetKClientEndPoint
            // 
            this.cboTargetKClientEndPoint.FormattingEnabled = true;
            this.cboTargetKClientEndPoint.Location = new System.Drawing.Point(59, 20);
            this.cboTargetKClientEndPoint.Name = "cboTargetKClientEndPoint";
            this.cboTargetKClientEndPoint.Size = new System.Drawing.Size(155, 20);
            this.cboTargetKClientEndPoint.TabIndex = 4;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 23);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(47, 12);
            this.label4.TabIndex = 2;
            this.label4.Text = "接入点:";
            // 
            // mainMenu
            // 
            this.mainMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.mnuFile,
            this.mnuOp,
            this.mnuHelp});
            // 
            // mnuFile
            // 
            this.mnuFile.Index = 0;
            this.mnuFile.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.mnuFileExit});
            this.mnuFile.Text = "文件(&F)";
            // 
            // mnuFileExit
            // 
            this.mnuFileExit.Index = 0;
            this.mnuFileExit.Text = "退出(&X)";
            // 
            // mnuOp
            // 
            this.mnuOp.Index = 1;
            this.mnuOp.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.mnuOpForceBroadcast,
            this.mnuOpSeparator1,
            this.mnuOpOptions});
            this.mnuOp.Text = "操作(&P)";
            // 
            // mnuOpForceBroadcast
            // 
            this.mnuOpForceBroadcast.Index = 0;
            this.mnuOpForceBroadcast.Text = "强制广播(&B)";
            // 
            // mnuOpSeparator1
            // 
            this.mnuOpSeparator1.Index = 1;
            this.mnuOpSeparator1.Text = "-";
            // 
            // mnuOpOptions
            // 
            this.mnuOpOptions.Index = 2;
            this.mnuOpOptions.Text = "选项(&O)...";
            // 
            // mnuHelp
            // 
            this.mnuHelp.Index = 2;
            this.mnuHelp.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.mnuHelpContent,
            this.mnuHelpAbout});
            this.mnuHelp.Text = "帮助(&H)";
            // 
            // mnuHelpContent
            // 
            this.mnuHelpContent.Index = 0;
            this.mnuHelpContent.Shortcut = System.Windows.Forms.Shortcut.F1;
            this.mnuHelpContent.Text = "帮助主题(&H)";
            // 
            // mnuHelpAbout
            // 
            this.mnuHelpAbout.Index = 1;
            this.mnuHelpAbout.Text = "关于(&A)";
            // 
            // statusBar
            // 
            this.statusBar.Location = new System.Drawing.Point(0, 328);
            this.statusBar.Name = "statusBar";
            this.statusBar.Panels.AddRange(new System.Windows.Forms.StatusBarPanel[] {
            this.statusBarPanel1});
            this.statusBar.ShowPanels = true;
            this.statusBar.Size = new System.Drawing.Size(503, 22);
            this.statusBar.SizingGrip = false;
            this.statusBar.TabIndex = 2;
            // 
            // statusBarPanel1
            // 
            this.statusBarPanel1.AutoSize = System.Windows.Forms.StatusBarPanelAutoSize.Spring;
            this.statusBarPanel1.Name = "statusBarPanel1";
            this.statusBarPanel1.Width = 503;
            // 
            // ctxMenu
            // 
            this.ctxMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.ctxShowMainForm,
            this.ctxSeparator1,
            this.ctxForceBroadcast,
            this.ctxSeparator2,
            this.ctxExit});
            // 
            // ctxShowMainForm
            // 
            this.ctxShowMainForm.Index = 0;
            this.ctxShowMainForm.Text = "显示(&S)";
            // 
            // ctxSeparator1
            // 
            this.ctxSeparator1.Index = 1;
            this.ctxSeparator1.Text = "-";
            // 
            // ctxForceBroadcast
            // 
            this.ctxForceBroadcast.Index = 2;
            this.ctxForceBroadcast.Text = "强制广播(&F)";
            // 
            // ctxSeparator2
            // 
            this.ctxSeparator2.Index = 3;
            this.ctxSeparator2.Text = "-";
            // 
            // ctxExit
            // 
            this.ctxExit.Index = 4;
            this.ctxExit.Text = "退出(&X)";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.txtLocalTrackerAddress);
            this.groupBox3.Controls.Add(this.txtLocalKClientEndPoint);
            this.groupBox3.Controls.Add(this.label6);
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Location = new System.Drawing.Point(12, 243);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(479, 79);
            this.groupBox3.TabIndex = 3;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "信息";
            // 
            // txtLocalTrackerAddress
            // 
            this.txtLocalTrackerAddress.Location = new System.Drawing.Point(114, 47);
            this.txtLocalTrackerAddress.Name = "txtLocalTrackerAddress";
            this.txtLocalTrackerAddress.ReadOnly = true;
            this.txtLocalTrackerAddress.Size = new System.Drawing.Size(357, 21);
            this.txtLocalTrackerAddress.TabIndex = 3;
            // 
            // txtLocalKClientEndPoint
            // 
            this.txtLocalKClientEndPoint.Location = new System.Drawing.Point(114, 20);
            this.txtLocalKClientEndPoint.Name = "txtLocalKClientEndPoint";
            this.txtLocalKClientEndPoint.ReadOnly = true;
            this.txtLocalKClientEndPoint.Size = new System.Drawing.Size(357, 21);
            this.txtLocalKClientEndPoint.TabIndex = 2;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(7, 50);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(83, 12);
            this.label6.TabIndex = 1;
            this.label6.Text = "本地 Tracker:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(7, 23);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(101, 12);
            this.label5.TabIndex = 0;
            this.label5.Text = "本地K客户端端点:";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.lstConnectionList);
            this.groupBox4.Location = new System.Drawing.Point(240, 12);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(251, 225);
            this.groupBox4.TabIndex = 4;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "连接列表";
            // 
            // lstConnectionList
            // 
            this.lstConnectionList.FormattingEnabled = true;
            this.lstConnectionList.ItemHeight = 12;
            this.lstConnectionList.Location = new System.Drawing.Point(6, 20);
            this.lstConnectionList.Name = "lstConnectionList";
            this.lstConnectionList.Size = new System.Drawing.Size(237, 196);
            this.lstConnectionList.TabIndex = 0;
            // 
            // fMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(503, 350);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.statusBar);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Menu = this.mainMenu;
            this.Name = "fMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "KeiSystem GUI";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.statusBarPanel1)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.NotifyIcon notifier;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox txtLocalTrackerServerPort;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtLocalKClientPort;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cboPossibleAddresses;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button cmdStartServers;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ComboBox cboTargetKClientEndPoint;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button cmdConnectToTargetKClient;
        private System.Windows.Forms.MainMenu mainMenu;
        private System.Windows.Forms.MenuItem mnuFile;
        private System.Windows.Forms.MenuItem mnuOp;
        private System.Windows.Forms.MenuItem mnuHelp;
        private System.Windows.Forms.MenuItem mnuFileExit;
        private System.Windows.Forms.MenuItem mnuOpForceBroadcast;
        private System.Windows.Forms.MenuItem mnuOpSeparator1;
        private System.Windows.Forms.MenuItem mnuOpOptions;
        private System.Windows.Forms.MenuItem mnuHelpContent;
        private System.Windows.Forms.MenuItem mnuHelpAbout;
        private System.Windows.Forms.StatusBar statusBar;
        private System.Windows.Forms.StatusBarPanel statusBarPanel1;
        private System.Windows.Forms.ContextMenu ctxMenu;
        private System.Windows.Forms.MenuItem ctxShowMainForm;
        private System.Windows.Forms.MenuItem ctxSeparator1;
        private System.Windows.Forms.MenuItem ctxForceBroadcast;
        private System.Windows.Forms.MenuItem ctxSeparator2;
        private System.Windows.Forms.MenuItem ctxExit;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox txtLocalTrackerAddress;
        private System.Windows.Forms.TextBox txtLocalKClientEndPoint;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Timer tmrForceBroadcast;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.ListBox lstConnectionList;
    }
}

