using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;

namespace BatteryTrackerWinform
{
    public partial class Form1 : Form
    {
        int powerPercentage;

        public Form1()
        {
            InitializeComponent();
            ShowInTaskbar = false;
            WindowState = FormWindowState.Minimized;
        }

        private void toolStripMenuItemExit_Click(object sender, EventArgs e)
        {
            mainNotifyIcon.Visible = false;
            Close();
            Dispose();
            Environment.Exit(Environment.ExitCode);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            powerPercentage = (int)(SystemInformation.PowerStatus.BatteryLifePercent * 100);

            #region Update notifyIcon
            mainNotifyIcon.Icon = (Icon)typeof(ResourceIcon)
                .GetProperty($"_{powerPercentage}")
                .GetValue(null, null);
            #endregion
        }
    }
}
