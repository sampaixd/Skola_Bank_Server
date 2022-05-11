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

                case "deleteDeposit":
                    DeleteDeposit();
                    break;

                case "deleteAccount":
                    DeleteUser();
                    break;

                case "changeInfo":  //scrapped idea
                    ChangeUserInformation();
                    break;
            }
        }

        void PerformTransaction()
        {
            string transactionTarget = SocketComm.RecvMsg(connection);

            if (transactionTarget == "back")
                return;
            else if (transactionTarget == "local")
                LocalTransaction();
            else
                OnlineTransaction();
        }

        // transaction within the current users deposits
        void LocalTransaction()
        {
            // givingdepositId|recievingDepositId|transactionAmmount
            string recievedDepositsAndAmmount = SocketComm.RecvMsg(connection);
            if (recievedDepositsAndAmmount == "back")
                return;

            string[] recievedDepositsAndAmmountArr = recievedDepositsAndAmmount.Split('|');
            UserManager.LocalTransaction(this, recievedDepositsAndAmmountArr);


        }
        
        // transaction with another consumer
        void OnlineTransaction()
        {
            throw new NotImplementedException();
        }

        void AddDeposit()
        {
            // depositName|depositId|depositBalance
            string newDeposit = SocketComm.RecvMsg(connection);
            if (newDeposit == "back")
                return;
            string[] newDepositArr = newDeposit.Split('|');
            UserManager.AddDeposit(new Deposit(newDepositArr[0], Convert.ToInt32(newDepositArr[1]), Convert.ToDouble(newDepositArr[2])), this);
        }

        void DeleteDeposit()
        {
            string depositId = SocketComm.RecvMsg(connection);
            if (depositId == "back")
                return;
            deposits.RemoveAt(Convert.ToInt32(depositId));
            UserManager.DeleteDeposit(depositId, this);
        }

        void DeleteUser()
        {
            UserManager.DeleteUser(this);
        }

        protected override void ChangeUserInformation()
        {
            throw new NotImplementedException();
        }

        protected override void ChangeUsername(string[] SelectedNameAndNewName)
        {
            throw new NotImplementedException();
        }

        protected override void ChangeFirstName(string newFirstName)
        {
            throw new NotImplementedException();
        }

        protected override void ChangeLastName(string newLastName)
        {
            throw new NotImplementedException();
        }

        public override string FormatInfo()
        {
            string allDeposits = FormatDeposits();
            return $"{firstName}|{lastName}|{socialSecurityNumber}|{allDeposits}depositEnd";
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
