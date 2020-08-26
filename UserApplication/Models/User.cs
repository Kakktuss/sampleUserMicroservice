using System;
using BuildingBlock.DataAccess;

namespace UserApplication.Models
{
    public class User : Entity
    {
        public User(string uuid, string username, string email, Country country)
        {
            Uuid = uuid ?? throw new ArgumentNullException("uuid", "The uuid is null");

            Username = username ?? throw new ArgumentNullException("username", "The username is null");

            Email = email ?? throw new ArgumentNullException("email", "The email is null");

            Country = country ?? throw new ArgumentNullException("country", "The country is null");
        }

        public string Uuid { get; }

        public string Username { get; }

        public string Email { get; }

        #region Country relationship

        public int CountryId { get; }

        public Country Country { get; }

        #endregion
    }
}