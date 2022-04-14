using System;
using System.Collections.Generic;
using System.Linq;
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
