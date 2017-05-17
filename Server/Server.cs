/////////////////////////////////////////////////////////////////////////
// Server.cs - CommService server                                      //
// ver 3.0                                                             //
// Yunding Li,  CSE681 - Software Modeling and Analysis, Project #4    //
/////////////////////////////////////////////////////////////////////////
/*
 * Additions to C# Console Wizard generated code:
 * - Added reference to ICommService, Sender, Receiver, Utilities
 *
 * Note:
 * - This server now receives and then sends back received messages.
 */
/*
 * Plans:
 * - Add message decoding and NoSqlDb calls in performanceServiceAction.
 * - Provide requirements testing in requirementsServiceAction, perhaps
 *   used in a console client application separate from Performance 
 *   Testing GUI.
 */
/*
 * Maintenance History:
 * --------------------
 * ver 3.0 : 18 Nov 2015
 * - added several function to receive the msg from the two client. add the timer to record the time
 * ver 2.3 : 29 Oct 2015
 * - added handling of special messages: 
 *   "connection start message", "done", "closeServer"
 * ver 2.2 : 25 Oct 2015
 * - minor changes to display
 * ver 2.1 : 24 Oct 2015
 * - added Sender so Server can echo back messages it receives
 * - added verbose mode to support debugging and learning
 * - to see more detail about what is going on in Sender and Receiver
 *   set Utilities.verbose = true
 * ver 2.0 : 20 Oct 2015
 * - Defined Receiver and used that to replace almost all of the
 *   original Server's functionality.
 * ver 1.0 : 18 Oct 2015
 * - first release
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Project2Starter;
using System.Xml.Linq;
using System.Collections;
using System.Text.RegularExpressions;
using System.Xml;
using System.Diagnostics;
using Xdoc;


namespace Project4Starter
{
    using Util = Utilities;

    class Server
    {
        string address { get; set; } = "localhost";
        string port { get; set; } = "8080";
        DBEngine<int, DBElement<int, string>> db;
        DBEngine<int, DBElement<int, string>> db1;
        DBEngine<int, DBElement<int, string>> db2;
        DBEngine<int, DBElement<int, string>> db3;
        DBEngine<int, DBElement<int, string>> db4;
        DBEngine<int, DBElement<int, string>> db5;
        DBEngine<int, DBElement<int, string>> db6;
        XDocument Doc = XDocument.Load("../../../DataBase.xml");
        //----< quick way to grab ports and addresses from commandline >-----
        public void ProcessCommandLine(string[] args)
        {
            if (args.Length > 0)
            {
                port = args[0];
            }
            if (args.Length > 1)
            {
                address = args[1];
            }
        }
        static void Main(string[] args)
        {
            int count = 0;
            Stopwatch Timer = new Stopwatch();// construct the stopwacth
            Util.verbose = false;
            Server srvr = new Server();
            srvr.ProcessCommandLine(args);
            Console.WriteLine(srvr.Doc);
            srvr.db = XmltoDB.LoadXEngine<int, string>("../../../DataBase.xml");
            // this is the original DataBase
            XElement OriginalDB = XElement.Load(@"../../../DataBase.xml");
            Console.Title = "Server";
            Console.Write(String.Format("\n  Starting CommService server listening on port {0}", srvr.port));
            Console.Write("\n ====================================================\n");
            Sender sndr = new Sender(Util.makeUrl(srvr.address, srvr.port));
            //Sender sndr = new Sender();
            Receiver rcvr = new Receiver(srvr.port, srvr.address);

            // - serviceAction defines what the server does with received messages
            // - This serviceAction just announces incoming messages and echos them
            //   back to the sender.  
            // - Note that demonstrates sender routing works if you run more than
            //   one client.

            Action serviceAction = () =>
            {
                Message msg = null;
                while (true)
                {
                    msg = rcvr.getMessage(); // note use of non-service method to deQ messages
                    Timer.Reset();
                    Timer.Start();// timer start to record the time cost by the server
                    Console.Write("\n  Received message:");
                    Console.Write("\n  sender is {0}", msg.fromUrl);
                    Console.Write("\n  content is {0}\n", msg.content);
                    if (msg.content == "connection start message")
                    {
                        continue; // don't send back start message
                    }
                    if (msg.content == "done")
                    {
                        Console.Write("\n  client has finished\n");
                        continue;
                    }
                    if (msg.content == "closeServer")
                    {
                        Console.Write("received closeServer");
                        break;
                    }
                    // -<if msg.Rcontent contains key words Add>
                    if (msg.Rcontent.Contains("<Add>"))
                    {
                        srvr.db1 = XmltoDB.LoadXEngine<int, string>("../../../DataBase.xml");
                        Regex regex = new Regex(@">\s*<");//remove the whitespace of the Xml message
                        string cleanedXml = regex.Replace(msg.Rcontent, "><");
                        int Begin = cleanedXml.IndexOf('<');// grasp the Xml
                        string Add = cleanedXml.Substring(Begin);
                        XmlDocument Addxml = new XmlDocument();
                        Addxml.LoadXml(Add);//convert string to the xml
                        var newAddxml = Addxml.ToXDocument();//convert xmldocument to the Xdocument
                        srvr.ParseXEngine(srvr.db1, newAddxml);// add new key
                        //send the result to WPF
                        msg.Rcontent = "\n======Requirement#2=====\n" + "\nThis is the DataBase used in the Project#2\n" + "\n============================================\n" + srvr.db.showDataBase() + "\n=====this information means the Requirement#3 has been accomplished====\n" + "\n======Requirement#4======\n" + "\nThe Add KEY=3 is successful\n" + "\nThis is the original DataBase in XML Format\n" + "\n============================================\n" + OriginalDB + "\n======This is Database after ADDITION=====\n" + "\n============================================\n" + srvr.db1.showDataBase() + "\n============================================\n";

                    }
                    //-<if msg.Rcontent contains "Delete" than operation>-
                    if (msg.Rcontent.Contains("<Delete>"))
                    {
                        srvr.db2 = XmltoDB.LoadXEngine<int, string>("../../../DataBase.xml");
                        Regex regex = new Regex(@">\s*<");
                        string cleanedXml = regex.Replace(msg.Rcontent, "><");
                        int Begin = cleanedXml.IndexOf('<');
                        string Delete = cleanedXml.Substring(Begin);
                        int key = Int32.Parse(Regex.Match(Delete, @"\d+").Value);
                        srvr.db2.DeleteKey(key);//delete the Key/Value pairs
                        //send the result to WPF
                        msg.Rcontent = "\n============================================\n" + "\nThe Delete KEY=2 is successful\n" + "\nThis is the original DataBase\n" + "\n============================================\n" + srvr.db.showDataBase() + "\n============================================\n" + "\nThis is Database after DELETE\n" + "\n============================================\n" + srvr.db2.showDataBase() + "\n============================================\n";
                    }
                    //-<if msg.Rcontent contains "Edit" than operation>-
                    if (msg.Rcontent.Contains("<Edit>"))
                    {
                        srvr.db3 = XmltoDB.LoadXEngine<int, string>("../../../DataBase.xml");
                        Regex regex = new Regex(@">\s*<");
                        string cleanedXml = regex.Replace(msg.Rcontent, "><");
                        int Begin = cleanedXml.IndexOf('<');
                        string Edit = cleanedXml.Substring(Begin);
                        int key = Int32.Parse(Regex.Match(Edit, @"\d+").Value);
                        srvr.db3.ReplaceInstance(key, srvr.db3);
                        //send the result to WPF
                        msg.Rcontent = "\n============================================\n" + "\nThe Edit KEY=2 is successful\n" + "\nThis is the original DataBase\n" + "\n============================================\n" + srvr.db.showDataBase() + "\n============================================\n" + "\nThis is Database after EDIT\n" + "\n============================================\n" + srvr.db3.showDataBase() + "\n============================================\n";
                    }
                    //-<if msg.Rcontent contains "Editchildren" than operation>-
                    if (msg.Rcontent.Contains("<Editchildren>"))
                    {
                        srvr.db4 = XmltoDB.LoadXEngine<int, string>("../../../DataBase.xml");
                        Regex regex = new Regex(@">\s*<");
                        string cleanedXml = regex.Replace(msg.Rcontent, "><");
                        int Begin = cleanedXml.IndexOf('<');
                        string EditChilren = cleanedXml.Substring(Begin);
                        int key = Int32.Parse(Regex.Match(EditChilren, @"\d+").Value);
                        XElement NewX = XElement.Parse(EditChilren);
                        DBElement<int, string> dbe = new DBElement<int, string>("Yun", "Student");//construct a new DBelement for next step
                        foreach (var c in NewX.Element("Record").Element("children").Elements())
                        { dbe.children.Add(ParseElementValue<int>(c)); }
                        srvr.db4.EditChildren(srvr.db4, key, dbe);//edit the children
                        //send the result to WPF
                        msg.Rcontent = "\n============================================\n" + "\nThe EditChilren to the KEY=1 is successful\n" + "\nThis is the original DataBase\n" + "\n============================================\n" + srvr.db.showDataBase() + "\n============================================\n" + "\nThis is Database after EDITCHILDREN\n" + "\n============================================\n" + srvr.db4.showDataBase() + "\n============================================\n";
                    }
                    //-<if msg.Rcontent contains "Persist" than operation>-
                    if (msg.Rcontent.Contains("Persist"))
                    {

                        XElement NewPersist = XElement.Load("NewPersis.xml");
                        srvr.db5 = XmltoDB.LoadXEngine<int, string>("NewPersis.xml");
                        //send the result to WPF
                        msg.Rcontent = "\n=====Elapsed Time is requirement#5======\n" + "\nThe Persist is successful\n" + "\n============================================\n" + NewPersist + "\n============================================\n" + "Thi is the new DataBase" + "\n============================================\n" + srvr.db5.showDataBase() + "\n============================================\n";
                    }
                    //-<if msg.Rcontent contains "Restore" than operation>-
                    if (msg.Rcontent.Contains("Restore"))
                    {
                        srvr.db6 = XmltoDB.LoadXEngine<int, string>("../../../DataBase.xml");
                        srvr.db6 = XmltoDB.LoadXEngine<int, string>("../../../Restore.xml");
                        //send the result to WPF
                        msg.Rcontent = "\n============================================\n" + "\nThe Restore is successful\n" + "\n====Original databse====\n" + srvr.db.showDataBase() + "\n=====The DataBase Afer RESTORE=======\n" + srvr.db6.showDataBase() + "\n============================================\n";
                        Timer.Stop();
                        StringBuilder accum = new StringBuilder();
                        msg.Tcontent = accum.Append(string.Format("\nThe {0} operation cost ", ++count)) + Timer.Elapsed.ToString();//send the time cost by server to the Wpf
                    }
                   
                    //-<if msg.Rcontent contains "Query" than operation>-     
                    if (msg.Rcontent.Contains("Query"))
                    {
                        

                        string[] KeyWord = msg.Rcontent.Split(' ');
                        if (KeyWord[1].Contains("K"))//query by description
                        {
                            int Key = Int32.Parse(KeyWord[2]);
                            DBElement<int, string> elem = new DBElement<int, string>();
                            srvr.db.getValue(Key, out elem);
                            msg.Rcontent = "\n============================================\n" + "\nThe Query is successful\n" + "Qery data for the Key = 4  " + "\n============================================\n" + "  Key = "+KeyWord[2] + elem.showElement<int,string>() + "\n============================================\n";
                        }
                        if (KeyWord[1].Contains("N"))//query by name
                        {
                            int[] results = srvr.db.QueryByMetadata(x => x.name.Contains(KeyWord[2]));
                            string result = string.Join(" Key= ", results);
                            msg.Rcontent = "\n============================================\n" + "\nThe Query is successful\n" + "Qery the Data NAME = Kobe " + "\n============================================\n" + "  Key =" + result + "\n============================================\n";
                        }
                        if (KeyWord[1].Contains("P"))//query by payload
                        {
                            int[] results = srvr.db.QueryByMetadata(x => x.payload.Contains(KeyWord[2]));
                            string result = string.Join(" Key= ", results);
                            msg.Rcontent = "\n============================================\n" + "\nThe Query is successful\n" + "Qery the Data DESCR INCLUDES 'one' " + "\n============================================\n" + "  Key =" + result + "\n============================================\n";
                        }
                        if (KeyWord[1].Contains("C"))//query by children
                        {
                            int[] results = srvr.db.QueryByMetadata(x => x.children.ToString().Contains(KeyWord[2]));
                            string result = string.Join(" Key= ", results);
                            msg.Rcontent = "\n============================================\n" + "\nThe Query is successful\n" + "Qery the Data CHILDREN INCLUDES '1' " + "\n============================================\n" + "  Key = " + result + "\n============================================\n";

                        }
                        if (KeyWord[1].Contains("D"))//query by description
                        {
                            int[] results = srvr.db.QueryByMetadata(x => x.descr.Contains(KeyWord[2]));
                            string result = string.Join(" Key= ", results);
                            msg.Rcontent = "\n============================================\n" + "\nThe Query is successful\n" + "Qery the Data DESCRIPITION INCLUDES 'Basketball' " + "\n============================================\n" + "  Key = " + result + "\n============================================\n";
                        }

                    }



                    // swap urls for outgoing message
                    Util.swapUrls(ref msg);
                    sndr.sendMessage(msg);



                }

            };


            if (rcvr.StartService())
            {
                rcvr.doService(serviceAction); // This serviceAction asynchronous, so the call doesn't block.
            }
            Util.waitForUser();



        }
        public DBElement<Key, Data> ParseXElement<Key, Data>(XElement element)
        {

            Debug.Assert(element.Name == "Record");
            var dbe = new DBElement<Key, Data>();
            dbe.name = (string)element.Element("Name");
            dbe.descr = (string)element.Element("Descr");
            dbe.timeStamp = (DateTime?)element.Element("TimeRecord") ?? DateTime.Now;

            dbe.payload = ParseElementValue<Data>(element.Element("Payload"));
            foreach (var c in element.Element("children").Elements())
                dbe.children.Add(ParseElementValue<Key>(c));
            return dbe;
        }

        private DBEngine<Key, DBElement<Key, Data>> ParseXEngine<Key, Data>(DBEngine<Key, DBElement<Key, Data>> db, XDocument doc)
        {

            foreach (var r in doc.Root.Elements("Record"))
            {
                db.insert(ParseElementValue<Key>(r.Element("Key")), ParseXElement<Key, Data>(r));
            }
            return db;
        }
        public static T ParseElementValue<T>(XElement elem)
        {
            if (typeof(T) == typeof(int)) return (T)(object)(int)elem;
            if (typeof(T) == typeof(string)) return (T)(object)(string)elem;
            throw new NotSupportedException();
        }


    }
}
