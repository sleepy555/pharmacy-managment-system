using Microsoft.Data.SqlClient;
using Microsoft.VisualBasic.Logging;
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
                con.Open();

                SqlCommand cmd = new SqlCommand(
                    @"INSERT INTO Products 
                      (ProductName, Category, Price, Quantity, IsActive)
                      VALUES (@name, @cat, @price, @qty, 1)", con);

                cmd.Parameters.AddWithValue("@name", productNametxt.Text);
                cmd.Parameters.AddWithValue("@cat", CategoryComboBx.Text);
                cmd.Parameters.AddWithValue("@price", decimal.Parse(Pricetxt.Text));
                cmd.Parameters.AddWithValue("@qty", int.Parse(Quantitytxt.Text));

                cmd.ExecuteNonQuery();

                MessageBox.Show("Product Added Successfully");

                DisplayProducts();
                ResetFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
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
                MessageBox.Show("Select product to update");
                return;
            }

            int id = Convert.ToInt32(
                dataGridView1.SelectedRows[0].Cells["ProductId"].Value);

            try
            {
                con.Open();

                SqlCommand cmd = new SqlCommand(
                    @"UPDATE Products 
                      SET ProductName=@name,
                          Category=@cat,
                          Price=@price,
                          Quantity=@qty
                      WHERE ProductId=@id", con);

                cmd.Parameters.AddWithValue("@id", id);
                cmd.Parameters.AddWithValue("@name", productNametxt.Text);
                cmd.Parameters.AddWithValue("@cat", CategoryComboBx.Text);
                cmd.Parameters.AddWithValue("@price", decimal.Parse(Pricetxt.Text));
                cmd.Parameters.AddWithValue("@qty", int.Parse(Quantitytxt.Text));

                cmd.ExecuteNonQuery();

                MessageBox.Show("Product Updated Successfully");

                DisplayProducts();
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

        // ================= DELETE (SOFT DELETE) =================
        private void deletebtn_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Select product to delete");
                return;
            }

            int id = Convert.ToInt32(
                dataGridView1.SelectedRows[0].Cells["ProductId"].Value);

            try
            {
                con.Open();

                SqlCommand cmd = new SqlCommand(
                    "UPDATE Products SET IsActive = 0 WHERE ProductId=@id", con);

                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();

                MessageBox.Show("Product Deleted Successfully");

                DisplayProducts();
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

        // ================= DATAGRID DOUBLE CLICK =================
        private void dataGridView1_DoubleClick(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow != null)
            {
                productNametxt.Text =
                    dataGridView1.CurrentRow.Cells["ProductName"].Value?.ToString() ?? "";

                CategoryComboBx.Text =
                    dataGridView1.CurrentRow.Cells["Category"].Value?.ToString() ?? "";

                Pricetxt.Text =
                    dataGridView1.CurrentRow.Cells["Price"].Value?.ToString() ?? "";

                Quantitytxt.Text =
                    dataGridView1.CurrentRow.Cells["Quantity"].Value?.ToString() ?? "";
            }
        }

        // ================= CLEAR BUTTON =================
        private void clearbtn_Click(object sender, EventArgs e)
        {
            ResetFields();
        }

        // ================= FORM LOAD =================
        private void product_Load(object sender, EventArgs e)
        {
            DisplayProducts();

            Color customColor = Color.FromArgb(44, 62, 80);

            dataGridView1.EnableHeadersVisualStyles = false;
            dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = customColor;
            dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dataGridView1.DefaultCellStyle.SelectionBackColor = customColor;
            dataGridView1.DefaultCellStyle.SelectionForeColor = Color.White;
        }

        // ====== EMPTY EVENTS TO AVOID DESIGNER ERRORS ======
        private void product_DoubleClick(object sender, EventArgs e) { }
        private void label9_Click(object sender, EventArgs e) { }
        private void productNametxt_TextChanged(object sender, EventArgs e) { }

        // ================= SIDE MENU =================
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

       

    }
}
