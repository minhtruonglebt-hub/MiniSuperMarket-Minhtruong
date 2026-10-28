using System.Net.Http.Json;

namespace MiniSuperMarket.WinForm
{
    public partial class FormCategoryManagement : Form
    {
        public FormCategoryManagement()
        {
            InitializeComponent();
            BackColor = Color.FromArgb(244, 246, 249);
            dgvCategories.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            dgvCategories.BackgroundColor = Color.White;
            dgvCategories.RowHeadersVisible = false;
            dgvCategories.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvCategories.ColumnHeadersHeight = 38;
            dgvCategories.RowTemplate.Height = 32;
            foreach (var control in new Control[] { lblRole, txtKeyword, btnSearch, btnReload })
                control.Anchor = AnchorStyles.Top | AnchorStyles.Left | (control == txtKeyword ? AnchorStyles.Right : 0);
            Resize += (_, _) => LayoutManagementControls();
            LayoutManagementControls();
        }

        private void LayoutManagementControls()
        {
            const int left = 16, gap = 10, buttonWidth = 104;
            var width = Math.Max(0, ClientSize.Width - left * 2);
            lblRole.Location = new Point(left, 10);
            txtKeyword.SetBounds(left, 42, Math.Max(180, width - buttonWidth * 2 - gap * 2), 30);
            btnSearch.SetBounds(txtKeyword.Right + gap, 41, buttonWidth, 32);
            btnReload.SetBounds(btnSearch.Right + gap, 41, buttonWidth, 32);

            var editorY = Math.Max(136, ClientSize.Height - 96);
            dgvCategories.SetBounds(left, 84, width, Math.Max(80, editorY - 100));
            lblId.SetBounds(left, editorY + 4, 36, 28);
            txtId.SetBounds(left + 40, editorY, 62, 30);
            lblName.SetBounds(left + 116, editorY + 4, 112, 28);
            txtName.SetBounds(left + 230, editorY, Math.Max(120, width * 32 / 100), 30);
            var descLabelX = txtName.Right + gap;
            lblDesc.SetBounds(descLabelX, editorY + 4, 48, 28);
            txtDesc.SetBounds(descLabelX + 50, editorY, Math.Max(100, ClientSize.Width - descLabelX - 66), 30);
            var actionY = editorY + 42;
            btnAdd.SetBounds(left, actionY, 112, 34);
            btnUpdate.SetBounds(btnAdd.Right + 10, actionY, 120, 34);
            btnDelete.SetBounds(btnUpdate.Right + 10, actionY, 120, 34);
        }

        private async void FormCategoryManagement_Load(object? sender, EventArgs e)
        {
            lblRole.Text = $"Tài khoản: {SessionManager.CurrentUsername} | Vai trò hiện tại: {SessionManager.CurrentRole}";
            await LoadDataAsync();
        }

        // Tải danh sách (hoặc kết quả tìm kiếm) từ API lên DataGridView
        private async Task LoadDataAsync(string? keyword = null)
        {
            try
            {
                string url = string.IsNullOrWhiteSpace(keyword)
                    ? "categories"
                    : $"categories/search?keyword={Uri.EscapeDataString(keyword.Trim())}";

                var categories = await ApiClientService.GetListAsync<CategoryDto>(url);
                if (categories != null) dgvCategories.DataSource = categories;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi kết nối Server: " + ex.Message, "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnReload_Click(object? sender, EventArgs e)
        {
            txtKeyword.Clear();
            await LoadDataAsync();
        }

        private async void btnSearch_Click(object? sender, EventArgs e)
        {
            await LoadDataAsync(txtKeyword.Text);
        }

        // Click vào một dòng: đưa dữ liệu lên các ô nhập để Sửa/Xóa
        private void dgvCategories_CellClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (dgvCategories.Rows[e.RowIndex].DataBoundItem is CategoryDto cat)
            {
                txtId.Text = cat.CategoryId.ToString();
                txtName.Text = cat.CategoryName;
                txtDesc.Text = cat.Description ?? string.Empty;
            }
        }

        private async void btnAdd_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Nhập tên nhóm hàng!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var dto = new { CategoryName = txtName.Text.Trim(), Description = txtDesc.Text.Trim() };
                var res = await ApiClientService.Client.PostAsJsonAsync("categories", dto);
                if (res.IsSuccessStatusCode)
                {
                    MessageBox.Show("Thêm mới thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearInputs();
                    await LoadDataAsync();
                }
                else
                {
                    MessageBox.Show("Thêm thất bại: " + await ApiClientService.GetErrorMessageAsync(res),
                        "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnUpdate_Click(object? sender, EventArgs e)
        {
            if (!int.TryParse(txtId.Text, out int id))
            {
                MessageBox.Show("Vui lòng chọn nhóm hàng cần sửa!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Nhập tên nhóm hàng!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var dto = new { CategoryId = id, CategoryName = txtName.Text.Trim(), Description = txtDesc.Text.Trim() };
                var res = await ApiClientService.Client.PutAsJsonAsync($"categories/{id}", dto);
                if (res.IsSuccessStatusCode)
                {
                    MessageBox.Show("Cập nhật thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearInputs();
                    await LoadDataAsync();
                }
                else
                {
                    MessageBox.Show("Cập nhật thất bại: " + await ApiClientService.GetErrorMessageAsync(res),
                        "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnDelete_Click(object? sender, EventArgs e)
        {
            if (!int.TryParse(txtId.Text, out int id))
            {
                MessageBox.Show("Vui lòng chọn nhóm hàng cần xóa!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var confirm = MessageBox.Show($"Bạn có chắc muốn xóa nhóm hàng ID = {id}?", "Xác nhận",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes) return;

            try
            {
                var res = await ApiClientService.Client.DeleteAsync($"categories/{id}");
                if (res.IsSuccessStatusCode)
                {
                    MessageBox.Show("Xóa thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearInputs();
                    await LoadDataAsync();
                }
                else
                {
                    MessageBox.Show(await ApiClientService.GetErrorMessageAsync(res), "Xóa thất bại",
                        MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Mở màn hình Khách hàng (Admin, Cashier)
        private void btnCustomers_Click(object? sender, EventArgs e)
        {
            using var form = new FormCustomerManagement();
            form.ShowDialog(this);
        }

        // Mở màn hình Vai trò (chỉ Admin - Cashier sẽ nhận 403)
        private void btnRoles_Click(object? sender, EventArgs e)
        {
            using var form = new FormRoleManagement();
            form.ShowDialog(this);
        }

        private void ClearInputs()
        {
            txtId.Clear();
            txtName.Clear();
            txtDesc.Clear();
        }
    }
}
