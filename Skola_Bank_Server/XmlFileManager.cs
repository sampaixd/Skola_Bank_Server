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
        static string path;
        static XmlDocument xmlAccounts;
        static XmlFileManager()
        {
            path = "accounts.xml";

        }

    }
}
