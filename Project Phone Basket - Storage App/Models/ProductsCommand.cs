using System;
using System.Collections.Generic;

namespace Project_Phone_Basket___Storage_App.Model
{
    public partial class ProductsCommand
    {
        public int Id { get; set; }

        public int CommandId { get; set; }

        public int ProductId { get; set; }

        public int Quantity { get; set; }
    }
}
