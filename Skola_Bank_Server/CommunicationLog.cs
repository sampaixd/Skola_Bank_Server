using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skola_Bank_Server
{
    // logs all communication between server and client
    internal class CommunicationLog : Log
    {
        public CommunicationLog(string time, string socialSecurityNumber, string ip, string message) : base(time, socialSecurityNumber, ip, message)
        { }
        public CommunicationLog(string time, string ip, string message) : base(time, ip, message)
        { }

        public override string FormatLog()
        {
            return base.FormatLog() + "|CommunicationLog";
        }
    }
}
