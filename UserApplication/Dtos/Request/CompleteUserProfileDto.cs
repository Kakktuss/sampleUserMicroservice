using System;

namespace UserApplication.Dtos.Request
{
    public class CompleteUserProfileDto
    {

        public CompleteUserProfileDto(string userUuid,
            Guid countryUuid)
        {
            UserUuid = userUuid;

            CountryUuid = countryUuid;
        }
        
        public string UserUuid { get; set; }
        
        public Guid CountryUuid { get; set; }

    }
}