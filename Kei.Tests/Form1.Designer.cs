namespace Kei.Tests
{
    partial class Form1
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
            _logger.Dispose();
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            // TODO: 这个地方很危险……但是还无法保证 baase.Dispose(bool) 的时候不出错……
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cmdStartServers = new System.Windows.Forms.Button();
            this.txtKClientPort = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtTrackerServerPort = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cboPossibleAddresses = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.cboTargetKClientAddress = new System.Windows.Forms.ComboBox();
            this.cmdConnectTargetKClient = new System.Windows.Forms.Button();
            this.txtTargetKClientPort = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.txtMessage = new System.Windows.Forms.TextBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.txtLocalKClientAddrAndPort = new System.Windows.Forms.TextBox();
            this.txtTrackerURL = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.tmrRegardAsStart = new System.Windows.Forms.Timer(this.components);
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.label8 = new System.Windows.Forms.Label();
            this.cboRegardAsStartTime = new System.Windows.Forms.ComboBox();
            this.cmdStartRegardAsStart = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cmdStartServers);
            this.groupBox1.Controls.Add(this.txtKClientPort);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.txtTrackerServerPort);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.cboPossibleAddresses);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(307, 184);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "本地服务器";
            // 
            // cmdStartServers
            // 
            this.cmdStartServers.Location = new System.Drawing.Point(185, 137);
            this.cmdStartServers.Name = "cmdStartServers";
            this.cmdStartServers.Size = new System.Drawing.Size(104, 33);
            this.cmdStartServers.TabIndex = 7;
            this.cmdStartServers.Text = "启动(&S)";
            this.cmdStartServers.UseVisualStyleBackColor = true;
            // 
            // txtKClientPort
            // 
            this.txtKClientPort.Location = new System.Drawing.Point(144, 99);
            this.txtKClientPort.Name = "txtKClientPort";
            this.txtKClientPort.Size = new System.Drawing.Size(94, 21);
            this.txtKClientPort.TabIndex = 6;
            this.txtKClientPort.Text = "9029";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(19, 102);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(77, 12);
            this.label3.TabIndex = 5;
            this.label3.Text = "K客户端端口:";
            // 
            // txtTrackerServerPort
            // 
            this.txtTrackerServerPort.Location = new System.Drawing.Point(144, 68);
            this.txtTrackerServerPort.Name = "txtTrackerServerPort";
            this.txtTrackerServerPort.Size = new System.Drawing.Size(94, 21);
            this.txtTrackerServerPort.TabIndex = 4;
            this.txtTrackerServerPort.Text = "9057";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(19, 71);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(119, 12);
            this.label2.TabIndex = 3;
            this.label2.Text = "Tracker 服务器端口:";
            // 
            // cboPossibleAddresses
            // 
            this.cboPossibleAddresses.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboPossibleAddresses.FormattingEnabled = true;
            this.cboPossibleAddresses.Location = new System.Drawing.Point(96, 32);
            this.cboPossibleAddresses.Name = "cboPossibleAddresses";
            this.cboPossibleAddresses.Size = new System.Drawing.Size(193, 20);
            this.cboPossibleAddresses.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(19, 35);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(71, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "局域网地址:";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.cboTargetKClientAddress);
            this.groupBox2.Controls.Add(this.cmdConnectTargetKClient);
            this.groupBox2.Controls.Add(this.txtTargetKClientPort);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Location = new System.Drawing.Point(12, 202);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(307, 143);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "连接";
            // 
            // cboTargetKClientAddress
            // 
            this.cboTargetKClientAddress.FormattingEnabled = true;
            this.cboTargetKClientAddress.Items.AddRange(new object[] {
            "192.168.46.38"});
            this.cboTargetKClientAddress.Location = new System.Drawing.Point(86, 28);
            this.cboTargetKClientAddress.Name = "cboTargetKClientAddress";
            this.cboTargetKClientAddress.Size = new System.Drawing.Size(203, 20);
            this.cboTargetKClientAddress.TabIndex = 9;
            // 
            // cmdConnectTargetKClient
            // 
            this.cmdConnectTargetKClient.Location = new System.Drawing.Point(185, 97);
            this.cmdConnectTargetKClient.Name = "cmdConnectTargetKClient";
            this.cmdConnectTargetKClient.Size = new System.Drawing.Size(104, 33);
            this.cmdConnectTargetKClient.TabIndex = 8;
            this.cmdConnectTargetKClient.Text = "连接(&N)";
            this.cmdConnectTargetKClient.UseVisualStyleBackColor = true;
            // 
            // txtTargetKClientPort
            // 
            this.txtTargetKClientPort.Location = new System.Drawing.Point(144, 61);
            this.txtTargetKClientPort.Name = "txtTargetKClientPort";
            this.txtTargetKClientPort.Size = new System.Drawing.Size(94, 21);
            this.txtTargetKClientPort.TabIndex = 5;
            this.txtTargetKClientPort.Text = "9029";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(21, 64);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(59, 12);
            this.label5.TabIndex = 2;
            this.label5.Text = "对方端口:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(21, 31);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(59, 12);
            this.label4.TabIndex = 0;
            this.label4.Text = "对方地址:";
            // 
            // txtMessage
            // 
            this.txtMessage.Location = new System.Drawing.Point(325, 12);
            this.txtMessage.Multiline = true;
            this.txtMessage.Name = "txtMessage";
            this.txtMessage.ReadOnly = true;
            this.txtMessage.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtMessage.Size = new System.Drawing.Size(492, 501);
            this.txtMessage.TabIndex = 2;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.txtLocalKClientAddrAndPort);
            this.groupBox3.Controls.Add(this.txtTrackerURL);
            this.groupBox3.Controls.Add(this.label7);
            this.groupBox3.Controls.Add(this.label6);
            this.groupBox3.Location = new System.Drawing.Point(13, 352);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(306, 100);
            this.groupBox3.TabIndex = 3;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "信息";
            // 
            // txtLocalKClientAddrAndPort
            // 
            this.txtLocalKClientAddrAndPort.Location = new System.Drawing.Point(103, 56);
            this.txtLocalKClientAddrAndPort.Name = "txtLocalKClientAddrAndPort";
            this.txtLocalKClientAddrAndPort.ReadOnly = true;
            this.txtLocalKClientAddrAndPort.Size = new System.Drawing.Size(185, 21);
            this.txtLocalKClientAddrAndPort.TabIndex = 3;
            // 
            // txtTrackerURL
            // 
            this.txtTrackerURL.Location = new System.Drawing.Point(103, 29);
            this.txtTrackerURL.Name = "txtTrackerURL";
            this.txtTrackerURL.ReadOnly = true;
            this.txtTrackerURL.Size = new System.Drawing.Size(185, 21);
            this.txtTrackerURL.TabIndex = 2;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(20, 59);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(77, 12);
            this.label7.TabIndex = 1;
            this.label7.Text = "本地K客户端:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(20, 32);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(77, 12);
            this.label6.TabIndex = 0;
            this.label6.Text = "Tracker URL:";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.cmdStartRegardAsStart);
            this.groupBox4.Controls.Add(this.cboRegardAsStartTime);
            this.groupBox4.Controls.Add(this.label8);
            this.groupBox4.Location = new System.Drawing.Point(12, 458);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(307, 55);
            this.groupBox4.TabIndex = 4;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "操作";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(19, 27);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(83, 12);
            this.label8.TabIndex = 0;
            this.label8.Text = "短时强制广播:";
            // 
            // cboRegardAsStartTime
            // 
            this.cboRegardAsStartTime.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboRegardAsStartTime.FormattingEnabled = true;
            this.cboRegardAsStartTime.Items.AddRange(new object[] {
            "1.5分钟",
            "2.5分钟"});
            this.cboRegardAsStartTime.Location = new System.Drawing.Point(104, 24);
            this.cboRegardAsStartTime.Name = "cboRegardAsStartTime";
            this.cboRegardAsStartTime.Size = new System.Drawing.Size(83, 20);
            this.cboRegardAsStartTime.TabIndex = 1;
            // 
            // cmdStartRegardAsStart
            // 
            this.cmdStartRegardAsStart.Location = new System.Drawing.Point(193, 22);
            this.cmdStartRegardAsStart.Name = "cmdStartRegardAsStart";
            this.cmdStartRegardAsStart.Size = new System.Drawing.Size(96, 23);
            this.cmdStartRegardAsStart.TabIndex = 2;
            this.cmdStartRegardAsStart.Text = "计时开始(&E)";
            this.cmdStartRegardAsStart.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(829, 525);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.txtMessage);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "KeiSystem Tests";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox cboPossibleAddresses;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtKClientPort;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtTrackerServerPort;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button cmdStartServers;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox txtTargetKClientPort;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button cmdConnectTargetKClient;
        private System.Windows.Forms.TextBox txtMessage;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox txtLocalKClientAddrAndPort;
        private System.Windows.Forms.TextBox txtTrackerURL;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox cboTargetKClientAddress;
        private System.Windows.Forms.Timer tmrRegardAsStart;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Button cmdStartRegardAsStart;
        private System.Windows.Forms.ComboBox cboRegardAsStartTime;
        private System.Windows.Forms.Label label8;
    }
}

