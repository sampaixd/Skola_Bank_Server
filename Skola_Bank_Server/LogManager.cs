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
    internal static class LogManager
    {
        static DateTime dateTime;
        static XmlFileManager xmlManager;
        static List<Log> logs;
        static LogManager()
        {
            dateTime = new DateTime();
            xmlManager = new XmlFileManager("logs.xml", "logs", "log");
            logs = ExtractAllLogs();
        }

        static List<Log> ExtractAllLogs()
        {
            List<Log> logs = new List<Log>();
            XmlNodeList extractLogs = xmlManager.XmlDocument.SelectNodes("logs/log");
            foreach (XmlNode log in extractLogs)
            {
                logs.Add(ExtractSingleLog((XmlElement)log));
            }
            return logs;
        }

        static Log ExtractSingleLog(XmlElement log)
        {
            logType type = (logType)Enum.Parse(typeof(logType), log.SelectSingleNode("type").InnerText);
            string time = log.SelectSingleNode("time").InnerText;
            string ip = log.SelectSingleNode("ip").InnerText;
            string socialSecurityNumber = log.SelectSingleNode("socialSecurityNumber").InnerText;
            string message = log.SelectSingleNode("message").InnerText;

            return DefineLogType(new Log(time, socialSecurityNumber, ip, message), type);
        }
        // defines the log type and returns the results
        static Log DefineLogType(Log newLog, logType type)
        {
            // (DowncastLog) newLog nor newLog as DowncastLog worked, so this is the best option I could fix
            switch(type)
            {
                case logType.ErrorLog:
                    return new ErrorLog(newLog.Time, newLog.Ip, newLog.SocialSecurityNumber, newLog.Message);
                
                case logType.LoginLog:
                    return new LoginLog(newLog.Time, newLog.Ip, newLog.SocialSecurityNumber, newLog.Message);
                
                case logType.ModificationLog:
                    return new ModificationLog(newLog.Time, newLog.Ip, newLog.SocialSecurityNumber, newLog.Message);
                
                case logType.ConnectionLog:
                    return new ConnectionLog(newLog.Time, newLog.Ip, newLog.SocialSecurityNumber, newLog.Message);
                    
                
                case logType.TransactionLog:
                    return new TransactionLog(newLog.Time, newLog.Ip, newLog.SocialSecurityNumber, newLog.Message);

                case logType.CommunicationLog:
                    return new CommunicationLog(newLog.Time, newLog.Ip, newLog.SocialSecurityNumber, newLog.Message);
                default:
                    throw new InvalidLogTypeException();

            }
        }

        // used if the client is closed unexpectedly, and socket is no longer avalible
        static public void AddLog(string ip, string message, logType type)
        {
            dateTime = DateTime.Now;
            string time = dateTime.ToString("yyyy/MM/dd - HH:mm:ss");
            AttemptToAddLog(new Log(time, ip, message), type);
        }
        static public void AddLog(Socket user, string message, logType type)
        {
            dateTime = DateTime.Now;
            string time = dateTime.ToString("yyyy/MM/dd - HH:mm:ss");
            string ip = user.RemoteEndPoint.ToString();
        }

        // with user instead of socket we can also log the current user by social security number
        static public void AddLog(User user, string message, logType type)
        {
            dateTime = DateTime.Now;
            string time = dateTime.ToString("yyyy-MM-dd HH:mm:ss");
            string ip = user.IP;
            string socialSecurityNumber = user.SocialSecurityNumber;
            AttemptToAddLog(new Log(time, socialSecurityNumber, ip, message), type);
        }
        // for changes made by the server, no ip is neccesary
        static public void AddLog(string message, logType type)
        {
            dateTime = DateTime.Now;
            string time = dateTime.ToString("yyyy-MM-dd HH:mm:ss");
            string ip = "local";
            AttemptToAddLog(new Log(time, ip, message), type);

        }

        static void AttemptToAddLog(Log newLog, logType type)
        {
            try
            {
                logs.Add(DefineLogType(newLog, type));
                XmlElement parentNode = xmlManager.CreateParentMode();
                XmlDocument xmlDocument = xmlManager.XmlDocument;

                XmlElement xmlType = xmlDocument.CreateElement("type");
                xmlType.InnerText = type.ToString();
                parentNode.AppendChild(xmlType);

                XmlElement time = xmlDocument.CreateElement("time");
                time.InnerText = newLog.Time;
                parentNode.AppendChild(time);

                XmlElement socialSecurityNumber = xmlDocument.CreateElement("socialSecurityNumber");
                socialSecurityNumber.InnerText = newLog.SocialSecurityNumber;
                parentNode.AppendChild(socialSecurityNumber);

                XmlElement ip = xmlDocument.CreateElement("ip");
                ip.InnerText = newLog.Ip;
                parentNode.AppendChild(ip);

                XmlElement message = xmlDocument.CreateElement("message");
                message.InnerText = newLog.Message;
                parentNode.AppendChild(message);

                xmlDocument.Save(xmlManager.Path);
            }
            catch (InvalidLogTypeException)
            {
                newLog.Message = $"Could not add log \"{newLog.Message}\"";
                AttemptToAddLog(newLog, logType.ErrorLog);
            }
        }

        //does not send error or communication logs
        static public void SendLogs(Socket recievingAdmin)
        {
            foreach(Log log in logs)
            {   // TODO reverse statement so it instead checks if its not one of the above, and then sends the data
                if (log is CommunicationLog || log is ErrorLog)
                    continue;
                SocketComm.SendMsg(recievingAdmin, log.FormatLog());
            }
            SocketComm.SendMsg(recievingAdmin, "end");
        }
    }
}
