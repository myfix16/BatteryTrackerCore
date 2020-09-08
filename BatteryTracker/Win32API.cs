using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace BatteryTracker
{
/*    internal class Win32API
    {
        /// <summary>
        /// The function retrieves system power status.
        /// </summary>
        /// <param name="sps"></param>
        /// <returns>
        ///     If the function succeeds, the return value is nonzero. Otherwise, the return value is zero.
        /// </returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool GetSystemPowerStatus(out SystemPowerStatus sps);

        /// <summary>
        /// Returns true if a an AC Power Line is detected
        /// </summary>
        public static bool ACPowerPluggedIn()
        {
            SystemPowerStatus SPS = new SystemPowerStatus();
            GetSystemPowerStatus(out SPS);

            return (SPS.LineStatus == ACLineStatus.Online);
        }

        /// <summary>
        /// Returns an integer representing the life time of battery charge remaining.
        /// </summary>
        public static int GetBatteryLifeTime()
        {
            SystemPowerStatus SPS = new SystemPowerStatus();
            GetSystemPowerStatus(out SPS);

            return (int)SPS.BatteryLifeTime;
        }
    }

    public enum ACLineStatus : byte
    {
        Offline = 0,
        Online = 1,
        Unknown = 255
    }

    public enum BatteryFlag : byte
    {
        High = 1,
        Low = 2,
        Critical = 4,
        Charging = 8,
        NoSystemBattery = 128,
        Unknown = 255
    }

    public struct SystemPowerStatus
    {
        public ACLineStatus LineStatus;
        public BatteryFlag flgBattery;
        public byte BatteryLifePercent;
        public byte Reserved1;
        public int BatteryLifeTime;
        public int BatteryFullLifeTime;
    }*/
}
