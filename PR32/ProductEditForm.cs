using PR32.Properties;
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
    public partial class ProductEditForm : Form
    {
        public ProductEditForm()
        {
            InitializeComponent();
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void ProductEditForm_Load(object sender, EventArgs e)
        {
            string ID = OrderData.ID;
            string connStr = $"server={Settings.Default.host};user={Settings.Default.uid};password={Settings.Default.pwd};database={Settings.Default.database}";

            string sql = $@"SELECT 
	                                orderproduct.OrderProductID,
                                    orderproduct.OrderProductAtricle,
                                    product.ProductName,
                                    product.ProductCost,
                                    orderproduct.OrderProductCount,
                                    `user`.UserSourname,
                                    `user`.UserName,
                                    `user`.UserPpatronymic,
                                    `order`.OrderDateOrder,  
                                    `order`.OrderDateDel, 
                                    deliverypoint.DPName,
                                    status.StatusName
                               FROM orderproduct
	                                JOIN `order` ON orderproduct.OrderProductID = `order`.OrderID
                                    JOIN `user` ON `order`.OrderClient = `user`.UserID
	                                JOIN product ON orderproduct.OrderProductAtricle = product.ProductArticle
                                    JOIN status ON `order`.OrderStatus = status.StatusID
                                    JOIN deliverypoint ON `order`.OrderDelivery = deliverypoint.DPID
                                WHERE OrderProductID = '{ID}';
            ";


            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                conn.Open();

                using(MySqlCommand cmd = new MySqlCommand(sql, conn))
                {

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if(reader.Read())
                        {
                            textBox1.Text = reader["OrderProductAtricle"].ToString();
                            textBox5.Text = reader["ProductName"].ToString();
                            textBox3.Text = reader["ProductCost"].ToString();
                            textBox6.Text = reader["OrderProductCount"].ToString();

                            textBox2.Text = reader["UserSourname"].ToString();
                            textBox4.Text = reader["UserName"].ToString();
                            textBox7.Text = reader["UserPpatronymic"].ToString();

                            textBox10.Text = reader["OrderDateOrder"].ToString();
                            textBox12.Text = reader["OrderDateDel"].ToString();
                            textBox9.Text = reader["DPName"].ToString();
                            textBox11.Text = reader["StatusName"].ToString();

                            int a = Convert.ToInt32(textBox3.Text);
                            int b = Convert.ToInt32(textBox6.Text);

                            int result = a * b;

                            textBox8.Text = result.ToString();
                        }
                        

                    }
                        
                }


            }



        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }
    }
}
