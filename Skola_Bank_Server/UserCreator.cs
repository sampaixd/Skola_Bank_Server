using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace Skola_Bank_Server
{
    internal class UserCreator
    {
        static public void CreateUser(Socket client)
        {
            string userCredentials = SocketComm.RecvMsg(client);
            if (string.IsNullOrEmpty(userCredentials))
                return;
            string[] decipheredCredentials = userCredentials.Split('|'); // 0 is firstName, 1 is lastName and 2 is socialSecurityNumber
            for (int i = 0; i < decipheredCredentials.Length; i++)
                Console.WriteLine(decipheredCredentials[i]);
            if (IsSocialSecurityNumberAvalible(decipheredCredentials[2]))
            {
                UserManager.AddConsumer(new Consumer(decipheredCredentials[0], decipheredCredentials[1], decipheredCredentials[2]));
                LogManager.AddLog(client.RemoteEndPoint.ToString(), $"Created new user {decipheredCredentials[2]}", logType.ModificationLog);
                SocketComm.SendMsg(client, "True");
                return;
            }
            else
            {
                LogManager.AddLog(client.RemoteEndPoint.ToString(), "attempted to create a user with an already taken social security number", logType.ModificationLog);
                SocketComm.SendMsg(client, "False");
                return;
            }
        }

        static bool IsSocialSecurityNumberAvalible(string socialSecurityNumber)
        {
            try
            {
                UserManager.FindSocialSecurityNumber(socialSecurityNumber);
                return true;
            }
            catch(SocialSecurityNumberTaken)
            {
                return false;
            }
        }
    }
}
