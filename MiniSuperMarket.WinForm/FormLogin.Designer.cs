namespace MiniSuperMarket.WinForm
{
    partial class FormLogin
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            lblUser = new Label();
            lblPass = new Label();
            txtUser = new TextBox();
            txtPass = new TextBox();
            btnLogin = new Button();
            lblHint = new Label();
            SuspendLayout();

            lblUser.AutoSize = true;
            lblUser.Location = new Point(30, 30);
            lblUser.Text = "Tài khoản:";

            txtUser.Location = new Point(120, 27);
            txtUser.Name = "txtUser";
            txtUser.Size = new Size(200, 27);

            lblPass.AutoSize = true;
            lblPass.Location = new Point(30, 75);
            lblPass.Text = "Mật khẩu:";

            txtPass.Location = new Point(120, 72);
            txtPass.Name = "txtPass";
            txtPass.Size = new Size(200, 27);
            txtPass.UseSystemPasswordChar = true;

            btnLogin.Location = new Point(120, 120);
            btnLogin.Name = "btnLogin";
            btnLogin.Size = new Size(200, 36);
            btnLogin.Text = "Đăng nhập hệ thống";
            btnLogin.UseVisualStyleBackColor = true;
            btnLogin.Click += btnLogin_Click;

            lblHint.AutoSize = true;
            lblHint.ForeColor = Color.Gray;
            lblHint.Location = new Point(30, 175);
            lblHint.Text = "Thử: admin / 123456  hoặc  cashier / 123456";

            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(370, 215);
            Controls.Add(lblUser);
            Controls.Add(txtUser);
            Controls.Add(lblPass);
            Controls.Add(txtPass);
            Controls.Add(btnLogin);
            Controls.Add(lblHint);
            AcceptButton = btnLogin;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            Name = "FormLogin";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Đăng nhập - Bách hóa Tổng hợp Thiên Ân";
            ResumeLayout(false);
            PerformLayout();
        }

        private Label lblUser;
        private Label lblPass;
        private TextBox txtUser;
        private TextBox txtPass;
        private Button btnLogin;
        private Label lblHint;
    }
}
