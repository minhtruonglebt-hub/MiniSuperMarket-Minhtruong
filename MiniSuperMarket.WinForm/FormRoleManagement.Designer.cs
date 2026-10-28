namespace MiniSuperMarket.WinForm
{
    partial class FormRoleManagement
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            dgvRoles = new DataGridView();
            lblId = new Label();
            txtId = new TextBox();
            lblRoleName = new Label();
            txtRoleName = new TextBox();
            lblDescription = new Label();
            txtDescription = new TextBox();
            btnLoad = new Button();
            btnAdd = new Button();
            btnUpdate = new Button();
            btnDelete = new Button();
            ((System.ComponentModel.ISupportInitialize)dgvRoles).BeginInit();
            SuspendLayout();

            dgvRoles.AllowUserToAddRows = false;
            dgvRoles.AllowUserToDeleteRows = false;
            dgvRoles.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvRoles.Location = new Point(15, 15);
            dgvRoles.MultiSelect = false;
            dgvRoles.Name = "dgvRoles";
            dgvRoles.ReadOnly = true;
            dgvRoles.RowHeadersWidth = 51;
            dgvRoles.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvRoles.Size = new Size(670, 220);
            dgvRoles.CellClick += dgvRoles_CellClick;

            lblId.AutoSize = true;
            lblId.Location = new Point(15, 258);
            lblId.Name = "lblId";
            lblId.Text = "Mã:";

            txtId.Location = new Point(60, 255);
            txtId.Name = "txtId";
            txtId.Size = new Size(60, 27);
            txtId.ReadOnly = true;

            lblRoleName.AutoSize = true;
            lblRoleName.Location = new Point(140, 258);
            lblRoleName.Name = "lblRoleName";
            lblRoleName.Text = "Tên vai trò:";

            txtRoleName.Location = new Point(235, 255);
            txtRoleName.Name = "txtRoleName";
            txtRoleName.Size = new Size(180, 27);

            lblDescription.AutoSize = true;
            lblDescription.Location = new Point(430, 258);
            lblDescription.Name = "lblDescription";
            lblDescription.Text = "Mô tả:";

            txtDescription.Location = new Point(485, 255);
            txtDescription.Name = "txtDescription";
            txtDescription.Size = new Size(200, 27);

            btnLoad.Location = new Point(15, 305);
            btnLoad.Name = "btnLoad";
            btnLoad.Size = new Size(120, 36);
            btnLoad.Text = "Tải lại";
            btnLoad.UseVisualStyleBackColor = true;
            btnLoad.Click += btnLoad_Click;

            btnAdd.Location = new Point(145, 305);
            btnAdd.Name = "btnAdd";
            btnAdd.Size = new Size(120, 36);
            btnAdd.Text = "Thêm";
            btnAdd.UseVisualStyleBackColor = true;
            btnAdd.Click += btnAdd_Click;

            btnUpdate.Location = new Point(275, 305);
            btnUpdate.Name = "btnUpdate";
            btnUpdate.Size = new Size(120, 36);
            btnUpdate.Text = "Cập nhật";
            btnUpdate.UseVisualStyleBackColor = true;
            btnUpdate.Click += btnUpdate_Click;

            btnDelete.Location = new Point(405, 305);
            btnDelete.Name = "btnDelete";
            btnDelete.Size = new Size(120, 36);
            btnDelete.Text = "Xóa";
            btnDelete.UseVisualStyleBackColor = true;
            btnDelete.Click += btnDelete_Click;

            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(700, 360);
            Controls.Add(dgvRoles);
            Controls.Add(lblId);
            Controls.Add(txtId);
            Controls.Add(lblRoleName);
            Controls.Add(txtRoleName);
            Controls.Add(lblDescription);
            Controls.Add(txtDescription);
            Controls.Add(btnLoad);
            Controls.Add(btnAdd);
            Controls.Add(btnUpdate);
            Controls.Add(btnDelete);
            Name = "FormRoleManagement";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Quản lý vai trò (Admin) - Bách hóa Tổng hợp Thiên Ân";
            Load += FormRoleManagement_Load;
            ((System.ComponentModel.ISupportInitialize)dgvRoles).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        private DataGridView dgvRoles;
        private Label lblId;
        private TextBox txtId;
        private Label lblRoleName;
        private TextBox txtRoleName;
        private Label lblDescription;
        private TextBox txtDescription;
        private Button btnLoad;
        private Button btnAdd;
        private Button btnUpdate;
        private Button btnDelete;
    }
}
