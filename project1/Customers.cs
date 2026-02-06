using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;

namespace project1
{
    public partial class Customers : Form
    {
        SqlConnection con = new SqlConnection(@"Data Source=DESKTOP-4A2FR2S\SQLEXPRESS;Initial Catalog=""PHARMACY STORE MS"";Integrated Security=True;Encrypt=False;Trust Server Certificate=True");
        public Customers()
        {
            InitializeComponent();
            DisplayCustomers();
        }
        private void DisplayCustomers()
        {
            try
            {
                if (con.State == ConnectionState.Open)
                    con.Close();

                con.Open();

                SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM Customer", con);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dataGridView1.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                    con.Close();
            }
        }


        private void ResetFields()
        {
            customertxt.Text = "";
            phonetxt.Text = "";
            emailtxt.Text = "";

        }
        private void label5_Click(object sender, EventArgs e)
        {
            login login = new login();
            login.Show();
            this.Hide();

        }

        private void cross_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void label2_Click(object sender, EventArgs e)
        {
            product product = new product();
            product.Show();
            this.Hide();
        }

        private void label3_Click(object sender, EventArgs e)
        {
            /// nothing
        }

        private void label4_Click(object sender, EventArgs e)
        {
            Sales Sales = new Sales();
            Sales.Show();
            this.Hide();
        }


        private void insertbtn_Click(object sender, EventArgs e)
        {
            try
            {
                con.Open();

                SqlCommand cmd = new SqlCommand(
                    "INSERT INTO Customer (Cname,Cphone, Cemail) VALUES (@Cname, @Cphone, @Cemail)",
                    con);

                cmd.Parameters.AddWithValue("@Cname", customertxt.Text);
                cmd.Parameters.AddWithValue("@Cphone", phonetxt.Text);
                cmd.Parameters.AddWithValue("@Cemail", emailtxt.Text);

                cmd.ExecuteNonQuery();

                MessageBox.Show("Customer added successfully");
                con.Close();

                DisplayCustomers();
                ResetFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                con.Close();
            }
        }

        private void clearbtn_Click(object sender, EventArgs e)
        {
            ResetFields();
        }

        private void updatebtn_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a customer to update");
                return;
            }

            int customerId = Convert.ToInt32(
                dataGridView1.SelectedRows[0].Cells["CustomerId"].Value);

            try
            {
                // ✅ SAFE OPEN
                if (con.State == ConnectionState.Open)
                    con.Close();

                con.Open();

                // 1️⃣ Update Customer table
                SqlCommand updateCustomer = new SqlCommand(
                    @"UPDATE Customer 
              SET Cname=@Cname, Cphone=@Cphone, Cemail=@Cemail 
              WHERE CustomerId=@CustomerId", con);

                updateCustomer.Parameters.AddWithValue("@CustomerId", customerId);
                updateCustomer.Parameters.AddWithValue("@Cname", customertxt.Text);
                updateCustomer.Parameters.AddWithValue("@Cphone", phonetxt.Text);
                updateCustomer.Parameters.AddWithValue("@Cemail", emailtxt.Text);
                updateCustomer.ExecuteNonQuery();

                // 2️⃣ Sync Sales table
                SqlCommand updateSales = new SqlCommand(
                    @"UPDATE Sales 
              SET CustomerName=@NewName 
              WHERE CustomerID=@CID", con);

                updateSales.Parameters.AddWithValue("@NewName", customertxt.Text);
                updateSales.Parameters.AddWithValue("@CID", customerId);
                updateSales.ExecuteNonQuery();

                MessageBox.Show("Customer updated successfully");
                DisplayCustomers();
                ResetFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                    con.Close();
            }
        }



        private void deletebtn_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a customer to delete");
                return;
            }

            int customerId = Convert.ToInt32(
                dataGridView1.SelectedRows[0].Cells["CustomerId"].Value);

            try
            {
                con.Open();

                // Check if customer has sales
                SqlCommand checkSales = new SqlCommand(
                    "SELECT COUNT(*) FROM Sales WHERE CustomerID=@CID", con);

                checkSales.Parameters.AddWithValue("@CID", customerId);

                int salesCount = (int)checkSales.ExecuteScalar();

                if (salesCount > 0)
                {
                    MessageBox.Show(
                        "This customer has sales records.\nDelete sales first.",
                        "Delete blocked",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                SqlCommand deleteCustomer = new SqlCommand(
                    "DELETE FROM Customer WHERE CustomerId=@CustomerId", con);

                deleteCustomer.Parameters.AddWithValue("@CustomerId", customerId);
                deleteCustomer.ExecuteNonQuery();

                MessageBox.Show("Customer deleted successfully");
                DisplayCustomers();
                ResetFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                con.Close();
            }
        }


        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            ///nothing
        }

        private void dataGridView1_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                if (dataGridView1.CurrentRow != null && dataGridView1.CurrentRow.Index != -1)
                {
                    customertxt.Text = dataGridView1.CurrentRow.Cells["Cname"].Value?.ToString();
                    phonetxt.Text = dataGridView1.CurrentRow.Cells["Cphone"].Value?.ToString();
                    emailtxt.Text = dataGridView1.CurrentRow.Cells["Cemail"].Value?.ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading customer data: " + ex.Message);
            }
        }


        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void Customers_Load(object sender, EventArgs e)
        {
            Color customColor = Color.FromArgb(44, 62, 80);

            dataGridView1.EnableHeadersVisualStyles = false;
            dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = customColor;
            dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);

            dataGridView1.DefaultCellStyle.SelectionBackColor = customColor;
            dataGridView1.DefaultCellStyle.SelectionForeColor = Color.White;

            dataGridView1.GridColor = Color.LightGray;

        }


    }
}
