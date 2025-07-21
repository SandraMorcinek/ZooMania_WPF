using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZooMania.Model
{
    public class CustomerModel
    {
        public int Id { get; set; }
        public string First_name { get; set; }
        public string Last_name { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string Postal_code { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public CustomerModel(int id, string firstName, string lastName, string email, string address, string postalCode, string city, string country)
        {
            Id = id;
            First_name = firstName;
            Last_name = lastName;
            Email = email;
            Address = address;
            Postal_code = postalCode;
            City = city;
            Country = country;
        }

    }
}
