using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Project_Phone_Basket___Storage_App.Models
{
    public class ProductDTO
    {
        public int Id { get; set; }

        public Image Picture { get; set; }

        public string Barcode { get; set; } = null;

        public string Image { get; set; } = null;

        public string Name { get; set; } = null;

        public float Price { get; set; }

    }
}
