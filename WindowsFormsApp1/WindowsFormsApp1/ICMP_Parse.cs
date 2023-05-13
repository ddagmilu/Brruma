using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
    internal class ICMP_Parse
    {
        //   --------------------------------------- !!!  [ CHANGE PATH ] !!! ---------------------------------------
        static byte[] ICMPString = Properties.Resources.ports;
        //   --------------------------------------------------------------------------------------------------------
        dynamic ICMPMap = JsonConvert.DeserializeObject(Encoding.ASCII.GetString(ICMPString));

        public static string ICMP_type(string type)
        {
            dynamic ICMPMap = JsonConvert.DeserializeObject(Encoding.ASCII.GetString(ICMPString));
            if (ICMPMap["type"].ContainsKey(type))
            {
                return ICMPMap["type"][type]["name"].Value;
            }
            return "8888888888";
        }
        public static string ICMP_code(string type, string code)
        {
            dynamic ICMPMap = JsonConvert.DeserializeObject(Encoding.ASCII.GetString(ICMPString));
            if (ICMPMap["type"][type]["code"].ContainsKey(code))
            {
                return ICMPMap["type"][type]["code"][code].Value;
            }
            return "99999999999";
        }
    }
}
