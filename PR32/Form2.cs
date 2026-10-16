using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace PR32
{
    public partial class Form2 : Form
    {

        private int failedAttempts = 0;
        private string currentCaptchaText = "";
        private int blockTimeRemaining = 0;


        private string GetConnectionString()
        {
            string server = Properties.Settings.Default.host;
            string user = Properties.Settings.Default.uid;
            string pwd = Properties.Settings.Default.pwd;
            string db = Properties.Settings.Default.database;

            return $"server={server};user={user};password={pwd};database={db}";
        }





        public Form2()
        {
            InitializeComponent();
            pictureBoxCaptcha.Visible = false;
            textBoxCaptcha.Visible = false;

          
            loginBlockTimer.Interval = 1000;
            loginBlockTimer.Tick += LoginBlockTimer_Tick;

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                using (Form1 form1 = new Form1()) 
                {
                    this.Hide(); 


                    if (form1.ShowDialog() == DialogResult.OK)
                    {
                       
                        this.Close();
                        return;
                    }
                }
 
                this.Show();
            }
            catch (Exception ex)
            {

            }          
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            string server = Properties.Settings.Default.host;
            string user = Properties.Settings.Default.uid;
            string pwd = Properties.Settings.Default.pwd;
            string db = Properties.Settings.Default.database;

            string conn = $"server={server};user={user};password={pwd};database={db}";
            
            using (MySqlConnection c = new MySqlConnection(conn))
            {
                try
                {
                    c.Open();
                    MessageBox.Show("Успешное подключение к БД", "Успех!", MessageBoxButtons.OK, MessageBoxIcon.Information);

                }
                catch(Exception ex)
                {
                    MessageBox.Show("Ошибка! Не удалось подключиться к БД", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                    
            }
               
        }
        

        private void btnGuest_Click(object sender, EventArgs e)
        {

            SessionManager.IsGuest = true;
            SessionManager.CurrentUserFullName = null;

            this.Hide();
            Product mainForm = new Product();
            mainForm.ShowDialog();
            this.Close();
            ResetForm();


        }

       
        private async void btnLogin_Click(object sender, EventArgs e)
        {
            if (blockTimeRemaining > 0) return;

            string login = textBoxLogin.Text.Trim();
            string password = textBoxPassword.Text.Trim();


            if (failedAttempts > 0)
            {
                if (textBoxCaptcha.Text != currentCaptchaText)
                {
                    MessageBox.Show("Неверный код CAPTCHA!", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    BlockFormAfterFailedAttempt();
                    return;
                }
            }

            if (TryAuthenticate(login, password))
            {
                
                failedAttempts = 0;
                this.Hide();

                PO mainForm = new PO();
                mainForm.ShowDialog();

                this.Close(); 
                ResetForm();
            }
            else
            {
                failedAttempts++;
                MessageBox.Show("Неверный логин или пароль!", "Ошибка авторизации", MessageBoxButtons.OK, MessageBoxIcon.Error);

                ShowCaptcha();
                
                
                if (failedAttempts > 1)
                {
                    BlockFormAfterFailedAttempt();
                }
            }

        }

        private bool TryAuthenticate(string login, string password)
        {
            
            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                try
                {
                    conn.Open();
                    
                    string query = "SELECT CONCAT(UserSourname, ' ', UserName, ' ', UserPpatronymic) AS FullName " +
                                   "FROM user WHERE UserLogin = @login AND UserPassword = @password";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@login", login);
                        cmd.Parameters.AddWithValue("@password", password);

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                SessionManager.CurrentUserFullName = reader["FullName"].ToString();
                                SessionManager.IsGuest = false;
                                return true;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка соединения с базой данных: {ex.Message}", "Критическая ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            return false;
        }

        private void ShowCaptcha()
        {
            currentCaptchaText = CaptchaHelper.GenerateText();
            pictureBoxCaptcha.Image = CaptchaHelper.GenerateImage(currentCaptchaText, pictureBoxCaptcha.Width, pictureBoxCaptcha.Height);
            pictureBoxCaptcha.Visible = true;
            textBoxCaptcha.Visible = true;
            textBoxCaptcha.Text = "";
        }


        private void BlockFormAfterFailedAttempt()
        {
            blockTimeRemaining = 10;
            btnLogin.Enabled = false;
            btnGuest.Enabled = false;
            loginBlockTimer.Start();
            MessageBox.Show("Система временно заблокирована. Подождите 10 секунд.", "Блокировка", MessageBoxButtons.OK, MessageBoxIcon.Stop);
        }

        private void LoginBlockTimer_Tick(object sender, EventArgs e)
        {
            blockTimeRemaining--;
            if (blockTimeRemaining <= 0)
            {
                loginBlockTimer.Stop();
                btnLogin.Enabled = true;
                btnGuest.Enabled = true;
                ShowCaptcha(); 
            }
        }


        private void ResetForm()
        {
            textBoxLogin.Text = "";
            textBoxPassword.Text = "";
            textBoxCaptcha.Text = "";
            pictureBoxCaptcha.Visible = false;
            textBoxCaptcha.Visible = false;
            failedAttempts = 0;
        }

        private void label5_Click(object sender, EventArgs e)
        {

        }
    }
}
