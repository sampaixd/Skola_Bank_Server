using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Skola_Bank_Server
{
    internal class Consumer : User
    {
        List<Deposit> deposits;
        public Consumer(string firstName, string lastName, string socialSecurityNumber, List<Deposit> deposits) : base(firstName, lastName, socialSecurityNumber)
        {
            this.deposits = deposits;
        }

        public Consumer(string firstName, string lastName, string socialSecurityNumber) : base(firstName, lastName, socialSecurityNumber)
        {
            this.deposits = new List<Deposit>();
        }

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

        void ForwardSelectedOption(string selectedOption)
        {
            switch(selectedOption)
            {
                case "transaction":
                    PerformTransaction();
                    break;

                case "addDeposit":
                    AddDeposit();
                    break;

                case "editDeposit":
                    EditDeposit();
                    break;

                case "changeInfo":
                    ChangeUserInformation();
                    break;
            }
        }

        void PerformTransaction()
        {
            string transactionTarget = SocketComm.RecvMsg(connection);
            if (transactionTarget == "self")
                LocalTransaction();
            else
                OnlineTransaction();
        }

        // transaction within the current users deposits
        void LocalTransaction()
        {
            throw new NotImplementedException();
        }
        
        // transaction with another consumer
        void OnlineTransaction()
        {
            throw new NotImplementedException();
        }
        public string FormatInfo()
        {
            string allDeposits = FormatDeposits();
            return $"{firstName}|{lastName}|{socialSecurityNumber}|depositStart|{allDeposits}depositEnd";
        }

        string FormatDeposits()
        {
            string allDeposits = "";
            foreach (Deposit deposit in deposits)
            {
                allDeposits += $"{deposit.Name}|{deposit.Id}|{deposit.Balance}|";
            }
            return allDeposits;
        }
    }
}
