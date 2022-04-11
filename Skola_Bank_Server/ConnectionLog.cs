using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skola_Bank_Server
{
    // keeps track of who connects and disconnects to the server
    internal class ConnectionLog : Log
    {
        public ConnectionLog(string time, string socialSecurityNumber, string ip, string message) : base(time, socialSecurityNumber, ip, message)
        { }
        public ConnectionLog(string time, string ip, string message) : base(time, ip, message)
        { }
    }
}
