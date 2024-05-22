using Project_Phone_Basket___Storage_App.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Phone_Basket___Storage_App.Models
{
    public class UserDTO
    {
        public int Id { get; set; }

        public string FirstName { get; set; } = null;

        public string LastName { get; set; } = null;

        public string Name { get; set; } = null;

        public string Username { get; set; } = null;

        public string Password { get; set; } = null;

        public int RoleId { get; set; }

        public string Role { get; set; }
    }
}
