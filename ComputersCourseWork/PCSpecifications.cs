using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace ComputersCourseWork
{
    public static class PCSpecifications
    {
        public enum PcType
        {
            Laptop,
            Desktop
        }

        public static string GetWindowsKey()
        {
            // Старый код
            /*string key = "";
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_OperatingSystem");

            try
            {
                foreach (ManagementObject obj in searcher.Get())
                    key = obj["SerialNumber"].ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex.Message}","Ошибка",MessageBoxButton.OK,MessageBoxImage.Error);
            }

            if (string.IsNullOrWhiteSpace(key))
                return "Неизвестно";

            return key;*/

            var key = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine,
                                          RegistryView.Default);
            const string keyPath = @"Software\Microsoft\Windows NT\CurrentVersion";
            var digitalProductId = (byte[])key.OpenSubKey(keyPath).GetValue("DigitalProductId");

            var productKey = DecodeProductKey(digitalProductId);
            return productKey;
        }

        public static string GetOperatingSystem()
        {
            string operatingSystem = "";
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_OperatingSystem");

            try
            {
                foreach (ManagementObject obj in searcher.Get())
                {
                    string[] system = obj["Caption"].ToString().Split(' ');

                    operatingSystem = $"{system[1]} {system[2]} {system[3]}";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            if (string.IsNullOrWhiteSpace(operatingSystem))
                operatingSystem = "Неизвестно";

            return operatingSystem.Trim();
        }

        static string DecodeProductKey(byte[] digitalProductId)
        {
            var key = String.Empty;
            const int keyOffset = 52;
            var isWin8 = (byte)((digitalProductId[66] / 6) & 1);
            digitalProductId[66] = (byte)((digitalProductId[66] & 0xf7) | (isWin8 & 2) * 4);

            const string digits = "BCDFGHJKMPQRTVWXY2346789";
            int last = 0;

            for (var i = 24; i >= 0; i--)
            {
                var current = 0;
                for (var j = 14; j >= 0; j--)
                {
                    current = current*256;
                    current = digitalProductId[j + keyOffset] + current;
                    digitalProductId[j + keyOffset] = (byte)(current/24);
                    current = current%24;
                    last = current;
                }
                key = digits[current] + key;
            }

            var keypart1 = key.Substring(1, last);
            const string insert = "N";
            key = key.Substring(1).Replace(keypart1, keypart1 + insert);

            if (last == 0)
                key = insert + key;

            for (var i = 5; i < key.Length; i += 6)
            {
                key = key.Insert(i, "-");
            }

            return key;
        }
        
        public static string GetPcName()
        {
            string name = "";
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_Desktop");

            try
            {
                foreach (ManagementObject obj in searcher.Get())
                    if (obj["Name"].ToString().Contains("DESKTOP"))
                        name = obj["Name"].ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            if (name.Contains("\\"))
                name = name.Substring(0, name.IndexOf("\\"));

            if (string.IsNullOrWhiteSpace(name))
                name = "Неизвестно";

            return name;
        }

        public static PcType? GetPcType()
        {
            PcType? currentType = null;
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_ComputerSystem");

            try
            {
                foreach (ManagementObject obj in searcher.Get())
                    currentType = obj["PCSystemType"].ToString().Equals("2") ? PcType.Laptop : PcType.Desktop;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            if (currentType == null)
                currentType = PcType.Desktop;

            return currentType;
        }

        public static void ChangePcName(string newName)
        {
            RegistryKey key = Registry.LocalMachine;

            string activeComputerName = "SYSTEM\\CurrentControlSet\\Control\\ComputerName\\ActiveComputerName";
            RegistryKey activeCmpName = key.CreateSubKey(activeComputerName);
            activeCmpName.SetValue("ComputerName", newName);
            activeCmpName.Close();
            string computerName = "SYSTEM\\CurrentControlSet\\Control\\ComputerName\\ComputerName";
            RegistryKey cmpName = key.CreateSubKey(computerName);
            cmpName.SetValue("ComputerName", newName);
            cmpName.Close();
            string _hostName = "SYSTEM\\CurrentControlSet\\services\\Tcpip\\Parameters\\";
            RegistryKey hostName = key.CreateSubKey(_hostName);
            hostName.SetValue("Hostname", newName);
            hostName.SetValue("NV Hostname", newName);
            hostName.Close();

            // Restart PC
            ProcessStartInfo proc = new ProcessStartInfo();
            proc.FileName = "cmd";
            proc.WindowStyle = ProcessWindowStyle.Hidden;
            proc.Arguments = "/C shutdown " + "-f -r -t 5";
            Process.Start(proc);
        }

        public static List<string> GetVideoControllers()
        {
            List<string> videocontrollers = new List<string>();

            ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_VideoController");

            try
            {
                foreach (ManagementObject obj in searcher.Get())
                    videocontrollers.Add(obj["Name"].ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            for (int i = 0; i < videocontrollers.Count; i++)
            {
                if (string.IsNullOrWhiteSpace(videocontrollers[i]))
                    videocontrollers[i] = "Неизвестно";
            }

            return videocontrollers;
        }

        public static string GetProcessor()
        {
            string processor = "";
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_Processor");

            try
            {
                foreach (ManagementObject obj in searcher.Get())
                    processor = obj["Name"].ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            if (string.IsNullOrWhiteSpace(processor))
                return "Неизвестно";

            return processor;
        }

        public static List<string> GetDisks()
        {
            List<string> disks = new List<string>();

            ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_DiskDrive");

            try
            {
                foreach (ManagementObject obj in searcher.Get())
                    disks.Add(obj["Model"].ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            for (int i = 0; i < disks.Count; i++)
            {
                if (string.IsNullOrWhiteSpace(disks[i]))
                    disks[i] = "Неизвестно";
            }
            
            return disks;
        }

        public static List<string> GetRAMS()
        {
            List<string> rams = new List<string>();

            ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PhysicalMemory");

            try
            {
                foreach (ManagementObject obj in searcher.Get())
                    rams.Add($"{obj["Manufacturer"].ToString().Trim()} {obj["PartNumber"].ToString().Trim()} {obj["Capacity"].ToString()[0]}GB");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            for (int i = 0; i < rams.Count; i++)
            {
                if (string.IsNullOrWhiteSpace(rams[i]))
                    rams[i] = "Неизвестно";
            }

            return rams;
        }

        // NIC - сетевое устройство
        public static List<string> GetNICS()
        {
            List<string> nics = new List<string>();

            ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_NetworkAdapterConfiguration");

            try
            {
                foreach (ManagementObject obj in searcher.Get())
                    if ((bool)obj["IPEnabled"])
                        nics.Add(obj["Caption"].ToString().Substring(10).Trim());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            for (int i = 0; i < nics.Count; i++)
            {
                if (string.IsNullOrWhiteSpace(nics[i]))
                    nics[i] = "Неизвестно";
            }

            return nics;
        }
    }
}
