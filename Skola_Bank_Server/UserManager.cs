using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.IO;
using System.Net.Sockets;

namespace Skola_Bank_Server
{
    internal class UserManager
    {
        static XmlFileManager xmlManager;
        static List<User> users;
        static UserManager()
        {
            xmlManager = new XmlFileManager("users.xml", "users", "user");   
            users = ExtractAllUserInfo();
        }

        static public List<User> ExtractAllUserInfo()
        {
            List<User> users = new List<User>();
            XmlNodeList extractUserInfo = xmlManager.XmlDocument.SelectNodes("users/user");
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

        static public void AddConsumer(Consumer newConsumer)
        {
            XmlDocument xmlDocument = xmlManager.XmlDocument;
            XmlElement userElement = xmlManager.CreateParentMode();
            AddUserCredentials(newConsumer, userElement);

            XmlElement deposits = xmlDocument.CreateElement("deposits");
            userElement.AppendChild(deposits);
            xmlDocument.Save(xmlManager.Path);
        }

        static public void AddAdmin(Admin newAdmin)
        {
            XmlDocument xmlDocument = xmlManager.XmlDocument;
            XmlElement userElement = xmlManager.CreateParentMode();
            AddUserCredentials(newAdmin, userElement);

            XmlElement password = xmlDocument.CreateElement("password");
            password.InnerText = newAdmin.Password;
            userElement.AppendChild(password);

            xmlDocument.Save(xmlManager.Path);
            LogManager.AddLog("Admin added", logType.ModificationLog);
        }

        static void AddUserCredentials(User newUser, XmlElement userElement)
        {
            XmlDocument xmlDocument = xmlManager.XmlDocument;
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
        static public void AddDeposit(Deposit newDeposit, User currentUser)
        {
            XmlElement selectedUser = null; // assigns value to make it possible to compile
            try
            {
                selectedUser = xmlManager.FindParentNodeByChildNode("socialSecurityNumber", currentUser.SocialSecurityNumber);
            }
            catch(NonExistingElementException)
            {
                LogManager.AddLog(currentUser, "Could not find user in database", logType.ErrorLog);
                return;
            }
            XmlElement depositParent = (XmlElement)selectedUser.SelectSingleNode("deposits");
            CreateDeposit(depositParent, newDeposit);
            LogManager.AddLog(currentUser, $"added deposit {newDeposit.Name}", logType.ModificationLog);
            
        }

        static void CreateDeposit(XmlElement depositParent, Deposit newDeposit)
        {
            XmlDocument xmlDocument = xmlManager.XmlDocument;
            XmlElement deposit = xmlDocument.CreateElement("deposit");
            depositParent.AppendChild(deposit);

            XmlElement name = xmlDocument.CreateElement("name");
            name.InnerText = newDeposit.Name;
            deposit.AppendChild(name);

            XmlElement id = xmlDocument.CreateElement("id");
            id.InnerText = newDeposit.Id.ToString();
            deposit.AppendChild(id);

            XmlElement balance = xmlDocument.CreateElement("balance");
            balance.InnerText = newDeposit.Balance.ToString();
            deposit.AppendChild(balance);

            xmlDocument.Save(xmlManager.Path);
        }

        static public void FindSocialSecurityNumber(string searchedSocialSecurityNumber)
        {
            foreach (User user in users)
            {
                if (user.SocialSecurityNumber == searchedSocialSecurityNumber)
                    throw new SocialSecurityNumberTaken();
            }
        }

        static public User GetUser(string[] searchedUserCredentials)
        {
            foreach (User user in users)
            {
                if (user.GetCredentials() == searchedUserCredentials)
                    return user;
            }
            throw new NonExistingElementException();
        }

        static public User GetUser(int userId)
        {
            return users[userId];
        }

        static public int GetUserId(string[] searchedUserCredentials)
        {
            int currentUserId = 0;
            foreach (User user in users)
            {
                if (user.GetCredentials() == searchedUserCredentials)
                    return currentUserId;
                currentUserId++;
            }
            throw new NonExistingElementException();
        }

        static public string GetUserType(int userId)
        {
            if (users[userId] is Admin)
                return "admin";
            return "consumer";
        }

        static public bool IsCorrectPassword(int userId, string insertedPassword)
        {
            Admin tempUser = (Admin)users[userId];
            if (tempUser.Password == insertedPassword)
                return true;
            return false;
        }

        static public void SendAllConsumerInfo(Socket client)
        {
            foreach(User user in users)
            {
                if (user is Consumer)
                    SocketComm.SendMsg(client, user.FormatInfo());
            }
            SocketComm.SendMsg(client, "end");
        }

        // information 0-2 is the users credentials, information 3 is true or false where true is suspended
        static public void ManageSuspension(string[] information, Admin responsibleAdmin)
        {
            User user = null;
            try
            {
                user = GetUser(new string[] { information[0], information[1], information[2] });
            }
            catch (NonExistingElementException)
            {
                LogManager.AddLog(responsibleAdmin, $"could not find user with social security number {information[2]}", logType.ModificationLog);
                SocketComm.SendMsg(responsibleAdmin, "error");
                return;
            }
            ChangeSuspension(user, information[3]);
        }
        // if changes are made by server there is no responsible admin
        static public void ManageSuspension(string[] information)
        {
            User user = null;
            try
            {
                user = GetUser(new string[] { information[0], information[1], information[2] });
            }
            catch (NonExistingElementException)
            {
                LogManager.AddLog($"could not find user with social security number {information[2]}", logType.ModificationLog);
                return;
            }
            ChangeSuspension(user, information[3]);
        }

        static void ChangeSuspension(User user, string state)
        {
            xmlManager.ChangeElement(user, "suspended", state);
            if (state == "True")
                user.Suspended = true;
            else
                user.Suspended = false;
        }

        static public void ChangeFirstName(User user, string newFirstName)
        {
            xmlManager.ChangeElement(user, "firstName", newFirstName);
        }

        static public void ChangeLastName(User user, string newLastName)
        {
            xmlManager.ChangeElement(user, "lastName", newLastName);
        }

        static public void ChangePassword(Admin admin, string newPassword)
        {
            xmlManager.ChangeElement(admin, "password", newPassword);
        }

        // 0 is giving deposit, 1 is recieving deposit and 2 is ammount
        static public void LocalTransaction(Consumer consumer, string[] depositsAndAmount)
        {
            XmlElement consumerNode = xmlManager.FindParentNodeByChildNode("socialSecurityNumber", consumer.SocialSecurityNumber);
            XmlNodeList depositNodes = consumerNode.SelectNodes("deposits");
            XmlElement givingDeposit = null;
            XmlElement recievingDeposit = null;
            foreach (XmlNode deposit in depositNodes)
            {
                if (deposit.SelectSingleNode("id").InnerText == depositsAndAmount[0])
                    givingDeposit = (XmlElement)deposit;
                else if (deposit.SelectSingleNode("id").InnerText == depositsAndAmount[1])
                    recievingDeposit = (XmlElement)deposit;
            }

            if (givingDeposit == null || recievingDeposit == null)
                throw new NonExistingElementException();

            PerformTransaction(givingDeposit, recievingDeposit, depositsAndAmount[2]);

            string logMessage = $"{consumer.SocialSecurityNumber} transferred {depositsAndAmount[2]} from deposit {depositsAndAmount[0]} t0 {depositsAndAmount[1]}";
            LogManager.AddLog(consumer, logMessage, logType.TransactionLog);

        }
        
        // 0 is deposit id, 1 is amount (the recieving deposit is automatically the deposit with id 0
        static void OnlineTransaction(Consumer givingConsumer, Consumer recievingConsumer, string[] depositAndAmount)
        {
            XmlElement givingConsumerNode = xmlManager.FindParentNodeByChildNode("socialSecurityNumber", givingConsumer.SocialSecurityNumber);
            XmlElement recievingConsumerNode = xmlManager.FindParentNodeByChildNode("socialSecurityNumber", recievingConsumer.SocialSecurityNumber);


            PerformTransaction()
        }

        static void PerformTransaction(XmlElement givingDeposit, XmlElement recievingDeposit, string amount)
        {
            double givingDepositAmount = Convert.ToDouble(givingDeposit.SelectSingleNode("balance").InnerText);
            double recievingDepositAmount = Convert.ToDouble(recievingDeposit.SelectSingleNode("balance").InnerText);
            double transactionAmount = Convert.ToDouble(amount);

            givingDepositAmount -= transactionAmount;
            recievingDepositAmount += transactionAmount;

            givingDeposit.SelectSingleNode("balance").InnerText = Convert.ToString(givingDepositAmount);
            recievingDeposit.SelectSingleNode("balance").InnerText = Convert.ToString(recievingDepositAmount);

            xmlManager.Save();
        }
    }
}
