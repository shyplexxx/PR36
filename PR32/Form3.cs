using MySql.Data.MySqlClient;
using Mysqlx.Crud;
using PR32.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlTypes;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace PR32
{
    public partial class Form3 : Form
    {
        public Form3()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string server = Settings.Default.host;
            string user = Settings.Default.uid;
            string password = Settings.Default.pwd;
            string db = Settings.Default.database;

            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "SQL файлы (*.sql)|*.sql";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                string connStr = $"server={server};user={user};password={password}";

                try
                {
                    string originalScript = File.ReadAllText(ofd.FileName);
                    string finalScript = "SET FOREIGN_KEY_CHECKS = 0;\n" +
                                         "SET SQL_SAFE_UPDATES = 0;\n" +
                                         "SET UNIQUE_CHECKS = 0;\n\n" +
                                         originalScript +
                                         "\n\nSET FOREIGN_KEY_CHECKS = 1;\n" +
                                         "SET SQL_SAFE_UPDATES = 1;\n" +
                                         "SET UNIQUE_CHECKS = 1;";

                    using (MySqlConnection conn = new MySqlConnection(connStr))
                    {
                        conn.Open();

                        MySqlScript mysqlScript = new MySqlScript(conn, finalScript);
                        mysqlScript.Execute();
                    }

                    MessageBox.Show("Структура и данные успешно восстановлены!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка восстановления:\n{ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem == null)
            {
                MessageBox.Show("Упс... Вы не выбрали таблицу", "Системные оповещения", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            
            string table = comboBox1.SelectedItem.ToString();
            int columns = 0;
            if (table == "user")
            {
                columns = 6;
            }
            if(table == "product")
            {
                columns = 12;
            }
            if(table == "deliverypoint")
            {
                columns = 1;
            }
            if(table == "order")
            {
                columns = 6;
            }
            string server = Settings.Default.host;
            string user = Settings.Default.uid;
            string password = Settings.Default.pwd;
            string db = Settings.Default.database;

            OpenFileDialog openFileDialog = new OpenFileDialog();
            if(openFileDialog.ShowDialog() == DialogResult.OK)
            {
                var lines = File.ReadAllLines(openFileDialog.FileName, System.Text.Encoding.GetEncoding(1251)).Skip(1);
                int count = 0;
                
                string conn = $"server={server};user={user};password={password};database={db}";
                using(MySqlConnection mySqlConnection = new MySqlConnection(conn))
                {
                    try
                    {
                        mySqlConnection.Open();
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Ипортируй базу, после добавляй", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }


                    foreach (string line in lines)
                    {

                        try
                        {
                            if (string.IsNullOrWhiteSpace(line)) continue;
                            string[] cells = line.TrimEnd(';').Split(';');

                            string sql;
                            if (table == "user")
                            {
                                if (cells.Length != 7)
                                {
                                    MessageBox.Show("Ошибка, кол-во полей в файле не совпадает с таблицей", "Ошибка");
                                    return;
                                }

                                sql = $"INSERT INTO user (UserID, UserSourname, UserName, UserPpatronymic, UserLogin, UserPassword, UserRole) VALUES ('{cells[0]}', '{cells[1]}', '{cells[2]}', '{cells[3]}', '{cells[4]}', '{cells[5]}', '{cells[6]}');";

                                using (MySqlCommand mySqlCommand = new MySqlCommand(sql, mySqlConnection))
                                {
                                    mySqlCommand.ExecuteNonQuery();
                                    count++;
                                }
                            }


                            if (table == "product")
                            {
                                if (cells.Length != 12)
                                {
                                    MessageBox.Show("Ошибка, кол-во полей в файле не совпадаетс с таблицей", "Ошибка");
                                    return;
                                }

                                sql = $"INSERT INTO product (ProductArticle, ProductName, ProductUnit, ProductCost, ProductMaxSale, ProductManufacture, ProductSypplier, ProductCategory, ProductNowSale, ProductCountWH, ProductDesk, ProductImage) VALUES ('{cells[0]}', '{cells[1]}', '{cells[2]}', '{cells[3]}', '{cells[4]}', '{cells[5]}', '{cells[6]}', '{cells[7]}', '{cells[8]}', '{cells[9]}', '{cells[10]}', '{cells[11]}' )";

                                using (MySqlCommand mySqlCommand = new MySqlCommand(sql, mySqlConnection))
                                {
                                    mySqlCommand.ExecuteNonQuery();
                                    count++;
                                }
                            }


                            if (table == "deliverypoint")
                            {
                                if (cells.Length != 1)
                                {
                                    MessageBox.Show("Ошибка, кол-во полей в файле не совпадаетс с таблицей", "Ошибка");
                                    return;
                                }

                                sql = $"INSERT INTO deliverypoint (DPName) VALUES('{cells[0]}');";
                                using (MySqlCommand cmd = new MySqlCommand(sql, mySqlConnection))
                                {
                                    cmd.ExecuteNonQuery();
                                    count++;
                                }
                            }

                            if (table == "order")
                            {
                                if (cells.Length != 7)
                                {
                                    MessageBox.Show("Ошибка, кол-во полей в файле не совпадаетс с таблицей", "Ошибка");
                                    return;
                                }

                                sql = $"INSERT INTO `order` (OrderID, OrderDateOrder, OrderDateDel, OrderDelivery, OrderClient, OrderCode, OrderStatus) VALUES ('{cells[0]}', '{cells[1]}', '{cells[2]}', '{cells[3]}', '{cells[4]}', '{cells[5]}', '{cells[6]}');";
                                using (MySqlCommand cmd = new MySqlCommand(sql, mySqlConnection))
                                {
                                    cmd.ExecuteNonQuery();
                                    count++;
                                }
                            }


                            if (table == "role")
                            {
                                if (cells.Length != 2)
                                {
                                    MessageBox.Show("Ошибка, кол-во полей в файле не совпадаетс с таблицей", "Ошибка");
                                    return;
                                }

                                sql = $"INSERT INTO `role` (RoleID, RoleName) VALUES ('{cells[0]}', '{cells[1]}' );";
                                using (MySqlCommand cmd = new MySqlCommand(sql, mySqlConnection))
                                {
                                    cmd.ExecuteNonQuery();
                                    count++;
                                }
                            }


                            if (table == "category")
                            {
                                if (cells.Length != 2)
                                {
                                    MessageBox.Show("Ошибка, кол-во полей в файле не совпадаетс с таблицей", "Ошибка");
                                    return;
                                }

                                sql = $"INSERT INTO `Category` (CategoryID, CategoryName) VALUES ('{cells[0]}', '{cells[1]}' );";
                                using (MySqlCommand cmd = new MySqlCommand(sql, mySqlConnection))
                                {
                                    cmd.ExecuteNonQuery();
                                    count++;
                                }
                            }


                            if (table == "status")
                            {
                                if (cells.Length != 2)
                                {
                                    MessageBox.Show("Ошибка, кол-во полей в файле не совпадаетс с таблицей", "Ошибка");
                                    return;
                                }

                                sql = $"INSERT INTO `status` (StatusID, StatusName) VALUES ('{cells[0]}', '{cells[1]}' );";
                                using (MySqlCommand cmd = new MySqlCommand(sql, mySqlConnection))
                                {
                                    cmd.ExecuteNonQuery();
                                    count++;
                                }
                               
                            }

                            if (table == "Sypplier")
                            {
                                if (cells.Length != 2)
                                {
                                    MessageBox.Show("Ошибка, кол-во полей в файле не совпадаетс с таблицей", "Ошибка");
                                    return;
                                }

                                sql = $"INSERT INTO `sypplier` (SypplierID, SypplierName) VALUES ('{cells[0]}', '{cells[1]}' );";
                                using (MySqlCommand cmd = new MySqlCommand(sql, mySqlConnection))
                                {
                                    cmd.ExecuteNonQuery();
                                    count++;
                                }
                               
                            }
                            if (table == "unit")
                            {
                                if (cells.Length != 2)
                                {
                                    MessageBox.Show("Ошибка, кол-во полей в файле не совпадаетс с таблицей", "Ошибка");
                                    return;
                                }

                                sql = $"INSERT INTO `unit` (UnitID, UnitName) VALUES ('{cells[0]}', '{cells[1]}' );";
                                using (MySqlCommand cmd = new MySqlCommand(sql, mySqlConnection))
                                {
                                    cmd.ExecuteNonQuery();
                                    count++;
                                }

                            }


                            if (table == "orderproduct")
                            {
                                if (cells.Length != 3)
                                {
                                    MessageBox.Show("Ошибка, кол-во полей в файле не совпадаетс с таблицей", "Ошибка");
                                    return;
                                }

                                sql = $"INSERT INTO `orderproduct` (OrderProductID, OrderProductCount) VALUES ('{cells[0]}', '{cells[1]}' );";
                                using (MySqlCommand cmd = new MySqlCommand(sql, mySqlConnection))
                                {
                                    cmd.ExecuteNonQuery();
                                    count++;
                                }

                            }

                        }
                        catch(Exception)
                        {
                            MessageBox.Show("Упс... Какая-то ошибка, разберись сам", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                    }
                    MessageBox.Show("Данные успешно импортированы", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);



                }
            }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }


        string connStr = $"server={Settings.Default.host};user={Settings.Default.uid};password={Settings.Default.pwd};database={Settings.Default.database}";
        private void button3_Click(object sender, EventArgs e)
        {
            string tableName = comboBox2.SelectedItem?.ToString();

            if (string.IsNullOrEmpty(tableName)) return;

            DataTable table = new DataTable();

            string sql = $"SELECT * FROM `{tableName}`;";

            try
            {
                using(MySqlConnection  mySqlConnection = new MySqlConnection(connStr))
                {
                    mySqlConnection.Open();

                    using (MySqlDataAdapter mySqlDataAdapter = new MySqlDataAdapter(sql, mySqlConnection))
                    {
                        mySqlDataAdapter.Fill(table);
                    }
                }

                if(table.Rows.Count == 0)
                {
                    MessageBox.Show($"Таблица пустая!");
                    return;
                }

                using (SaveFileDialog saveFileDialog = new SaveFileDialog())
                {
                    saveFileDialog.Filter = "CSV файлы (*.csv)|*.csv";
                    saveFileDialog.FileName = $"{tableName}_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
                    if(saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        ExprotToCsv(table, saveFileDialog.FileName);
                        MessageBox.Show("Успех");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка! {ex.Message}");
            }
        }

        private void ExprotToCsv(DataTable table, string tableName)
        {
            StringBuilder sb = new StringBuilder();
            string[] columsName = new string[table.Columns.Count];
            for(int i = 0; i < table.Columns.Count; i++)
            {
                columsName[i] = $"\"{table.Columns[i].ColumnName.Replace("\"", "\"\"")}\"";
            }

            sb.AppendLine(string.Join(";", columsName));

            foreach(DataRow row in table.Rows)
            {
                string[] Fields = new string[table.Columns.Count];
                for(int i = 0;i < table.Columns.Count; i++)
                {
                    Fields[i] = $"\"{row[i].ToString().Replace("\"", "\"\"")}\"";
                }
                sb.AppendLine(string.Join(";", Fields));
            }

            File.WriteAllText(tableName, sb.ToString(), Encoding.UTF8);
        }

        private void Form3_FormClosing(object sender, FormClosingEventArgs e)
        {
            string connStr = $"server={Settings.Default.host};user={Settings.Default.uid};password={Settings.Default.pwd};database={Settings.Default.database}";
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"dump_{DateTime.Now:yyyyMMdd_HHmmss}.sql");

            using (MySqlConnection conn = new MySqlConnection(connStr))
            using (MySqlCommand cmd = new MySqlCommand { Connection = conn })
            using (MySqlBackup m = new MySqlBackup(cmd))
            {
                conn.Open();
                m.ExportToFile(path);
            }
        }
    }
}
