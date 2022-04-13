using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace Skola_Bank_Server
{
    internal class LoginManager
    {
        static public void Login(Socket client)
        {
            bool findingUser = true;
            int userId = -1;
            while (findingUser)
            {
                string credentials = SocketComm.RecvMsg(client);
                string[] decipheredCredentials = credentials.Split('|');
                try
                {
                    userId = UserManager.GetUserId(decipheredCredentials);    // instead of finding the user over and over again, we get the id instead
                    findingUser = false;
                }
                catch (NonExistingElementException)
                {
                    SocketComm.SendMsg(client, "False");
                    LogManager.AddLog(client, "failed attempt to login", logType.LoginLog);
                }
            }
            string userType = UserManager.GetUserType(userId);
            if (userType == "Admin")
                AdminLogin(client, userId);


        }

        static void AdminLogin(Socket client, int userId)
        {
            int attempts = 0;
            while (attempts < 3)
            {
                string password = SocketComm.RecvMsg(client);
                if (UserManager.IsCorrectPassword(userId, password))
                {
                    Admin activeUser = (Admin)UserManager.GetUser(userId);
                    SocketComm.SendMsg(client, activeUser.FormatInfo());
                }
            }
        }

        static void ConsumerLogin(Socket client, int userId)
    }
}
