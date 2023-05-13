using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Windows.Forms.VisualStyles;
using System.Threading;

namespace WindowsFormsApp1
{
    internal class IPv4
    {        
        public static string TypeOfService(string tos_bits)
        {
            switch (tos_bits)
            {
                case "000":
                    return "Basse";
                    break;
                case "001":
                    return "Normale";
                    break;
                case "010":
                    return "URGENT";
                    break;
            }
            return "Unkown";
        }
        public static string Flags(string flag_bits)
        {
            switch (flag_bits)
            {
                case "000":
                    return "Fragmented - Last Fragment";
                    break;
                case "011":
                    return "Don't Fragment - More Fragment";
                    break;
                case "010":
                    return "Don't Fragment - Last Fragment";
                    break;
                case "001":
                    return "Fragmented - More Fragment";
                    break;
            }
            return "Unkown Fragment";
        }
        public static string TCP_Flags(string flag_bits)
        {
            string[] FlagMap = {"CWR", "ECN-Echo", "URG", "ACK", "PSH", "RST", "SYN", "FIN"};
            string[] Result = new string[8];
            int y = 0;
            for (int i = 0; i < flag_bits.Length; i++)
            {
                if (flag_bits[i].ToString() == "1")
                {
                    Result[y] = FlagMap[i];
                    y++;
                }
            }
            return string.Join(" ", Result);
        }

        public static IPAddress GetLocalIPAddress()
        {
            NetworkInterface[] adapters = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface adapter in adapters)
            {
                if (adapter.NetworkInterfaceType == NetworkInterfaceType.Loopback || adapter.OperationalStatus != OperationalStatus.Up)
                {
                    continue;
                }
                IPInterfaceProperties properties = adapter.GetIPProperties();
                foreach (IPAddressInformation ipAddress in properties.UnicastAddresses)
                {
                    if (ipAddress.Address.AddressFamily != AddressFamily.InterNetwork || IPAddress.IsLoopback(ipAddress.Address))
                    {
                        continue;
                    }
                    return ipAddress.Address;
                }
            }
            return IPAddress.Parse("0.0.0.0"); ;
        }

        public static IPAddress GetPublicIPAddress()
        {
            IPAddress Global_IP;
            try
            {
                using (var client = new WebClient())
                {

                    IPAddress.TryParse(client.DownloadString("http://icanhazip.com").Trim(), out Global_IP);
                    if (Global_IP != null)
                    {
                        return Global_IP;
                    }
                    return IPAddress.Parse("0.0.0.0");
                }
            }
            catch (Exception)
            {

                return IPAddress.Parse("0.0.0.0"); ;
            }
        }

        public static bool LocalCheck(IPAddress IP)
        {
            IPAddress ipAddress = IP;
            byte[] addressBytes = ipAddress.GetAddressBytes();
            int firstOctet = addressBytes[0];

            if (firstOctet == 10 || firstOctet == 172 && (addressBytes[1] & 0xf0) == 16 || firstOctet == 192 && addressBytes[1] == 168 || firstOctet == 255 && addressBytes[3] == 255)
            {
                // IP address is on a local network
                return true;
            }
            else
            {
                // IP address is on the internet
                return false;
            }
        }


        public static string Identify(IPAddress IP)
        {
            Console.WriteLine("In IPv4.Identify : " + Thread.CurrentThread.Name);
            IPAddress Local_IP = GetLocalIPAddress();
            Console.WriteLine("Identify, your local IP : "+Local_IP+ " "+Local_IP.GetType()+" " + Local_IP.GetAddressBytes()[3]);

            if (LocalCheck(IP))
            {
                if (IP.GetAddressBytes()[3] == Local_IP.GetAddressBytes()[3])
                {
                    return "Localhost";
                }
                switch (IP.GetAddressBytes()[3])
                {
                    case 1:
                        return "Node";
                    case 240:
                        return "Node";
                    case 239:
                        return "Node";
                    case 255:
                        return "NULL";
                    default:
                        return "PC";
                }
            } else
            { 
                return "Outsider";
            }
            return "NULL";
        }


    }
}
