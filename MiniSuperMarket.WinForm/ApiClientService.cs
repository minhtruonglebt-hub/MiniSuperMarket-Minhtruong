using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace MiniSuperMarket.WinForm
{
    // Dịch vụ dùng chung: một HttpClient duy nhất, tự gắn Bearer Token ở mỗi lần gọi
    public static class ApiClientService
    {
        private static readonly HttpClient _client = new()
        {
            BaseAddress = new Uri(SessionManager.ApiBaseUrl)
        };

        public static HttpClient Client
        {
            get
            {
                _client.DefaultRequestHeaders.Authorization =
                    string.IsNullOrEmpty(SessionManager.JwtToken)
                        ? null
                        : new AuthenticationHeaderValue("Bearer", SessionManager.JwtToken);
                return _client;
            }
        }

        // Gọi GET và đọc danh sách JSON; nếu lỗi thì hiện thông báo và trả về null
        public static async Task<List<T>?> GetListAsync<T>(string url)
        {
            var res = await Client.GetAsync(url);
            if (!res.IsSuccessStatusCode)
            {
                MessageBox.Show(await GetErrorMessageAsync(res), "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return null;
            }
            return await res.Content.ReadFromJsonAsync<List<T>>();
        }

        // Chuyển phản hồi lỗi của API thành thông báo dễ hiểu
        public static async Task<string> GetErrorMessageAsync(HttpResponseMessage res)
        {
            if (res.StatusCode == HttpStatusCode.Unauthorized)
                return "401: Phiên làm việc hết hạn hoặc chưa đăng nhập!";
            if (res.StatusCode == HttpStatusCode.Forbidden)
                return "403 Forbidden: Tài khoản của bạn không đủ quyền thực hiện thao tác này!";

            try
            {
                var body = await res.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(body);
                if (doc.RootElement.ValueKind == JsonValueKind.Object)
                {
                    // Dạng { "message": "..." }
                    if (doc.RootElement.TryGetProperty("message", out var msg) &&
                        msg.ValueKind == JsonValueKind.String)
                        return msg.GetString() ?? string.Empty;

                    // Dạng lỗi ModelState: { "errors": { "Field": ["..."] } }
                    if (doc.RootElement.TryGetProperty("errors", out var errs) &&
                        errs.ValueKind == JsonValueKind.Object)
                    {
                        var lines = new List<string>();
                        foreach (var prop in errs.EnumerateObject())
                        {
                            if (prop.Value.ValueKind != JsonValueKind.Array) continue;
                            foreach (var item in prop.Value.EnumerateArray())
                                lines.Add(item.GetString() ?? string.Empty);
                        }
                        if (lines.Count > 0) return string.Join(Environment.NewLine, lines);
                    }
                }
            }
            catch
            {
                // Nội dung không phải JSON: dùng thông báo mặc định bên dưới
            }
            return $"{(int)res.StatusCode} {res.ReasonPhrase}";
        }
    }
}
