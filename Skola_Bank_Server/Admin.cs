using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public override string FormatInfo()
        {
            return $"{firstName}|{lastName}|{socialSecurityNumber}|{password}";
        }
    }
}
