using System;
using System.Collections.Generic;

namespace Project_Phone_Basket___Storage_App.Model
{
    public partial class Product
    {
        public int Id { get; set; }

        public string Barcode { get; set; } = null;

        public string Image { get; set; } = null;

        public string Name { get; set; } = null;

        public float Price { get; set; }

        public virtual ICollection<ProductsCommand> ProductsCommands { get; set; } = new List<ProductsCommand>();
    }
}
