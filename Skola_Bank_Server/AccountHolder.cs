using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skola_Bank_Server
{
    internal class AccountHolder <User>
    {
        User[] AccountArray;
        int pointer = 0;
        int currentArraySize;
        public AccountHolder()
        {
            AccountArray = new User[currentArraySize];
        }

        static void LoadAllAccounts()
        {

        }

        
    }
}
