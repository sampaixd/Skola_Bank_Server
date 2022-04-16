using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skola_Bank_Server
{
    // used for logging error messages
    internal class ErrorLog : Log
    {
        public ErrorLog(string time, string socialSecurityNumber, string ip, string message) : base(time, socialSecurityNumber, ip, message)
        { }
        public ErrorLog(string time, string ip, string message) : base(time, ip, message)
        { }

        public override string FormatLog()
        {
            return base.FormatLog() + "|ErrorLog";
        }
    }
}
