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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace PR32
{
    public partial class ChekOrder : Form
    {
        public ChekOrder()
        {
            InitializeComponent();
        }


        string connStr = $"server={Settings.Default.host};user={Settings.Default.uid};password={Settings.Default.pwd};database={Settings.Default.database}";


        private void ChekOrder_Load(object sender, EventArgs e)
        {
            using (MySqlConnection  conn = new MySqlConnection(connStr))
            {
                conn.Open();
                string sql = @"SELECT DISTINCT
	                                orderproduct.OrderProductID,
                                    orderproduct.OrderProductAtricle,
                                    product.ProductName,
                                    product.ProductCost,
                                    orderproduct.OrderProductCount,
                                    `user`.UserSourname,
                                    `user`.UserName,
                                    `user`.UserPpatronymic,
                                    `order`.OrderDateOrder,
                                    status.StatusName
                               FROM orderproduct
	                                JOIN `order` ON orderproduct.OrderProductID = `order`.OrderID
                                    JOIN `user` ON `order`.OrderClient = `user`.UserID
	                                JOIN product ON orderproduct.OrderProductAtricle = product.ProductArticle
                                    JOIN status ON `order`.OrderStatus = status.StatusID
                ";

                MySqlDataAdapter mySqlDataAdapter = new MySqlDataAdapter(sql, conn);
                DataTable dt = new DataTable();
                mySqlDataAdapter.Fill(dt);

                foreach (DataRow dr in dt.Rows)
                {
                    string userSourname = dr["UserSourname"].ToString();
                    string userName = dr["UserName"].ToString();
                    string userPpatronymic = dr["UserPpatronymic"].ToString();



                    if (userSourname.Length > 4)
                    {
                        dr["UserSourname"] = userSourname.Substring(0,4) + new string('*', userSourname.Length - 4);
                    }

                    if(userName.Length > 3)
                    {
                        dr["UserName"] = userName.Substring(0,3) + new string('*', userName.Length - 3);
                    }

                    if(userPpatronymic.Length > 4)
                    {
                        dr["UserPpatronymic"] = userPpatronymic.Substring(0, 4) + new string('*', userPpatronymic.Length - 4);
                    }



                }

                dataGridView1.DataSource = dt;

                dataGridView1.Columns["OrderProductAtricle"].HeaderText = "Артикул";
                dataGridView1.Columns["ProductName"].HeaderText = "Наименование товара";
                dataGridView1.Columns["ProductCost"].HeaderText = "Стоимость товара";
                dataGridView1.Columns["OrderProductCount"].HeaderText = "Количество";
                dataGridView1.Columns["UserSourname"].HeaderText = "Фамилия";
                dataGridView1.Columns["UserName"].HeaderText = "Имя";
                dataGridView1.Columns["UserPpatronymic"].HeaderText = "Отчество";
                dataGridView1.Columns["OrderDateOrder"].HeaderText = "Дата заказа";
                dataGridView1.Columns["StatusName"].HeaderText = "Статус заказа";

                dataGridView1.Columns["OrderProductID"].Visible = false;
                
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MessageBox.Show($"{OrderData.ID}");
        }

        private void dataGridView1_Click(object sender, EventArgs e)
        {
            
            
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            var ss = dataGridView1.Rows[e.RowIndex].Cells["OrderProductID"].Value;
            OrderData.ID = ss.ToString();
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            var ss = dataGridView1.Rows[e.RowIndex].Cells["OrderProductID"].Value;
            OrderData.ID = ss.ToString();

            this.Hide();
            using (ProductEditForm productEditForm = new ProductEditForm())
            {
                productEditForm.ShowDialog();
            }
            this.Show();
            
            
        }
    }
}
