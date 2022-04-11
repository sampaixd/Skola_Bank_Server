using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skola_Bank_Server
{
    // logs login and logout attemps, both successful and unsuccessful
    internal class LoginLog : Log
    {
        public LoginLog(string time, string socialSecurityNumber, string ip, string message) : base(time, socialSecurityNumber, ip, message)
        { }
        public LoginLog(string time, string ip, string message) : base(time, ip, message)
        { }
    }
}
