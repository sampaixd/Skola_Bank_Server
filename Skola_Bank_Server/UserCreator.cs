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
            bool creatingUser = true;
            while (creatingUser)
            {

                string userCredentials = SocketComm.RecvMsg(client);
                if (string.IsNullOrEmpty(userCredentials))
                    return;
                string[] decipheredCredentials = userCredentials.Split('|'); // 0 is firstName, 1 is lastName and 2 is socialSecurityNumber
                if (IsSocialSecurityNumberAvalible(decipheredCredentials[2]))
                {
                    UserManager.AddConsumer(new Consumer(decipheredCredentials[0], decipheredCredentials[1], decipheredCredentials[2]));
                    LogManager.AddLog(client.RemoteEndPoint.ToString(), $"Created new user {decipheredCredentials[2]}", logType.ModificationLog);
                    SocketComm.SendMsg(client, "True");
                    return;
                }
                LogManager.AddLog(client.RemoteEndPoint.ToString(), "attempted to create a user with an already taken social security number", logType.ModificationLog);
                SocketComm.SendMsg(client, "False");
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
