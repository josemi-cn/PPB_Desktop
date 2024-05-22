using Project_Phone_Basket___Storage_App.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Project_Phone_Basket___Storage_App.Models
{
    public class ProductsCommandsDTO
    {
        public int Id { get; set; }

        public Image Picture { get; set; } = null;

        public string Barcode { get; set; } = null;

        public string Image { get; set; } = null;

        public string Name { get; set; } = null;

        public float Price { get; set; }

        public int Quantity { get; set; }

        public float FinalPrice { get; set; }
    }
}
