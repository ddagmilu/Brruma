using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    internal class Represent
    {
        Form1 fm;
        IPv4 IPv4 = new IPv4();
        public static volatile System.Windows.Forms.ListView TrafficListView;
        public static System.Windows.Forms.TreeView Traffic_Tree_View;

        string[] red_protocoles = new string[] { "http", "domain", "dhcp", "telnet", "ftp"};
        string[] blue_protocoles = new string[] { "https", "arp", "tftp", "ssh" };

        List<string> Hosts = new List<string>();

        public void Table_View(int cmpt, string src, string dest, string protocol, string length, string description)
        {
            try
            {

                Console.WriteLine("In Table_View : " + Thread.CurrentThread.Name);
                ListViewItem LItem;

                LItem = TrafficListView.Items.Add(cmpt.ToString());
                LItem.Text = cmpt.ToString();
                LItem.SubItems.Add(src);
                LItem.SubItems.Add(dest);
                LItem.SubItems.Add(protocol);
                LItem.SubItems.Add(length);
                LItem.SubItems.Add(description);
                
                if (red_protocoles.Contains(protocol))
                {
                    LItem.BackColor = Color.MediumVioletRed;
                    LItem.ForeColor = Color.White;
                } else if (blue_protocoles.Contains(protocol))
                {
                    LItem.BackColor = Color.AliceBlue;
                }
                

            } catch (Exception e)
            {
                Debug.WriteLine($"[!] catch at Represent.Table_View : {e.Message}");
            }
        }

        public void Tree_View(string nodeString, int layer, int line)
        {
            try
            {
                Console.WriteLine("In Tree_View : " + Thread.CurrentThread.Name);
                TreeNode node = new TreeNode();
                node = new TreeNode(nodeString);
                // Tree View
                if (layer == 0 || layer == 1 || layer == 2 || layer == 3)
                {
                    Traffic_Tree_View.Nodes.Add(node);
                }
                switch (layer)
                {
                    case 10:
                        Traffic_Tree_View.Nodes[0].Nodes.Add(node);
                        break;
                    case 11:
                        Traffic_Tree_View.Nodes[1].Nodes.Add(node);
                        break;
                    case 12:
                        Traffic_Tree_View.Nodes[2].Nodes.Add(node);
                        break;
                    case 111:
                        Traffic_Tree_View.Nodes[1].Nodes[line].Nodes.Add(node);
                        break;
                    case 112:
                        Traffic_Tree_View.Nodes[2].Nodes[line].Nodes.Add(node);
                        break;
                }
                // Table View
            }
            catch (Exception e)
            {
                Console.WriteLine($"[!] catch at Tree_View : {e.Message}");
            }
        }

        public void Network_Map(string src, string dest, string protocol, Form1 f)
        {
            Console.WriteLine("In Network_Map : " + Thread.CurrentThread.Name);
            fm = f;
            string src_type, dest_type;
            if (!src.Contains(":") && !dest.Contains(":"))
            {
                src_type = IPv4.Identify(IPAddress.Parse(src));
                dest_type = IPv4.Identify(IPAddress.Parse(dest));
            } else
            {
                src_type = "NULL";
                dest_type = "NULL";
            }

            if (!Hosts.Contains(src) || !Hosts.Contains(dest)) // Mafihach
            {
                if (!Hosts.Contains(src))
                { 
                    Hosts.Add(src);
                    fm.MakePictureBox(src, src_type);
                }
                if (!Hosts.Contains(dest)) 
                { 
                    Hosts.Add(dest);
                    fm.MakePictureBox(dest, dest_type);
                }
            }
        }
    }
}
