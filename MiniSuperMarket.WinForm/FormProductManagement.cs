using System.Net.Http.Json;

namespace MiniSuperMarket.WinForm
{
    public sealed class ProductViewDto
    {
        public int ProductId { get; set; }
        public string Barcode { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
    }

    public sealed class FormProductManagement : Form
    {
        private readonly DataGridView _grid = new();
        private readonly TextBox _barcode = new();
        private readonly TextBox _name = new();
        private readonly NumericUpDown _price = new() { DecimalPlaces = 2, Maximum = 1000000000, ThousandsSeparator = true };
        private readonly NumericUpDown _stock = new() { Maximum = 1000000000, ThousandsSeparator = true };
        private readonly ComboBox _category = new() { DropDownStyle = ComboBoxStyle.DropDownList };
        private readonly TextBox _search = new();
        private List<ProductViewDto> _products = new();
        private List<CategoryDto> _categories = new();
        private int _selectedId;

        public FormProductManagement()
        {
            Text = "Quản lý sản phẩm";
            BackColor = Color.White;
            BuildUi();
            Load += async (_, _) => await LoadDataAsync();
        }

        private void BuildUi()
        {
            var toolbar = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 56, Padding = new Padding(12, 10, 8, 4), WrapContents = false };
            _search.Width = 320; _search.PlaceholderText = "Tìm theo mã vạch hoặc tên sản phẩm";
            var search = Button("Tìm", Color.FromArgb(41, 100, 180)); search.Click += async (_, _) => await LoadDataAsync(_search.Text.Trim());
            var reload = Button("Tải lại", Color.FromArgb(84, 92, 110)); reload.Click += async (_, _) => { _search.Clear(); await LoadDataAsync(); };
            toolbar.Controls.AddRange([_search, search, reload]);
            Controls.Add(toolbar);

            _grid.Dock = DockStyle.Fill; _grid.ReadOnly = true; _grid.AllowUserToAddRows = false; _grid.AllowUserToDeleteRows = false;
            _grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill; _grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            _grid.MultiSelect = false; _grid.RowHeadersVisible = true; _grid.RowHeadersWidth = 54; _grid.BackgroundColor = Color.White;
            _grid.EnableHeadersVisualStyles = false;
            _grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(240, 243, 248);
            _grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(35, 43, 58);
            _grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            _grid.ColumnHeadersHeight = 38;
            _grid.RowTemplate.Height = 32;
            _grid.RowHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            _grid.RowPostPaint += (_, e) =>
            {
                var bounds = new Rectangle(e.RowBounds.Left, e.RowBounds.Top, _grid.RowHeadersWidth, e.RowBounds.Height);
                TextRenderer.DrawText(e.Graphics, (e.RowIndex + 1).ToString(), _grid.Font, bounds,
                    Color.FromArgb(90, 98, 112), TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
            };
            _grid.CellClick += OnRowClick;
            Controls.Add(_grid);

            var editor = new TableLayoutPanel { Dock = DockStyle.Bottom, Height = 184, ColumnCount = 4, RowCount = 4, Padding = new Padding(12), BackColor = Color.FromArgb(247, 248, 250) };
            editor.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110)); editor.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            editor.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110)); editor.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            editor.RowStyles.Add(new RowStyle(SizeType.Absolute, 38)); editor.RowStyles.Add(new RowStyle(SizeType.Absolute, 38)); editor.RowStyles.Add(new RowStyle(SizeType.Absolute, 38)); editor.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            AddField(editor, "Mã vạch", _barcode, 0, 0); AddField(editor, "Tên sản phẩm", _name, 2, 0);
            AddField(editor, "Giá bán", _price, 0, 1); AddField(editor, "Tồn kho", _stock, 2, 1); AddField(editor, "Nhóm hàng", _category, 0, 2, 3);
            var actions = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.RightToLeft, WrapContents = false, Padding = new Padding(0, 4, 0, 0) };
            var add = Button("Thêm sản phẩm", Color.FromArgb(34, 139, 94)); add.Click += async (_, _) => await SaveAsync(false);
            var update = Button("Cập nhật", Color.FromArgb(41, 100, 180)); update.Click += async (_, _) => await SaveAsync(true);
            var clear = Button("Xóa chọn", Color.FromArgb(84, 92, 110)); clear.Click += (_, _) => ClearInputs();
            actions.Controls.AddRange([add, update, clear]); editor.Controls.Add(actions, 0, 3); editor.SetColumnSpan(actions, 4);
            Controls.Add(editor);
        }

        private static void AddField(TableLayoutPanel panel, string label, Control input, int col, int row, int span = 1)
        {
            panel.Controls.Add(new Label { Text = label, Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft, Font = new Font("Segoe UI", 9, FontStyle.Bold) }, col, row);
            input.Dock = DockStyle.Fill; input.Margin = new Padding(3, 4, 12, 4); panel.Controls.Add(input, col + 1, row);
            if (span > 1) panel.SetColumnSpan(input, span);
        }

        private static Button Button(string text, Color color) => new() { Text = text, BackColor = color, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Height = 34, Width = 130, Margin = new Padding(4), Cursor = Cursors.Hand };

        private async Task LoadDataAsync(string? keyword = null)
        {
            try
            {
                _categories = await ApiClientService.GetListAsync<CategoryDto>("categories") ?? new();
                _category.DataSource = null; _category.DataSource = _categories; _category.DisplayMember = nameof(CategoryDto.CategoryName); _category.ValueMember = nameof(CategoryDto.CategoryId);
                _products = await ApiClientService.GetListAsync<ProductViewDto>("products/with-category") ?? new();
                if (!string.IsNullOrWhiteSpace(keyword))
                    _products = _products.Where(x => x.ProductName.Contains(keyword, StringComparison.CurrentCultureIgnoreCase) || x.Barcode.Contains(keyword, StringComparison.OrdinalIgnoreCase)).ToList();
                _grid.DataSource = null; _grid.DataSource = _products;
                if (_grid.Columns[nameof(ProductViewDto.ProductId)] != null) _grid.Columns[nameof(ProductViewDto.ProductId)]!.HeaderText = "Mã SP";
                if (_grid.Columns[nameof(ProductViewDto.Barcode)] != null) _grid.Columns[nameof(ProductViewDto.Barcode)]!.HeaderText = "Mã vạch";
                if (_grid.Columns[nameof(ProductViewDto.ProductName)] != null) _grid.Columns[nameof(ProductViewDto.ProductName)]!.HeaderText = "Tên sản phẩm";
                if (_grid.Columns[nameof(ProductViewDto.Price)] != null) { _grid.Columns[nameof(ProductViewDto.Price)]!.HeaderText = "Đơn giá"; _grid.Columns[nameof(ProductViewDto.Price)]!.DefaultCellStyle.Format = "N0"; }
                if (_grid.Columns[nameof(ProductViewDto.StockQuantity)] != null) _grid.Columns[nameof(ProductViewDto.StockQuantity)]!.HeaderText = "Tồn kho";
                if (_grid.Columns[nameof(ProductViewDto.CategoryName)] != null) _grid.Columns[nameof(ProductViewDto.CategoryName)]!.HeaderText = "Nhóm hàng";
                if (_grid.Columns[nameof(ProductViewDto.CategoryId)] != null) _grid.Columns[nameof(ProductViewDto.CategoryId)]!.Visible = false;
                SetColumn(nameof(ProductViewDto.ProductId), "Mã SP", 65, 8);
                SetColumn(nameof(ProductViewDto.Barcode), "Mã vạch", 145, 17);
                SetColumn(nameof(ProductViewDto.ProductName), "Tên sản phẩm", 210, 27);
                SetColumn(nameof(ProductViewDto.Price), "Đơn giá", 115, 14);
                SetColumn(nameof(ProductViewDto.StockQuantity), "Tồn kho", 95, 11);
                SetColumn(nameof(ProductViewDto.CategoryName), "Nhóm hàng", 190, 23);
            }
            catch (Exception ex) { MessageBox.Show("Không tải được sản phẩm: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void SetColumn(string name, string header, int minimumWidth, float fillWeight)
        {
            var column = _grid.Columns[name];
            if (column == null) return;
            column.HeaderText = header;
            column.MinimumWidth = minimumWidth;
            column.FillWeight = fillWeight;
            column.DefaultCellStyle.Alignment = name is nameof(ProductViewDto.Price) or nameof(ProductViewDto.StockQuantity)
                ? DataGridViewContentAlignment.MiddleRight
                : DataGridViewContentAlignment.MiddleLeft;
            column.HeaderCell.Style.Alignment = name is nameof(ProductViewDto.Price) or nameof(ProductViewDto.StockQuantity)
                ? DataGridViewContentAlignment.MiddleRight
                : DataGridViewContentAlignment.MiddleLeft;
            column.DefaultCellStyle.Padding = new Padding(6, 0, 6, 0);
        }

        private void OnRowClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || _grid.Rows[e.RowIndex].DataBoundItem is not ProductViewDto item) return;
            _selectedId = item.ProductId; _barcode.Text = item.Barcode; _name.Text = item.ProductName;
            _price.Value = Math.Min(_price.Maximum, item.Price); _stock.Value = item.StockQuantity;
            _category.SelectedValue = item.CategoryId;
        }

        private async Task SaveAsync(bool update)
        {
            if (string.IsNullOrWhiteSpace(_barcode.Text) || string.IsNullOrWhiteSpace(_name.Text) || _category.SelectedValue is not int categoryId)
            { MessageBox.Show("Nhập mã vạch, tên sản phẩm và nhóm hàng.", "Thiếu dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            if (update && _selectedId == 0) { MessageBox.Show("Chọn sản phẩm cần cập nhật.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            var payload = new { Barcode = _barcode.Text.Trim(), ProductName = _name.Text.Trim(), Price = _price.Value, StockQuantity = (int)_stock.Value, CategoryId = categoryId };
            try
            {
                using var response = update ? await ApiClientService.Client.PutAsJsonAsync($"products/{_selectedId}", payload) : await ApiClientService.Client.PostAsJsonAsync("products", payload);
                if (!response.IsSuccessStatusCode) { MessageBox.Show(await ApiClientService.GetErrorMessageAsync(response), "Không lưu được", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
                ClearInputs(); await LoadDataAsync(_search.Text.Trim());
            }
            catch (Exception ex) { MessageBox.Show("Lỗi kết nối API: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void ClearInputs() { _selectedId=0; _barcode.Clear(); _name.Clear(); _price.Value=0; _stock.Value=0; if (_category.Items.Count>0) _category.SelectedIndex=0; _grid.ClearSelection(); }
    }
}
