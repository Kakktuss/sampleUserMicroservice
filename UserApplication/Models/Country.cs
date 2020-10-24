using System;
using System.Collections.Generic;
using BuildingBlock.DataAccess;

namespace UserApplication.Models
{
    public class Country : Entity
    {
        protected Country()
        {
            
        }
        
        public Country(Guid uuid)
        {
            Uuid = uuid;
        }

        public Guid Uuid { get; }
        
        #region User relationship
        
        public List<User> Users { get; }

        #endregion
    }
}