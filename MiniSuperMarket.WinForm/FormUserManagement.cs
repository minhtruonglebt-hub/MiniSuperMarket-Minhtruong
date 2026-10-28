using System.Net.Http.Json;

namespace MiniSuperMarket.WinForm
{
    public sealed class FormUserManagement : Form
    {
        private readonly DataGridView _grid = new();
        private readonly TextBox _username = new();
        private readonly TextBox _fullName = new();
        private readonly TextBox _password = new() { UseSystemPasswordChar = true };
        private readonly ComboBox _role = new() { DropDownStyle = ComboBoxStyle.DropDownList, DataSource = new[] { "Admin", "Cashier", "Warehouse" } };
        private List<UserDto> _users = new();
        private int _selectedId;

        public FormUserManagement()
        {
            Text = "Quản trị tài khoản"; BackColor = Color.White;
            BuildUi();
            Load += async (_, _) => await LoadUsersAsync();
        }

        private void BuildUi()
        {
            _grid.Dock = DockStyle.Fill; _grid.ReadOnly = true; _grid.AllowUserToAddRows = false; _grid.AllowUserToDeleteRows = false;
            _grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill; _grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            _grid.MultiSelect = false; _grid.RowHeadersVisible = false; _grid.CellClick += (_, e) =>
            {
                if (e.RowIndex < 0 || _grid.Rows[e.RowIndex].DataBoundItem is not UserDto u) return;
                _selectedId = u.UserId; _username.Text = u.Username; _fullName.Text = u.FullName; _role.SelectedItem = u.Role;
            };
            Controls.Add(_grid);

            var editor = new TableLayoutPanel { Dock = DockStyle.Bottom, Height = 185, ColumnCount = 4, RowCount = 3, Padding = new Padding(14), BackColor = Color.FromArgb(247, 248, 250) };
            editor.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 105)); editor.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50)); editor.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 105)); editor.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            for (var i=0;i<3;i++) editor.RowStyles.Add(new RowStyle(SizeType.Absolute, 42));
            AddField(editor,"Tài khoản",_username,0,0); AddField(editor,"Họ tên",_fullName,2,0); AddField(editor,"Mật khẩu mới",_password,0,1); AddField(editor,"Vai trò",_role,2,1);
            var actions = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.RightToLeft };
            var add = Button("Tạo tài khoản"); add.Click += async (_, _) => await CreateAsync();
            var toggle = Button("Khóa / Mở khóa", Color.FromArgb(84,92,110)); toggle.Click += async (_, _) => await ToggleStatusAsync();
            var reload = Button("Làm mới", Color.FromArgb(41,100,180)); reload.Click += async (_, _) => await LoadUsersAsync();
            actions.Controls.AddRange([add,toggle,reload]); editor.Controls.Add(actions,3,2); Controls.Add(editor);
        }

        private static Button Button(string text, Color? color = null) => new() { Text=text, Width=145, Height=34, BackColor=color ?? Color.FromArgb(34,139,94), ForeColor=Color.White, FlatStyle=FlatStyle.Flat, Margin=new Padding(4) };
        private static void AddField(TableLayoutPanel panel,string label,Control control,int col,int row)
        {
            panel.Controls.Add(new Label { Text=label, Dock=DockStyle.Fill, TextAlign=ContentAlignment.MiddleLeft, Font=new Font("Segoe UI",9,FontStyle.Bold) },col,row);
            control.Dock=DockStyle.Fill; control.Margin=new Padding(3,4,12,4); panel.Controls.Add(control,col+1,row);
        }

        private async Task LoadUsersAsync()
        {
            _users = await ApiClientService.GetListAsync<UserDto>("users") ?? new();
            _grid.DataSource = null; _grid.DataSource = _users;
            foreach (DataGridViewColumn col in _grid.Columns) col.HeaderText = col.Name switch { "UserId"=>"Mã", "Username"=>"Tài khoản", "FullName"=>"Họ tên", "Role"=>"Vai trò", "IsActive"=>"Đang hoạt động", _=>col.HeaderText };
            _grid.ClearSelection();
        }

        private async Task CreateAsync()
        {
            if (string.IsNullOrWhiteSpace(_username.Text) || string.IsNullOrWhiteSpace(_fullName.Text) || _password.Text.Length < 6)
            { MessageBox.Show("Nhập tài khoản, họ tên và mật khẩu tối thiểu 6 ký tự.","Thiếu dữ liệu",MessageBoxButtons.OK,MessageBoxIcon.Warning); return; }
            try
            {
                using var response = await ApiClientService.Client.PostAsJsonAsync("users", new { Username=_username.Text.Trim(), FullName=_fullName.Text.Trim(), Role=_role.Text, Password=_password.Text });
                if (!response.IsSuccessStatusCode) { MessageBox.Show(await ApiClientService.GetErrorMessageAsync(response),"Không tạo được",MessageBoxButtons.OK,MessageBoxIcon.Warning); return; }
                _username.Clear(); _fullName.Clear(); _password.Clear(); await LoadUsersAsync();
            }
            catch (Exception ex) { MessageBox.Show("Lỗi kết nối API: "+ex.Message,"Lỗi",MessageBoxButtons.OK,MessageBoxIcon.Error); }
        }

        private async Task ToggleStatusAsync()
        {
            var selected = _users.FirstOrDefault(x=>x.UserId==_selectedId);
            if (selected == null) { MessageBox.Show("Chọn tài khoản cần đổi trạng thái.","Cảnh báo",MessageBoxButtons.OK,MessageBoxIcon.Warning); return; }
            try
            {
                using var response = await ApiClientService.Client.PutAsJsonAsync($"users/{selected.UserId}/status", new { IsActive=!selected.IsActive });
                if (!response.IsSuccessStatusCode) { MessageBox.Show(await ApiClientService.GetErrorMessageAsync(response),"Không cập nhật được",MessageBoxButtons.OK,MessageBoxIcon.Warning); return; }
                _selectedId=0; await LoadUsersAsync();
            }
            catch (Exception ex) { MessageBox.Show("Lỗi kết nối API: "+ex.Message,"Lỗi",MessageBoxButtons.OK,MessageBoxIcon.Error); }
        }

        private sealed class UserDto { public int UserId {get;set;} public string Username {get;set;}=""; public string FullName {get;set;}=""; public string Role {get;set;}=""; public bool IsActive {get;set;} }
    }
}
