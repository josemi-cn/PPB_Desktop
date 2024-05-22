using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project_Phone_Basket___Storage_App.Model
{
    public class User
    {
        public int Id { get; set; }

        public string FirstName { get; set; } = null;

        public string LastName { get; set; } = null;

        public string Username { get; set; } = null;

        public string Password { get; set; } = null;

        public int RoleId { get; set; }
    }
}
