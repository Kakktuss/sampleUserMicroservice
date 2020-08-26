using System;
using BuildingBlock.DataAccess;

namespace UserApplication.Models
{
    public class Country : Entity
    {
        public Country(Guid uuid,
            string code)
        {
            Uuid = uuid;

            Code = code;
        }

        public Guid Uuid { get; }

        public string Code { get; }
    }
}