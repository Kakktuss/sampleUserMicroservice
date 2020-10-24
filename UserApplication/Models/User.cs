using System;
using BuildingBlock.DataAccess;

namespace UserApplication.Models
{
    public class User : Entity
    {
        protected User()
        {
            
        }

        public User(string uuid, string username, string email)
        {
            Uuid = uuid ?? throw new ArgumentNullException("uuid", "The uuid is null");

            Username = username ?? throw new ArgumentNullException("username", "The username is null");

            Email = email ?? throw new ArgumentNullException("email", "The email is null"); 
        }

        public string Uuid { get; }

        public string Username { get; }

        public string Email { get; }
        
        public bool IsCompleted { get; protected set; }

        #region Country relationship

        public int? CountryId { get; }

        public Country? Country { get; protected set; }

        #endregion

        public void SetCountry(Country country)
        {
            Country = country;
        }

        public void MarkAsComplete()
        {
            IsCompleted = true;
        }
    }
}