using Microsoft.Data.SqlClient;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace project1
{
    public partial class product : Form
    {
        SqlConnection con = new SqlConnection(
            @"Data Source=DESKTOP-4A2FR2S\SQLEXPRESS;
              Initial Catalog=PHARMACY STORE MS;
              Integrated Security=True;
              Encrypt=False;
              Trust Server Certificate=True");

        public product()
        {
            InitializeComponent();

            // IMPORTANT: attach CellFormatting event
            dataGridView1.CellFormatting += dataGridView1_CellFormatting;
        }

        // ================= DISPLAY PRODUCTS =================
        private void DisplayProducts()
        {
            try
            {
                if (con.State == ConnectionState.Closed)
                    con.Open();

                SqlDataAdapter da = new SqlDataAdapter(
                    "SELECT * FROM Products WHERE IsActive = 1", con);

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

        // ================= RESET FIELDS =================
        private void ResetFields()
        {
            productNametxt.Clear();
            CategoryComboBx.SelectedIndex = -1;
            Pricetxt.Clear();
            Quantitytxt.Clear();
            dtpExpiryDate.Value = DateTime.Today;
        }

        private void label3_Click(object sender, EventArgs e)
        {
            Customers c = new Customers();
            c.Show();
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
            Application.Exit();
        }


        private void product_Activated(object sender, EventArgs e)
        {
            DisplayProducts();
        }

        // ================= INSERT =================
        private void insertbtn_Click(object sender, EventArgs e)
        {
            if (productNametxt.Text == "" ||
                CategoryComboBx.Text == "" ||
                Pricetxt.Text == "" ||
                Quantitytxt.Text == "")
            {
                MessageBox.Show("Please fill all fields");
                return;
            }

            try
            {
                if (con.State == ConnectionState.Closed)
                    con.Open();

                SqlCommand cmd = new SqlCommand(
                    @"INSERT INTO Products
                      (ProductName, Category, Price, Quantity, ExpiryDate, IsActive)
                      VALUES (@name, @cat, @price, @qty, @exp, 1)", con);

                cmd.Parameters.AddWithValue("@name", productNametxt.Text);
                cmd.Parameters.AddWithValue("@cat", CategoryComboBx.Text);
                cmd.Parameters.AddWithValue("@price", decimal.Parse(Pricetxt.Text));
                cmd.Parameters.AddWithValue("@qty", int.Parse(Quantitytxt.Text));
                cmd.Parameters.AddWithValue("@exp", dtpExpiryDate.Value.Date);

                cmd.ExecuteNonQuery();

                MessageBox.Show("Product added successfully");

                DisplayProducts();
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

        // ================= UPDATE =================
        private void updatebtn_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Select a product to update");
                return;
            }

            int id = Convert.ToInt32(
                dataGridView1.SelectedRows[0].Cells["ProductId"].Value);

            try
            {
                if (con.State == ConnectionState.Closed)
                    con.Open();

                SqlCommand cmd = new SqlCommand(
                    @"UPDATE Products SET
                        ProductName = @name,
                        Category = @cat,
                        Price = @price,
                        Quantity = @qty,
                        ExpiryDate = @exp
                      WHERE ProductId = @id", con);

                cmd.Parameters.AddWithValue("@id", id);
                cmd.Parameters.AddWithValue("@name", productNametxt.Text);
                cmd.Parameters.AddWithValue("@cat", CategoryComboBx.Text);
                cmd.Parameters.AddWithValue("@price", decimal.Parse(Pricetxt.Text));
                cmd.Parameters.AddWithValue("@qty", int.Parse(Quantitytxt.Text));
                cmd.Parameters.AddWithValue("@exp", dtpExpiryDate.Value.Date);

                cmd.ExecuteNonQuery();

                MessageBox.Show("Product updated successfully");

                DisplayProducts();
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

        // ================= SOFT DELETE =================
        private void deletebtn_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Select a product to delete");
                return;
            }

            int id = Convert.ToInt32(
                dataGridView1.SelectedRows[0].Cells["ProductId"].Value);

            try
            {
                if (con.State == ConnectionState.Closed)
                    con.Open();

                SqlCommand cmd = new SqlCommand(
                    "UPDATE Products SET IsActive = 0 WHERE ProductId = @id", con);

                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();

                MessageBox.Show("Product deleted (soft delete)");

                DisplayProducts();
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

        // ================= DATAGRID DOUBLE CLICK =================
        private void dataGridView1_DoubleClick(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow == null) return;

            productNametxt.Text =
                dataGridView1.CurrentRow.Cells["ProductName"].Value?.ToString();

            CategoryComboBx.Text =
                dataGridView1.CurrentRow.Cells["Category"].Value?.ToString();

            Pricetxt.Text =
                dataGridView1.CurrentRow.Cells["Price"].Value?.ToString();

            Quantitytxt.Text =
                dataGridView1.CurrentRow.Cells["Quantity"].Value?.ToString();

            if (dataGridView1.CurrentRow.Cells["ExpiryDate"].Value != DBNull.Value)
            {
                dtpExpiryDate.Value =
                    Convert.ToDateTime(dataGridView1.CurrentRow.Cells["ExpiryDate"].Value);
            }
        }

        // ================= CLEAR =================
        private void clearbtn_Click(object sender, EventArgs e)
        {
            ResetFields();
        }

        // ================= FORM LOAD =================
        private void product_Load(object sender, EventArgs e)
        {
            DisplayProducts();

            Color headerColor = Color.FromArgb(44, 62, 80);

            dataGridView1.EnableHeadersVisualStyles = false;
            dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = headerColor;
            dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
        }

        // ================= EXPIRY COLOR LOGIC =================
        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dataGridView1.Columns[e.ColumnIndex].Name == "ExpiryDate" &&
                e.Value != null &&
                DateTime.TryParse(e.Value.ToString(), out DateTime expiryDate))
            {
                DateTime today = DateTime.Today;

                if (expiryDate < today)
                {
                    dataGridView1.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.LightCoral;
                }
                else if (expiryDate <= today.AddDays(30))
                {
                    dataGridView1.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.Khaki;
                }
                else
                {
                    dataGridView1.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.LightGreen;
                }
            }
        }
    }
}
