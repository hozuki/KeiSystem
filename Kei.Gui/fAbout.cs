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
using System.Text;
using System.Windows.Forms;

namespace Kei.Gui
{
    public partial class fAbout : Form
    {
        public fAbout()
        {
            InitializeComponent();
            InitializeEventHandlers();
        }

        private void InitializeEventHandlers()
        {
            cmdClose.Click += cmdClose_Click;
            lnkProjectHomepage.LinkClicked += lnkProjectHomepage_LinkClicked;
        }

        void lnkProjectHomepage_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            const string HP = @"https://github.com/GridScience/KeiSystem/";
            Process process = new Process();
            process.StartInfo = new ProcessStartInfo(HP);
            process.Start();
        }

        void cmdClose_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
