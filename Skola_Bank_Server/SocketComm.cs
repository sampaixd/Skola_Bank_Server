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
                byte[] msgB = new byte[256];
                int msgSize = client.Receive(msgB);
                string msg = "";
                for (int i = 0; i < msgSize; i++)
                    msg += Convert.ToChar(msgB[i]);
                Console.WriteLine($"{client.RemoteEndPoint} says {msg}");

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
                Console.WriteLine($"Sending message {msg} to {client.RemoteEndPoint}");
                byte[] bSend = new byte[256];
                bSend = Encoding.UTF8.GetBytes(msg);
                client.Send(bSend);
                Thread.Sleep(50);   // avoids multiple messages being sent at once
            }
            catch (Exception)
            {
                throw new ClientDisconnectedException();
            }
        }
        /*public static void SendOnlineStatus(Socket client, int ownId)
        {
            List<string> allOnlineStatus = new List<string>();
            allOnlineStatus = UserInfo.GetAllOnlineStatus();
            int currentUserID = 0;
            foreach (string onlineStatus in allOnlineStatus)
            {
                if (currentUserID != ownId) // does not send the client information about themselves
                    SendMsg(client, onlineStatus);
                currentUserID++;
            }
            SendMsg(client, "end");
        }

        public static void SendChatLogs(Socket client, int roomId)
        {
            List<Message> chatLog = ChatRoomManager.GetChatLog(roomId);
            foreach (Message message in chatLog)
            {
                SendMsg(client, message.ConvertInfoToString());
            }
            SendMsg(client, "end");
        }

        public static void SendAllChatRoomInfo(Socket client)
        {
            List<string> chatRoomInfo = ChatRoomManager.FormatChatRoomsToString();
            foreach (string roomInfo in chatRoomInfo)
            {
                SendMsg(client, roomInfo);
            }
            SendMsg(client, "end");
        }*/
    }
}
