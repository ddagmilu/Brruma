using System;
using System.Collections;// ArrayList, etc.
using System.Runtime.InteropServices;// DllImport, StructLayout, etc.
using System.Text; // StringBuilder, etc.
using System.Windows.Forms; // form, MessageBox, etc.
using System.Threading; // Thread
using System.Linq;
using System.Collections.Generic;
using System.IO;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Sockets;
using System.Linq.Expressions;
using PacketDotNet;
using Newtonsoft.Json.Schema;
using static System.Net.Mime.MediaTypeNames;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Tab;
using System.Diagnostics;
using System.Net.NetworkInformation;
using WindowsFormsApp1.Properties;
//using System.Text.Json;

//using PacketDotNet;
//using SharpPcap;




namespace WindowsFormsApp1
{
    internal class Capture
    {
        public delegate void Disp(); //déclaration d'un type délégué appelé Disp

        //Form1 Form = new Form1();

        Thread ListenThread = null;
        IntPtr header = IntPtr.Zero;
        IntPtr data = IntPtr.Zero;

        ArrayList deviceList = new ArrayList();
        string device_name;
        IntPtr pcap_t;
        
        Form1 fm1;
        Represent Rep = new Represent();
        int cmpt = 0;
        public static System.Windows.Forms.TreeView Traffic_Tree_View;
        public static System.Windows.Forms.TabControl tabControl1;
        public static RichTextBox TrafficRTBox_RawPacket;
        public static System.Windows.Forms.ListView TrafficListView; 

        public static int IPv4_length = 0;
        public static int Packet_Length = 0;
        public static int tcp_udp_Length = 0;

        public static bool StopListenCheck = false;

        public static string src;
        public static string dest;
        public static string protocol;
        public static string length;
        public static string description;

        List<byte[]> Headers = new List<byte[]>();
        List<byte[]> Datas = new List<byte[]>();
        List<Object> Hosts = new List<Object>();



        //   --------------------------------------- !!!  [ CHANGE PATH ] !!! ---------------------------------------
        //static string EtherTypesString = File.ReadAllText(@"C:\Users\AthKa\Desktop\NetworkAnalyser\EtherTypes.json");
        static byte[] EtherTypesString = Properties.Resources.EtherTypes;
        //static byte[] PortsString = File.ReadAllText(@"C:\Users\AthKa\Desktop\NetworkAnalyser\ports.json");
        static byte[] PortsString = Properties.Resources.ports;
        //   --------------------------------------------------------------------------------------------------------


        dynamic EtherTypesMap = JsonConvert.DeserializeObject(Encoding.ASCII.GetString(EtherTypesString));
        dynamic PortsMap = JsonConvert.DeserializeObject(Encoding.ASCII.GetString(PortsString));

        //------------------------     Déclaration des DLLs    -------------------------------------------------------
        [DllImport("wpcap.dll", CharSet = CharSet.Ansi)] //C:\Windows\System32\Npcap
        private extern static IntPtr pcap_open_live(string dev, int packetLen, short mode, short timeout, StringBuilder errbuf);

        [DllImport("wpcap.dll", CharSet = CharSet.Ansi)]
        private extern static int pcap_findalldevs(ref IntPtr alldevs, StringBuilder errbuf);

        [DllImport("wpcap.dll", CharSet = CharSet.Ansi)]
        private extern static void pcap_freealldevs(IntPtr alldevs);

        [DllImport("wpcap.dll")]
        private static extern int pcap_next_ex(IntPtr p, ref IntPtr pkt_header, ref IntPtr packetdata);

        [DllImport("wpcap.dll")]
        private static extern void pcap_close(IntPtr p);

        //[StructLayout(LayoutKind.Sequential)]
        //public string 
        //    {}
        [StructLayout(LayoutKind.Sequential)]
        public struct pcap_if
        {
            public IntPtr Next;
            public string Name;
            public string Description;
            public IntPtr Addresses;
            public uint Flags;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct pcap_pkthdr
        {
            public int tv_sec;      // seconds
            public int tv_usec;     // microseconds
            public int caplen;      //length of portion present 
            public int len;         //length this packet (off wire) 
        };

        [StructLayout(LayoutKind.Sequential)]
        internal struct pcap_pktdata
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 65536)]
            public byte[] bytes;
        };

        //------------------------     Les fonctions    --------------------------------------------------------------

        public ArrayList GetDevicesList()
        {
            deviceList.Clear();
            IntPtr ptrDevs = IntPtr.Zero, next = IntPtr.Zero;
            pcap_if dev;
            StringBuilder errbuf = new StringBuilder(256);
            string dev_name;

            int res = pcap_findalldevs(ref ptrDevs, errbuf);
            if (res == -1)
            {
                string err = "Error in findalldevs(): " + errbuf;
                throw new Exception(err);
            }
            else
            {
                next = ptrDevs;
                while (next != IntPtr.Zero)
                {
                    dev = (pcap_if)Marshal.PtrToStructure(next, typeof(pcap_if));

                    dev_name = dev.Name.ToString() + " : " + dev.Description.ToString();

                    deviceList.Add(dev_name);
                    next = dev.Next;
                }
            }
            pcap_freealldevs(ptrDevs);
            return deviceList;
        }

        public void LiveCapture(string dev_name, Form1 f)
        {
            StopListenCheck = false;
            fm1 = f;

            device_name = dev_name;

            TrafficRTBox_RawPacket.Clear();
            //cmpt = 0;

            if (ListenThread != null)
            {
                ListenThread.Abort();
            }
            ListenThread = new Thread(new ThreadStart(Listen));
            ListenThread.IsBackground = true;
            ListenThread.Name = "Listen Thread";
            ListenThread.Start();
            Console.WriteLine("LiveCapture End");
        }

        public void Listen()
        {
            Console.WriteLine("In Listen : "+Thread.CurrentThread.Name);

            StringBuilder errbuf = new StringBuilder();
            pcap_t = pcap_open_live(device_name, 1514, 1, 1000, errbuf);

            if (pcap_t == null)
            {
                MessageBox.Show(" erreur de sniffing");
            }
            else
            {
                IntPtr pkthdr = IntPtr.Zero;
                IntPtr pktdata = IntPtr.Zero;

                while (true)
                {
                    int state = pcap_next_ex(pcap_t, ref pkthdr, ref pktdata);
                    if (state == 1) /* SUCCESS */
                    {
                        header = pkthdr;
                        data = pktdata;
                        Console.WriteLine("GET DATA! Listen");

                        Disp Analyze = new Disp(Analyzing);
                        fm1.Invoke(Analyze);
                    }
                }/* end_while */
            }
        }

        public void Analyzing()
        {
            try
            {
                Console.WriteLine("In Analyzing : "+Thread.CurrentThread.Name);

                
                pcap_pkthdr h = (pcap_pkthdr)Marshal.PtrToStructure(header, typeof(pcap_pkthdr));

                Console.WriteLine("Captured Packet Header Length : "+h.len);

                byte[] packet_header = new byte[16];
                Marshal.Copy(header, packet_header, 0, 16); // Structure in winpcap not the same as in C#, so we convert with Marshal f
                
                byte[] packet_data = new byte[(int)h.len];

                if (!StopListenCheck)
                {
                    Marshal.Copy(data, packet_data, 0, (int)h.len);
                } else
                {
                    return;
                }
                
                
                
                
                Packet_Length = h.len;

                //--------------- 
                ListViewItem LItem;

                Console.WriteLine("Packet length "+ packet_data.Length) ;
                IPv4_length = 0;


                Headers.Add(packet_header);
                Datas.Add(packet_data);

                Console.WriteLine("GET DATA! Analyzing");

                //Thread Parsing_Thread = new Thread(() => Deep_Parsing(EtherTypesMap, packet_data, false, 0));
                //Parsing_Thread.Name = "Parsing Thread";
                //Parsing_Thread.IsBackground = true;
                //Parsing_Thread.Start();

                                                         // tree
                Deep_Parsing(EtherTypesMap, packet_data, false, 0);

                Console.WriteLine($"Analyze End : {src} {dest} {length}");
                cmpt++;

                var capturedCmpt = cmpt;
                var capturedSrc = src;
                var capturedDest = dest;
                var capturedProtocol = protocol;
                var capturedLength = length;
                var CurrentFm1 = fm1;
                var capturedDescription = description;

                Rep.Table_View(cmpt, src, dest, protocol, length, description);

                /*
                Thread Table_Thread = new Thread(delegate () {
                    Rep.Table_View(capturedCmpt, capturedSrc, capturedDest, capturedProtocol, capturedLength, capturedDescription);
                    });
                Table_Thread.Name = "Table Thread";
                Table_Thread.Start();
  
                */

                //Rep.Network_Map(src, dest, protocol, fm1);
                
                Thread Map_Thread = new Thread(delegate () {
                    Rep.Network_Map(capturedSrc, capturedDest, capturedProtocol, CurrentFm1);
                    });
                Map_Thread.Name = "Map Thread";
                Map_Thread.Start();
                

                Console.WriteLine(length);
                src = "";
                dest = "";
                protocol = "";
                length = "";
                description = "";
            }
            catch  (Exception e) { 
                Debug.WriteLine($"[!] catch at Analyzing : {e.Message}");
            }
        }

        public void Packet_Info_Set(int row_index)
        {
            Console.WriteLine(Thread.CurrentThread.Name);
            try { 
            byte[] tmp_packet = Datas[row_index];
            for (int i = 0; i < tmp_packet.Length; i++)
            {
                TrafficRTBox_RawPacket.AppendText(tmp_packet[i].ToString("x02")+" ");
            }
            Traffic_Tree_View.Nodes.Clear();
            tabControl1.SelectTab(0);
            Deep_Parsing(EtherTypesMap, tmp_packet, true ,0);
            IPv4_length = 0;
            Console.WriteLine("######## IPv4 length in Packet Info Set " + IPv4_length);
            }
            catch  (Exception e) { 
                Console.WriteLine($"[!] catch at Packet_Info_Set : {e.Message}");
            }
        }

        public void StopListen()
        {
            try
            {
                StopListenCheck = true;
                if (ListenThread != null)
                {
                    if (ListenThread.IsAlive)
                    {
                        ListenThread.Abort();
                    }

                    if (pcap_t != IntPtr.Zero)
                    {
                        pcap_close(pcap_t);
                    }

                    ListenThread = null;
                    cmpt = 0;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"[!] catch at StopListen : {e.Message}");
            }
        }



        private void Table_Traffic(string name, string value) {
            try {
                Console.WriteLine("In Table_Traffic : "+Thread.CurrentThread.Name);
                // LItem;
                string tmp;
                switch (name)
                {
                    case "Source MAC Address":
                        Console.WriteLine(" //////////////// " + name + "  " + value);
                        src = value;
                        break;
                    case "Destination MAC Address":
                        Console.WriteLine(" //////////////// " + name + "  " + value);
                        dest = value;
                        break;
                    case "Source IP Address":
                        Console.WriteLine(" //////////////// " + name + "  " + value);
                        src = value;
                        break;
                    case "Destination IP Address":
                        Console.WriteLine(" //////////////// " + name + "  " + value);
                        dest = value;
                        break;
                    case "Total Length":
                        Console.WriteLine(" //////////////// " + name + "  " + value);
                        length = value;
                        break;
                    case "Destination Port":
                        Console.WriteLine(" //////////////// " + name + "  " + value);
                        tmp = protocol;
                        tmp = value+"/"+protocol;
                        if (PortsMap.ContainsKey(tmp))
                        {
                            Console.WriteLine(PortsMap[tmp]["name"]);
                            Console.WriteLine(PortsMap[tmp]["description"]);
                            description += tmp+", ";
                            protocol = PortsMap[tmp]["name"].ToString();
                            //Tree_View(protocol, 3, 0);
                        } else
                        {
                            description += value + ", ";
                        }
                        break;
                    case "Source Port":
                        Console.WriteLine(" //////////////// " + name + "  " + value);
                        tmp = protocol;
                        tmp = value + "/" + protocol;
                        if (PortsMap.ContainsKey(tmp))
                        {
                            Console.WriteLine(PortsMap[tmp]["name"]);
                            Console.WriteLine(PortsMap[tmp]["description"]);
                            description += tmp+" -> ";
                            protocol = PortsMap[tmp]["name"].ToString();
                            //Tree_View(protocol, 3, 0);
                        } else
                        {
                            description += value+" -> ";
                        }
                        break;
                    case "Flags":
                        description += IPv4.TCP_Flags(value);
                        break;
                    default:
                        break;
                }
                if (name == value)
                {
                    protocol = name.ToLower();
                }
            }
                catch (Exception e)
                {
                    Console.WriteLine($"[!] catch at Table_Traffic : {e.Message}");
                }
        }
                                 // *   /ethernetII / protocoles / IPv4 / protocoles / ICMP
        public void Deep_Parsing(dynamic Path, byte[] packet_data, bool tree, int options_len)
        {
            Console.WriteLine("In Deep Parsing : "+Thread.CurrentThread.Name);
            try { 
            string Next, name, type;
            name = Path["name"];
            type = (string)GetOctet(packet_data, (int)Path["type"][0], (int)Path["type"][1], 16, "");

            if (tree) {
                    Rep.Tree_View(name, (int)Path["type"][4], 0);
            } else {
                    Table_Traffic(name, name);
            }

            if (Path.ContainsKey("feld"))
            {
                foreach (var item in Path["feld"])
                {
                    if (item.Value is JObject obj && obj.ContainsKey("feld"))
                    {
                        if (tree) { Rep.Tree_View(item.Name, (int)Path["feld"][item.Name]["type"][4], 0); }
                        foreach (JProperty subitem in Path["feld"][item.Name]["feld"])
                        {
                            if (tree) { Rep.Tree_View(subitem.Name + " : " + 
                                (string)GetOctet(packet_data, (int)subitem.Value[0] + options_len, (int)subitem.Value[1] + options_len, 
                                (int)subitem.Value[2], 
                                (string)subitem.Value[3]), 
                                (int)subitem.Value[4], 
                                (int)subitem.Value[5]); }
                        }
                    } 
                    else
                    {
                        if (tree)
                        {
                            Rep.Tree_View(item.Name + " : " + (string)GetOctet(packet_data, (int)item.Value[0] + options_len, (int)item.Value[1] + options_len, (int)item.Value[2], (string)item.Value[3]), (int)item.Value[4], 0);
                        } else {
                            Table_Traffic(item.Name, (string)GetOctet(packet_data, (int)item.Value[0] + options_len, (int)item.Value[1] + options_len, (int)item.Value[2], (string)item.Value[3]));
                        }
                        Console.WriteLine(item.Name + " ----> " + (string)GetOctet(packet_data, (int)item.Value[0] + options_len, (int)item.Value[1] + options_len, (int)item.Value[2], (string)item.Value[3]));
                    }
                }




                if (Path["protocoles"].ContainsKey(type))
                {         // /ehternet / protocoles / 0800 / 
                    Path = Path["protocoles"][type];
                        ///ehternet / protocoles / 0800 / protocoles / 01
                    Console.WriteLine("Going to recurse for protocole : "+ IPv4_length);
                    Deep_Parsing(Path, packet_data, tree, IPv4_length);
                }
                    Console.WriteLine("NO TYPE");

                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"[!] catch at Deep_Parsing : {e.Message}");
            }

        }

        public static object GetOctet(byte[] Packet_info, int begin, int end, int type, string split)
        {
            try
            {
                Console.WriteLine("In GetOctet : "+Thread.CurrentThread.Name);
                object[] Result = new object[end + 1 - begin];
                int index = 0;
                int o = 0;
                int a, b, c;
                string x, y, z;
                for (int i = begin; i < end + 1; i++)
                {
                    switch (type)
                    {
                        case 2:
                            Result[index] = Convert.ToString(Convert.ToInt32(Packet_info[i].ToString(), 16), 2);
                            break;
                        case 10:
                            Result[index] = Convert.ToInt64(Packet_info[i]);
                            break;
                        case 16: // 0800
                            Result[index] = Packet_info[i].ToString("x02");
                            break;
                        case 98: // MAC Address
                            Result[index] = Packet_info[i].ToString("x02");
                            break;
                        case 99: // 0x0800
                            Result[index] = "0x" + Packet_info[i].ToString("x02");
                            break;
                        case 100: // 192.168.1.1
                            Result[index] = Convert.ToInt64(Packet_info[i]);
                            break;
                        case 101: // ICMP type
                            a = int.Parse(Packet_info[i].ToString("x02").ToString());
                            Console.WriteLine("a is : "+a);
                            x = ICMP_Parse.ICMP_type(a.ToString());
                            return $"{a} ({x})";
                            break;
                        case 102: // ICMP code
                            //a = int.Parse(Packet_info[i].ToString("x02").ToString());
                            b = int.Parse(Packet_info[i].ToString("x02").ToString());
                            Console.WriteLine("a and b : "+b);
                            x = ICMP_Parse.ICMP_code("3", b.ToString());
                            return x;
                            break;
                        case 104: // IPv4 Options
                            Result[index] = Packet_info[i].ToString("x02");
                            i = IPv4_length;
                            Console.WriteLine(IPv4_length+" ---------------------------------------- "+i);
                            break;
                        case 210: // Split/2 -> Bin - Bin -> Dec - Dec (Version and IHL)
                            a = int.Parse(Packet_info[i].ToString("x02")[0].ToString());
                            b = int.Parse(Packet_info[i].ToString("x02")[1].ToString());
                            c = b * 4;
                            IPv4_length = c - 20;
                            Console.WriteLine("############### Found IPv4 Option Size "+IPv4_length);
                            Result[index] = $" {a} / {b} ({c} byte)";
                            break;
                        case 211: // Bin -> Split {...identify,.....} (Type of Service)
                            x = Convert.ToString(Convert.ToInt32(Packet_info[i].ToString(), 16), 2).PadLeft(8, '0');
                            y = IPv4.TypeOfService(x.Substring(0, 3));
                            Result[index] = $" {x.Substring(0, 3)} ( {y} ) _ {x.Substring(2, 5)} (D T R C X)";
                            break;
                        case 1610: // 0x06ba (Identifaction)
                            Result[index] = Packet_info[i].ToString("x02");
                            o++;
                            if (o == 2)
                            {
                                x = string.Join(split, Result);
                                return $"0x{x}";
                            }
                            break;
                        case 2121: // Flag
                            x = Convert.ToString(Convert.ToInt32(Packet_info[i].ToString("x02"), 16), 2).PadLeft(8, '0');
                            y = IPv4.Flags(x.Substring(0, 3));
                            
                            Result[index] = $" {x.Substring(0, 3)} - ({y})";
                            break;
                        case 2122: // Fragment Offset
                            Result[index] = Convert.ToString(Convert.ToInt32(Packet_info[i].ToString(), 16), 2).PadLeft(8, '0');
                            o++;
                            if (o == 2)
                            {
                                x = string.Join(split, Result);
                                return $"_ _ _ {x.Substring(3)}";
                            }
                            //Result[index] = $" _ _ _ {x.Substring(2)}";
                            break;
                        case 300: // [ce,b7] -> ceb7 -> 443        Ports
                            Result[index] = Packet_info[i].ToString("x02");
                            o++;
                            if (o == 2)
                            {
                                x = string.Join(split, Result); // ceb7
                                a = Convert.ToInt32(x, 16);
                                return a.ToString();
                            }
                            break;
                        case 301: // 50 (hex) -> 5 -> 0101 (20 byte) TCP Length
                            x = Convert.ToString(Convert.ToInt32(Packet_info[i].ToString("x02"), 16), 2).PadLeft(8, '0');
                            y = x.Substring(0, 4);

                            //tcp_udp_Length = Convert.ToInt32(y, 2) * 4;
                            Console.WriteLine("        tcp udp length "+tcp_udp_Length);

                            return $" {y} ...... ( {Convert.ToInt32(y, 2)*4} bytes)";
                            break;
                        case 302: // TCP Flags
                            x = Convert.ToString(Convert.ToInt32(Packet_info[i].ToString("x02"), 16), 2).PadLeft(8, '0'); // Binary
                            y = IPv4.TCP_Flags(x);
                            return $"{x} ({string.Join(" ", y)})";
                            break;
                    }

                    index++;
                }
                return string.Join(split, Result);
            }
            catch (Exception e)
            {
                return $"[!] catch at GetOctet : {e.Message}";
            }
        }
        
    }
}
