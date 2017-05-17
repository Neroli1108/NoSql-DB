/////////////////////////////////////////////////////////////////////////
//Xdoc.cs - Help to convert the xdocument to the xmldocument or reverse//
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

namespace Xdoc
{
    
        public static class DocumentExtensions
        {

        //-<Function to convert XDocument to XMLDocument>-
            public static XmlDocument ToXmlDocument(this XDocument xDocument)
            {
                var xmlDocument = new XmlDocument();
                using (var xmlReader = xDocument.CreateReader())
                {
                    xmlDocument.Load(xmlReader);
                }
                return xmlDocument;
            }
        //-<Function to convert XMLDocument to XDocument>-
            public static XDocument ToXDocument(this XmlDocument xmlDocument)
            {
                using (var nodeReader = new XmlNodeReader(xmlDocument))
                {
                    nodeReader.MoveToContent();
                    return XDocument.Load(nodeReader);
                }
            }
        //Test the function
        internal class Program
        {
            private static void Main(string[] args)
            {

                var xmlDocument = new XmlDocument();
                xmlDocument.LoadXml("<Root><Child>Test</Child></Root>");

                var xDocument = xmlDocument.ToXDocument();
                var newXmlDocument = xDocument.ToXmlDocument();
                Console.ReadLine();
            }
        }
    }
    }

