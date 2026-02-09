using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Text;
using System.Windows.Forms;

namespace project1
{
    public partial class Sales : Form
    {

        SqlConnection con = new SqlConnection(@"Data Source=DESKTOP-4A2FR2S\SQLEXPRESS;Initial Catalog=""PHARMACY STORE MS"";Integrated Security=True;Encrypt=False;Trust Server Certificate=True");
        public Sales()
        {
            InitializeComponent();
            DisplaySales();
        }

        private void DisplaySales()
        {
            try
            {
                SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM Sales", con);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dataGridView1.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        private void ResetFields()
        {
            CustIdComboBx.Text = "";
            CustomerNametxt.Text = "";
            ProductIdComboBox.Text = "";
            ProductNametxt.Text = "";
            Quantitytxt.Text = "";
            Totaltxt.Text = "";
            dateTimePicker1.Text = "";

        }

        private void CustIdComboBx_SelectedIndexChanged(object sender, EventArgs e)
        {
            // 🔒 IMPORTANT GUARD
            if (CustIdComboBx.SelectedItem == null)
            {
                CustomerNametxt.Clear();
                return;
            }

            try
            {
                con.Open();

                SqlCommand cmd = new SqlCommand(
                    "SELECT Cname FROM Customer WHERE CustomerId = @ID", con);

                cmd.Parameters.AddWithValue("@ID", CustIdComboBx.SelectedItem.ToString());

                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    CustomerNametxt.Text = reader["Cname"].ToString();
                }
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



        private void cross_Click(object sender, EventArgs e)
        {
            this.Close();

        }

        private void label5_Click(object sender, EventArgs e)
        {
            login login = new login();
            login.Show();
            this.Hide();
        }

        private void label3_Click(object sender, EventArgs e)
        {
            Customers Customers = new Customers();
            Customers.Show();
            this.Hide();
        }

        private void label4_Click(object sender, EventArgs e)
        {
            ///sales
        }

        private void label2_Click(object sender, EventArgs e)
        {
            product product = new product();
            product.Show();
            this.Hide();
        }

        private void LoadCustomerId()
        {
            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(
                    "SELECT CustomerId FROM Customer", con);

                SqlDataReader reader = cmd.ExecuteReader();

                CustIdComboBx.Items.Clear();

                while (reader.Read())
                {
                    CustIdComboBx.Items.Add(reader["CustomerId"].ToString());
                }
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


        private void LoadProductId()
        {
            try
            {
                con.Open();
                // OLD CODE: SqlCommand cmd = new SqlCommand("SELECT ProductId FROM Products", con);

                // NEW CODE: Add "WHERE IsActive = 1"
                // This prevents users from selecting deleted products for NEW sales.
                SqlCommand cmd = new SqlCommand("SELECT ProductId FROM Products WHERE IsActive = 1", con);

                SqlDataReader SqlDataReader = cmd.ExecuteReader();
                while (SqlDataReader.Read())
                {
                    ProductIdComboBox.Items.Add(SqlDataReader["ProductId"].ToString());
                }
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

        private void ProductIdComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            con.Open();
            SqlCommand sqlCommand = new SqlCommand("SELECT ProductName, Price from Products WHERE ProductId=@ID", con);
            sqlCommand.Parameters.AddWithValue("@Id", ProductIdComboBox.Text);
            SqlDataReader reader = sqlCommand.ExecuteReader();
            if (reader.Read())
            {
                ProductNametxt.Text = reader["ProductName"].ToString();
                Totaltxt.Text = reader["Price"].ToString();
            }
            con.Close();
        }

        private void Sales_Load(object sender, EventArgs e)
        {
            LoadCustomerId();
            LoadProductId();

            Color customColor = Color.FromArgb(44, 62, 80);

            dataGridView1.EnableHeadersVisualStyles = false;
            dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = customColor;
            dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);

            dataGridView1.DefaultCellStyle.SelectionBackColor = customColor;
            dataGridView1.DefaultCellStyle.SelectionForeColor = Color.White;

            dataGridView1.GridColor = Color.LightGray;
            printDocument1.PrintPage += PrintDocument1_PrintPage;


        }

        private void insertbtn_Click(object sender, EventArgs e)
        {
            if (CustIdComboBx.Text == "" || ProductIdComboBox.Text == "" || Quantitytxt.Text == "")
            {
                MessageBox.Show("Please fill all required fields");
                return;
            }

            if (!int.TryParse(Quantitytxt.Text, out int quantitySold) || quantitySold <= 0)
            {
                MessageBox.Show("Enter a valid quantity");
                return;
            }

            try
            {
                con.Open();

                // 1️⃣ Get stock, price & expiry date
                SqlCommand getProduct = new SqlCommand(
                    "SELECT Quantity, Price, ExpiryDate FROM Products WHERE ProductId=@PID", con);
                getProduct.Parameters.AddWithValue("@PID", ProductIdComboBox.Text);

                int availableQty;
                decimal unitPrice;

                using (SqlDataReader reader = getProduct.ExecuteReader())
                {
                    if (!reader.Read())
                    {
                        MessageBox.Show("Product not found");
                        return;
                    }

                    availableQty = Convert.ToInt32(reader["Quantity"]);
                    unitPrice = Convert.ToDecimal(reader["Price"]);

                    // 🔴 EXPIRED MEDICINE CHECK
                    DateTime expiryDate = Convert.ToDateTime(reader["ExpiryDate"]);

                    if (expiryDate < DateTime.Today)
                    {
                        MessageBox.Show(
                            "This medicine is expired and cannot be sold.",
                            "Expired Medicine",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning
                        );
                        return; // ⛔ stop sale
                    }
                }

                if (quantitySold > availableQty)
                {
                    MessageBox.Show("Stock is not available");
                    return;
                }

                decimal totalAmount = quantitySold * unitPrice;

                // 2️⃣ Insert sale
                SqlCommand insertSale = new SqlCommand(
                    @"INSERT INTO Sales 
              (CustomerID, CustomerName, ProductID, ProductName, QuantitySold, TotalAmount, SaleDate)
              VALUES (@CID,@CN,@PID,@PN,@QS,@TA,@SD)", con);

                insertSale.Parameters.AddWithValue("@CID", CustIdComboBx.Text);
                insertSale.Parameters.AddWithValue("@CN", CustomerNametxt.Text);
                insertSale.Parameters.AddWithValue("@PID", ProductIdComboBox.Text);
                insertSale.Parameters.AddWithValue("@PN", ProductNametxt.Text);
                insertSale.Parameters.AddWithValue("@QS", quantitySold);
                insertSale.Parameters.AddWithValue("@TA", totalAmount);
                insertSale.Parameters.AddWithValue("@SD", dateTimePicker1.Value);
                insertSale.ExecuteNonQuery();

                // 3️⃣ Deduct stock
                SqlCommand updateStock = new SqlCommand(
                    "UPDATE Products SET Quantity = Quantity - @Qty WHERE ProductId=@PID", con);
                updateStock.Parameters.AddWithValue("@Qty", quantitySold);
                updateStock.Parameters.AddWithValue("@PID", ProductIdComboBox.Text);
                updateStock.ExecuteNonQuery();

                MessageBox.Show("Sale added successfully");
                DisplaySales();
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



        private void updatebtn_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a sale to update");
                return;
            }

            if (!int.TryParse(Quantitytxt.Text, out int newQty) || newQty <= 0)
            {
                MessageBox.Show("Enter a valid quantity");
                return;
            }

            int saleId = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["SaleID"].Value);
            int oldQty = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["QuantitySold"].Value);
            string productId = dataGridView1.SelectedRows[0].Cells["ProductID"].Value.ToString();

            try
            {
                con.Open();

                // 1️⃣ Restore old stock
                SqlCommand restore = new SqlCommand(
                    "UPDATE Products SET Quantity = Quantity + @Q WHERE ProductId=@PID", con);
                restore.Parameters.AddWithValue("@Q", oldQty);
                restore.Parameters.AddWithValue("@PID", productId);
                restore.ExecuteNonQuery();

                // 2️⃣ Get stock & price
                SqlCommand getData = new SqlCommand(
                    "SELECT Quantity, Price FROM Products WHERE ProductId=@PID", con);
                getData.Parameters.AddWithValue("@PID", productId);

                int availableStock;
                decimal unitPrice;

                using (SqlDataReader reader = getData.ExecuteReader())
                {
                    reader.Read();
                    availableStock = Convert.ToInt32(reader["Quantity"]);
                    unitPrice = Convert.ToDecimal(reader["Price"]);
                }

                if (newQty > availableStock)
                {
                    MessageBox.Show("Not enough stock available");

                    // rollback
                    SqlCommand rollback = new SqlCommand(
                        "UPDATE Products SET Quantity = Quantity - @Q WHERE ProductId=@PID", con);
                    rollback.Parameters.AddWithValue("@Q", oldQty);
                    rollback.Parameters.AddWithValue("@PID", productId);
                    rollback.ExecuteNonQuery();

                    return;
                }

                decimal newTotal = newQty * unitPrice;

                // 3️⃣ Update sale
                SqlCommand updateSale = new SqlCommand(
                    @"UPDATE Sales SET
                CustomerID=@CID,
                CustomerName=@CN,
                ProductID=@PID,
                ProductName=@PN,
                QuantitySold=@QS,
                TotalAmount=@TA,
                SaleDate=@SD
              WHERE SaleID=@SID", con);

                updateSale.Parameters.AddWithValue("@CID", CustIdComboBx.Text);
                updateSale.Parameters.AddWithValue("@CN", CustomerNametxt.Text);
                updateSale.Parameters.AddWithValue("@PID", productId);
                updateSale.Parameters.AddWithValue("@PN", ProductNametxt.Text);
                updateSale.Parameters.AddWithValue("@QS", newQty);
                updateSale.Parameters.AddWithValue("@TA", newTotal);
                updateSale.Parameters.AddWithValue("@SD", dateTimePicker1.Value);
                updateSale.Parameters.AddWithValue("@SID", saleId);
                updateSale.ExecuteNonQuery();

                // 4️⃣ Deduct new stock
                SqlCommand deduct = new SqlCommand(
                    "UPDATE Products SET Quantity = Quantity - @Q WHERE ProductId=@PID", con);
                deduct.Parameters.AddWithValue("@Q", newQty);
                deduct.Parameters.AddWithValue("@PID", productId);
                deduct.ExecuteNonQuery();

                MessageBox.Show("Sale updated successfully");
                DisplaySales();
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

        private void deletebtn_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a sale to delete");
                return;
            }

            int saleId = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["SaleID"].Value);
            int qtyToRestore = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["QuantitySold"].Value);
            string productId = dataGridView1.SelectedRows[0].Cells["ProductID"].Value.ToString();

            DialogResult dr = MessageBox.Show(
                "Are you sure you want to delete this sale?",
                "Confirm",
                MessageBoxButtons.YesNo);

            if (dr == DialogResult.No)
                return;

            try
            {
                if (con.State == ConnectionState.Closed)
                    con.Open();

                // restore stock
                SqlCommand restore = new SqlCommand(
                    "UPDATE Products SET Quantity = Quantity + @Q WHERE ProductId=@PID", con);
                restore.Parameters.AddWithValue("@Q", qtyToRestore);
                restore.Parameters.AddWithValue("@PID", productId);
                restore.ExecuteNonQuery();

                // delete sale
                SqlCommand deleteSale = new SqlCommand(
                    "DELETE FROM Sales WHERE SaleID=@SID", con);
                deleteSale.Parameters.AddWithValue("@SID", saleId);
                deleteSale.ExecuteNonQuery();

                MessageBox.Show("Sale deleted successfully");
                DisplaySales();
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




        private void dataGridView1_DoubleClick(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow != null && dataGridView1.CurrentRow.Index != -1)
            {
                CustIdComboBx.Text = dataGridView1.CurrentRow.Cells["CustomerID"].Value.ToString();
                CustomerNametxt.Text = dataGridView1.CurrentRow.Cells["CustomerName"].Value.ToString();
                ProductIdComboBox.Text = dataGridView1.CurrentRow.Cells["ProductID"].Value.ToString();
                ProductNametxt.Text = dataGridView1.CurrentRow.Cells["ProductName"].Value.ToString();
                Quantitytxt.Text = dataGridView1.CurrentRow.Cells["QuantitySold"].Value.ToString();
                Totaltxt.Text = dataGridView1.CurrentRow.Cells["TotalAmount"].Value.ToString();
                dateTimePicker1.Text = dataGridView1.CurrentRow.Cells["SaleDate"].Value.ToString();
            }
        }

        private PrintDocument printDocument1 = new PrintDocument();
        private int currentRow = 0;

        private void PrintDocument1_PrintPage(object sender, PrintPageEventArgs e)
        {
            Font titleFont = new Font("Segoe UI", 14, FontStyle.Bold);
            Font headerFont = new Font("Segoe UI", 9, FontStyle.Bold);
            Font cellFont = new Font("Segoe UI", 9);

            int startX = 40;
            int startY = 80;
            int rowHeight = 30;

            // ===== TITLE =====
            e.Graphics.DrawString(
                "Sales Report",
                titleFont,
                Brushes.Black,
                new PointF(startX + 250, 30)
            );

            // ===== TABLE HEADERS =====
            int x = startX;
            foreach (DataGridViewColumn col in dataGridView1.Columns)
            {
                e.Graphics.FillRectangle(Brushes.LightGray, x, startY, col.Width, rowHeight);
                e.Graphics.DrawRectangle(Pens.Black, x, startY, col.Width, rowHeight);

                e.Graphics.DrawString(
                    col.HeaderText,
                    headerFont,
                    Brushes.Black,
                    new RectangleF(x + 5, startY + 7, col.Width, rowHeight)
                );

                x += col.Width;
            }

            startY += rowHeight;

            // ===== TABLE ROWS =====
            while (currentRow < dataGridView1.Rows.Count)
            {
                DataGridViewRow row = dataGridView1.Rows[currentRow];

                if (row.IsNewRow)
                {
                    currentRow++;
                    continue;
                }

                x = startX;

                foreach (DataGridViewCell cell in row.Cells)
                {
                    e.Graphics.DrawRectangle(Pens.Black, x, startY, cell.OwningColumn.Width, rowHeight);

                    string value = cell.Value != null ? cell.Value.ToString() : "";

                    e.Graphics.DrawString(
                        value,
                        cellFont,
                        Brushes.Black,
                        new RectangleF(x + 5, startY + 7, cell.OwningColumn.Width, rowHeight)
                    );

                    x += cell.OwningColumn.Width;
                }

                startY += rowHeight;
                currentRow++;

                // ===== PAGE BREAK =====
                if (startY + rowHeight > e.MarginBounds.Bottom)
                {
                    e.HasMorePages = true;
                    return;
                }
            }

            // ===== DONE =====
            currentRow = 0;
            e.HasMorePages = false;
        }

        private void reportbtn_Click(object sender, EventArgs e)
        {
            currentRow = 0;

            PrintPreviewDialog preview = new PrintPreviewDialog();
            preview.Document = printDocument1;
            preview.WindowState = FormWindowState.Maximized;
            preview.ShowDialog();
        }

        private void clearbtn_Click(object sender, EventArgs e)
        {
            CustIdComboBx.SelectedIndex = -1;
            CustomerNametxt.Clear();

            ProductIdComboBox.SelectedIndex = -1;
            ProductNametxt.Clear();

            Quantitytxt.Clear();
            Totaltxt.Clear();

            dateTimePicker1.Value = DateTime.Now;

            dataGridView1.ClearSelection();
        }
    }


}






