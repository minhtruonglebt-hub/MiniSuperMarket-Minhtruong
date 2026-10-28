using System.Net.Http.Json;

namespace MiniSuperMarket.WinForm
{
    public partial class FormRoleManagement : Form
    {
        public FormRoleManagement()
        {
            InitializeComponent();
        }

        private async void FormRoleManagement_Load(object? sender, EventArgs e)
        {
            await LoadRolesAsync();
        }

        private async Task LoadRolesAsync()
        {
            try
            {
                var roles = await ApiClientService.GetListAsync<RoleDto>("roles");
                if (roles != null) dgvRoles.DataSource = roles;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi kết nối Server: " + ex.Message, "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnLoad_Click(object? sender, EventArgs e) => await LoadRolesAsync();

        private void dgvRoles_CellClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (dgvRoles.Rows[e.RowIndex].DataBoundItem is RoleDto role)
            {
                txtId.Text = role.Id.ToString();
                txtRoleName.Text = role.RoleName;
                txtDescription.Text = role.Description ?? string.Empty;
            }
        }

        private async void btnAdd_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtRoleName.Text))
            {
                MessageBox.Show("Nhập tên vai trò!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var dto = new { RoleName = txtRoleName.Text.Trim(), Description = txtDescription.Text.Trim() };
                var res = await ApiClientService.Client.PostAsJsonAsync("roles", dto);
                if (res.IsSuccessStatusCode)
                {
                    MessageBox.Show("Thêm vai trò mới thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearInputs();
                    await LoadRolesAsync();
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
                MessageBox.Show("Vui lòng chọn vai trò cần sửa!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var dto = new { Id = id, RoleName = txtRoleName.Text.Trim(), Description = txtDescription.Text.Trim() };
                var res = await ApiClientService.Client.PutAsJsonAsync($"roles/{id}", dto);
                if (res.IsSuccessStatusCode)
                {
                    MessageBox.Show("Cập nhật vai trò thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearInputs();
                    await LoadRolesAsync();
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
                MessageBox.Show("Vui lòng chọn vai trò cần xóa!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var confirm = MessageBox.Show($"Bạn có chắc muốn xóa vai trò ID = {id}?", "Xác nhận",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes) return;

            try
            {
                var res = await ApiClientService.Client.DeleteAsync($"roles/{id}");
                if (res.IsSuccessStatusCode)
                {
                    MessageBox.Show("Xóa vai trò thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearInputs();
                    await LoadRolesAsync();
                }
                else
                {
                    MessageBox.Show(await ApiClientService.GetErrorMessageAsync(res), "Xóa thất bại",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClearInputs()
        {
            txtId.Clear();
            txtRoleName.Clear();
            txtDescription.Clear();
        }
    }
}
