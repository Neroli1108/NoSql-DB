/////////////////////////////////////////////////////////////////////////
//WriteData.cs - Provide some Function WpfWriteclient will used        //
// ver 1.0                                                             //
// YundingLI, CSE681 - Software Modeling and Analysis, Project #4      //
/////////////////////////////////////////////////////////////////////////
/*
 * Maintenance History:
 * --------------------
 * ver 1.0 : 18 Nov 2015
 * - first release
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml;
using Xdoc;
using System.IO;

namespace Project4Starter
{
   public class WriteClient
    {
        //-<Add function and construct the xml for sending to the server>-
        public static string AddKey()
        {
            XElement Add = new XElement("Add",
                            new XElement("Record",
                            new XElement("Key", "3"),
                            new XElement("Name", "Lebron James"),
                            new XElement("Descr", "King"),
                            new XElement("TimeRecord", "2015-11-17T14:19:19.0755013+08:00"),
                            new XElement("Payload", "He is a best basketball player."),
                            new XElement("children",
                            new XElement("child", "23"))
                            ));
            return Add.ToString();

           
        }
        //-<Delete function and construct the xml for sending to the server>-
        public static string DeleteKey()
        {
            XElement Delete = new XElement("Delete",
                              new XElement("Record",
                               new XElement("Key", 2)));
            return Delete.ToString();
        }
        //-<Edit key function and construct the xml for sending to the server>-
        public static string EditKey()
        {
            XElement Edit = new XElement("Edit",
                            new XElement("Record",
                            new XElement("Key", 2)));
            return Edit.ToString();
        }
        //-<edit children function and construct the xml for sending to the server>-
        public static string EditChidren()
        {
            XElement Editchildren =new XElement("Editchildren",
                                   new XElement("Record",
                                   new XElement("Key", 1),
                                   new XElement("Name", "KOBE"),
                                   new XElement("Descr", "BlackMaBa"),
                                   new XElement("TimeRecord", "2015-11-17T14:19:19.0755013+08:00"),
                                   new XElement("Payload","Star"),
                                   new XElement("children",
                                   new XElement("child", 8),
                                   new XElement("child", 24))));
            return Editchildren.ToString();
        }

       
        static void Main()
        {
            Console.WriteLine();
        }
    }
}


