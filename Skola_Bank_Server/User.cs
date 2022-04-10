using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skola_Bank_Server
{
    internal abstract class User
    {
        protected string firstName;
        protected string lastName;
        protected string socialSecurityNumber;
        protected bool suspended;
        public User(string firstName, string lastName, string socialSecurityNumber)
        {
            this.firstName = firstName;
            this.lastName = lastName;
            this.socialSecurityNumber = socialSecurityNumber;
            this.suspended = false;
        }
        protected abstract void LoggedinMenu();
        protected abstract void ChangeUserInformation();
        protected abstract void ChangePassword();
        protected abstract void ChangeUsername();
        protected abstract void ChangeSocialSecurityNumber();

        public string FirstName { get { return firstName; } }
        public string LastName { get { return lastName;} }
        public string SocialSecurityNumber { get { return socialSecurityNumber;} }

        public bool Suspended 
        { 
            get { return suspended; }
            set { suspended = value; }
        }

    }
}
