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
        string documentElement;
        string parentNode;
        public XmlFileManager(string path, string documentElement, string parentNode)
        {
            this.path = path;
            this.documentElement = documentElement;
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
            XmlElement newDocumentElement = xmlDocument.CreateElement(documentElement);
            xmlDocument.AppendChild(newDocumentElement);
            xmlDocument.Save(path);
        }
        // creates a new element and returns it, making it possible to do more with the added element in their respective classes
        public XmlElement CreateParentMode()
        {
            XmlElement documentElement = xmlDocument.DocumentElement;

            XmlElement addedElement = xmlDocument.CreateElement(parentNode);
            documentElement.AppendChild(addedElement);
            return addedElement;
        }
        // finds and returns a parent node that has a certain child node (for example social security number)
        public XmlElement FindParentNodeByChildNode(string childNode, string searchedResult)
        {
            XmlNodeList parentNodes = xmlDocument.SelectNodes($"{documentElement}/{parentNode}");
            foreach (XmlNode parentNode in parentNodes)
            {
                string currentChildNode = parentNode.SelectSingleNode(childNode).InnerText;
                if (currentChildNode == searchedResult)
                    return (XmlElement)parentNode;
            }
            throw new NonExistingElementException();
        }

        public string FindElementByValue(string childNode, string searchedResult)
        {
            XmlNodeList parentNodes = xmlDocument.SelectNodes($"{documentElement}/{parentNode}");
            foreach (XmlNode parentNode in parentNodes)
            {
                string currentChildNode = parentNode.SelectSingleNode(childNode).InnerText;
                if (currentChildNode == searchedResult)
                    return currentChildNode;
            }
            throw new NonExistingElementException();
        }

        public void ChangeElement(User selectedUser, string childNode, string newInnerText)
        {
            XmlElement parentNode = FindParentNodeByChildNode("socialSecurityNumber", selectedUser.SocialSecurityNumber);
            XmlElement selectedChildNode = (XmlElement)parentNode.SelectSingleNode(childNode);
            selectedChildNode.InnerText = newInnerText;
        }

        public string Path { get { return path; } }
        public XmlDocument XmlDocument { get { return xmlDocument; } }


    }
}
