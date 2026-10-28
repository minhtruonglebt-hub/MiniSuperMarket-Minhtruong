using System.Net.Http.Json;

namespace MiniSuperMarket.WinForm
{
    public sealed class FormQuickReport : Form
    {
        private readonly DateTimePicker _date = new() { Format = DateTimePickerFormat.Custom, CustomFormat = "dd/MM/yyyy", Width = 150 };
        private readonly Label _orders = Metric("0", "Tổng số hóa đơn");
        private readonly Label _revenue = Metric("0 ₫", "Tổng doanh thu");
        private readonly Label _bestSeller = Metric("Chưa có dữ liệu", "Mặt hàng bán chạy nhất");

        public FormQuickReport()
        {
            Text = "Báo cáo nhanh"; BackColor = Color.FromArgb(244, 245, 247);
            var toolbar = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 66, Padding = new Padding(10, 14, 10, 8), WrapContents = false, BackColor = Color.White };
            toolbar.Controls.Add(new Label { Text = "Ngày báo cáo", AutoSize = true, TextAlign = ContentAlignment.MiddleCenter, Padding = new Padding(0, 8, 4, 0) });
            toolbar.Controls.Add(_date);
            var load = new Button { Text = "Xem doanh thu", Width = 145, Height = 34, BackColor = Color.FromArgb(41, 100, 180), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            load.Click += async (_, _) => await LoadReportAsync(); toolbar.Controls.Add(load);
            Controls.Add(toolbar);

            var cards = new TableLayoutPanel { Dock = DockStyle.Top, Height = 190, ColumnCount = 3, Padding = new Padding(8) };
            for (var i = 0; i < 3; i++) cards.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.333f));
            cards.Controls.Add(Card(_orders, Color.FromArgb(41, 100, 180)), 0, 0);
            cards.Controls.Add(Card(_revenue, Color.FromArgb(34, 139, 94)), 1, 0);
            cards.Controls.Add(Card(_bestSeller, Color.FromArgb(226, 142, 40)), 2, 0);
            Controls.Add(cards);
            Shown += async (_, _) => await LoadReportAsync();
        }

        private static Label Metric(string value, string caption) => new() { Text = $"{value}\n{caption}", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter, Font = new Font("Segoe UI", 15, FontStyle.Bold), ForeColor = Color.White };
        private static Panel Card(Label label, Color color) => new() { Dock = DockStyle.Fill, BackColor = color, Margin = new Padding(8), Padding = new Padding(12), Controls = { label } };

        private async Task LoadReportAsync()
        {
            try
            {
                using var response = await ApiClientService.Client.GetAsync($"reports/quick?date={_date.Value:yyyy-MM-dd}");
                if (!response.IsSuccessStatusCode) { MessageBox.Show(await ApiClientService.GetErrorMessageAsync(response), "Không tải được báo cáo", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
                var report = await response.Content.ReadFromJsonAsync<ReportDto>();
                if (report == null) return;
                _orders.Text = $"{report.TotalOrders:N0}\nTổng số hóa đơn";
                _revenue.Text = $"{report.TotalRevenue:N0} ₫\nTổng doanh thu";
                _bestSeller.Text = $"{report.BestSeller}\nBán chạy: {report.BestSellerQuantity:N0} sản phẩm";
            }
            catch (Exception ex) { MessageBox.Show("Lỗi kết nối API: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private sealed class ReportDto { public int TotalOrders { get; set; } public decimal TotalRevenue { get; set; } public string BestSeller { get; set; } = ""; public int BestSellerQuantity { get; set; } }
    }
}
