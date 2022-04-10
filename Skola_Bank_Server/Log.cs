using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skola_Bank_Server
{
    internal class Log
    {
        string time;
        string user;
        string ip;
        string message;
        public Log(string time, string user, string ip, string message)
        {
            this.time = time;
            this.user = user;
            this.ip = ip;
            this.message = message;
        }
        public Log(string time, string ip, string message)
        {
            this.time = time;
            this.user = "none";
            this.ip = ip;
            this.message = message;
        }

        public void DisplayLog()
        {
            Console.WriteLine($"{time} - ip: {ip} - user: {user}  - message: {message}");
        }
    }
}
