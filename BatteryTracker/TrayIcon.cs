using System;
using System.Drawing;
using System.IO;
using System.Text.Unicode;
using System.Windows.Forms;
using Microsoft.Win32;
using RegistryUtils;

namespace BatteryTracker
{
    class TrayIcon
    {
        #region Fields and Properties

        NotifyIcon MainNotifyIcon { get; }

        /// <summary>
        /// The main entry of context menu.
        /// </summary>
        ContextMenuStrip MainContextMenuStrip { get; }

        ToolStripMenuItem ChangeScanIntervalTerm { get; }

        ContextMenuStrip ScanIntervalMenuStrip { get; }

        ToolStripMenuItem HalfAMinute { get; }

        ToolStripMenuItem OneMinute { get; }

        ToolStripMenuItem TwoMinutes { get; }

        ToolStripMenuItem ExitTerm { get; }

        Timer MainTimer { get; }

        /// <summary>
        /// Remaining battery percentage.
        /// </summary>
        int powerPercentage;

        const string subKeyName = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Themes\\Personalize";

        string colorState = "dark";

        string path = "/BatteryTrackerConfig.cfg";

        #endregion

        public TrayIcon()
        {
            MainNotifyIcon = new NotifyIcon();
            MainContextMenuStrip = new ContextMenuStrip();
            ChangeScanIntervalTerm = new ToolStripMenuItem();
            ScanIntervalMenuStrip = new ContextMenuStrip();
            HalfAMinute = new ToolStripMenuItem();
            OneMinute = new ToolStripMenuItem();
            TwoMinutes = new ToolStripMenuItem();
            ExitTerm = new ToolStripMenuItem();
            MainTimer = new Timer();

            // initialize MainContextMenuStrip.
            MainContextMenuStrip.Items.AddRange(new ToolStripItem[] { ChangeScanIntervalTerm, ExitTerm });

            // initialize ChangeScanIntervalTerm.
            HalfAMinute.Text = "30s";
            HalfAMinute.Click += (o, e) => ChangeScanInterval(30000);

            OneMinute.Text = "60s";
            OneMinute.Click += (o, e) => ChangeScanInterval(60000);

            TwoMinutes.Text = "2m";
            TwoMinutes.Click += (o, e) => ChangeScanInterval(120000);

            ScanIntervalMenuStrip.Items.AddRange(new ToolStripItem[] { HalfAMinute, OneMinute, TwoMinutes });

            ChangeScanIntervalTerm.Text = "Scan Interval";
            ChangeScanIntervalTerm.DropDown = ScanIntervalMenuStrip;

            // initialize ExitTerm.
            ExitTerm.Text = "Exit";
            ExitTerm.Click += new EventHandler(ToolStripMenuItemExit_Click);

            // Initialize MainTimer.
            MainTimer.Tick += (o, e) => UpdateIcon();

            if (File.Exists(path))
            {
                using var file = new StreamReader(path);
                MainTimer.Interval = int.Parse(file.ReadLine().Trim());
            }
            else
            {
                MainTimer.Interval = 30000;
            }

            MainTimer.Start();

            // Initialize MainNotifyIcon.
            MainNotifyIcon.ContextMenuStrip = MainContextMenuStrip;
            MainNotifyIcon.Text = $"Scan Interval: {MainTimer.Interval / 1000} s";
            MainNotifyIcon.Visible = true;

            // Initialize color mode.
            DetectColorMode();
            UpdateIcon();

            // Listening to change of color mode.
            var monitor = new RegistryMonitor(
                RegistryHive.CurrentUser,
                subKeyName);

            monitor.RegChanged += (o, e) => { DetectColorMode(); UpdateIcon(); };
            monitor.Start();
        }

        private void ToolStripMenuItemExit_Click(object sender, EventArgs e)
        {
            MainNotifyIcon.Visible = false;
            MainNotifyIcon.Dispose();
            Environment.Exit(Environment.ExitCode);
        }

        private void ChangeScanInterval(int newInterval)
        {
            MainTimer.Interval = newInterval;
            MainNotifyIcon.Text = $"Scan Interval: {MainTimer.Interval / 1000} s";

            // Write the scan interval into a config file.
            using var file = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            using var config = new StreamWriter(file);
            config.Write(newInterval);
        }

        /// <summary>
        /// Detects remaining percentage and updates the tray icon.
        /// </summary>
        private void UpdateIcon()
        {
            powerPercentage = Convert.ToInt16(SystemInformation.PowerStatus.BatteryLifePercent * 100);

            MainNotifyIcon.Icon = (Icon)typeof(ResourceIcon)
                .GetProperty($"{colorState}_{powerPercentage}")
                .GetValue(null, null);
        }

        /// <summary>
        /// A function that detect current color mode by look up in the corresponding registry key.
        /// </summary>
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
