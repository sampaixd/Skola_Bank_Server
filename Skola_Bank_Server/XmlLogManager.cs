using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.IO;
using System.Net.Sockets;

namespace Skola_Bank_Server
{
    internal static class XmlLogManager
    {
        static DateTime dateTime;
        static XmlFileManager logManager;
        static List<Log> logs;
        static XmlLogManager()
        {
            dateTime = new DateTime();
            logManager = new XmlFileManager("logs.xml", "logs", "log");
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
            string type = log.SelectSingleNode("type").InnerText;
            string time = log.SelectSingleNode("time").InnerText;
            string ip = log.SelectSingleNode("ip").InnerText;
            string user = log.SelectSingleNode("user").InnerText;
            string message = log.SelectSingleNode("message").InnerText;
            return DefineLogType(new Log(time, ip, user, message), type);
        }
        // defines the log type and returns the results
        static Log DefineLogType(Log newLog, string type)
        {
            switch(type)
            {
                case "ErrorLog":
                    return (ErrorLog)newLog;
                case "LoginLog":
                    return (LoginLog)newLog;
                case "ModificationLog":
                    return (ModificationLog)newLog;
                case "ConnectionLog":
                    return (ConnectionLog)newLog;
                default:
                    throw new InvalidLogTypeException();

            }
        }

        static public void AddLog(Socket user, string message, string type)
        {
            dateTime = DateTime.Now;
            string time = dateTime.ToString("yyyy/MM/dd - HH:mm:ss");
            string ip = user.RemoteEndPoint.ToString();
            logs.Add(DefineLogType(new Log(time, ip, message), type));
        }

        // with user instead of socket we can also log the current user by social security number
        static public void AddLog(User user, string message, string type)
        {
            dateTime = DateTime.Now;
            string time = dateTime.ToString("yyyy-MM-dd HH:mm:ss");
            string ip = user.IP;
            string socialSecurityNumber = user.SocialSecurityNumber;
            try
            {
                logs.Add(DefineLogType(new Log(time, socialSecurityNumber, ip, message), type));
            }
            catch (InvalidLogTypeException)
            {

            }
        }
    }
}
