namespace MiniSuperMarket.WinForm
{
    partial class FormCategoryManagement
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            lblRole = new Label();
            txtKeyword = new TextBox();
            btnSearch = new Button();
            btnReload = new Button();
            btnCustomers = new Button();
            btnRoles = new Button();
            dgvCategories = new DataGridView();
            lblId = new Label();
            txtId = new TextBox();
            lblName = new Label();
            txtName = new TextBox();
            lblDesc = new Label();
            txtDesc = new TextBox();
            btnAdd = new Button();
            btnUpdate = new Button();
            btnDelete = new Button();
            ((System.ComponentModel.ISupportInitialize)dgvCategories).BeginInit();
            SuspendLayout();

            lblRole.AutoSize = true;
            lblRole.Location = new Point(15, 12);
            lblRole.Name = "lblRole";
            lblRole.Text = "Vai trò hiện tại:";
            lblRole.Font = new Font("Segoe UI", 10F, FontStyle.Bold);

            txtKeyword.Location = new Point(15, 45);
            txtKeyword.Name = "txtKeyword";
            txtKeyword.Size = new Size(280, 27);

            btnSearch.Location = new Point(305, 43);
            btnSearch.Name = "btnSearch";
            btnSearch.Size = new Size(100, 31);
            btnSearch.Text = "Tìm kiếm";
            btnSearch.UseVisualStyleBackColor = true;
            btnSearch.Click += btnSearch_Click;

            btnReload.Location = new Point(415, 43);
            btnReload.Name = "btnReload";
            btnReload.Size = new Size(100, 31);
            btnReload.Text = "Tải lại";
            btnReload.UseVisualStyleBackColor = true;
            btnReload.Click += btnReload_Click;

            btnCustomers.Location = new Point(525, 43);
            btnCustomers.Name = "btnCustomers";
            btnCustomers.Size = new Size(105, 31);
            btnCustomers.Text = "Khách hàng";
            btnCustomers.UseVisualStyleBackColor = true;
            btnCustomers.Click += btnCustomers_Click;

            btnRoles.Location = new Point(640, 43);
            btnRoles.Name = "btnRoles";
            btnRoles.Size = new Size(105, 31);
            btnRoles.Text = "Vai trò";
            btnRoles.UseVisualStyleBackColor = true;
            btnRoles.Click += btnRoles_Click;

            dgvCategories.AllowUserToAddRows = false;
            dgvCategories.AllowUserToDeleteRows = false;
            dgvCategories.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvCategories.Location = new Point(15, 85);
            dgvCategories.MultiSelect = false;
            dgvCategories.Name = "dgvCategories";
            dgvCategories.ReadOnly = true;
            dgvCategories.RowHeadersWidth = 51;
            dgvCategories.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvCategories.Size = new Size(730, 230);
            dgvCategories.CellClick += dgvCategories_CellClick;

            lblId.AutoSize = true;
            lblId.Location = new Point(15, 338);
            lblId.Name = "lblId";
            lblId.Text = "Mã:";

            txtId.Location = new Point(60, 335);
            txtId.Name = "txtId";
            txtId.Size = new Size(60, 27);
            txtId.ReadOnly = true;

            lblName.AutoSize = true;
            lblName.Location = new Point(140, 338);
            lblName.Name = "lblName";
            lblName.Text = "Tên nhóm hàng:";

            txtName.Location = new Point(260, 335);
            txtName.Name = "txtName";
            txtName.Size = new Size(190, 27);

            lblDesc.AutoSize = true;
            lblDesc.Location = new Point(465, 338);
            lblDesc.Name = "lblDesc";
            lblDesc.Text = "Mô tả:";

            txtDesc.Location = new Point(520, 335);
            txtDesc.Name = "txtDesc";
            txtDesc.Size = new Size(225, 27);

            btnAdd.Location = new Point(15, 385);
            btnAdd.Name = "btnAdd";
            btnAdd.Size = new Size(120, 36);
            btnAdd.Text = "Thêm";
            btnAdd.UseVisualStyleBackColor = true;
            btnAdd.Click += btnAdd_Click;

            btnUpdate.Location = new Point(145, 385);
            btnUpdate.Name = "btnUpdate";
            btnUpdate.Size = new Size(120, 36);
            btnUpdate.Text = "Cập nhật";
            btnUpdate.UseVisualStyleBackColor = true;
            btnUpdate.Click += btnUpdate_Click;

            btnDelete.Location = new Point(275, 385);
            btnDelete.Name = "btnDelete";
            btnDelete.Size = new Size(120, 36);
            btnDelete.Text = "Xóa (Admin)";
            btnDelete.UseVisualStyleBackColor = true;
            btnDelete.Click += btnDelete_Click;

            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(760, 440);
            Controls.Add(lblRole);
            Controls.Add(txtKeyword);
            Controls.Add(btnSearch);
            Controls.Add(btnReload);
            Controls.Add(btnCustomers);
            Controls.Add(btnRoles);
            Controls.Add(dgvCategories);
            Controls.Add(lblId);
            Controls.Add(txtId);
            Controls.Add(lblName);
            Controls.Add(txtName);
            Controls.Add(lblDesc);
            Controls.Add(txtDesc);
            Controls.Add(btnAdd);
            Controls.Add(btnUpdate);
            Controls.Add(btnDelete);
            Name = "FormCategoryManagement";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Quản lý nhóm hàng - Bách hóa Tổng hợp Thiên Ân";
            Load += FormCategoryManagement_Load;
            ((System.ComponentModel.ISupportInitialize)dgvCategories).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        private Label lblRole;
        private TextBox txtKeyword;
        private Button btnSearch;
        private Button btnReload;
        private Button btnCustomers;
        private Button btnRoles;
        private DataGridView dgvCategories;
        private Label lblId;
        private TextBox txtId;
        private Label lblName;
        private TextBox txtName;
        private Label lblDesc;
        private TextBox txtDesc;
        private Button btnAdd;
        private Button btnUpdate;
        private Button btnDelete;
    }
}
