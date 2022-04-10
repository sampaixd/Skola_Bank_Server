using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.IO;

namespace Skola_Bank_Server
{
    internal class XmlFileManager
    {
        string path;
        XmlDocument xmlDocument;
        string parentNode;
        public XmlFileManager(string path, string documentElement, string parentNode)
        {
            this.path = path;
            this.parentNode = parentNode;
            if (!File.Exists(path))
                CreateXml();
            xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(path);
        }
        void CreateXml()
        {
            XmlDeclaration xmldeclaration = xmlDocument.CreateXmlDeclaration("1.0", "utf-8", null);
            xmlDocument.AppendChild(xmldeclaration);
            XmlElement documentElement = xmlDocument.CreateElement("users");
            xmlDocument.AppendChild(documentElement);
            xmlDocument.Save(path);
        }
        // creates a new element and returns it, making it possible to do more with the added element in their respective classes
        public XmlElement CreateNewElement()
        {
            XmlElement documentElement = xmlDocument.DocumentElement;

            XmlElement addedElement = xmlDocument.CreateElement(parentNode);
            documentElement.AppendChild(addedElement);
            return addedElement;
        }

        public string Path { get { return path; } }
        public XmlDocument XmlDocument { get { return xmlDocument; } }


    }
}
