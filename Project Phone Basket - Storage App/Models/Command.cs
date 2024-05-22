using System;
using System.Collections.Generic;

namespace Project_Phone_Basket___Storage_App.Model
{
    public class Command
    {
        public int Id { get; set; }

        public int Number { get; set; }

        public DateTime Date { get; set; }

        public bool Ready { get; set; }

        public bool Delivered { get; set; }
    }
}
