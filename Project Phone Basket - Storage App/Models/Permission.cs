using System;
using System.Collections.Generic;

namespace Project_Phone_Basket___Storage_App.Model
{
    public partial class Permission
    {
        public int Id { get; set; }

        public string Name { get; set; } = null;

        public virtual ICollection<PermissionsRole> PermissionsRoles { get; set; } = new List<PermissionsRole>();
    }
}
