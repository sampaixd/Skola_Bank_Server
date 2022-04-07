using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skola_Bank_Server
{
    internal class Consumer : User
    {
        public Consumer(string firstName, string lastName, string socialSecurityNumber) : base(firstName, lastName, socialSecurityNumber)
        {
            
        }
    }
}
