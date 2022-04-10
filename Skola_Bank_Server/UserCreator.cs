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
            string[] decipheredCredentials = DecipherMessage(userCredentials);
        }

        static string[] DecipherMessage(string message)
        {
            string[] credentials = new string[3];
            char[] messageChars = message.ToCharArray();
            int[] separationCharLocation = FindAllSeparationChars(messageChars);
            // +1 is in place to prevent a | to appear at the start of the string, the -1 does the same but in the end instead
            string firstName = message.Substring(0, separationCharLocation[0]);
            
            string lastName = message.Substring(separationCharLocation[0] + 1, separationCharLocation[1] - separationCharLocation[0] - 1);
            
            string socialSecurityNumber = message.Substring(separationCharLocation[1] + 1, message.Length - separationCharLocation[1] - 1);

            return new string[] { firstName, lastName, socialSecurityNumber };

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


        static void CheckIfSocialSecurityNumberIsTaken()
        {
            if ()
        }
    }
}
