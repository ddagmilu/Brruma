using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;



namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        ArrayList devicesList = new ArrayList();
        Capture cap = new Capture();
        Tools Tools = new Tools();

        string[] str_tab;
        string dev_name;

        int row_index;
        int IPv4_length;

        List<PictureBox> Hosts = new List<PictureBox>();
        Random rand = new Random();

        private bool isDragging = false;
        private Point lastLocation;

        public Form1()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        } 


        private void button1_Click(object sender, EventArgs e)
        {
            cap.LiveCapture(dev_name, this);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.CenterToScreen();

            devicesList = cap.GetDevicesList();
            string name, secondname;
            for (int i = 0; i < devicesList.Count; i++)
            {
                name = (string)devicesList[i];
                Console.WriteLine(name);
                secondname = name.Split(':')[0];
                Console.WriteLine(" --> " + secondname);
                comboBox1.Items.Add((string)devicesList[i]);
            }


            if (comboBox1.Items.Count > 0)
            {
                comboBox1.SelectedIndex = 0;
                str_tab = ((string)devicesList[comboBox1.SelectedIndex]).Split(':');
                //Console.WriteLine("Third line : " + str_tab.ToString());
                dev_name = str_tab[0].Trim();
                Console.WriteLine("Third line : " + str_tab[1].Trim());
            }

            IPAddress Local_IP = IPv4.GetLocalIPAddress();
            IPAddress Global_IP = IPv4.GetPublicIPAddress();

            toolStripStatusLabel1.Text = "Local IP : ["+Local_IP.ToString()+"] Public IP : ["+Global_IP.ToString()+"]";

            WindowsFormsApp1.Capture.Traffic_Tree_View = this.treeView1;
            //WindowsFormsApp1.Capture.TrafficRTBox = this.richTextBox1;
            WindowsFormsApp1.Capture.TrafficRTBox_RawPacket = this.richTextBox2;
            WindowsFormsApp1.Capture.TrafficListView = this.TrafficListView;
            WindowsFormsApp1.Capture.tabControl1 = this.tabControl1;
            WindowsFormsApp1.Represent.TrafficListView = this.TrafficListView;
            WindowsFormsApp1.Represent.Traffic_Tree_View = this.treeView1;
            WindowsFormsApp1.Tools.nmapBox = this.nmapBox;

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            str_tab = ((string)devicesList[comboBox1.SelectedIndex]).Split(':');
            dev_name = str_tab[0].Trim();
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            cap.StopListen();
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {

        }

        private void richTextBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void groupBox3_Enter(object sender, EventArgs e)
        {

        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void groupBox1_Enter_1(object sender, EventArgs e)
        {

        }

        private void flowLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void statusStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void toolStripStatusLabel1_Click(object sender, EventArgs e)
        {

        }

        private void documentatioToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void TrafficListView_MouseClick(object sender, MouseEventArgs e)
        {
            Raw_Packet_Set();
        }

        private void Raw_Packet_Set()
        {
            richTextBox2.Clear();
            row_index = TrafficListView.SelectedIndices[0];

            cap.Packet_Info_Set(row_index);
            Console.WriteLine(row_index);
        }


        private void treeView1_AfterSelect_1(object sender, TreeViewEventArgs e)
        {

        }

        private void TrafficListView_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void FilterListView(System.Windows.Forms.ListView listView, System.Windows.Forms.TextBox textBox)
        {
            string searchText = textBox.Text.ToLower();
            bool Found = false;

            foreach (ListViewItem item in listView.Items)
            {
                Console.WriteLine("Item text "+item.Text);
                foreach (ListViewItem.ListViewSubItem subItem in item.SubItems)
                {
                    Console.WriteLine("subItem : "+subItem.Text);
                    if (subItem.Text.ToLower().Contains(searchText))
                    {
                        Console.WriteLine("Got a match : "+subItem.Text+" : "+searchText);
                        Found = true;
                        //break;
                    }
                }
                if (Found)
                {
                    Console.WriteLine("COLORING : " + item.Text + " : " + searchText);
                    item.Selected = true;
                    item.Font = new Font(item.Font, FontStyle.Bold | FontStyle.Underline);
                    item.EnsureVisible();
                    //break;
                }
                else
                {
                    Found = false;
                    item.Selected = false;
                    item.Font = TrafficListView.Font;
                }
                Found = false;
            }
        }


        public void MakePictureBox(string host_address, string type)
        {
            if (type != "NULL")
            {
                Panel panel1 = new Panel();
                PictureBox newPic = new PictureBox();
                Label label = new Label();

                panel1.AutoSize = true;
                panel1.Height = 80;
                panel1.Width = 75;
                panel1.BackColor = Color.FromArgb(0, 255, 255, 255);

                newPic.Height = 65;
                newPic.Width = 65;
                newPic.SizeMode = PictureBoxSizeMode.StretchImage;
                newPic.Name = host_address;
                newPic.Tag = ""; // Domain Name


                label.Text = $"{host_address} {newPic.Tag}";
                label.AutoSize = true;

                switch (type)
                {
                    case "PC":
                        newPic.Image = global::WindowsFormsApp1.Properties.Resources.icons8_desktop_computer_96__1_;
                        break;
                    case "Phone":
                        newPic.Image = global::WindowsFormsApp1.Properties.Resources.icons8_mobile_phone_100;
                        break;
                    case "Node":
                        newPic.Image = global::WindowsFormsApp1.Properties.Resources.icons8_router_100;
                        break;
                    case "Outsider":
                        newPic.Image = global::WindowsFormsApp1.Properties.Resources.icons8_server_100;
                        break;
                    case "Localhost":
                        newPic.Image = global::WindowsFormsApp1.Properties.Resources.icons8_desktop_computer_100_Local;
                        break;
                }

                int x, y;
                //x = rand.Next(10, panel1.ClientSize.Width - newPic.Width);
                //y = rand.Next(10, panel1.ClientSize.Height - newPic.Height);

                //panel1.Controls.Add(label);
                //panel1.Controls.Add(newPic);

                newPic.Location = new Point(0, 0);
                label.Location = new Point(0, 67);

                x = rand.Next(10, groupBox1.ClientSize.Width - panel1.Width);
                y = rand.Next(10, groupBox1.ClientSize.Height - panel1.Height);
                panel1.Location = new Point(x, y);

                newPic.MouseDown += Image_MouseDown;

                panel1.MouseDown += Panel_MouseDown;
                panel1.MouseUp += Panel_MouseUp;
                panel1.MouseMove += Panel_MouseMove;

                Hosts.Add(newPic);
                panel1.Controls.Add(newPic);
                panel1.Controls.Add(label);

                groupBox1.Invoke((MethodInvoker)(() => groupBox1.Controls.Add(panel1)));
                //groupBox1.Controls.Add(panel1);
                
            }
        }
        
        private void Image_MouseDown(object sender, MouseEventArgs e)
        {
            PictureBox temPic = sender as PictureBox;
            if (e.Button == MouseButtons.Right)
            {
                contextMenuStrip1.Show(temPic, e.Location);
                Console.WriteLine(temPic.Name);
            }
        }

        private void Panel_MouseDown(object sender, MouseEventArgs e)
        {
            isDragging = true;
            lastLocation = e.Location;
        }

        private void Panel_MouseMove(object sender, MouseEventArgs e)
        {
            Panel temPic = sender as Panel;

            if (isDragging)
            {
                int dx = e.X - lastLocation.X;
                int dy = e.Y - lastLocation.Y;
                temPic.Location = new Point(temPic.Location.X + dx, temPic.Location.Y + dy);
            }
        }

        private void Panel_MouseUp(object sender, MouseEventArgs e)
        {
            isDragging = false;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //FilterListView(TrafficListView, textBox1);
        }

        private void contextMenuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            Control sourceControl = contextMenuStrip1.SourceControl;
            PictureBox temPic = sourceControl as PictureBox;
            String ip = temPic.Name as String;

            //Console.WriteLine(e.ClickedItem.Text);
            switch (e.ClickedItem.Text) 
            {
                case "Nmap":
                    nmapBox.Clear();
                    tabControl1.SelectTab(1);
                    new Thread(delegate () {
                       Tools.nmap(ip, "");
                    }).Start();
                    break;
                case "Metasploit":
                    
                    break;
                case "DNS Lookup":
                    nmapBox.Clear();
                    string domain_name = "";
                    new Thread(delegate () {
                        domain_name = Tools.DNSLookup(ip);
                    }).Start();
                    temPic.Tag = domain_name;
                    Console.WriteLine("Tag : "+temPic.Tag.ToString());
                    break;
                case "Ping":
                    nmapBox.Clear();
                    new Thread(delegate () {
                        Tools.Ping(ip);
                    }).Start();
                    break;
            }
        }

        private void sCToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tabControl1.SelectTab(1);
            Control sourceControl = contextMenuStrip1.SourceControl;
            PictureBox temPic = sourceControl as PictureBox;
            String ip = temPic.Name as String;

            Console.WriteLine("Clicked " + sender.ToString());
            nmapBox.Clear();
            new Thread(delegate () {
                Tools.nmap(ip, "-sC");
            }).Start();
            
        }

        private void pnToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tabControl1.SelectTab(1);
            Control sourceControl = contextMenuStrip1.SourceControl;
            PictureBox temPic = sourceControl as PictureBox;
            String ip = temPic.Name as String;

            Console.WriteLine("Clicked " + sender.ToString());
            nmapBox.Clear();
            new Thread(delegate () {
                Tools.nmap(ip, "-Pn");
            }).Start();
        }

        private void sNToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tabControl1.SelectTab(1);
            Control sourceControl = contextMenuStrip1.SourceControl;
            PictureBox temPic = sourceControl as PictureBox;
            String ip = temPic.Name as String;

            Console.WriteLine("Clicked " + sender.ToString());
            nmapBox.Clear();
            new Thread(delegate () {
                Tools.nmap(ip, "-sN");
            }).Start();
        }

        private void dNSToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void identifyToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            FilterListView(TrafficListView, textBox1);
        }
    }
}
