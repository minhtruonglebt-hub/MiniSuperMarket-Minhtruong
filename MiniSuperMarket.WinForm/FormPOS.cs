using System.Net.Http.Json;

namespace MiniSuperMarket.WinForm
{
    public sealed class FormPOS : Form
    {
        private readonly TextBox _barcode = new();
        private readonly DataGridView _grid = new();
        private readonly TextBox _phone = new();
        private readonly Label _customerName = new();
        private readonly TextBox _cash = new();
        private readonly Label _total = new();
        private readonly Label _change = new();
        private readonly Label _points = new();
        private readonly List<CartLine> _cart = new();
        private CustomerDto? _customer;
        private decimal _totalAmount;

        public FormPOS()
        {
            Text = "Bán hàng POS"; BackColor = Color.FromArgb(244, 245, 247); KeyPreview = true;
            BuildUi();
            KeyDown += async (_, e) => { if (e.KeyCode == Keys.F9) { e.Handled = true; await CheckoutAsync(); } };
            Shown += (_, _) => _barcode.Focus();
        }

        private void BuildUi()
        {
            var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 1, Padding = new Padding(10) };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 66)); layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 34));
            Controls.Add(layout);

            var left = new Panel { Dock = DockStyle.Fill, BackColor = Color.White, Padding = new Padding(10) };
            var scan = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 52, WrapContents = false, Padding = new Padding(0, 5, 0, 0) };
            scan.Controls.Add(new Label { Text = "Mã vạch", AutoSize = true, Padding = new Padding(0, 8, 8, 0), Font = new Font("Segoe UI", 9, FontStyle.Bold) });
            _barcode.Width = 360; _barcode.PlaceholderText = "Quét mã rồi nhấn Enter"; _barcode.Font = new Font("Segoe UI", 11);
            _barcode.KeyDown += async (_, e) => { if (e.KeyCode == Keys.Enter) { e.SuppressKeyPress = true; await AddBarcodeAsync(); } };
            scan.Controls.Add(_barcode);
            var add = Button("Thêm vào giỏ", Color.FromArgb(41, 100, 180)); add.Click += async (_, _) => await AddBarcodeAsync(); scan.Controls.Add(add);
            left.Controls.Add(scan);

            _grid.Dock = DockStyle.Fill; _grid.AllowUserToAddRows = false; _grid.AllowUserToDeleteRows = false; _grid.RowHeadersVisible = false;
            _grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill; _grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect; _grid.BackgroundColor = Color.White;
            _grid.Columns.Add("ProductId", "Mã SP"); _grid.Columns.Add("ProductName", "Tên sản phẩm"); _grid.Columns.Add("UnitPrice", "Đơn giá"); _grid.Columns.Add("Quantity", "SL"); _grid.Columns.Add("LineTotal", "Thành tiền");
            _grid.Columns["ProductId"]!.FillWeight = 40; _grid.Columns["UnitPrice"]!.FillWeight = 55; _grid.Columns["Quantity"]!.FillWeight = 35; _grid.Columns["LineTotal"]!.FillWeight = 60;
            left.Controls.Add(_grid);
            layout.Controls.Add(left, 0, 0);

            var right = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 13, Padding = new Padding(18), BackColor = Color.White };
            for (var i=0; i<13; i++) right.RowStyles.Add(new RowStyle(SizeType.Absolute, i is 2 or 5 or 7 or 9 or 11 ? 48 : 34));
            right.Controls.Add(Heading("Khách hàng thành viên"), 0, 0);
            _phone.PlaceholderText = "Số điện thoại khách hàng"; right.Controls.Add(_phone, 0, 1);
            var lookup = Button("Tra cứu khách hàng", Color.FromArgb(84, 92, 110)); lookup.Click += async (_, _) => await LookupCustomerAsync(); right.Controls.Add(lookup, 0, 2);
            _customerName.Text = "Khách lẻ"; _customerName.Dock = DockStyle.Fill; _customerName.TextAlign = ContentAlignment.MiddleLeft; _customerName.Font = new Font("Segoe UI", 9, FontStyle.Bold); right.Controls.Add(_customerName, 0, 3);
            _points.Text = "Điểm thưởng sẽ cộng sau thanh toán"; _points.Dock = DockStyle.Fill; _points.ForeColor = Color.Gray; right.Controls.Add(_points, 0, 4);
            right.Controls.Add(Heading("Tổng thanh toán"), 0, 5);
            _total.Text = "0 ₫"; _total.Dock = DockStyle.Fill; _total.TextAlign = ContentAlignment.MiddleLeft; _total.Font = new Font("Segoe UI", 23, FontStyle.Bold); _total.ForeColor = Color.FromArgb(34, 139, 94); right.Controls.Add(_total, 0, 6);
            right.Controls.Add(new Label { Text = "Tiền khách đưa", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft }, 0, 7);
            _cash.Dock = DockStyle.Fill; _cash.Font = new Font("Segoe UI", 12); _cash.TextChanged += (_, _) => UpdateChange(); right.Controls.Add(_cash, 0, 8);
            _change.Text = "Tiền thừa: 0 ₫"; _change.Dock = DockStyle.Fill; _change.TextAlign = ContentAlignment.MiddleLeft; _change.Font = new Font("Segoe UI", 12, FontStyle.Bold); right.Controls.Add(_change, 0, 9);
            var checkout = Button("THANH TOÁN  (F9)", Color.FromArgb(24, 120, 76)); checkout.Dock = DockStyle.Fill; checkout.Click += async (_, _) => await CheckoutAsync(); right.Controls.Add(checkout, 0, 10);
            var clear = Button("HỦY GIỎ HÀNG", Color.FromArgb(183, 55, 55)); clear.Dock = DockStyle.Fill; clear.Click += (_, _) => ClearCart(true); right.Controls.Add(clear, 0, 11);
            right.Controls.Add(new Label { Text = "Giá bán và tồn kho được xác nhận lại từ máy chủ khi thanh toán.", Dock = DockStyle.Fill, ForeColor = Color.Gray, AutoSize = false }, 0, 12);
            layout.Controls.Add(right, 1, 0);
        }

        private static Label Heading(string text) => new() { Text = text, Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft, Font = new Font("Segoe UI", 11, FontStyle.Bold) };
        private static Button Button(string text, Color color) => new() { Text = text, Width = 145, Height = 36, BackColor = color, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand, Margin = new Padding(4) };

        private async Task AddBarcodeAsync()
        {
            var barcode = _barcode.Text.Trim();
            if (barcode.Length == 0) return;
            _barcode.Clear();
            try
            {
                using var response = await ApiClientService.Client.GetAsync($"ado-products/barcode/{Uri.EscapeDataString(barcode)}");
                if (!response.IsSuccessStatusCode) { MessageBox.Show(await ApiClientService.GetErrorMessageAsync(response), "Không tìm thấy", MessageBoxButtons.OK, MessageBoxIcon.Information); _barcode.Focus(); return; }
                var product = await response.Content.ReadFromJsonAsync<ProductViewDto>();
                if (product == null) return;
                var line = _cart.FirstOrDefault(x => x.Product.ProductId == product.ProductId);
                if (line == null) _cart.Add(new CartLine(product));
                else if (line.Quantity < product.StockQuantity) line.Quantity++;
                else { MessageBox.Show("Số lượng trong giỏ đã bằng tồn kho hiện tại.", "Hết hàng", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
                RefreshCart();
            }
            catch (Exception ex) { MessageBox.Show("Lỗi kết nối API: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            _barcode.Focus();
        }

        private async Task LookupCustomerAsync()
        {
            var phone = _phone.Text.Trim();
            if (phone.Length == 0) { _customer = null; _customerName.Text = "Khách lẻ"; return; }
            try
            {
                var matches = await ApiClientService.GetListAsync<CustomerDto>($"customers/search?keyword={Uri.EscapeDataString(phone)}");
                _customer = matches?.FirstOrDefault(x => x.PhoneNumber == phone);
                _customerName.Text = _customer == null ? "Không tìm thấy - thanh toán khách lẻ" : $"{_customer.CustomerName}  |  Hạng {_customer.MembershipRank}";
                if (_customer == null) _customerName.ForeColor = Color.DarkOrange; else _customerName.ForeColor = Color.FromArgb(34, 139, 94);
            }
            catch (Exception ex) { MessageBox.Show("Không tra được khách hàng: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void RefreshCart()
        {
            _grid.Rows.Clear(); _totalAmount = 0;
            foreach (var line in _cart)
            {
                var amount = line.Product.Price * line.Quantity; _totalAmount += amount;
                _grid.Rows.Add(line.Product.ProductId, line.Product.ProductName, line.Product.Price.ToString("N0"), line.Quantity, amount.ToString("N0"));
            }
            _total.Text = $"{_totalAmount:N0} ₫"; UpdateChange();
        }

        private void UpdateChange()
        {
            var received = decimal.TryParse(_cash.Text, out var value) ? value : 0;
            _change.Text = $"Tiền thừa: {Math.Max(0, received - _totalAmount):N0} ₫";
        }

        private async Task CheckoutAsync()
        {
            if (_cart.Count == 0) { MessageBox.Show("Giỏ hàng đang trống.", "Chưa thể thanh toán", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            if (!decimal.TryParse(_cash.Text, out var received) || received < _totalAmount)
            { MessageBox.Show("Tiền khách đưa chưa đủ hoặc không hợp lệ.", "Kiểm tra thanh toán", MessageBoxButtons.OK, MessageBoxIcon.Warning); _cash.Focus(); return; }
            if (MessageBox.Show($"Xác nhận thanh toán {_totalAmount:N0} ₫?", "Thanh toán", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
            var payload = new { CustomerId = _customer?.CustomerId, Items = _cart.Select(x => new { ProductId = x.Product.ProductId, Quantity = x.Quantity }).ToList() };
            try
            {
                using var response = await ApiClientService.Client.PostAsJsonAsync("checkout", payload);
                if (!response.IsSuccessStatusCode) { MessageBox.Show(await ApiClientService.GetErrorMessageAsync(response), "Thanh toán thất bại", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
                var result = await response.Content.ReadFromJsonAsync<CheckoutResult>();
                MessageBox.Show($"Thanh toán thành công.\nHóa đơn: {result?.OrderCode}\nTổng tiền: {result?.FinalAmount:N0} ₫\nĐiểm cộng: {result?.RewardPoints}", "Hoàn tất", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearCart(false); _cash.Clear(); _customer = null; _phone.Clear(); _customerName.Text = "Khách lẻ";
            }
            catch (Exception ex) { MessageBox.Show("Lỗi kết nối API: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void ClearCart(bool ask)
        {
            if (ask && _cart.Count > 0 && MessageBox.Show("Hủy toàn bộ giỏ hàng hiện tại?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
            _cart.Clear(); RefreshCart();
        }

        private sealed class CartLine(ProductViewDto product) { public ProductViewDto Product { get; } = product; public int Quantity { get; set; } = 1; }
        private sealed class CheckoutResult { public string OrderCode { get; set; } = ""; public decimal FinalAmount { get; set; } public int RewardPoints { get; set; } }
    }
}
