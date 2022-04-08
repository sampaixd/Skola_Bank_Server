using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.IO;

namespace Skola_Bank_Server
{
    internal class XmlFileManager
    {
        static string path;
        static XmlDocument xmlUsers;
        static List<User> users = new List<User>();
        static XmlFileManager()
        {
            path = "accounts.xml";
            if (!File.Exists(path))
                CreateXml();
            xmlUsers = new XmlDocument();
            xmlUsers.LoadXml(path);
            users = ExtractAllUserInfo();
        }

        static void CreateXml()
        {
            XmlDeclaration xmldeclaration = xmlUsers.CreateXmlDeclaration("1.0", "utf-8", null);
            xmlUsers.AppendChild(xmldeclaration);
            XmlElement messages = xmlUsers.CreateElement("users");
            xmlUsers.AppendChild(messages);
            xmlUsers.Save(path);
        }

        static List<User> ExtractAllUserInfo()
        { 
            List<User> users = new List<User>();
            XmlNodeList extractUserInfo = xmlUsers.SelectNodes("users/user");
            foreach (XmlNode node in extractUserInfo)
            {
                string accountType = node.SelectSingleNode("accountType").InnerText;
                if (accountType == "admin")
                    users.Add(ExtractAdminInfo(node));
                else
                    users.Add(ExtractConsumerInfo(node));
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

    }
}
