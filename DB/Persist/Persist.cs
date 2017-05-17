///////////////////////////////////////////////////////////////
// Persist.cs - Save DB to XML and augument the DB from XML  //
// Ver 1.0                                                   //
// Application: This package meets the requirement5          //
// Language:    C#, ver 6.0, Visual Studio 2015              //
// Author:      Yunding Li,Syracuse University               //
//                                                           //
///////////////////////////////////////////////////////////////
/*
 * Package Operations:
 * -------------------
 * This package meets the requirement5 
 *
/*
 * Maintenance:
 * ------------
 * Required Files: 
 *  DBElement.cs, DBEngine.cs,  
 *  
 *
 * Maintenance History:
 * --------------------
 * ver 1.0 : 3/Oct/ 15
 * - first release
 *
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Diagnostics;
using System.IO;

namespace Project2Starter
{
    public static class XmltoDB
    {
        #region Save
        private static XElement BuildXElement<Key, Data>(DBElement<Key, Data> element, Key thisKey)
        {
            // build <children> element
            var children = new XElement("children");
            foreach (var c in element.children)
                children.Add(new XElement("child", c));
            // return <record> with basic properties
            return new XElement("Record",
                new XElement("key", thisKey),
                new XElement("Name", element.name),
                new XElement("Descr", element.descr),
                new XElement("TimeRecord", element.timeStamp),
                new XElement("Payload", element.payload),
                children);
        }

        /// <summary>
        /// Build XDocument from DBEngine.
        /// </summary>
        private static XDocument BuildXEngine<Key, TData>(DBEngine<Key, DBElement<Key, TData>> engine)
        {
            var doc = new XDocument(new XDeclaration("1.0", "UTF-8", "YES"), new XElement("dataBase"));
            // Just stuck records into root element.
            foreach (var key in engine.Keys())
            {
                DBElement<Key, TData> element;
                if (engine.getValue(key, out element))
                    doc.Root.Add(BuildXElement(element, key));
            }
            return doc;
        }

        /// <summary>
        /// Save the content of DBEngine into a file.
        /// </summary>
        public static void SaveXEngine<Key, Data>(DBEngine<Key, DBElement<Key, Data>> engine, string path)
        {
            BuildXEngine(engine).Save(path);
        }
        #endregion

        #region Load

        private static DBElement<Key, Data> ParseXElement<Key, Data>(XElement element)
        {
            //XElement child1 = new XElement("Keytype", "int");
            //XElement child2 = new XElement("payloadtype", "String");
            //XElement child3 = new XAttribute("Key", 4);
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

        public static DBEngine<Key, DBElement<Key, Data>> ParseXEngine<Key, Data>(XDocument doc)
        {
            var eng = new DBEngine<Key, DBElement<Key, Data>>();
            foreach (var r in doc.Root.Elements("Record"))
            {
                eng.insert(ParseElementValue<Key>(r.Element("Key")), ParseXElement<Key, Data>(r));
            }
            return eng;
        }

        /// <summary>
        /// Load the content of DBEngine from a file.
        /// </summary>
        public static DBEngine<Key, DBElement<Key, Data>> LoadXEngine<Key, Data>(string path)
        {
            return ParseXEngine<Key, Data>(XDocument.Load(path));
        }
        #endregion

        /// <summary>
        /// A helper function, which is used to extract value from XElement.
        /// </summary>
        private static T ParseElementValue<T>(XElement elem)
        {
            if (typeof(T) == typeof(int)) return (T)(object)(int)elem;
            if (typeof(T) == typeof(string)) return (T)(object)(string)elem;
            throw new NotSupportedException();
        }
    }


  
}


#if (TEST_Persist)


class TestPersist
  {
    static void Main(string[] args)
    {
      
      Console.WriteLine();

      Console.Write("\n  All testing of DBElement class moved to DBElementTest package.");
        Console.Write("\n  This allow use of DBExtensions package without circular dependencies.");

        Console.Write("\n\n");
    }
  }
#endif
