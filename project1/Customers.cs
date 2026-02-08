using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;

namespace project1
{
    public partial class Customers : Form
    {
        SqlConnection con = new SqlConnection(
            @"Data Source=DESKTOP-4A2FR2S\SQLEXPRESS;
              Initial Catalog=""PHARMACY STORE MS"";
              Integrated Security=True;
              Encrypt=False;
              Trust Server Certificate=True");

        public Customers()
        {
            InitializeComponent();
            DisplayCustomers();
        }

        // ================= DISPLAY CUSTOMERS =================
        private void DisplayCustomers()
        {
            try
            {
                if (con.State == ConnectionState.Open)
                    con.Close();

                con.Open();

                SqlDataAdapter da = new SqlDataAdapter(
                    "SELECT * FROM Customer", con);

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

        // ================= RESET =================
        private void ResetFields()
        {
            customertxt.Clear();
            phonetxt.Clear();
            emailtxt.Clear();
        }

        // ================= GMAIL VALIDATION =================
        private bool IsValidGmail(string email)
        {
            return email.EndsWith("@gmail.com", StringComparison.OrdinalIgnoreCase);
        }

        // ================= INSERT =================
        private void insertbtn_Click(object sender, EventArgs e)
        {
            if (customertxt.Text == "" || phonetxt.Text == "" || emailtxt.Text == "")
            {
                MessageBox.Show(
                    "Please fill all fields",
                    "Missing Data",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            if (!IsValidGmail(emailtxt.Text))
            {
                MessageBox.Show(
                    "Invalid Email Address!\n\nEmail must end with @gmail.com",
                    "Invalid Email",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                emailtxt.Focus();
                return;
            }

            try
            {
                con.Open();

                SqlCommand cmd = new SqlCommand(
                    "INSERT INTO Customer (Cname, Cphone, Cemail) VALUES (@Cname, @Cphone, @Cemail)",
                    con);

                cmd.Parameters.AddWithValue("@Cname", customertxt.Text);
                cmd.Parameters.AddWithValue("@Cphone", phonetxt.Text);
                cmd.Parameters.AddWithValue("@Cemail", emailtxt.Text);

                cmd.ExecuteNonQuery();

                MessageBox.Show(
                    "Customer added successfully",
                    "Success",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

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

        // ================= UPDATE =================
        private void updatebtn_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show(
                    "Please select a customer to update",
                    "No Selection",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            if (!IsValidGmail(emailtxt.Text))
            {
                MessageBox.Show(
                    "Invalid Email Address!\n\nEmail must end with @gmail.com",
                    "Invalid Email",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                emailtxt.Focus();
                return;
            }

            int customerId = Convert.ToInt32(
                dataGridView1.SelectedRows[0].Cells["CustomerId"].Value);

            try
            {
                if (con.State == ConnectionState.Open)
                    con.Close();

                con.Open();

                // Update Customer
                SqlCommand updateCustomer = new SqlCommand(
                    @"UPDATE Customer 
                      SET Cname=@Cname, Cphone=@Cphone, Cemail=@Cemail 
                      WHERE CustomerId=@CustomerId", con);

                updateCustomer.Parameters.AddWithValue("@CustomerId", customerId);
                updateCustomer.Parameters.AddWithValue("@Cname", customertxt.Text);
                updateCustomer.Parameters.AddWithValue("@Cphone", phonetxt.Text);
                updateCustomer.Parameters.AddWithValue("@Cemail", emailtxt.Text);
                updateCustomer.ExecuteNonQuery();

                // Sync Sales table
                SqlCommand updateSales = new SqlCommand(
                    @"UPDATE Sales 
                      SET CustomerName=@NewName 
                      WHERE CustomerID=@CID", con);

                updateSales.Parameters.AddWithValue("@NewName", customertxt.Text);
                updateSales.Parameters.AddWithValue("@CID", customerId);
                updateSales.ExecuteNonQuery();

                MessageBox.Show(
                    "Customer updated successfully",
                    "Success",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

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

        // ================= DELETE =================
        private void deletebtn_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show(
                    "Please select a customer to delete",
                    "No Selection",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            int customerId = Convert.ToInt32(
                dataGridView1.SelectedRows[0].Cells["CustomerId"].Value);

            try
            {
                con.Open();

                SqlCommand checkSales = new SqlCommand(
                    "SELECT COUNT(*) FROM Sales WHERE CustomerID=@CID", con);
                checkSales.Parameters.AddWithValue("@CID", customerId);

                int salesCount = (int)checkSales.ExecuteScalar();

                if (salesCount > 0)
                {
                    MessageBox.Show(
                        "This customer has sales records.\nDelete sales first.",
                        "Delete Blocked",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                SqlCommand deleteCustomer = new SqlCommand(
                    "DELETE FROM Customer WHERE CustomerId=@CustomerId", con);
                deleteCustomer.Parameters.AddWithValue("@CustomerId", customerId);
                deleteCustomer.ExecuteNonQuery();

                MessageBox.Show(
                    "Customer deleted successfully",
                    "Success",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

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

        // ================= GRID DOUBLE CLICK =================
        private void dataGridView1_DoubleClick(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow != null)
            {
                customertxt.Text = dataGridView1.CurrentRow.Cells["Cname"].Value?.ToString();
                phonetxt.Text = dataGridView1.CurrentRow.Cells["Cphone"].Value?.ToString();
                emailtxt.Text = dataGridView1.CurrentRow.Cells["Cemail"].Value?.ToString();
            }
        }

        // ================= CLEAR =================
        private void clearbtn_Click(object sender, EventArgs e)
        {
            ResetFields();
        }

        // ================= FORM LOAD =================
        private void Customers_Load(object sender, EventArgs e)
        {
            Color customColor = Color.FromArgb(44, 62, 80);

            dataGridView1.EnableHeadersVisualStyles = false;
            dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = customColor;
            dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dataGridView1.ColumnHeadersDefaultCellStyle.Font =
                new Font("Segoe UI", 9, FontStyle.Bold);

            dataGridView1.DefaultCellStyle.SelectionBackColor = customColor;
            dataGridView1.DefaultCellStyle.SelectionForeColor = Color.White;
            dataGridView1.GridColor = Color.LightGray;
        }

        // ================= NAVIGATION =================
        private void label2_Click(object sender, EventArgs e)
        {
            product p = new product();
            p.Show();
            this.Hide();
        }

        private void label4_Click(object sender, EventArgs e)
        {
            Sales s = new Sales();
            s.Show();
            this.Hide();
        }

        private void label5_Click(object sender, EventArgs e)
        {
            login l = new login();
            l.Show();
            this.Hide();
        }

        private void cross_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
