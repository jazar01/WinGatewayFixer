/*
 * WinGatewayFixer checks the specified network adapter to see if it has
 * at least one gateway specified.  If not the adapter is set to DHCP.
 * 
 * This program was written because my computer was constantly losing its
 * gateway for no apparent reason. 
 * 
 * Usage: 
 * 
 * WinGatewayFixer "Adapter" [-q]
 * 
 * the optional -q causes this program to run in silent mode.
 * 
 * JRA March 16, 2018
 * 
 */


using System;
using System.Diagnostics;
using System.Net.NetworkInformation;


namespace WinGatewayFixer
{
    class Program
    {
        static int Main(string[] args)
        {
            string AdapterName;
            if (args.Length == 0)
            {
                Console.WriteLine("WinGatewayFixer - Must specify adapter");
                return -1;
            }
            else if (string.IsNullOrWhiteSpace(args[0]))
            {
                Console.WriteLine("WinGatewayFixer - Adapter cannot be null or empty");
                return -2;
            }
            else
              AdapterName = args[0];

            bool quiet;

            if (args.Length == 1)
                quiet = false;
            else
            {
                 
                switch (args[1].ToLower())
                    {
                    case "-s":
                    case "-q":
                    case "--silent":
                    case "--quiet":
                        quiet = true;
                        break;
                    default:
                        quiet = false;
                        break;
                }
            }

            if (!quiet) Console.WriteLine("WinGatewayFixer Starting");

            IPGlobalProperties computerProperties = IPGlobalProperties.GetIPGlobalProperties();
            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
            GatewayIPAddressInformationCollection gatewayAddresses;

            foreach (NetworkInterface adapter in nics)
            {
                if (!quiet) Console.WriteLine("Checking adapter: " + adapter.Name);
                if (AdapterName == adapter.Name)
                {
                    IPInterfaceProperties properties = adapter.GetIPProperties();
                    UnicastIPAddressInformationCollection uniCast = properties.UnicastAddresses;

                    gatewayAddresses = properties.GatewayAddresses;

                    bool fix = true;

                    if (gatewayAddresses.Count == 0)
                        fix = true;
                    else foreach (GatewayIPAddressInformation gatewayAddress in gatewayAddresses)
                            if (gatewayAddress.Address != new System.Net.IPAddress(new byte[] { 0, 0, 0, 0 }))
                                fix = false;

                    if (fix)
                    {
                        if (!quiet) Console.WriteLine("Fixing: " + AdapterName);
                        SetDHCP(adapter.Name);
                    }
                }          
            }
        return 0;
        }

            static void SetDHCP(string AdapterName)
            {
                Process p = new Process();
                ProcessStartInfo psi = new ProcessStartInfo("netsh", "interface ip set address \"" + AdapterName + "\" dhcp");
                p.StartInfo = psi;
                p.Start();

            }


        
    }
}
