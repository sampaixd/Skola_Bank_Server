using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skola_Bank_Server
{
    internal class UserManager
    {
        static List<User> users;
        static UserManager()
        {
            users = XmlUserManager.ExtractAllUserInfo();
        }

        static public void AddNewConsumer(Consumer newConsumer)
        {

        }

        static public void AddNewAdmin()
        {

        }
        static public void SuspendUser(string socialSecurityNumber)
        {
            foreach(User user in users)
            {
                if (user.SocialSecurityNumber == socialSecurityNumber)
                {

                }
            }
        }
        static public void RemoveUser()
        {

        }

        static void CheckIfSocialSecurityNumberIsTaken(string testingSocialSecurityNumber)
        {
            foreach (User user in users)
            {
                if (user.SocialSecurityNumber == testingSocialSecurityNumber)
                    throw new SocialSecurityNumberTaken();
            }
        }

    }
}
