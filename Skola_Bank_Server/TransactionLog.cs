using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skola_Bank_Server
{
    internal class TransactionLog : Log
    {
        public TransactionLog(string time, string user, string ip, string message) : base(time, user, ip, message)
        { }
        public TransactionLog(string time, string ip, string message) : base(time, ip, message)
        { }
    }
}
