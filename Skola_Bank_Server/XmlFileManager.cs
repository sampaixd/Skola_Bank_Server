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
        public XmlFileManager(string path, string documentElement)
        {
            this.path = path;
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

        public string Path { get { return path; } }
        public XmlDocument XmlDocument { get { return xmlDocument; } }


    }
}
