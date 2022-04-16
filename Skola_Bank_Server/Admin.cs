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

        public override void LoggedinMenu(Socket client)
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
        // forwards the selected option to the method it represents
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
                    SendLogs();
                    break;
            }
        }

        void SuspendMenu()
        {
            bool inSuspendMenu = true;
            while (inSuspendMenu)
            {
                UserManager.SendAllConsumerInfo(connection);
                string selectedAction = SocketComm.RecvMsg(connection); // example action : selectedConsumer|True/False
                if (selectedAction != "back")
                {
                    string[] splitAction = selectedAction.Split('|');
                    UserManager.ManageSuspension(splitAction);
                }
                else
                    return;
            }
        }

        void SendLogs()
        {
            LogManager.SendLogs(connection);
        }

        protected override void ChangeUserInformation()
        {
            // example selectedChange: firstName|newFirstName
            string selectedChange = SocketComm.RecvMsg(connection);
            string[] splitSelectedChange = selectedChange.Split('|');
            if (splitSelectedChange[0] == "firstName")
                ChangeFirstName(splitSelectedChange[1]);

            else if (splitSelectedChange[0] == "lastName")
                ChangeLastName(splitSelectedChange[1]);
            else
                ChangePassword(splitSelectedChange[1]);
        }

        protected override void ChangeFirstName(string newFirstName)
        {
            firstName = newFirstName;
            UserManager.ChangeFirstName(this, newFirstName);
        }

        protected override void ChangeLastName(string newLastName)
        {
            lastName = newLastName;
            UserManager.ChangeLastName(this, newLastName);
        }

        void ChangePassword(string newPassword)
        {
            password = newPassword;
            UserManager.ChangePassword(this, newPassword);
        }

        public override string FormatInfo()
        {
            return $"{firstName}|{lastName}|{socialSecurityNumber}|{password}";
        }
    }
}
