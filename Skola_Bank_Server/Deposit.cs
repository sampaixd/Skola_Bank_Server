using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skola_Bank_Server
{
    internal class Deposit
    {
        string name;
        int id;
        double balance;
        public Deposit(string name, int id, double balance)
        {
            this.name = name;
            this.id = id;
            this.balance = balance;
        }
        public Deposit(int id, double balance)
        {
            this.name = $"Deposit {id}";
            this.id = id;
            this.balance = balance;
        }
    }
}
