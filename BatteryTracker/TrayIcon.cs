using System;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Win32;
using RegistryUtils;

namespace BatteryTracker
{
    class TrayIcon
    {
        NotifyIcon MainNotifyIcon { get; }
        ContextMenuStrip MainContextMenuStrip { get; }
        ToolStripMenuItem ToolStripMenuItemExit { get; }
        Timer MainTimer { get; }

        int powerPercentage;

        const string subKeyName = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Themes\\Personalize";
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
            MainTimer.Start();

            // Initialize color mode.
            DetectColorMode();
            UpdateIcon();

            // Listening to change of color mode.
            var monitor = new RegistryMonitor(
                RegistryHive.CurrentUser,
                subKeyName);
            monitor.RegChanged += new EventHandler(OnRegChanged);
            monitor.Start();
        }

        private void OnRegChanged(object sender, EventArgs e)
        {
            DetectColorMode();
            UpdateIcon();
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
            UpdateIcon();
        }

        private void UpdateIcon()
        {
            powerPercentage = Convert.ToInt16(SystemInformation.PowerStatus.BatteryLifePercent * 100);
            MainNotifyIcon.Icon = (Icon)typeof(ResourceIcon)
                .GetProperty($"{colorState}_{powerPercentage}")
                .GetValue(null, null);
            MainNotifyIcon.Text = $"remaining: {powerPercentage}%";
        }

        private void DetectColorMode()
        {
            using var key = Registry.CurrentUser.OpenSubKey(subKeyName);
            var info = int.TryParse(
                key.GetValue("SystemUsesLightTheme").ToString(),
                out int result);

            if (info)
            {
                colorState = result switch
                {
                    0 => "dark",
                    1 => "light",
                    _ => throw new ArgumentException($"The argument {result} is not valid.")
                };
            }
        }
    }
}
