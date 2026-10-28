using System.Drawing;

namespace MiniSuperMarket.WinForm
{
    public sealed class FormMainShell : Form
    {
        private readonly Panel _sidebar = new();
        private readonly Panel _workspace = new();
        private readonly Label _title = new();
        private readonly Label _userInfo = new();
        private readonly Dictionary<string, Button> _menu = new();
        private Form? _activeForm;

        public FormMainShell()
        {
            Text = "Hệ thống Quản lý Bán lẻ & Tồn kho Siêu thị Mini";
            StartPosition = FormStartPosition.CenterScreen;
            MinimumSize = new Size(1000, 650);
            Size = new Size(1280, 760);
            BackColor = Color.FromArgb(244, 245, 247);
            BuildLayout();
            Load += OnShellLoad;
        }

        private void BuildLayout()
        {
            // Use a real two-column layout so the fixed navigation bar never
            // covers the left edge of the content when child forms are embedded.
            var shell = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                Margin = Padding.Empty,
                Padding = Padding.Empty,
                BackColor = BackColor
            };
            shell.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 230));
            shell.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            shell.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            Controls.Add(shell);

            _sidebar.Dock = DockStyle.Left;
            _sidebar.Width = 230;
            _sidebar.BackColor = Color.FromArgb(24, 30, 48);
            shell.Controls.Add(_sidebar, 0, 0);

            var logo = new Label { Text = "🛒 MiniMart POS", ForeColor = Color.White, Font = new Font("Segoe UI", 15, FontStyle.Bold), Dock = DockStyle.Top, Height = 76, TextAlign = ContentAlignment.MiddleCenter };
            _sidebar.Controls.Add(logo);

            var footer = new Panel { Dock = DockStyle.Bottom, Height = 62, Padding = new Padding(12, 8, 12, 8), BackColor = Color.FromArgb(19, 24, 39) };
            var logout = MakeMenuButton("🚪  Đăng xuất");
            logout.Dock = DockStyle.Fill;
            logout.Click += (_, _) => Logout();
            footer.Controls.Add(logout);
            _sidebar.Controls.Add(footer);

            AddMenu("POS", "🛒  Bán hàng (POS)", (_, b) => Open(new FormPOS(), "BÁN HÀNG TẠI QUẦY", b));
            AddMenu("Category", "📁  Quản lý Danh mục", (_, b) => Open(new FormCategoryManagement(), "QUẢN LÝ DANH MỤC", b));
            AddMenu("Product", "📦  Quản lý Sản phẩm", (_, b) => Open(new FormProductManagement(), "QUẢN LÝ SẢN PHẨM & KHO", b));
            AddMenu("Customer", "👥  Quản lý Khách hàng", (_, b) => Open(new FormCustomerManagement(), "KHÁCH HÀNG THÀNH VIÊN", b));
            AddMenu("Reports", "📊  Báo cáo Doanh thu", (_, b) => OpenReport(b));
            AddMenu("UserManage", "🛡  Quản trị Tài khoản", (_, b) => Open(new FormUserManagement(), "QUẢN TRỊ TÀI KHOẢN", b));

            var header = new Panel { Dock = DockStyle.Top, Height = 62, BackColor = Color.White, Padding = new Padding(24, 0, 24, 0) };
            _title.Text = "BÀN LÀM VIỆC HỆ THỐNG";
            _title.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            _title.Dock = DockStyle.Left;
            _title.AutoSize = false;
            _title.Width = 560;
            _title.TextAlign = ContentAlignment.MiddleLeft;
            _userInfo.Font = new Font("Segoe UI", 9.5f);
            _userInfo.ForeColor = Color.FromArgb(72, 79, 95);
            _userInfo.Dock = DockStyle.Right;
            _userInfo.Width = 380;
            _userInfo.TextAlign = ContentAlignment.MiddleRight;
            header.Controls.Add(_userInfo);
            header.Controls.Add(_title);
            var content = new Panel { Dock = DockStyle.Fill, BackColor = Color.FromArgb(244, 245, 247) };
            _workspace.Dock = DockStyle.Fill;
            _workspace.Padding = new Padding(18);
            _workspace.BackColor = Color.FromArgb(244, 245, 247);
            content.Controls.Add(_workspace);
            content.Controls.Add(header);
            shell.Controls.Add(content, 1, 0);
        }

        private void AddMenu(string key, string text, Action<object?, Button> action)
        {
            var button = MakeMenuButton(text);
            button.Click += (sender, _) => action(sender, button);
            _menu[key] = button;
            _sidebar.Controls.Add(button);
        }

        private static Button MakeMenuButton(string text) => new()
        {
            Text = text, Height = 48, Dock = DockStyle.Top, FlatStyle = FlatStyle.Flat,
            FlatAppearance = { BorderSize = 0 }, ForeColor = Color.White,
            BackColor = Color.FromArgb(24, 30, 48), TextAlign = ContentAlignment.MiddleLeft,
            Padding = new Padding(18, 0, 8, 0), Font = new Font("Segoe UI", 9.5f), Cursor = Cursors.Hand
        };

        private void OnShellLoad(object? sender, EventArgs e)
        {
            _userInfo.Text = $"Nhân viên: {SessionManager.CurrentFullName}  |  {SessionManager.CurrentRole}";
            foreach (var button in _menu.Values) button.Visible = false;
            switch (SessionManager.CurrentRole)
            {
                case "Admin":
                    Show("POS", "Category", "Product", "Customer", "Reports", "UserManage");
                    Open(new FormCategoryManagement(), "QUẢN LÝ DANH MỤC", _menu["Category"]);
                    break;
                case "Cashier":
                    Show("POS", "Customer");
                    Open(new FormPOS(), "BÁN HÀNG TẠI QUẦY", _menu["POS"]);
                    break;
                case "Warehouse":
                    Show("Category", "Product");
                    Open(new FormProductManagement(), "QUẢN LÝ SẢN PHẨM & KHO", _menu["Product"]);
                    break;
                default:
                    MessageBox.Show("Tài khoản chưa được cấp quyền hợp lệ.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    Close();
                    break;
            }
        }

        private void Show(params string[] keys) { foreach (var key in keys) _menu[key].Visible = true; }

        private void OpenReport(Button button)
        {
            if (!string.Equals(SessionManager.CurrentRole, "Admin", StringComparison.OrdinalIgnoreCase))
            {
                MessageBox.Show("Bạn không có quyền xem dữ liệu tài chính của siêu thị!", "Từ chối", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }
            Open(new FormQuickReport(), "BÁO CÁO DOANH THU & HIỆU SUẤT", button);
        }

        private void Open(Form child, string screenTitle, Button activeButton)
        {
            if (!IsAllowed(child))
            {
                child.Dispose();
                MessageBox.Show("Vai trò hiện tại không được phép mở chức năng này.", "Từ chối", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }
            _activeForm?.Close();
            _activeForm?.Dispose();
            _activeForm = child;
            foreach (var button in _menu.Values) button.BackColor = Color.FromArgb(24, 30, 48);
            activeButton.BackColor = Color.FromArgb(41, 100, 180);
            _title.Text = screenTitle;

            // Bỏ các nút điều hướng đời cũ khi các màn hình Buổi 3 được nhúng vào shell.
            foreach (var name in new[] { "btnCustomers", "btnRoles" })
                foreach (var control in child.Controls.Find(name, true)) control.Visible = false;
            if (child is FormCategoryManagement categoryForm)
            {
                var delete = categoryForm.Controls.Find("btnDelete", true).FirstOrDefault();
                if (delete != null) delete.Visible = SessionManager.CurrentRole == "Admin";
            }

            child.TopLevel = false;
            child.FormBorderStyle = FormBorderStyle.None;
            child.Dock = DockStyle.Fill;
            _workspace.Controls.Clear();
            _workspace.Controls.Add(child);
            child.Show();
            child.BringToFront();
        }

        private bool IsAllowed(Form child) => SessionManager.CurrentRole switch
        {
            "Admin" => true,
            "Cashier" => child is FormPOS or FormCustomerManagement,
            "Warehouse" => child is FormProductManagement or FormCategoryManagement,
            _ => false
        };

        private void Logout()
        {
            if (MessageBox.Show("Bạn có chắc chắn muốn đăng xuất phiên làm việc?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
            SessionManager.Clear();
            Close();
        }
    }
}
