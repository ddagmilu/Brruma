using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace WindowsFormsApp1
{
    internal class Tools
    {
        //Form1 fm = new Form1();
        public static System.Windows.Forms.RichTextBox nmapBox;

        public void nmap(string ip, string scan_type)
        {
            try
            {
                if (File.Exists(@"C:\Program Files (x86)\Nmap\nmap.exe"))
                {
                    Process process = new Process();
                    nmapBox.Invoke((MethodInvoker)(() => nmapBox.Text += $">> nmap {scan_type} {ip} \n"));
                    process.StartInfo.FileName = "C:\\Program Files (x86)\\Nmap\\nmap.exe";
                    process.StartInfo.Arguments = $"/c nmap {ip}";
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.RedirectStandardOutput = true;
                    process.StartInfo.CreateNoWindow = true;

                    process.Start();

                    string output = process.StandardOutput.ReadToEnd();
                    nmapBox.Invoke((MethodInvoker)(() => nmapBox.Text += output));
                    nmapBox.Invoke((MethodInvoker)(() => nmapBox.Text += ">> DONE"));
                    //nmapBox.Text = output;
                    process.WaitForExit();
                } else
                {
                    DialogResult res = MessageBox.Show("C:\\Program Files (x86)\\Nmap\\nmap.exe not Found \n You need to install Nmap first", "Nmap not found", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);

                }
            }
            catch (Exception e)
            {
                Debug.WriteLine($"[!] catch at nmap : {e.Message}");
            }

        }

        public static string DNSLookup(string ipAddress)
        {
            string domainName = string.Empty;
            try
            {
                IPHostEntry hostEntry = Dns.GetHostEntry(ipAddress);
                domainName = hostEntry.HostName;
                nmapBox.Invoke((MethodInvoker)(() => nmapBox.Text += $"DNS Lookup : {domainName}"));
                return domainName;
            }
            catch (SocketException)
            {
                nmapBox.Invoke((MethodInvoker)(() => nmapBox.Text += "DNS Lookup : No domain name"));
                return "No domain name";
            }
        }

        public void Ping(string ip)
        {
            try
            {
                Process process = new Process();

                nmapBox.Invoke((MethodInvoker)(() => nmapBox.Text += $">> ping {ip} \n"));

                process.StartInfo.FileName = "C:\\windows\\system32\\cmd.exe";
                process.StartInfo.Arguments = $"/c ping {ip}";
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.CreateNoWindow = true;

                process.Start();

                string output = process.StandardOutput.ReadToEnd();
                nmapBox.Invoke((MethodInvoker)(() => nmapBox.Text += output));
                nmapBox.Invoke((MethodInvoker)(() => nmapBox.Text += ">> DONE"));
                //nmapBox.Text = output;
                process.WaitForExit();
            }
            catch (Exception e)
            {
                Debug.WriteLine($"[!] catch at ping : {e.Message}");
            }

        }

    }
}
