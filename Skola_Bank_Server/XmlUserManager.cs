using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.IO;

namespace Skola_Bank_Server
{
    internal class XmlUserManager
    {
        static XmlFileManager userManager;
        static List<User> users;
        static XmlUserManager()
        {
            userManager = new XmlFileManager("users.xml", "users", "user");   
            users = ExtractAllUserInfo();
        }

        static public List<User> ExtractAllUserInfo()
        {
            List<User> users = new List<User>();
            XmlNodeList extractUserInfo = userManager.XmlDocument.SelectNodes("users/user");
            foreach (XmlNode user in extractUserInfo)
            {
                string accountType = user.SelectSingleNode("accountType").InnerText;
                if (accountType == "admin")
                    users.Add(ExtractAdminInfo(user));
                else
                    users.Add(ExtractConsumerInfo(user));
            }
            return users;
        }

        static Admin ExtractAdminInfo(XmlNode admin)
        {
            string[] credentials = ExtractUserCredentials(admin);
            string password = admin.SelectSingleNode("password").InnerText;
            return new Admin(credentials[0], credentials[1], credentials[2], password);
        }

        // 0 is first name, 1 is last name and 2 is social security number
        static string[] ExtractUserCredentials(XmlNode user)
        {
            string[] credentials = new string[3];
            credentials[0] = user.SelectSingleNode("firstName").InnerText;
            credentials[1] = user.SelectSingleNode("lastName").InnerText;
            credentials[2] = user.SelectSingleNode("socialSecurityNumber").InnerText;
            return credentials;
        }

        static Consumer ExtractConsumerInfo(XmlNode consumer)
        {
            string[] credentials = ExtractUserCredentials(consumer);
            List<Deposit> deposits = ExtractUserDeposit(consumer);
            return new Consumer(credentials[0], credentials[1], credentials[2], deposits);
        }

        static List<Deposit> ExtractUserDeposit(XmlNode consumer)
        {
            List<Deposit> deposits = new List<Deposit>();
            XmlNodeList depositsXml = consumer.SelectNodes("users/user/deposits");
            foreach (XmlNode deposit in depositsXml)
                deposits.Add(ExtractSingleDeposit(deposit));

            return deposits;
        }

        static Deposit ExtractSingleDeposit(XmlNode deposit)
        {
            string depositName = deposit.SelectSingleNode("name").InnerText;
            int depositId = Convert.ToInt32(deposit.SelectSingleNode("id").InnerText);
            double balance = Convert.ToDouble(deposit.SelectSingleNode("balance").InnerText);
            return new Deposit(depositName, depositId, balance);
        }

        static void AddConsumer(Consumer newConsumer)
        {
            XmlDocument xmlDocument = userManager.XmlDocument;
            XmlElement userElement = userManager.CreateNewElement();
            AddUserCredentials(newConsumer, userElement);

            XmlElement deposits = xmlDocument.CreateElement("deposits");
            userElement.AppendChild(deposits);
            xmlDocument.Save(userManager.Path);
        }

        static void AddAdmin(Admin newAdmin)
        {
            XmlDocument xmlDocument = userManager.XmlDocument;
            XmlElement userElement = userManager.CreateNewElement();
            AddUserCredentials(newAdmin, userElement);

            XmlElement password = xmlDocument.CreateElement("password");
            password.InnerText = newAdmin.Password;
            userElement.AppendChild(password);

            xmlDocument.Save(userManager.Path);
        }

        static void AddUserCredentials(User newUser, XmlElement userElement)
        {
            XmlDocument xmlDocument = userManager.XmlDocument;
            XmlElement firstName = xmlDocument.CreateElement("firstName");
            firstName.InnerText = newUser.FirstName;
            userElement.AppendChild(firstName);

            XmlElement lastName = xmlDocument.CreateElement("lastName");
            lastName.InnerText = newUser.LastName;
            userElement.AppendChild(lastName);

            XmlElement socialSecurityNumber = xmlDocument.CreateElement("socialSecurityNumber");
            socialSecurityNumber.InnerText = newUser.SocialSecurityNumber;
            userElement.AppendChild(socialSecurityNumber);
        }

        // TODO make it so you can find the correct user by searching for the matching social security number
        static void AddDeposit(Deposit newDeposit, User currentUser)
        {
            try
            {
                XmlElement SelectedUser = userManager.FindElement("socialSecurityNumber", currentUser.SocialSecurityNumber);
            }
            catch(NonExistingElementException)
            {
                XmlLogManager.AddLog(currentUser, "Could not find user in database", "ErrorLog");
                return;
            }

            
        }
    }
}
