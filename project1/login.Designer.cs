namespace project1
{
    partial class login
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            label1 = new Label();
            pictureBox1 = new PictureBox();
            label2 = new Label();
            label3 = new Label();
            close = new Label();
            usertxt = new TextBox();
            passtxt = new TextBox();
            loginbtn = new Button();
            clearbtn = new Label();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Times New Roman", 15.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label1.ForeColor = Color.White;
            label1.Location = new Point(183, 56);
            label1.Name = "label1";
            label1.Size = new Size(155, 24);
            label1.TabIndex = 0;
            label1.Text = "Pharmacy Store";
            // 
            // pictureBox1
            // 
            pictureBox1.Image = Properties.Resources.vecteezy_pharmacy_store_front_60579583;
            pictureBox1.Location = new Point(206, 90);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(103, 80);
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox1.TabIndex = 1;
            pictureBox1.TabStop = false;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Times New Roman", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label2.ForeColor = Color.White;
            label2.Location = new Point(56, 222);
            label2.Name = "label2";
            label2.Size = new Size(92, 22);
            label2.TabIndex = 2;
            label2.Text = "Username";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Times New Roman", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label3.ForeColor = Color.White;
            label3.Location = new Point(56, 279);
            label3.Name = "label3";
            label3.Size = new Size(90, 22);
            label3.TabIndex = 3;
            label3.Text = "Password";
            // 
            // close
            // 
            close.AutoSize = true;
            close.Font = new Font("Times New Roman", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            close.ForeColor = Color.White;
            close.Location = new Point(496, 4);
            close.Name = "close";
            close.Size = new Size(23, 22);
            close.TabIndex = 4;
            close.Text = "X";
            close.Click += close_Click;
            // 
            // usertxt
            // 
            usertxt.Font = new Font("Times New Roman", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            usertxt.Location = new Point(172, 219);
            usertxt.Name = "usertxt";
            usertxt.Size = new Size(204, 25);
            usertxt.TabIndex = 5;
            // 
            // passtxt
            // 
            passtxt.Font = new Font("Times New Roman", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            passtxt.Location = new Point(172, 276);
            passtxt.Name = "passtxt";
            passtxt.PasswordChar = '*';
            passtxt.Size = new Size(204, 25);
            passtxt.TabIndex = 6;
            // 
            // loginbtn
            // 
            loginbtn.Font = new Font("Times New Roman", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            loginbtn.Location = new Point(211, 341);
            loginbtn.Name = "loginbtn";
            loginbtn.Size = new Size(90, 36);
            loginbtn.TabIndex = 7;
            loginbtn.Text = "Login";
            loginbtn.UseVisualStyleBackColor = true;
            loginbtn.Click += loginbtn_Click;
            // 
            // clearbtn
            // 
            clearbtn.AutoSize = true;
            clearbtn.Font = new Font("Times New Roman", 11.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            clearbtn.ForeColor = Color.White;
            clearbtn.Location = new Point(233, 390);
            clearbtn.Name = "clearbtn";
            clearbtn.Size = new Size(44, 17);
            clearbtn.TabIndex = 8;
            clearbtn.Text = "Clear";
            clearbtn.Click += clearbtn_Click;
            // 
            // login
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoSize = true;
            BackColor = Color.FromArgb(44, 62, 80);
            ClientSize = new Size(522, 460);
            Controls.Add(clearbtn);
            Controls.Add(loginbtn);
            Controls.Add(passtxt);
            Controls.Add(usertxt);
            Controls.Add(close);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(pictureBox1);
            Controls.Add(label1);
            FormBorderStyle = FormBorderStyle.None;
            Name = "login";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "login";
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private PictureBox pictureBox1;
        private Label label2;
        private Label label3;
        private Label close;
        private TextBox usertxt;
        private TextBox passtxt;
        private Button loginbtn;
        private Label clearbtn;
    }
}
