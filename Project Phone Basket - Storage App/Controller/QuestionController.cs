using Newtonsoft.Json;
using Project_Phone_Basket___Storage_App.Model;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using BCrypt.Net;
using System.Windows.Forms;
using Project_Phone_Basket___Storage_App.View;
using System.Configuration;
using System.Collections.Generic;
using Project_Phone_Basket___Storage_App.Models;
using System.Drawing;
using System.Reflection;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Text;
using System.Runtime.InteropServices.ComTypes;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;
using System.Security.Cryptography;
using System.Runtime.Remoting.Contexts;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;
using System.Net;

namespace Project_Phone_Basket___Storage_App.Controller
{
    public class QuestionController
    {
        QuestionForm f = new QuestionForm();
        string message = "";
        bool answer = false;

        public QuestionController(string message)
        {
            this.message = message;

            LoadData();
            InitListeners();
            f.ShowDialog();
        }

        void LoadData()
        {
            f.message.Text = message;
        }

        void InitListeners()
        {
            f.yes.Click += affirmative;
            f.no.Click += closeWindow;
        }

        public bool getAnswer() {
            return answer;
        }

        void affirmative(object sender, EventArgs e)
        {
            answer = true;
            f.Close();
        }

        void closeWindow(object sender, EventArgs e)
        {
            f.Close();
        }
    }
}