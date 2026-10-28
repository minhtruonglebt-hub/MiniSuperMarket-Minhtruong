namespace MiniSuperMarket.WinForm
{
    partial class FormCustomerManagement
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            txtKeyword = new TextBox();
            btnSearch = new Button();
            btnLoad = new Button();
            dgvCustomers = new DataGridView();
            lblCustomerId = new Label();
            txtCustomerId = new TextBox();
            lblCustomerName = new Label();
            txtCustomerName = new TextBox();
            lblPhoneNumber = new Label();
            txtPhoneNumber = new TextBox();
            lblAddress = new Label();
            txtAddress = new TextBox();
            lblRewardPoints = new Label();
            txtRewardPoints = new TextBox();
            lblMembershipRank = new Label();
            txtMembershipRank = new TextBox();
            btnAdd = new Button();
            btnUpdate = new Button();
            btnDelete = new Button();
            ((System.ComponentModel.ISupportInitialize)dgvCustomers).BeginInit();
            SuspendLayout();

            txtKeyword.Location = new Point(15, 15);
            txtKeyword.Name = "txtKeyword";
            txtKeyword.Size = new Size(300, 27);

            btnSearch.Location = new Point(325, 13);
            btnSearch.Name = "btnSearch";
            btnSearch.Size = new Size(100, 31);
            btnSearch.Text = "Tìm kiếm";
            btnSearch.UseVisualStyleBackColor = true;
            btnSearch.Click += btnSearch_Click;

            btnLoad.Location = new Point(435, 13);
            btnLoad.Name = "btnLoad";
            btnLoad.Size = new Size(100, 31);
            btnLoad.Text = "Tải lại";
            btnLoad.UseVisualStyleBackColor = true;
            btnLoad.Click += btnLoad_Click;

            dgvCustomers.AllowUserToAddRows = false;
            dgvCustomers.AllowUserToDeleteRows = false;
            dgvCustomers.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvCustomers.Location = new Point(15, 55);
            dgvCustomers.MultiSelect = false;
            dgvCustomers.Name = "dgvCustomers";
            dgvCustomers.ReadOnly = true;
            dgvCustomers.RowHeadersWidth = 51;
            dgvCustomers.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvCustomers.Size = new Size(830, 260);
            dgvCustomers.CellClick += dgvCustomers_CellClick;

            lblCustomerId.AutoSize = true;
            lblCustomerId.Location = new Point(15, 338);
            lblCustomerId.Name = "lblCustomerId";
            lblCustomerId.Text = "Mã KH:";

            txtCustomerId.Location = new Point(130, 335);
            txtCustomerId.Name = "txtCustomerId";
            txtCustomerId.Size = new Size(80, 27);
            txtCustomerId.ReadOnly = true;

            lblCustomerName.AutoSize = true;
            lblCustomerName.Location = new Point(230, 338);
            lblCustomerName.Name = "lblCustomerName";
            lblCustomerName.Text = "Họ tên:";

            txtCustomerName.Location = new Point(300, 335);
            txtCustomerName.Name = "txtCustomerName";
            txtCustomerName.Size = new Size(220, 27);

            lblPhoneNumber.AutoSize = true;
            lblPhoneNumber.Location = new Point(540, 338);
            lblPhoneNumber.Name = "lblPhoneNumber";
            lblPhoneNumber.Text = "SĐT:";

            txtPhoneNumber.Location = new Point(590, 335);
            txtPhoneNumber.Name = "txtPhoneNumber";
            txtPhoneNumber.Size = new Size(255, 27);

            lblAddress.AutoSize = true;
            lblAddress.Location = new Point(15, 388);
            lblAddress.Name = "lblAddress";
            lblAddress.Text = "Địa chỉ:";

            txtAddress.Location = new Point(130, 385);
            txtAddress.Name = "txtAddress";
            txtAddress.Size = new Size(390, 27);

            lblRewardPoints.AutoSize = true;
            lblRewardPoints.Location = new Point(540, 388);
            lblRewardPoints.Name = "lblRewardPoints";
            lblRewardPoints.Text = "Điểm:";

            txtRewardPoints.Location = new Point(590, 385);
            txtRewardPoints.Name = "txtRewardPoints";
            txtRewardPoints.Size = new Size(100, 27);

            lblMembershipRank.AutoSize = true;
            lblMembershipRank.Location = new Point(15, 438);
            lblMembershipRank.Name = "lblMembershipRank";
            lblMembershipRank.Text = "Hạng thẻ:";

            txtMembershipRank.Location = new Point(130, 435);
            txtMembershipRank.Name = "txtMembershipRank";
            txtMembershipRank.Size = new Size(150, 27);

            btnAdd.Location = new Point(15, 490);
            btnAdd.Name = "btnAdd";
            btnAdd.Size = new Size(120, 36);
            btnAdd.Text = "Thêm";
            btnAdd.UseVisualStyleBackColor = true;
            btnAdd.Click += btnAdd_Click;

            btnUpdate.Location = new Point(145, 490);
            btnUpdate.Name = "btnUpdate";
            btnUpdate.Size = new Size(120, 36);
            btnUpdate.Text = "Cập nhật";
            btnUpdate.UseVisualStyleBackColor = true;
            btnUpdate.Click += btnUpdate_Click;

            btnDelete.Location = new Point(275, 490);
            btnDelete.Name = "btnDelete";
            btnDelete.Size = new Size(120, 36);
            btnDelete.Text = "Xóa";
            btnDelete.UseVisualStyleBackColor = true;
            btnDelete.Click += btnDelete_Click;

            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(860, 545);
            Controls.Add(txtKeyword);
            Controls.Add(btnSearch);
            Controls.Add(btnLoad);
            Controls.Add(dgvCustomers);
            Controls.Add(lblCustomerId);
            Controls.Add(txtCustomerId);
            Controls.Add(lblCustomerName);
            Controls.Add(txtCustomerName);
            Controls.Add(lblPhoneNumber);
            Controls.Add(txtPhoneNumber);
            Controls.Add(lblAddress);
            Controls.Add(txtAddress);
            Controls.Add(lblRewardPoints);
            Controls.Add(txtRewardPoints);
            Controls.Add(lblMembershipRank);
            Controls.Add(txtMembershipRank);
            Controls.Add(btnAdd);
            Controls.Add(btnUpdate);
            Controls.Add(btnDelete);
            Name = "FormCustomerManagement";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Quản lý khách hàng thân thiết - Bách hóa Tổng hợp Thiên Ân";
            Load += FormCustomerManagement_Load;
            ((System.ComponentModel.ISupportInitialize)dgvCustomers).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        private TextBox txtKeyword;
        private Button btnSearch;
        private Button btnLoad;
        private DataGridView dgvCustomers;
        private Label lblCustomerId;
        private TextBox txtCustomerId;
        private Label lblCustomerName;
        private TextBox txtCustomerName;
        private Label lblPhoneNumber;
        private TextBox txtPhoneNumber;
        private Label lblAddress;
        private TextBox txtAddress;
        private Label lblRewardPoints;
        private TextBox txtRewardPoints;
        private Label lblMembershipRank;
        private TextBox txtMembershipRank;
        private Button btnAdd;
        private Button btnUpdate;
        private Button btnDelete;
    }
}
