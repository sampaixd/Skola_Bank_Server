using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace Skola_Bank_Server
{
    internal class UserCreator
    {
        static void CreateUser(Socket client)
        {
            string userCredentials = SocketComm.RecvMsg(client);
        }

        static string[] DecipherMessage(string message)
        {
            string[] credentials = new string[3];
            char[] messageChars = message.ToCharArray();
            int[] separationCharLocation = FindAllSeparationChars(messageChars);


        }
        // separation char is a |, which is used when sending data between server and client
        static int[] FindAllSeparationChars(char[] message)
        {
            int[] separationChars = new int[2];
            int arrayPointer = 0;
            for(int i = 0; i < message.Length; i++)
            {
                if (message[i] == '|')
                    separationChars[arrayPointer++] = i;
                if (arrayPointer == 2)
                    return separationChars;
            }
            throw new InvalidOperationException();
            
        }


        static void CheckIfNameIsTaken()
        {
            if ()
        }
    }
}
