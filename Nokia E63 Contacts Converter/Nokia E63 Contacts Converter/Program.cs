using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Nokia_E63_Contacts_Converter
{
    class Program
    {
        

        static void Main(string[] args)
        {
            string[] lines = File.ReadAllLines(@"C:\Users\Cristiano\Desktop\Nokia E63_2012-02-18.nbu");
            for (int i = 0; i < 2406; i++)
            {
                if(lines[i].Contains("BEGIN:VCARD"))
                {//contact founded 
                    string contactName = lines[i+3];
                    string contactPhone = lines[i+4];
                    contactName = contactName.Substring(1);
                    contactName = contactName.Replace(";", "");
                    contactName = contactName.Replace(":", "");
                    System.Console.WriteLine(contactName);
                    System.Console.WriteLine(contactPhone);
                }
            }
            while (true) { }
        }
    }
}
