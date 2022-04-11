using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skola_Bank_Server
{
    // logs changes in data, such as account creation, account changes and suspensions
    internal class ModificationLog : Log
    {
        public ModificationLog(string time, string socialSecurityNumber, string ip, string message) : base(time, socialSecurityNumber, ip, message)
        { }
        public ModificationLog(string time, string ip, string message) : base(time, ip, message)
        { }
    }
}
