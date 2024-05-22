using Newtonsoft.Json;
using Project_Phone_Basket___Storage_App.Model;
using Project_Phone_Basket___Storage_App.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Windows.Forms;

namespace Project_Phone_Basket___Storage_App.Controller
{
    public class LoginController
    {
        LoginForm f = new LoginForm();

        public LoginController()
        {
            InitListeners();
            LoadData();
            Application.Run(f);
        }

        public LoginController(bool s) // Only to separate constructors
        {
            InitListeners();
            LoadData();
            f.ShowDialog();
        }

        void LoadData()
        {

        }

        void InitListeners()
        {
            f.login.Click += Login;
        }

        async void Login(object sender, EventArgs e)
        {
            f.loadGIF.Visible = true;
            using (var login_client = new HttpClient())
            {
                login_client.BaseAddress = new Uri(f.server.Text);
                login_client.DefaultRequestHeaders.Accept.Clear();
                login_client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage login_response = await login_client.GetAsync("api/Users/Name/" + f.username.Text);
                if (login_response.IsSuccessStatusCode)
                {
                    User user = JsonConvert.DeserializeObject<User>(login_response.Content.ReadAsStringAsync().Result);

                    if (user != null) {
                        string salt = user.Password.PadLeft(28);
                        string hash = BCrypt.Net.BCrypt.HashPassword(f.password.Text, salt);

                        if (user.Password.Equals(hash))
                        {
                            using (var verification_client = new HttpClient())
                            {
                                verification_client.BaseAddress = new Uri(f.server.Text);
                                verification_client.DefaultRequestHeaders.Accept.Clear();
                                verification_client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                                HttpResponseMessage verification_response = await verification_client.GetAsync("api/verifyIdentity");
                                if (verification_response.IsSuccessStatusCode)
                                {
                                    Verification verification = JsonConvert.DeserializeObject<Verification>(verification_response.Content.ReadAsStringAsync().Result);

                                    f.Visible = false;
                                    new AdminController(user, f.server.Text, verification.name);
                                    f.username.Text = "";
                                    f.password.Text = "";
                                    f.Visible = true;
                                }
                            }
                        }
                        else
                        {
                            f.loadGIF.Visible = false;
                            new ErrorController("El nom d'usuari o la contrasenya no es correcte.");
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Internal server Error");
                }
            }
            f.loadGIF.Visible = false;
        }
    }
}
