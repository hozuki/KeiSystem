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

namespace Kei.Gui
{
    public partial class fOptions : Form
    {
        public fOptions()
        {
            InitializeComponent();
            InitializeEventHandlers();
        }

        private void InitializeEventHandlers()
        {
            this.Load += fOptions_Load;
            cmdOK.Click += cmdOK_Click;
            cmdCancel.Click += cmdCancel_Click;
            chkBeAPointInsertion.CheckedChanged += chkBeAPointInsertion_CheckedChanged;
        }

        void chkBeAPointInsertion_CheckedChanged(object sender, EventArgs e)
        {
            if (chkBeAPointInsertion.Checked)
            {
                chkUsePortMapping.Checked = false;
                chkUsePortMapping.Enabled = false;
            }
            else
            {
                chkUsePortMapping.Enabled = true;
                chkUsePortMapping.Checked = KeiGuiOptions.Modified.UsePortMapping;
            }
        }

        void cmdCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        void cmdOK_Click(object sender, EventArgs e)
        {
            KeiGuiOptions.Modified.EnableLogging = chkEnableLogging.Checked;
            KeiGuiOptions.Modified.ForceBroadcastTime = cboForceBroadcastTime.SelectedIndex == 0 ? TimeSpan.FromMinutes(1.5) : TimeSpan.FromMinutes(2);
            KeiGuiOptions.Modified.IsPointInsertion = chkBeAPointInsertion.Checked;
            KeiGuiOptions.Modified.UsePortMapping = chkUsePortMapping.Checked;
            Close();
        }

        void fOptions_Load(object sender, EventArgs e)
        {
            chkEnableLogging.Checked = KeiGuiOptions.Modified.EnableLogging;
            chkBeAPointInsertion.Checked = KeiGuiOptions.Modified.IsPointInsertion;
            chkUsePortMapping.Checked = KeiGuiOptions.Modified.UsePortMapping;
            if (KeiGuiOptions.Modified.ForceBroadcastTime.TotalMinutes == 1.5)
            {
                cboForceBroadcastTime.SelectedIndex = 0;
            }
            else
            {
                cboForceBroadcastTime.SelectedIndex = 1;
            }

        }
    }
}
