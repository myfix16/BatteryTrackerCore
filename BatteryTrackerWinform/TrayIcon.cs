using System;
using System.Drawing;
using System.Windows.Forms;

namespace BatteryTrackerWinform
{
    class TrayIcon
    {
        NotifyIcon MainNotifyIcon { get; }
        ContextMenuStrip MainContextMenuStrip { get; }
        ToolStripMenuItem ToolStripMenuItemExit { get; }
        Timer MainTimer { get; }

        int powerPercentage;
        string colorState = "dark";

        public TrayIcon()
        {
            MainNotifyIcon = new NotifyIcon();
            MainContextMenuStrip = new ContextMenuStrip();
            ToolStripMenuItemExit = new ToolStripMenuItem();
            MainTimer = new Timer();

            // initialize MainContextMenuStrip.
            MainContextMenuStrip.Items.AddRange(new ToolStripItem[] { ToolStripMenuItemExit });

            // initialize ToolStripMenuItemExit.
            ToolStripMenuItemExit.Text = "Exit";
            ToolStripMenuItemExit.Click += new EventHandler(ToolStripMenuItemExit_Click);

            // Initialize MainNotifyIcon.
            MainNotifyIcon.ContextMenuStrip = MainContextMenuStrip;
            MainNotifyIcon.Visible = true;

            // Initialize MainTimer.

            MainTimer.Tick += new EventHandler(MainTimer_Tick);
            MainTimer.Interval = 30000;
            MainTimer_Tick(null, null);
            MainTimer.Start();
        }

        private void ToolStripMenuItemExit_Click(object sender, EventArgs e)
        {
            MainNotifyIcon.Visible = false;
            MainNotifyIcon.Dispose();
            //Application.Exit();
            Environment.Exit(Environment.ExitCode);
        }

        private void MainTimer_Tick(object sender, EventArgs e)
        {
            powerPercentage = Convert.ToInt16(SystemInformation.PowerStatus.BatteryLifePercent * 100);

            #region Update notifyIcon
            MainNotifyIcon.Icon = (Icon)typeof(ResourceIcon)
                .GetProperty($"{colorState}_{powerPercentage}")
                .GetValue(null, null);
            MainNotifyIcon.Text = $"remaining: {powerPercentage}%";
            #endregion
        }
    }
}
