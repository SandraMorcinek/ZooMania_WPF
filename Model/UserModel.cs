using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace ZooMania.Models
{
    public class UserModel
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public byte[] ImageData { get; set; }

        public void User(string id, string username, string name, string lastName, string email, byte[] imageData)
        {
            Id = id;
            Username = username;
            Name = name;
            LastName = lastName;
            Email = email;
            ImageData = imageData;
        }
    }
}
