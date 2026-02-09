namespace project1
{
    public partial class login : Form
    {
        public login()
        {
            InitializeComponent();
        }

        private void close_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void loginbtn_Click(object sender, EventArgs e)
        {
            try
            {
                if (usertxt.Text == "admin" && passtxt.Text == "messi")
                {
                    MessageBox.Show("Login Successfull");
                    product Product = new product();
                    Product.Show();
                    this.Hide();
                }

                else
                {
                    MessageBox.Show("Invalid Username or Password!!!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void clearbtn_Click(object sender, EventArgs e)
        {
            ///usertxt.Clear();
            ///passtxt.Clear();
            /// usertxt.Focus();
            usertxt.Text = "";
            passtxt.Text = "";
        }

        private void login_Load(object sender, EventArgs e)
        {

        }
    }
}
