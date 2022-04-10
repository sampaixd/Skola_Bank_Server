using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.IO;

namespace Skola_Bank_Server
{
    internal static class XmlLogManager
    {
        static XmlFileManager logManager;
        static List<Log> logs;
        static XmlLogManager()
        {
            logManager = new XmlFileManager("logs.xml", "logs");
            logs = ExtractAllLogs();
        }

        static List<Log> ExtractAllLogs()
        {
            List<Log> logs = new List<Log>();
            XmlNodeList extractLogs = logManager.XmlDocument.SelectNodes("logs/log");
            foreach (XmlNode log in extractLogs)
            {
                logs.Add(ExtractSingleLog(log));
            }
            return logs;
        }

        static Log ExtractSingleLog(XmlNode log)
        {
            
            string time = log.SelectSingleNode("time").InnerText;
            string ip = log.SelectSingleNode("ip").InnerText;
            string user = log.SelectSingleNode("user").InnerText;
            string message = log.SelectSingleNode("message").InnerText;
            return new Log(time, ip, user, message);
        }
    }
}
