using System.Net.Http.Json;

namespace MiniSuperMarket.WinForm
{
    public partial class FormCustomerManagement : Form
    {
        public FormCustomerManagement()
        {
            InitializeComponent();
            ClearInputs();
            BackColor = Color.FromArgb(244, 246, 249);
            dgvCustomers.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            dgvCustomers.BackgroundColor = Color.White;
            dgvCustomers.RowHeadersVisible = false;
            dgvCustomers.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvCustomers.ColumnHeadersHeight = 38;
            dgvCustomers.RowTemplate.Height = 32;
            Resize += (_, _) => LayoutManagementControls();
            LayoutManagementControls();
        }

        private void LayoutManagementControls()
        {
            const int left = 16, gap = 10;
            var width = Math.Max(0, ClientSize.Width - left * 2);
            txtKeyword.SetBounds(left, 12, Math.Max(200, Math.Min(360, width - 220)), 30);
            btnSearch.SetBounds(txtKeyword.Right + gap, 11, 104, 32);
            btnLoad.SetBounds(btnSearch.Right + gap, 11, 104, 32);

            var editorTop = Math.Max(112, ClientSize.Height - 168);
            dgvCustomers.SetBounds(left, 52, width, Math.Max(80, editorTop - 62));

            var idLabelWidth = 68;
            lblCustomerId.SetBounds(left, editorTop + 4, idLabelWidth, 28);
            txtCustomerId.SetBounds(left + idLabelWidth, editorTop, 72, 30);
            var nameX = left + idLabelWidth + 82;
            lblCustomerName.SetBounds(nameX, editorTop + 4, 58, 28);
            var nameInputWidth = Math.Max(180, width * 37 / 100);
            txtCustomerName.SetBounds(nameX + 62, editorTop, nameInputWidth, 30);
            var phoneX = txtCustomerName.Right + gap;
            lblPhoneNumber.SetBounds(phoneX, editorTop + 4, 46, 28);
            txtPhoneNumber.SetBounds(phoneX + 50, editorTop, Math.Max(110, ClientSize.Width - phoneX - 66), 30);

            var secondRow = editorTop + 42;
            lblAddress.SetBounds(left, secondRow + 4, idLabelWidth, 28);
            var rewardX = Math.Max(left + 360, width * 62 / 100);
            txtAddress.SetBounds(left + idLabelWidth, secondRow, Math.Max(160, rewardX - left - idLabelWidth - gap), 30);
            lblRewardPoints.SetBounds(rewardX, secondRow + 4, 52, 28);
            txtRewardPoints.SetBounds(rewardX + 56, secondRow, 100, 30);

            var thirdRow = editorTop + 82;
            lblMembershipRank.SetBounds(left, thirdRow + 4, 94, 28);
            txtMembershipRank.SetBounds(left + 98, thirdRow, 160, 30);
            var actionY = editorTop + 120;
            btnAdd.SetBounds(left, actionY, 112, 34);
            btnUpdate.SetBounds(btnAdd.Right + 10, actionY, 120, 34);
            btnDelete.SetBounds(btnUpdate.Right + 10, actionY, 112, 34);
        }

        private async void FormCustomerManagement_Load(object? sender, EventArgs e)
        {
            await LoadDataAsync();
        }

        private async Task LoadDataAsync(string? keyword = null)
        {
            try
            {
                string url = string.IsNullOrWhiteSpace(keyword)
                    ? "customers"
                    : $"customers/search?keyword={Uri.EscapeDataString(keyword.Trim())}";

                var customers = await ApiClientService.GetListAsync<CustomerDto>(url);
                if (customers != null) dgvCustomers.DataSource = customers;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi kết nối Server: " + ex.Message, "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnLoad_Click(object? sender, EventArgs e)
        {
            txtKeyword.Clear();
            await LoadDataAsync();
        }

        private async void btnSearch_Click(object? sender, EventArgs e)
        {
            await LoadDataAsync(txtKeyword.Text);
        }

        private void dgvCustomers_CellClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (dgvCustomers.Rows[e.RowIndex].DataBoundItem is CustomerDto c)
            {
                txtCustomerId.Text = c.CustomerId.ToString();
                txtCustomerName.Text = c.CustomerName;
                txtPhoneNumber.Text = c.PhoneNumber;
                txtAddress.Text = c.Address ?? string.Empty;
                txtRewardPoints.Text = c.RewardPoints.ToString();
                txtMembershipRank.Text = c.MembershipRank;
            }
        }

        // Đọc và kiểm tra dữ liệu nhập; trả về null nếu không hợp lệ
        private CustomerDto? ReadInputs()
        {
            if (string.IsNullOrWhiteSpace(txtCustomerName.Text))
            {
                MessageBox.Show("Nhập họ tên khách hàng!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return null;
            }
            if (string.IsNullOrWhiteSpace(txtPhoneNumber.Text))
            {
                MessageBox.Show("Nhập số điện thoại!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return null;
            }
            if (!int.TryParse(txtRewardPoints.Text, out int points) || points < 0)
            {
                MessageBox.Show("Điểm tích lũy phải là số nguyên không âm!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return null;
            }

            return new CustomerDto
            {
                CustomerName = txtCustomerName.Text.Trim(),
                PhoneNumber = txtPhoneNumber.Text.Trim(),
                Address = txtAddress.Text.Trim(),
                RewardPoints = points,
                MembershipRank = string.IsNullOrWhiteSpace(txtMembershipRank.Text) ? "Chuẩn" : txtMembershipRank.Text.Trim()
            };
        }

        private async void btnAdd_Click(object? sender, EventArgs e)
        {
            var dto = ReadInputs();
            if (dto == null) return;

            try
            {
                var res = await ApiClientService.Client.PostAsJsonAsync("customers", dto);
                if (res.IsSuccessStatusCode)
                {
                    MessageBox.Show("Thêm khách hàng thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
            if (!int.TryParse(txtCustomerId.Text, out int id))
            {
                MessageBox.Show("Vui lòng chọn khách hàng cần sửa!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            var dto = ReadInputs();
            if (dto == null) return;
            dto.CustomerId = id;

            try
            {
                var res = await ApiClientService.Client.PutAsJsonAsync($"customers/{id}", dto);
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
            if (!int.TryParse(txtCustomerId.Text, out int id))
            {
                MessageBox.Show("Vui lòng chọn khách hàng cần xóa!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var confirm = MessageBox.Show($"Bạn có chắc muốn xóa khách hàng ID = {id}?", "Xác nhận",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes) return;

            try
            {
                var res = await ApiClientService.Client.DeleteAsync($"customers/{id}");
                if (res.IsSuccessStatusCode)
                {
                    MessageBox.Show("Xóa thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearInputs();
                    await LoadDataAsync();
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
            txtCustomerId.Clear();
            txtCustomerName.Clear();
            txtPhoneNumber.Clear();
            txtAddress.Clear();
            txtRewardPoints.Text = "0";
            txtMembershipRank.Text = "Chuẩn";
        }
    }
}
