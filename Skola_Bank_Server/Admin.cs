using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace Skola_Bank_Server
{
    internal class Admin : User
    {
        string password;
        public Admin(string firstName, string lastName, string socialSecurityNumber, string password) : base(firstName, lastName, socialSecurityNumber)
        {
            this.password = password;
        }
        public string Password { get { return password; } }

        public void LoggedinMenu(Socket client)
        {
            connection = client;
            try
            {
                RecieveSelectedOption();
            }
            catch (ClientDisconnectedException)
            {
                throw;
            }
            finally
            {
                connection = null;
            }
        }

        void RecieveSelectedOption()
        {
            bool loggedIn = true;
            while (loggedIn)
            {
                string selectedOption = SocketComm.RecvMsg(connection);
                if (selectedOption != "logout")
                    ForwardSelectedOption(selectedOption);
                
                else
                    return;
            }
        }

        void ForwardSelectedOption(string selectedOption)
        {
            switch(selectedOption)
            {
                case "suspend":
                    SuspendMenu();
                    break;

                case "changeInfo":
                    ChangeUserInformation();
                    break;

                case "viewLogs":
                    LogsMenu();
                    break;
            }
        }

        void SuspendMenu()
        {
            UserManager.SendAllConsumerInfo(connection);
            string selectedAction = SocketComm.RecvMsg(connection); // example action : selectedConsumer|true/false
            if (selectedAction != "back")
            {
                string[] splitAction = selectedAction.Split('|');
                UserManager.ManageSuspension(splitAction);
            }
        }



        public override string FormatInfo()
        {
            return $"{firstName}|{lastName}|{socialSecurityNumber}|{password}";
        }
    }
}
