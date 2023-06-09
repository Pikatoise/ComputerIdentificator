using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ComputersCourseWork
{
    public static class Network
    {
        [DllImport("wininet.dll")]
        public static extern bool InternetSetOption(IntPtr hInternet, int dwOption, IntPtr lpBuffer, int dwBufferLength);
        public const int INTERNET_OPTION_SETTINGS_CHANGED = 39;
        public const int INTERNET_OPTION_REFRESH = 37;
        static bool settingsReturn, refreshReturn;

        public struct StaticNetworkParams
        {
            public string IP;
            public string SubnetMask;
            public string Gateway;
            public string DNS;

            public StaticNetworkParams(string ip, string subnetmask, string gateway = "", string dns = "")
            {
                this.IP = ip;
                this.SubnetMask = subnetmask;
                this.Gateway = gateway;
                this.DNS = dns;
            }
        }

        public static void SetProxy(string ip, string port)
        {
            RegistryKey registry = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Internet Settings", true);
            registry.SetValue("ProxyServer", $"{ip}:{port}");

            settingsReturn = InternetSetOption(IntPtr.Zero, INTERNET_OPTION_SETTINGS_CHANGED, IntPtr.Zero, 0);
            refreshReturn = InternetSetOption(IntPtr.Zero, INTERNET_OPTION_REFRESH, IntPtr.Zero, 0);
        }

        public static void ChangeProxyStatus(bool isEnable)
        {
            RegistryKey registry = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Internet Settings", true);
            registry.SetValue("ProxyEnable", isEnable ? 1 : 0);

            settingsReturn = InternetSetOption(IntPtr.Zero, INTERNET_OPTION_SETTINGS_CHANGED, IntPtr.Zero, 0);
            refreshReturn = InternetSetOption(IntPtr.Zero, INTERNET_OPTION_REFRESH, IntPtr.Zero, 0);
        }

        public static bool GetProxyStatus()
        {
            RegistryKey registry = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Internet Settings", true);

            return registry.GetValue("ProxyEnable").ToString().Equals("1") ? true : false;
        }

        public static string GetCurrentProxy()
        {
            RegistryKey registry = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Internet Settings", true);

            return registry.GetValue("ProxyServer").ToString();
        }

        public static StaticNetworkParams GetNetworkParams(string NIC)
        {
            string currentIP = "";
            string currentSubnetMask = "";
            string currentGateway = "";
            string currentDNS = "";

            foreach (var ip in Dns.GetHostAddresses(Dns.GetHostName()).Where(ha => ha.AddressFamily == AddressFamily.InterNetwork))
            {
                currentIP = ip.ToString();
                currentSubnetMask = GetSubnetMask(ip).ToString();
                currentDNS = GetDnsAddresses(NIC);

                var targetInterface = NetworkInterface.GetAllNetworkInterfaces().SingleOrDefault(ni => ni.GetIPProperties().UnicastAddresses.OfType<UnicastIPAddressInformation>().Any(x => x.Address.Equals(ip)));
                if (targetInterface == null)
                    continue;

                var gates = targetInterface.GetIPProperties().GatewayAddresses;
                if (gates.Count == 0)
                    continue;

                foreach (var gateAddress in gates)
                    currentGateway = $"{gateAddress.Address:10}";
            }

            return new StaticNetworkParams(currentIP,currentSubnetMask,currentGateway,currentDNS);
        }

        static string GetDnsAddresses(string NIC)
        {
            string currentDNS = "";

            NetworkInterface[] adapters = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface adapter in adapters)
            {
                IPInterfaceProperties adapterProperties = adapter.GetIPProperties();
                IPAddressCollection dnsServers = adapterProperties.DnsAddresses;
                if (dnsServers.Count > 0)
                    if (adapter.Description.Contains(NIC))
                        foreach (IPAddress dns in dnsServers)
                            currentDNS = dns.ToString();
            }

            return currentDNS;
        }

        static IPAddress GetSubnetMask(IPAddress address)
        {
            foreach (NetworkInterface adapter in NetworkInterface.GetAllNetworkInterfaces())
            {
                foreach (UnicastIPAddressInformation unicastIPAddressInformation in adapter.GetIPProperties().UnicastAddresses)
                {
                    if (unicastIPAddressInformation.Address.AddressFamily == AddressFamily.InterNetwork)
                    {
                        if (address.Equals(unicastIPAddressInformation.Address))
                        {
                            return unicastIPAddressInformation.IPv4Mask;
                        }
                    }
                }
            }
            throw new ArgumentException($"Can't find subnetmask for IP address '{address}'");
        }

        public static void SetStaticNetwork(StaticNetworkParams networkParams)
        {
            ManagementClass objMC = new ManagementClass("Win32_NetworkAdapterConfiguration");
            ManagementObjectCollection objMOC = objMC.GetInstances();

            foreach (ManagementObject objMO in objMOC)
            {
                if ((bool)objMO["IPEnabled"])
                {
                    ManagementBaseObject setIP;
                    ManagementBaseObject newIP = objMO.GetMethodParameters("EnableStatic");

                    newIP["IPAddress"] = new string[] { networkParams.IP };
                    newIP["SubnetMask"] = new string[] { networkParams.SubnetMask };

                    setIP = objMO.InvokeMethod("EnableStatic", newIP, null);

                    if (!string.IsNullOrWhiteSpace(networkParams.Gateway))
                    {
                        ManagementBaseObject setGateway;
                        ManagementBaseObject newGateway = objMO.GetMethodParameters("SetGateways");

                        newGateway["DefaultIPGateway"] = new string[] { networkParams.Gateway };
                        newGateway["GatewayCostMetric"] = new int[] { 1 };

                        setGateway = objMO.InvokeMethod("SetGateways", newGateway, null);
                    }

                    if (!string.IsNullOrWhiteSpace(networkParams.DNS))
                    {
                        ManagementBaseObject newDNS = objMO.GetMethodParameters("SetDNSServerSearchOrder");
                        newDNS["DNSServerSearchOrder"] = networkParams.DNS.Split(',');
                        ManagementBaseObject setDNS = objMO.InvokeMethod("SetDNSServerSearchOrder", newDNS, null);
                    }
                }
            }
        }

        public static bool GetDHCPStatus()
        {
            bool isEnabled = false;
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_NetworkAdapterConfiguration");

            try
            {
                foreach (ManagementObject obj in searcher.Get())
                    if (bool.Parse(obj["IPEnabled"].ToString()))
                        isEnabled = bool.Parse(obj["DHCPEnabled"].ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }


            return isEnabled;
        }

        public static void EnableDHCP()
        {
            ManagementClass objMC = new ManagementClass("Win32_NetworkAdapterConfiguration");
            ManagementObjectCollection objMOC = objMC.GetInstances();

            foreach (ManagementObject objMO in objMOC)
            {
                if ((bool)objMO["IPEnabled"])
                {
                    ManagementBaseObject enableDHCP;

                    enableDHCP = objMO.InvokeMethod("EnableDHCP", null, null);

                    ManagementBaseObject newDNS = objMO.GetMethodParameters("SetDNSServerSearchOrder");
                    newDNS["DNSServerSearchOrder"] = null;
                    ManagementBaseObject setDNS = objMO.InvokeMethod("SetDNSServerSearchOrder", newDNS, null);
                }
            }
        }
    }
}
