using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Nokia_E63_Contacts_Converter
{
    class Program
    {
        static string spaces(int n)
        {
            string ret = "";
            for (int i = 0; i < n; i++)
                ret += " ";
            return ret;
        }

        static void Main(string[] args)
        {
            string[] lines = File.ReadAllLines(@"C:\Users\Cristiano\Desktop\Nokia E63_2012-02-18.nbu");
            File.Delete(@"C:\Users\Cristiano\Desktop\ContatosHugo.txt");
            StreamWriter output = new StreamWriter(@"C:\Users\Cristiano\Desktop\ContatosHugo.txt");                
            for (int i = 0; i < 2406; i++)
            {
                if(lines[i].Contains("BEGIN:VCARD"))
                {//contact founded 
                    string contactName = lines[i+3];
                    string contactPhone = lines[i+4];
                    contactName = contactName.Substring(1);
                    contactName = contactName.Replace(";", "");
                    contactName = contactName.Replace(":", "");
                    contactPhone =contactPhone.Substring(contactPhone.IndexOf(":")+1);
                    output.WriteLine(contactName + spaces(30 - contactName.Length) + contactPhone);
                    System.Console.WriteLine(contactName);
                    System.Console.WriteLine(contactPhone);
                    

                }
            }
            while (true) { }
        }
    }
}
