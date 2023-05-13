using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            try
            {
                Thread Main_Thread = Thread.CurrentThread;
                Main_Thread.Name = "Main Thread";

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Form1());
                
                Console.WriteLine("Hello");
            } catch (Exception ex) { Console.WriteLine("[!] catch at Main() "+ ex.Message); }
        }
    }
}
