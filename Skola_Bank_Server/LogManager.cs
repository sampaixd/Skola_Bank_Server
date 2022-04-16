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
        static XmlFileManager logManager;
        static List<Log> logs;
        static LogManager()
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
            logType type = (logType)Enum.Parse(typeof(logType), log.SelectSingleNode("type").InnerText);
            string time = log.SelectSingleNode("time").InnerText;
            string ip = log.SelectSingleNode("ip").InnerText;
            string user = log.SelectSingleNode("user").InnerText;
            string message = log.SelectSingleNode("message").InnerText;
            return DefineLogType(new Log(time, ip, user, message), type);
        }
        // defines the log type and returns the results
        static Log DefineLogType(Log newLog, logType type)
        {
            switch(type)
            {
                case logType.ErrorLog:
                    return (ErrorLog)newLog;
                
                case logType.LoginLog:
                    return (LoginLog)newLog;
                
                case logType.ModificationLog:
                    return (ModificationLog)newLog;
                
                case logType.ConnectionLog:
                    return (ConnectionLog)newLog;
                
                case logType.TransactionLog:
                    return (TransactionLog)newLog;

                case logType.CommunicationLog:
                    return (CommunicationLog)newLog;
                default:
                    throw new InvalidLogTypeException();

            }
        }

        // used if the client is closed unexpectedly, and socket is no longer avalible
        static public void AddLog(string ip, string message, logType type)
        {
            dateTime = DateTime.Now;
            string time = dateTime.ToString("yyyy/MM/dd - HH:mm:ss");
            logs.Add(DefineLogType(new Log(time, ip, message), type));
        }
        static public void AddLog(Socket user, string message, logType type)
        {
            dateTime = DateTime.Now;
            string time = dateTime.ToString("yyyy/MM/dd - HH:mm:ss");
            string ip = user.RemoteEndPoint.ToString();
            AttemptToAddLog(new Log(time, ip, message), type);
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
            }
            catch (InvalidLogTypeException)
            {
                newLog.Message = $"Could not add log \"{newLog.Message}\"";
                logs.Add(DefineLogType(newLog, logType.ErrorLog));
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
