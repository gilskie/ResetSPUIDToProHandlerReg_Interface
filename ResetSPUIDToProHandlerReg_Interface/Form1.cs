using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;
using System.Text.RegularExpressions;
using System.Data.SqlClient;

namespace ResetSPUIDToProHandlerReg_Interface
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void Button1_Click(object sender, EventArgs e)
        {
            if (comboBox1.Text.Equals("ESPPV/EMBFRA/ESPCB - Reset SPUID/order")) {

                if (richTextBox1.Text.Equals(""))
                    MessageBox.Show("Empty spuid filed, please check!", "Error");

                else
                {
                    string databaseConnection;
                    databaseConnection = ReadConfigurationFile();

                    if (databaseConnection == null)
                        MessageBox.Show("Missing input text!");
                    else
                        DeleteFromDatabase(databaseConnection, richTextBox1.Text);
                }
            }
            else
                MessageBox.Show("No selection!");
        }

        private static void DeleteFromDatabase(string dbConnection, string values) {
            
            values = Regex.Replace(values, "\n", "','");
            values = values.ToString().TrimEnd('\r', '\n');

            int NormalOrdersCount = 0, articlesCount = 0, OrderStatusCount = 0;
            string foundQuerriesMessage = "";

            var appSettings = ConfigurationManager.AppSettings;

            //SqlDataReader reader = null;
            SqlConnection connection = new SqlConnection(dbConnection);
            connection.Open();

            try
            {
                if(connection.State == ConnectionState.Open && connection != null)
                {
                    SqlCommand cmd = new SqlCommand(@"delete from NormalOrders where OrgOrderID in('" + values + "')", connection);
                    NormalOrdersCount = cmd.ExecuteNonQuery();

                    cmd = new SqlCommand(@"delete from articles where orgorderid in('" + values + "')", connection);
                    articlesCount = cmd.ExecuteNonQuery();

                    cmd = new SqlCommand(@"delete from OrderStatus where OrderID in('" + values + "')", connection);
                    OrderStatusCount = cmd.ExecuteNonQuery();

                    if(NormalOrdersCount >= 1)
                        foundQuerriesMessage = "NormalOrders,";
                    if(articlesCount >= 1)
                        foundQuerriesMessage = foundQuerriesMessage + ", Articles";
                    if(OrderStatusCount >= 1)
                        foundQuerriesMessage = foundQuerriesMessage + " and OrderStatus";

                    MessageBox.Show("Successfully deleted the following tables: "+ foundQuerriesMessage.ToString());
                }
            }

            catch (Exception E)
            {
                MessageBox.Show("Error:" + E.Message);
            }

            finally
            {
                if (connection != null)
                    connection.Close();
            }
        }

        private static string ReadConfigurationFile() {
            try
            {
                var appsettings = ConfigurationManager.AppSettings;
                string connectionString = appsettings["ConnectionString"].ToString();
                return connectionString;
            }
            catch(ConfigurationErrorsException e) {
                MessageBox.Show("Error: " + e.Message, "Error Message Box");
            }
            return null;
        }
    }
}
