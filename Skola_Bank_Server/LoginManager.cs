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
            string credentials = SocketComm.RecvMsg(client);
            string [] decipheredCredentials = credentials.Split('|');
            User selectedUser = null;
            try
            {
                selectedUser = UserManager.GetUser(decipheredCredentials);
            }
            catch (NonExistingElementException)
            {
                SocketComm.SendMsg(client, "False");
                LogManager.AddLog(client, "failed attempt to login", logType.LoginLog);
            }

        }
    }
}
