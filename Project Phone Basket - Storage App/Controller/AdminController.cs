using Newtonsoft.Json;
using Project_Phone_Basket___Storage_App.Model;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Windows.Forms;
using Project_Phone_Basket___Storage_App.View;
using System.Collections.Generic;
using Project_Phone_Basket___Storage_App.Models;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Text;
using System.Net;
using System.Drawing;
using System.Media;

namespace Project_Phone_Basket___Storage_App.Controller
{
    public class AdminController
    {
        AdminForm f = new AdminForm();
        User loggedUser = null;
        string serverURL = "";
        string lastImageSetted = "";

        string lastPasswordSetted = "";

        bool firstLoad = true;

        string idText;

        public AdminController(User loggedUser, string serverURL, string enterprise_name)
        {
            this.loggedUser = loggedUser;
            this.serverURL = serverURL;

            idText = "{}";

            f.Text = enterprise_name;

            LoadData();
            InitListeners();
            f.ShowDialog();
        }

        void LoadData()
        {
            if (loggedUser != null)
            {
                // USER PERMISSIONS
                if (!getPermission(loggedUser.RoleId, 1) && !getPermission(loggedUser.RoleId, 2))
                {
                    f.tabs.TabPages.RemoveAt(3);
                }
                else 
                {
                    if (!getPermission(loggedUser.RoleId, 1))
                    {
                        f.users_Search.Enabled = false;
                        f.users_Refresh.Enabled = false;
                        f.users_DGV.Enabled = false;
                        f.users_Mod.Enabled = false;
                        f.users_Del.Enabled = false;
                    }

                    if (!getPermission(loggedUser.RoleId, 2))
                    {
                        f.users_Send.Enabled = false;
                        f.users_IdReset.Enabled = false;
                        f.users_Mod.Enabled = false;
                        f.users_Del.Enabled = false;
                        f.users_Editor.Enabled = false;
                    }
                }

                // ROLE PERMISSIONS
                if (!getPermission(loggedUser.RoleId, 3) && !getPermission(loggedUser.RoleId, 4))
                {
                    f.tabs.TabPages.RemoveAt(2);
                }
                else
                {
                    if (!getPermission(loggedUser.Id, 3))
                    {
                        f.roles_Search.Enabled = false;
                        f.roles_Refresh.Enabled = false;
                        f.roles_DGV.Enabled = false;
                        f.roles_Mod.Enabled = false;
                        f.roles_Del.Enabled = false;
                    }

                    if (!getPermission(loggedUser.Id, 4))
                    {
                        f.roles_Send.Enabled = false;
                        f.roles_IdReset.Enabled = false;
                        f.roles_Mod.Enabled = false;
                        f.roles_Del.Enabled = false;
                        f.roles_Editor.Enabled = false;
                    }
                }

                // PRODUCT PERMISSIONS
                if (!getPermission(loggedUser.RoleId, 5) && !getPermission(loggedUser.RoleId, 6))
                {
                    f.tabs.TabPages.RemoveAt(1);
                }
                else
                {
                    if (!getPermission(loggedUser.Id, 5))
                    {
                        f.product_Search.Enabled = false;
                        f.products_DGV.Enabled = false;
                        f.product_Mod.Enabled = false;
                        f.product_Del.Enabled = false;
                    }

                    if (!getPermission(loggedUser.Id, 6))
                    {
                        f.product_Send.Enabled = false;
                        f.product_IdReset.Enabled = false;
                        f.product_Mod.Enabled = false;
                        f.product_Del.Enabled = false;
                        f.product_Editor.Enabled = false;
                    }
                }

                // COMMAND PERMISSIONS
                if (!getPermission(loggedUser.RoleId, 7) && !getPermission(loggedUser.RoleId, 8))
                {
                    f.tabs.TabPages.RemoveAt(0);
                }
                else
                {
                    if (!getPermission(loggedUser.Id, 7))
                    {
                        f.productCommand_DGV.Enabled = false;
                        f.command_DGV.Enabled = false;
                    }
                }
            }

            if (getPermission(loggedUser.Id, 7))
            {
                f.productCommand_DGV.DataSource = new List<ProductsCommandsDTO>();
                f.productCommand_DGV.Columns["Id"].Visible = false;
                f.productCommand_DGV.Columns["Image"].Visible = false;

                f.productCommand_DGV.Columns["Barcode"].HeaderText = "Codi de barres";
                f.productCommand_DGV.Columns["Picture"].HeaderText = "Imatge";
                f.productCommand_DGV.Columns["Name"].HeaderText = "Nom";
                f.productCommand_DGV.Columns["Quantity"].HeaderText = "Quantitat";
                f.productCommand_DGV.Columns["FinalPrice"].HeaderText = "Preu";
            }

            f.welcome.Text = "Benvingut " + loggedUser.FirstName + " " + loggedUser.LastName + "!";

            LoadDatabases();
            LoadProducts();
            LoadRoles();
            LoadUsers();
        }

        void InitListeners()
        {
            f.command_DGV.SelectionChanged += commandSelected;
            f.command_DGV.CellClick += commandPressed;

            f.product_Mod.Click += productSelected;
            f.product_Send.Click += productSend;
            f.product_FileUpload.Click += productFileUploader;
            f.product_Refresh.Click += refreshProducts;
            f.product_IdReset.Click += productIdReset;
            f.product_Del.Click += deleteSelectedProduct;
            f.product_Search.TextChanged += searchProduct;

            f.roles_Mod.Click += roleSelected;
            f.roles_Send.Click += roleSend;
            f.roles_Del.Click += deleteSelectedRole;
            f.roles_Refresh.Click += refreshRoles;
            f.roles_IdReset.Click += roleIdReset;
            f.roles_Search.TextChanged += searchRoles;

            f.users_Mod.Click += userSelected;
            f.users_Send.Click += userSend;
            f.users_Refresh.Click += refreshUsers;
            f.users_IdReset.Click += userIdReset;
            f.users_Del.Click += deleteSelectedUser;
            f.users_Search.TextChanged += searchUsers;

            f.closeSession.Click += closeSession;
        }

        public async Task LoadDatabases()
        {
            while (true)
            {
                if (!getPermission(loggedUser.Id, 7))
                {
                    break;
                }

                updateCommandDGV();

                await Task.Delay(1000);
            }
        }

        /* --------------------------------------------------- Command functions --------------------------------------------------- */

        async void LoadCommands()
        {
            if (getPermission(loggedUser.RoleId, 7))
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(serverURL);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage response = await client.GetAsync("api/Commands/Filter");
                    if (response.IsSuccessStatusCode)
                    {
                        List<Command> command = JsonConvert.DeserializeObject<List<Command>>(response.Content.ReadAsStringAsync().Result);

                        if (command.Count != 0)
                        {
                            f.command_DGV.DataSource = command;
                        }
                        else
                        {
                            f.command_DGV.DataSource = new List<Command>();
                        }

                        if (f.command_DGV.Columns["Id"] != null)
                        {
                            f.command_DGV.Columns["Id"].Visible = false;
                            f.command_DGV.Columns["Date"].Visible = false;

                            f.command_DGV.Columns["Number"].HeaderText = "Numero";
                            f.command_DGV.Columns["Ready"].HeaderText = "Preparat?";
                            f.command_DGV.Columns["Delivered"].HeaderText = "Entregat?";
                        }
                    }
                    else
                    {
                        Console.WriteLine(response.Content.ReadAsStringAsync().Result);
                    }
                }
            }
        }

        async void updateCommandDGV() {
            if (getPermission(loggedUser.RoleId, 7))
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(serverURL);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage response = await client.GetAsync("api/Commands/Filter");
                    if (response.IsSuccessStatusCode)
                    {
                        string newIdText = response.Content.ReadAsStringAsync().Result;

                        if (!idText.Equals(newIdText))
                        {
                            idText = newIdText;
                            LoadCommands();

                            if (firstLoad == true)
                            {
                                firstLoad = false;
                            }
                            else
                            {
                                SoundPlayer simpleSound = new SoundPlayer(Directory.GetParent(Environment.CurrentDirectory).Parent.FullName + @"\Media\bip.wav");
                                Console.WriteLine(Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName + @"\Media\bip.wav");
                                simpleSound.Play();
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine(response.Content.ReadAsStringAsync().Result);
                    }
                }
            }
        }

        async void commandSelected(object sender, EventArgs e)
        {
            if (f.command_DGV.SelectedRows.Count > 0)
            {
                Command c = f.command_DGV.SelectedRows[0].DataBoundItem as Command;

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(serverURL);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage response = await client.GetAsync("api/ProductsCommands/Command/" + c.Id);
                    if (response.IsSuccessStatusCode)
                    {
                        List<ProductsCommandsDTO> productscommands = JsonConvert.DeserializeObject<List<ProductsCommand>>(response.Content.ReadAsStringAsync().Result).Select(a => new ProductsCommandsDTO
                        {
                            Id = a.ProductId,
                            Picture = getImage(getProduct(a.ProductId).Image),
                            Barcode = getProduct(a.ProductId).Barcode,
                            Image = getProduct(a.ProductId).Image,
                            Name = getProduct(a.ProductId).Name,
                            Price = getProduct(a.ProductId).Price,
                            Quantity = a.Quantity,
                            FinalPrice = getProduct(a.ProductId).Price * a.Quantity
                        }).OrderBy(a => a.Name).ToList();

                        f.productCommand_DGV.RowTemplate.Height = 100;

                        f.productCommand_DGV.DataSource = productscommands;

                        f.productCommand_DGV.Columns["Id"].Visible = false;
                        f.productCommand_DGV.Columns["Image"].Visible = false;
                        f.productCommand_DGV.Columns["Price"].Visible = false; 
                        
                        f.productCommand_DGV.Columns["Barcode"].HeaderText = "Codi de barres";
                        f.productCommand_DGV.Columns["Picture"].HeaderText = "Imatge";
                        f.productCommand_DGV.Columns["Name"].HeaderText = "Nom";
                        f.productCommand_DGV.Columns["Quantity"].HeaderText = "Quantitat";
                        f.productCommand_DGV.Columns["FinalPrice"].HeaderText = "Preu";

                        DataGridViewImageColumn imgCol = (DataGridViewImageColumn)f.productCommand_DGV.Columns["Picture"];
                        imgCol.ImageLayout = DataGridViewImageCellLayout.Zoom;
                    }
                    else
                    {
                        Console.WriteLine(response.Content.ReadAsStringAsync().Result);
                    }
                }
            }
            else
            {
                f.productCommand_DGV.DataSource = new List<ProductsCommandsDTO>();
                f.productCommand_DGV.Columns["Id"].Visible = false;
                f.productCommand_DGV.Columns["Image"].Visible = false;
                f.productCommand_DGV.Columns["Price"].Visible = false;

                f.productCommand_DGV.Columns["Barcode"].HeaderText = "Codi de barres";
                f.productCommand_DGV.Columns["Picture"].HeaderText = "Imatge";
                f.productCommand_DGV.Columns["Name"].HeaderText = "Nom";
                f.productCommand_DGV.Columns["Quantity"].HeaderText = "Quantitat";
                f.productCommand_DGV.Columns["FinalPrice"].HeaderText = "Preu";
            }
        }

        async void commandPressed(object sender, DataGridViewCellEventArgs e)
        {
            if (getPermission(loggedUser.Id, 8))
            {
                Command c = (Command)f.command_DGV.Rows[e.RowIndex].DataBoundItem;
                switch (e.ColumnIndex)
                {
                    case 3:

                        if (c.Ready == false && getPermission(loggedUser.Id, 7))
                        {
                            QuestionController q = new QuestionController("Estas segur de la comanda numero " + c.Number + " está preparada?");
                            if (q.getAnswer())
                            {
                                c.Ready = true;
                                updateCommand(c);
                            }
                        }
                        break;
                    case 4:
                        if (c.Ready == true && c.Delivered == false && getPermission(loggedUser.Id, 8))
                        {
                            QuestionController q = new QuestionController("Estas segur que la comanda numero " + c.Number + " s'ha entregat?");
                            if (q.getAnswer())
                            {
                                c.Delivered = true;
                                updateCommand(c);
                            }
                        }
                        break;
                }
            }
        }

        async void updateCommand(Command c)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(serverURL);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                using (var form = new MultipartFormDataContent())
                {
                    string json = JsonConvert.SerializeObject(c);

                    StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.PutAsync("api/Commands/" + c.Id, content);
                    if (response.IsSuccessStatusCode)
                    {
                        LoadCommands();
                    }
                    else
                    {
                        Console.WriteLine(response.Content.ReadAsStringAsync().Result);
                    }
                }
            }
        }

        /* --------------------------------------------------- Product functions --------------------------------------------------- */

        async void LoadProducts()
        {
            if (getPermission(loggedUser.Id, 5))
            {
                if (f.product_Search.Text.Length == 0)
                {
                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(serverURL);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                        HttpResponseMessage response = await client.GetAsync("api/Products");
                        if (response.IsSuccessStatusCode)
                        {
                            List<ProductDTO> product = JsonConvert.DeserializeObject<List<Product>>(response.Content.ReadAsStringAsync().Result).Select(a => new ProductDTO
                            {
                                Id = a.Id,
                                Picture = getImage(a.Image),
                                Barcode = a.Barcode,
                                Image = a.Image,
                                Name = a.Name,
                                Price = a.Price,
                            }).ToList();

                            if (product.Count != 0)
                            {
                                f.products_DGV.DataSource = product;
                            }
                            else
                            {
                                f.products_DGV.DataSource = new List<ProductDTO>();
                            }

                            f.products_DGV.RowTemplate.Height = 100;

                            if (f.products_DGV.Columns["Id"] != null)
                            {
                                f.products_DGV.Columns["Barcode"].HeaderText = "Codi de barres";
                                f.products_DGV.Columns["Image"].Visible = false;
                                f.products_DGV.Columns["Picture"].HeaderText = "Imatge";
                                f.products_DGV.Columns["Name"].HeaderText = "Nom";
                                f.products_DGV.Columns["Price"].HeaderText = "Preu";

                                DataGridViewImageColumn imgCol = (DataGridViewImageColumn)f.products_DGV.Columns["Picture"];
                                if (imgCol != null)
                                {
                                    imgCol.ImageLayout = DataGridViewImageCellLayout.Zoom;
                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine(response.Content.ReadAsStringAsync().Result);
                        }
                    }
                }
                else
                {
                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(serverURL);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                        HttpResponseMessage response = await client.GetAsync("api/Products/Search/" + f.product_Search.Text);
                        if (response.IsSuccessStatusCode)
                        {
                            List<ProductDTO> product = JsonConvert.DeserializeObject<List<Product>>(response.Content.ReadAsStringAsync().Result).Select(a => new ProductDTO
                            {
                                Id = a.Id,
                                Picture = getImage(a.Image),
                                Barcode = a.Barcode,
                                Image = a.Image,
                                Name = a.Name,
                                Price = a.Price,
                            }).ToList();

                            if (product.Count != 0)
                            {
                                f.products_DGV.DataSource = product;
                            }
                            else
                            {
                                f.products_DGV.DataSource = new List<ProductDTO>();
                            }

                            f.products_DGV.RowTemplate.Height = 100;

                            if (f.products_DGV.Columns["Id"] != null)
                            {
                                f.products_DGV.Columns["Barcode"].HeaderText = "Codi de barres";
                                f.products_DGV.Columns["Image"].Visible = false;

                                DataGridViewImageColumn imgCol = (DataGridViewImageColumn)f.products_DGV.Columns["Picture"];
                                if (imgCol != null)
                                {
                                    imgCol.ImageLayout = DataGridViewImageCellLayout.Zoom;
                                }

                                f.products_DGV.Columns["Name"].HeaderText = "Nom";
                                f.products_DGV.Columns["Price"].HeaderText = "Preu";
                            }
                        }
                        else
                        {
                            Console.WriteLine(response.Content.ReadAsStringAsync().Result);
                        }
                    }
                }
            }
        }

        Product getProduct(int productId)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(serverURL);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = client.GetAsync("api/Products/" + productId).Result;
                if (response.IsSuccessStatusCode)
                {
                    return JsonConvert.DeserializeObject<Product>(response.Content.ReadAsStringAsync().Result);
                }
                else
                {
                    Console.WriteLine(response.Content.ReadAsStringAsync().Result);
                }
            }

            return null;
        }

        void productSelected(object sender, EventArgs e)
        {
            ProductDTO p = f.products_DGV.SelectedRows[0].DataBoundItem as ProductDTO;

            f.product_Id.Text = p.Id.ToString();
            f.product_Barcode.Text = p.Barcode.ToString();
            f.product_Image.Text = p.Image.ToString();
            f.product_Name.Text = p.Name.ToString();
            f.product_Price.Text = p.Price.ToString();

            f.product_Send.Enabled = true;

            lastImageSetted = p.Image.ToString();
        }

        void productIdReset(object sender, EventArgs e)
        {
            f.product_Id.Text = "";
        }

        void refreshProducts(object sender, EventArgs e)
        {
            LoadProducts();
        }

        async void productSend(object sender, EventArgs e)
        {
            if (f.product_Barcode.Text.Length > 0)
            {
                if (f.product_Image.Text.Length > 0)
                {
                    if (f.product_Name.Text.Length > 0)
                    {
                        if (f.product_Price.Text.Length > 0)
                        {
                            Product p = new Product();

                            /*---------------------------------------------------------------------------------------------*/

                            string filePath = f.product_Image.Text;
                            string filename = "";

                            if (filePath != lastImageSetted)
                            {
                                using (var client = new HttpClient())
                                {
                                    client.BaseAddress = new Uri(serverURL);
                                    client.DefaultRequestHeaders.Accept.Clear();
                                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                                    using (var form = new MultipartFormDataContent())
                                    {
                                        using (var fileContent = new ByteArrayContent(File.ReadAllBytes(filePath)))
                                        {
                                            fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data");

                                            form.Add(fileContent, "file", Path.GetFileName(filePath));

                                            HttpResponseMessage response = await client.PostAsync("api/Images/upload", form);
                                            if (response.IsSuccessStatusCode)
                                            {
                                                ImageConfirmation ic = JsonConvert.DeserializeObject<ImageConfirmation>(response.Content.ReadAsStringAsync().Result);

                                                filename = ic.filename;
                                            }
                                            else
                                            {
                                                Console.WriteLine(response.Content.ReadAsStringAsync().Result);
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                filename = f.product_Image.Text;
                            }

                            /*---------------------------------------------------------------------------------------------*/

                            p.Barcode = f.product_Barcode.Text;
                            p.Image = filename;
                            p.Name = f.product_Name.Text;
                            p.Price = float.Parse(f.product_Price.Text);

                            if (f.product_Id.Text.Length == 0)
                            {
                                sendProduct(p);
                            }
                            else
                            {
                                p.Id = int.Parse(f.product_Id.Text);
                                editProduct(p);
                            }

                            f.product_Id.Text = "";
                            f.product_Barcode.Text = "";
                            f.product_Image.Text = "";
                            f.product_Name.Text = "";
                            f.product_Price.Text = "";

                            if (!getPermission(loggedUser.RoleId, 7))
                            {
                                f.product_Send.Enabled = false;
                            }

                            lastImageSetted = "";
                        }
                        else
                        {
                            new ErrorController("El camp \"Preu\" no pot estar buit.");
                        }
                    }
                    else
                    {
                        new ErrorController("El camp \"Nom\" no pot estar buit.");
                    }
                }
                else
                {
                    new ErrorController("El camp \"Imatge\" no pot estar buit.");
                }
            }
            else
            {
                new ErrorController("El camp \"Codi de barres\" no pot estar buit.");
            }
        }

        void deleteSelectedProduct(object sender, EventArgs e)
        {
            ProductDTO pdto = f.products_DGV.SelectedRows[0].DataBoundItem as ProductDTO;

            QuestionController q = new QuestionController("Estas segur de que vols eliminar \"" + pdto.Name + "\"?");
            if (q.getAnswer())
            {
                deleteProduct(pdto.Id);
                LoadProducts();
            }
        }

        async void sendProduct(Product product)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(serverURL);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                using (var form = new MultipartFormDataContent())
                {
                    string json = JsonConvert.SerializeObject(product);

                    StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.PostAsync("api/Products", content);
                    if (response.IsSuccessStatusCode)
                    {
                        LoadProducts();
                    }
                    else
                    {
                        Console.WriteLine(response.Content.ReadAsStringAsync().Result);
                    }
                }
            }
        }

        async void editProduct(Product product)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(serverURL);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                using (var form = new MultipartFormDataContent())
                {
                    string json = JsonConvert.SerializeObject(product);

                    StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.PutAsync("api/Products/" + product.Id, content);
                    if (response.IsSuccessStatusCode)
                    {
                        LoadProducts();
                    }
                    else
                    {
                        Console.WriteLine(response.Content.ReadAsStringAsync().Result);
                    }
                }
            }
        }

        async void deleteProduct(int product_id)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(serverURL);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                using (var form = new MultipartFormDataContent())
                {
                    HttpResponseMessage response = await client.DeleteAsync("api/Products/" + product_id);
                    if (response.IsSuccessStatusCode)
                    {
                        LoadProducts();
                    }
                    else
                    {
                        Console.WriteLine(response.Content.ReadAsStringAsync().Result);
                    }
                }
            }
        }

        void productFileUploader(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = false;
            openFileDialog.Filter = "Image Files(*.PNG;*.JPG;*.JPEG)|*.PNG;*.JPG;*.JPEG|All files (*.*)|*.*";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                f.product_Image.Text = openFileDialog.FileName;

                using (StreamReader reader = new StreamReader(openFileDialog.OpenFile()))
                {
                    var fileContent = reader.ReadToEnd();
                }
            }
        }

        async void searchProduct(object sender, EventArgs e)
        {
            LoadProducts();
        }

        /* --------------------------------------------------- Roles functions --------------------------------------------------- */

        async void LoadRoles()
        {
            if (getPermission(loggedUser.Id, 3))
            {
                if (f.roles_Search.Text.Length == 0)
                {
                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(serverURL);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                        HttpResponseMessage response = await client.GetAsync("api/Roles");
                        if (response.IsSuccessStatusCode)
                        {
                            List<Role> role = JsonConvert.DeserializeObject<List<Role>>(response.Content.ReadAsStringAsync().Result).ToList();

                            if (role.Count != 0)
                            {
                                f.roles_DGV.DataSource = role;
                                f.users_Rol.DataSource = role;
                                f.users_Rol.DisplayMember = "Name";
                            }
                            else
                            {
                                f.roles_DGV.DataSource = new List<Role>();
                            }

                            if (f.roles_DGV.Columns["Id"] != null)
                            {
                                f.roles_DGV.Columns["Name"].HeaderText = "Nom";
                            }
                        }
                        else
                        {
                            Console.WriteLine(response.Content.ReadAsStringAsync().Result);
                        }
                    }
                }
                else
                {
                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(serverURL);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                        HttpResponseMessage response = await client.GetAsync("api/Roles/Search/" + f.roles_Search.Text);
                        if (response.IsSuccessStatusCode)
                        {
                            List<Role> role = JsonConvert.DeserializeObject<List<Role>>(response.Content.ReadAsStringAsync().Result).ToList();

                            if (role.Count != 0)
                            {
                                f.roles_DGV.DataSource = role;
                                f.users_Rol.DataSource = role;
                                f.users_Rol.DisplayMember = "Name";
                            }
                            else
                            {
                                f.roles_DGV.DataSource = new List<Role>();
                            }

                            if (f.roles_DGV.Columns["Id"] != null)
                            {
                                f.roles_DGV.Columns["Name"].HeaderText = "Nom";
                            }
                        }
                        else
                        {
                            Console.WriteLine(response.Content.ReadAsStringAsync().Result);
                        }
                    }
                }
            }
        }

        Role getRole(int roleId)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(serverURL);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = client.GetAsync("api/Roles/" + roleId).Result;
                if (response.IsSuccessStatusCode)
                {
                    return JsonConvert.DeserializeObject<Role>(response.Content.ReadAsStringAsync().Result);
                }
                else
                {
                    Console.WriteLine(response.Content.ReadAsStringAsync().Result);
                }
            }

            return null;
        }
        
        void roleSelected(object sender, EventArgs e)
        {
            Role r = f.roles_DGV.SelectedRows[0].DataBoundItem as Role;

            f.roles_Id.Text = r.Id.ToString();
            f.roles_Name.Text = r.Name.ToString();

            f.roles_UserRead.Checked = getPermission(r.Id, 1);
            f.roles_UserWrite.Checked = getPermission(r.Id, 2);

            f.roles_RoleRead.Checked = getPermission(r.Id, 3);
            f.roles_RoleWrite.Checked = getPermission(r.Id, 4);

            f.roles_ProductRead.Checked = getPermission(r.Id, 5);
            f.roles_ProductWrite.Checked = getPermission(r.Id, 6);

            f.roles_CommandRead.Checked = getPermission(r.Id, 7);
            f.roles_CommandWrite.Checked = getPermission(r.Id, 8);

            f.roles_Send.Enabled = true;
        }

        async Task<Role> sendRole(Role role)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(serverURL);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                using (var form = new MultipartFormDataContent())
                {
                    string json = JsonConvert.SerializeObject(role);

                    StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.PostAsync("api/Roles", content);
                    if (response.IsSuccessStatusCode)
                    {
                        LoadRoles(); 
                        return JsonConvert.DeserializeObject<Role>(response.Content.ReadAsStringAsync().Result);
                    }
                    else
                    {
                        Console.WriteLine(response.Content.ReadAsStringAsync().Result);
                    }
                }
            }
            return null;
        }

        async void editRole(Role role)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(serverURL);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                using (var form = new MultipartFormDataContent())
                {
                    string json = JsonConvert.SerializeObject(role);

                    StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.PutAsync("api/Roles/" + role.Id, content);
                    if (response.IsSuccessStatusCode)
                    {
                        LoadRoles();
                    }
                    else
                    {
                        Console.WriteLine(response.Content.ReadAsStringAsync().Result);
                    }
                }
            }
        }

        async void roleSend(object sender, EventArgs e)
        {
            if (f.roles_Name.Text.Length > 0)
            {
                Role r = new Role();

                r.Name = f.roles_Name.Text;

                if (f.roles_Id.Text.Length == 0)
                {
                    r = await sendRole(r);

                    for (int i = 1; i <= 8; i++) 
                    {
                        setOrTakePermission(r.Id, i);
                    }

                    f.roles_Id.Text = "";
                    f.roles_Name.Text = "";
                    f.roles_UserRead.Checked = false;
                    f.roles_UserWrite.Checked = false;
                    f.roles_RoleRead.Checked = false;
                    f.roles_RoleWrite.Checked = false;
                    f.roles_ProductRead.Checked = false;
                    f.roles_ProductWrite.Checked = false;
                    f.roles_CommandRead.Checked = false;
                    f.roles_CommandWrite.Checked = false;

                    if (!getPermission(loggedUser.RoleId, 4))
                    {
                        f.roles_Send.Enabled = false;
                    }
                }
                else
                {
                    r.Id = int.Parse(f.roles_Id.Text);

                    editRole(r);

                    for (int i = 1; i <= 8; i++)
                    {
                        setOrTakePermission(r.Id, i);
                    }

                    f.roles_Id.Text = "";
                    f.roles_Name.Text = "";
                    f.roles_UserRead.Checked = false;
                    f.roles_UserWrite.Checked = false;
                    f.roles_RoleRead.Checked = false;
                    f.roles_RoleWrite.Checked = false;
                    f.roles_ProductRead.Checked = false;
                    f.roles_ProductWrite.Checked = false;
                    f.roles_CommandRead.Checked = false;
                    f.roles_CommandWrite.Checked = false;

                    if (!getPermission(loggedUser.RoleId, 4))
                    {
                        f.roles_Send.Enabled = false;
                    }
                }
            }
            else
            {
                new ErrorController("El camp \"Nom\" no pot estar buit.");
            }
        }

        async void setOrTakePermission(int role_id, int permission_id)
        {
            CheckBox cb = null;

            switch (permission_id) {
                case 1: // 'READ USERS'
                    cb = f.roles_UserRead;
                    break;
                case 2: // 'WRITE USERS'
                    cb = f.roles_UserWrite;
                    break;
                case 3: // 'READ ROLES'
                    cb = f.roles_RoleRead;
                    break;
                case 4: // 'WRITE ROLES'
                    cb = f.roles_RoleWrite;
                    break;
                case 5: // 'READ PRODUCTS'
                    cb = f.roles_ProductRead;
                    break;
                case 6: // 'WRITE PRODUCTS'
                    cb = f.roles_ProductWrite;
                    break;
                case 7: // 'READ COMMANDS'
                    cb = f.roles_CommandRead;
                    break;
                case 8: // 'WRITE COMMANDS'
                    cb = f.roles_CommandWrite;
                    break;
            }

            if (cb.Checked == true && !getPermission(role_id, permission_id)) 
            {
                PermissionsRole pr = new PermissionsRole();

                pr.RoleId = role_id;
                pr.PermissionId = permission_id;

                sendPermissionRole(pr);
            } 
            else if (cb.Checked == false && getPermission(role_id, permission_id))
            {
                deletePermissionRol(role_id, permission_id);
            }
        }

        async void takePermission(int role_id, int permission_id)
        {
            if (getPermission(role_id, permission_id))
            {
                deletePermissionRol(role_id, permission_id);
            }
        }

        void deleteSelectedRole(object sender, EventArgs e)
        {
            Role r = f.roles_DGV.SelectedRows[0].DataBoundItem as Role;

            if (r.Id != loggedUser.RoleId)
            {
                QuestionController q = new QuestionController("Estas segur de que vols eliminar \"" + r.Name + "\"?");
                if (q.getAnswer())
                {
                    for (int i = 1; i <= 10; i++)
                    {
                        takePermission(r.Id, i);
                    }

                    deleteRole(r.Id);
                }
            }
            else
            {
                new ErrorController("No pots eliminar el teu rol.");
            }
        }

        async void deleteRole(int role_id)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(serverURL);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                using (var form = new MultipartFormDataContent())
                {
                    HttpResponseMessage response = await client.DeleteAsync("api/Roles/" + role_id);
                    if (response.IsSuccessStatusCode)
                    {
                        LoadRoles();
                    }
                    else
                    {
                        Console.WriteLine(response.Content.ReadAsStringAsync().Result);
                    }
                }
            }
        }

        void roleIdReset(object sender, EventArgs e)
        {
            f.roles_Id.Text = "";
        }

        void refreshRoles(object sender, EventArgs e)
        {
            LoadRoles();
        }

        async void searchRoles(object sender, EventArgs e)
        {
            LoadRoles();
        }

        /* --------------------------------------------------- Users functions --------------------------------------------------- */

        async void LoadUsers()
        {
            if (getPermission(loggedUser.RoleId, 1))
            {
                if (f.users_Search.Text.Length == 0)
                {
                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(serverURL);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                        HttpResponseMessage response = await client.GetAsync("api/Users");
                        if (response.IsSuccessStatusCode)
                        {
                            List<UserDTO> user = JsonConvert.DeserializeObject<List<User>>(response.Content.ReadAsStringAsync().Result).Select(a => new UserDTO
                            {
                                Id = a.Id,
                                FirstName = a.FirstName,
                                LastName = a.LastName,
                                Name = a.FirstName + " " + a.LastName,
                                Username = a.Username,
                                Password = a.Password,
                                RoleId = a.RoleId,
                                Role = getRole(a.RoleId).Name
                            }).ToList();

                            if (user.Count != 0)
                            {
                                f.users_DGV.DataSource = user;
                            }
                            else
                            {
                                f.users_DGV.DataSource = new List<UserDTO>();
                            }

                            if (f.users_DGV.Columns["Id"] != null)
                            {
                                f.users_DGV.Columns["FirstName"].Visible = false;
                                f.users_DGV.Columns["LastName"].Visible = false;
                                f.users_DGV.Columns["Name"].HeaderText = "Nom complert";
                                f.users_DGV.Columns["Username"].HeaderText = "Nom d'usuari";
                                f.users_DGV.Columns["Password"].HeaderText = "Contraseña";
                                f.users_DGV.Columns["Password"].AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;
                                f.users_DGV.Columns["RoleId"].Visible = false;
                                f.users_DGV.Columns["Role"].HeaderText = "Rol";
                            }
                        }
                        else
                        {
                            Console.WriteLine(response.Content.ReadAsStringAsync().Result);
                        }
                    }
                }
                else
                {
                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(serverURL);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                        HttpResponseMessage response = await client.GetAsync("api/Users/Search/" + f.users_Search.Text);
                        if (response.IsSuccessStatusCode)
                        {
                            List<UserDTO> user = JsonConvert.DeserializeObject<List<User>>(response.Content.ReadAsStringAsync().Result).Select(a => new UserDTO
                            {
                                Id = a.Id,
                                FirstName = a.FirstName,
                                LastName = a.LastName,
                                Name = a.FirstName + " " + a.LastName,
                                Username = a.Username,
                                Password = a.Password,
                                RoleId = a.RoleId,
                                Role = getRole(a.RoleId).Name
                            }).ToList();

                            if (user.Count != 0)
                            {
                                f.users_DGV.DataSource = user;
                            }
                            else
                            {
                                f.users_DGV.DataSource = new List<UserDTO>();
                            }

                            if (f.users_DGV.Columns["Id"] != null)
                            {
                                f.users_DGV.Columns["FirstName"].Visible = false;
                                f.users_DGV.Columns["LastName"].Visible = false;
                                f.users_DGV.Columns["Name"].HeaderText = "Nom";
                                f.users_DGV.Columns["Username"].HeaderText = "Nom d'usuari";
                                f.users_DGV.Columns["Password"].HeaderText = "Contraseña";
                                f.users_DGV.Columns["Password"].AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;
                                f.users_DGV.Columns["RoleId"].Visible = false;
                                f.users_DGV.Columns["Role"].HeaderText = "Rol";
                            }
                        }
                        else
                        {
                            Console.WriteLine(response.Content.ReadAsStringAsync().Result);
                        }
                    }
                }
            }
        }

        void userSelected(object sender, EventArgs e)
        {
            UserDTO p = f.users_DGV.SelectedRows[0].DataBoundItem as UserDTO;

            f.users_Id.Text = p.Id.ToString();
            f.users_FirstName.Text = p.FirstName.ToString();
            f.users_LastName.Text = p.LastName.ToString();
            f.users_Username.Text = p.Username.ToString();
            f.users_Password.Text = "";
            f.users_ConfirmPass.Text = "";
            f.users_Rol.SelectedIndex = f.users_Rol.FindString(p.Role);

            f.users_Send.Enabled = true;

            lastPasswordSetted = p.Password;
        }

        async void userSend(object sender, EventArgs e)
        {
            if (f.users_FirstName.Text.Length > 0)
            {
                if (f.users_LastName.Text.Length > 0)
                {
                    if (f.users_Username.Text.Length > 0)
                    {
                        User u = new User();

                        u.FirstName = f.users_FirstName.Text;
                        u.LastName = f.users_LastName.Text;
                        u.Username = f.users_Username.Text;
                        u.RoleId = (f.users_Rol.SelectedItem as Role).Id;

                        if (f.users_Id.Text.Length == 0)
                        {
                            if (f.users_Password.Text.Length > 0) {
                                if (f.users_ConfirmPass.Text.Length > 0)
                                {
                                    if (f.users_Password.Text.Equals(f.users_ConfirmPass.Text))
                                    {
                                        string salt = BCrypt.Net.BCrypt.GenerateSalt();
                                        string hash = BCrypt.Net.BCrypt.HashPassword(f.users_Password.Text, salt);

                                        u.Password = hash;

                                        sendUser(u);

                                        f.users_Id.Text = "";
                                        f.users_FirstName.Text = "";
                                        f.users_LastName.Text = "";
                                        f.users_Username.Text = "";
                                        f.users_Password.Text = "";
                                        f.users_ConfirmPass.Text = "";
                                        f.users_Rol.SelectedIndex = 0;

                                        if (!getPermission(loggedUser.RoleId, 1))
                                        {
                                            f.users_Send.Enabled = false;
                                        }

                                        lastPasswordSetted = "";
                                    }
                                    else
                                    {
                                        new ErrorController("Les contrasenyes no coincideixen.");
                                    }
                                }
                                else
                                {
                                    new ErrorController("El camp \"Confirmar contrasenya\" no pot estar buit.");
                                }
                            }
                            else
                            {
                                new ErrorController("El camp \"Contrasenya\" no pot estar buit.");
                            }
                        }
                        else
                        {
                            u.Id = int.Parse(f.users_Id.Text);

                            if (f.users_Password.Text.Length > 0 && f.users_ConfirmPass.Text.Length > 0)
                            {
                                if (f.users_Password.Text.Equals(f.users_ConfirmPass.Text))
                                {
                                    string salt = BCrypt.Net.BCrypt.GenerateSalt();
                                    string hash = BCrypt.Net.BCrypt.HashPassword(f.users_Password.Text, salt);

                                    u.Password = hash;
                                }
                                else
                                {
                                    new ErrorController("Les contrasenyes no coincideixen.");
                                }
                            }
                            else
                            {
                                u.Password = lastPasswordSetted;
                            }

                            editUser(u);

                            f.users_Id.Text = "";
                            f.users_FirstName.Text = "";
                            f.users_LastName.Text = "";
                            f.users_Username.Text = "";
                            f.users_Password.Text = "";
                            f.users_ConfirmPass.Text = "";
                            f.users_Rol.SelectedIndex = 0;

                            if (!getPermission(loggedUser.RoleId, 1))
                            {
                                f.users_Send.Enabled = false;
                            }

                            lastPasswordSetted = "";
                        }
                    }
                    else
                    {
                        new ErrorController("El camp \"Nom d'usuari\" no pot estar buit.");
                    }
                }
                else
                {
                    new ErrorController("El camp \"Cognom\" no pot estar buit.");
                }
            }
            else
            {
                new ErrorController("El camp \"Nom\" no pot estar buit.");
            }
        }

        void deleteSelectedUser(object sender, EventArgs e)
        {
            UserDTO udto = f.users_DGV.SelectedRows[0].DataBoundItem as UserDTO;

            if (udto.Id != loggedUser.Id)
            {
                QuestionController q = new QuestionController("Estas segur de que vols eliminar \"" + udto.Username + "\"?");
                if (q.getAnswer())
                {
                    deleteUser(udto.Id);
                }
            } 
            else
            {
                new ErrorController("No pots eliminar el teu usuari.");
            }
        }

        async void sendUser(User user)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(serverURL);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                using (var form = new MultipartFormDataContent())
                {
                    string json = JsonConvert.SerializeObject(user);

                    StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.PostAsync("api/Users", content);
                    if (response.IsSuccessStatusCode)
                    {
                        LoadUsers();
                    }
                    else
                    {
                        Console.WriteLine(response.Content.ReadAsStringAsync().Result);
                    }
                }
            }
        }

        async void editUser(User user)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(serverURL);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                using (var form = new MultipartFormDataContent())
                {
                    string json = JsonConvert.SerializeObject(user);

                    StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.PutAsync("api/Users/" + user.Id, content);
                    if (response.IsSuccessStatusCode)
                    {
                        LoadUsers();
                    }
                    else
                    {
                        Console.WriteLine(response.Content.ReadAsStringAsync().Result);
                    }
                }
            }
        }

        async void deleteUser(int user_id)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(serverURL);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                using (var form = new MultipartFormDataContent())
                {
                    HttpResponseMessage response = await client.DeleteAsync("api/Users/" + user_id);
                    if (response.IsSuccessStatusCode)
                    {
                        LoadUsers();
                    }
                    else
                    {
                        Console.WriteLine(response.Content.ReadAsStringAsync().Result);
                    }
                }
            }
        }

        void userIdReset(object sender, EventArgs e)
        {
            f.users_Id.Text = "";
        }

        void refreshUsers(object sender, EventArgs e)
        {
            LoadUsers();
        }

        async void searchUsers(object sender, EventArgs e)
        {
            LoadUsers();
        }

        /* --------------------------------------------------- Permissions functions --------------------------------------------------- */

        Permission getPermission(int permissionId)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(serverURL);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = client.GetAsync("api/Permissions/" + permissionId).Result;
                if (response.IsSuccessStatusCode)
                {
                    return JsonConvert.DeserializeObject<Permission>(response.Content.ReadAsStringAsync().Result);
                }
                else
                {
                    Console.WriteLine(response.Content.ReadAsStringAsync().Result);
                }
            }

            return null;
        }

        bool getPermission(int roleId, int permissionId)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(serverURL);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = client.GetAsync("api/PermissionsRoles/Role/" + roleId + "/Permission/" + permissionId).Result;
                if (response.IsSuccessStatusCode)
                {
                    return JsonConvert.DeserializeObject<bool>(response.Content.ReadAsStringAsync().Result);
                }
                else
                {
                    return false;
                }
            }
        }

        /* --------------------------------------------------- Other functions --------------------------------------------------- */

        System.Drawing.Image getImage(string filename)
        {
            using (WebResponse wrFileResponse = WebRequest.Create(serverURL + "api/Images/" + filename).GetResponse())
            using (Stream objWebStream = wrFileResponse.GetResponseStream())
            {
                MemoryStream ms = new MemoryStream();
                objWebStream.CopyTo(ms, 8192);
                return System.Drawing.Image.FromStream(ms);
            }
        }

        async void sendPermissionRole(PermissionsRole permissionRole)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(serverURL);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                using (var form = new MultipartFormDataContent())
                {
                    string json = JsonConvert.SerializeObject(permissionRole);

                    StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.PostAsync("api/PermissionsRoles", content);
                    if (response.IsSuccessStatusCode)
                    {
                    }
                    else
                    {
                        Console.WriteLine("PermissionRole");
                        Console.WriteLine(response.Content.ReadAsStringAsync().Result);
                    }
                }
            }
        }

        async void deletePermissionRol(int role_id, int permission_id)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(serverURL);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                using (var form = new MultipartFormDataContent())
                {
                    HttpResponseMessage response = await client.DeleteAsync("api/PermissionsRoles/Role/" + role_id + "/Permission/" + permission_id);
                    if (response.IsSuccessStatusCode)
                    {
                    }
                    else
                    {
                        Console.WriteLine(response.Content.ReadAsStringAsync().Result);
                    }
                }
            }
        }

        void closeSession(object sender, EventArgs e)
        {
            f.Close();
        }
    }
}