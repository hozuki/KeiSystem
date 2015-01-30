namespace Kei.Gui
{
    partial class fOptions
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(fOptions));
            this.chkEnableLogging = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cboForceBroadcastTime = new System.Windows.Forms.ComboBox();
            this.cmdOK = new System.Windows.Forms.Button();
            this.cmdCancel = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.chkBeAPointInsertion = new System.Windows.Forms.CheckBox();
            this.chkUsePortMapping = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // chkEnableLogging
            // 
            this.chkEnableLogging.AutoSize = true;
            this.chkEnableLogging.Location = new System.Drawing.Point(25, 21);
            this.chkEnableLogging.Name = "chkEnableLogging";
            this.chkEnableLogging.Size = new System.Drawing.Size(114, 16);
            this.chkEnableLogging.TabIndex = 0;
            this.chkEnableLogging.Text = "启用日志记录(&L)";
            this.chkEnableLogging.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(23, 49);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(83, 12);
            this.label1.TabIndex = 4;
            this.label1.Text = "强制广播时间:";
            // 
            // cboForceBroadcastTime
            // 
            this.cboForceBroadcastTime.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboForceBroadcastTime.FormattingEnabled = true;
            this.cboForceBroadcastTime.Items.AddRange(new object[] {
            "1.5 分钟",
            "2 分钟"});
            this.cboForceBroadcastTime.Location = new System.Drawing.Point(112, 46);
            this.cboForceBroadcastTime.Name = "cboForceBroadcastTime";
            this.cboForceBroadcastTime.Size = new System.Drawing.Size(109, 20);
            this.cboForceBroadcastTime.TabIndex = 1;
            // 
            // cmdOK
            // 
            this.cmdOK.Location = new System.Drawing.Point(54, 201);
            this.cmdOK.Name = "cmdOK";
            this.cmdOK.Size = new System.Drawing.Size(90, 26);
            this.cmdOK.TabIndex = 2;
            this.cmdOK.Text = "确定";
            this.cmdOK.UseVisualStyleBackColor = true;
            // 
            // cmdCancel
            // 
            this.cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cmdCancel.Location = new System.Drawing.Point(150, 201);
            this.cmdCancel.Name = "cmdCancel";
            this.cmdCancel.Size = new System.Drawing.Size(90, 26);
            this.cmdCancel.TabIndex = 3;
            this.cmdCancel.Text = "取消";
            this.cmdCancel.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(79, 177);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(161, 12);
            this.label2.TabIndex = 5;
            this.label2.Text = "* 选项在下次程序启动时生效";
            // 
            // chkBeAPointInsertion
            // 
            this.chkBeAPointInsertion.AutoSize = true;
            this.chkBeAPointInsertion.Location = new System.Drawing.Point(25, 73);
            this.chkBeAPointInsertion.Name = "chkBeAPointInsertion";
            this.chkBeAPointInsertion.Size = new System.Drawing.Size(126, 16);
            this.chkBeAPointInsertion.TabIndex = 6;
            this.chkBeAPointInsertion.Text = "作为接入点启动(&N)";
            this.chkBeAPointInsertion.UseVisualStyleBackColor = true;
            // 
            // chkUsePortMapping
            // 
            this.chkUsePortMapping.AutoSize = true;
            this.chkUsePortMapping.Location = new System.Drawing.Point(44, 95);
            this.chkUsePortMapping.Name = "chkUsePortMapping";
            this.chkUsePortMapping.Size = new System.Drawing.Size(156, 16);
            this.chkUsePortMapping.TabIndex = 7;
            this.chkUsePortMapping.Text = "尝试端口映射(&P) (beta)";
            this.chkUsePortMapping.UseVisualStyleBackColor = true;
            // 
            // fOptions
            // 
            this.AcceptButton = this.cmdOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cmdCancel;
            this.ClientSize = new System.Drawing.Size(252, 239);
            this.Controls.Add(this.chkUsePortMapping);
            this.Controls.Add(this.chkBeAPointInsertion);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cmdCancel);
            this.Controls.Add(this.cmdOK);
            this.Controls.Add(this.cboForceBroadcastTime);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.chkEnableLogging);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "fOptions";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "KeiSystem GUI 选项";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox chkEnableLogging;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cboForceBroadcastTime;
        private System.Windows.Forms.Button cmdOK;
        private System.Windows.Forms.Button cmdCancel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox chkBeAPointInsertion;
        private System.Windows.Forms.CheckBox chkUsePortMapping;
    }
}