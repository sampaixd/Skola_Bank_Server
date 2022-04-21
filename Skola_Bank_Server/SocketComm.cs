using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Threading;

namespace Skola_Bank_Server
{
    internal class SocketComm
    {
        public static string RecvMsg(Socket client)
        {
            try
            {
                byte[] msgB = new byte[1024];
                int msgSize = client.Receive(msgB);
                string msg = "";
                for (int i = 0; i < msgSize; i++)
                    msg += Convert.ToChar(msgB[i]);
                LogManager.AddLog(client.RemoteEndPoint.ToString(), $"message recieved: {msg}", logType.CommunicationLog);

                return msg;
            }
            catch (Exception)
            {
                throw new ClientDisconnectedException();
            }
        }
        // added in to make it possible to log the current user aswell
        public static string RecvMsg(User client)
        {
            try
            {
                byte[] msgB = new byte[1024];
                int msgSize = client.Connection.Receive(msgB);
                string msg = "";
                for (int i = 0; i < msgSize; i++)
                    msg += Convert.ToChar(msgB[i]);
                LogManager.AddLog(client, $"message recieved: {msg}", logType.CommunicationLog);

                return msg;
            }
            catch (Exception)
            {
                throw new ClientDisconnectedException();
            }
        }

        public static void SendMsg(Socket client, string msg)
        {
            try
            {
                LogManager.AddLog(client.RemoteEndPoint.ToString(), $"sent message: {msg}", logType.CommunicationLog);
                byte[] bSend = new byte[1024];
                bSend = Encoding.UTF8.GetBytes(msg);
                client.Send(bSend);
                Thread.Sleep(50);   // avoids multiple messages being sent at once
            }
            catch (Exception)
            {
                throw new ClientDisconnectedException();
            }
        }
        // added in to make it possible to log the current user aswell
        public static void SendMsg(User client, string msg)
        {
            try
            {
                LogManager.AddLog(client, $"sent message: {msg}", logType.CommunicationLog);
                byte[] bSend = new byte[1024];
                bSend = Encoding.UTF8.GetBytes(msg);
                client.Connection.Send(bSend);
                Thread.Sleep(50);   // avoids multiple messages being sent at once
            }
            catch (Exception)
            {
                throw new ClientDisconnectedException();
            }
        }
    }
}
