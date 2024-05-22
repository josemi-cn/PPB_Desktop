using Project_Phone_Basket___Storage_App.Controller;
using Project_Phone_Basket___Storage_App.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Project_Phone_Basket___Storage_App
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //User u = new User();
            //new AdminController(u, "https://localhost:7152/");
            new LoginController();
        }
    }
}
