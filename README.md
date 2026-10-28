# Hệ thống quản lý bán hàng và tồn kho Thiên Ân

Đề tài môn Lập trình Ứng dụng .NET Core: xây dựng hệ thống quản lý bán hàng cho Cửa hàng Bách hóa Tổng hợp Thiên Ân bằng ASP.NET Core Web API, Windows Forms và Entity Framework Core.

## Thành phần

- `MiniSuperMarket.API`: API .NET 8, EF Core, SQL Server, JWT và Swagger.
- `MiniSuperMarket.WinForm`: ứng dụng WinForms .NET 8 theo kiến trúc một cửa sổ, có sidebar và vùng hiển thị màn hình nghiệp vụ.
- `MiniSuperMarket.API/Data/Migrations`: migration tạo sáu bảng cùng dữ liệu mẫu.

## Chức năng Buổi 4

- Đăng nhập nhân viên từ bảng `Users`; mật khẩu demo được lưu bằng ASP.NET Core Identity PasswordHasher.
- Khung `FormMainShell` nhúng các màn hình con và áp dụng quyền theo vai trò.
- Admin: danh mục, sản phẩm, khách hàng, POS, báo cáo doanh thu và quản lý tài khoản.
- Cashier: POS và khách hàng.
- Warehouse: danh mục và sản phẩm/tồn kho.
- POS tra cứu barcode, nhận diện khách hàng, xác nhận tiền khách đưa, ghi hóa đơn, giảm tồn kho và cộng điểm thưởng.
- Báo cáo doanh thu chỉ dành cho Admin; API cũng chặn truy cập sai vai trò.

## Tài khoản mẫu

Tất cả tài khoản seed trong Buổi 4 dùng mật khẩu `123456`.

| Tài khoản | Vai trò |
|---|---|
| `admin01` | Admin |
| `cashier01` | Cashier |
| `ware01` | Warehouse |

Migration tạo 15 tài khoản, 15 nhóm hàng, 20 sản phẩm, 15 khách hàng, 15 hóa đơn và 30 dòng chi tiết hóa đơn.

## Chức năng Buổi 5

- `AdoProductController` minh họa `ExecuteScalar`, `ExecuteNonQuery`, `SqlDataReader`, `SqlParameter`, `SqlDataAdapter` và `DataTable`.
- API ADO.NET tra cứu barcode, xem snapshot disconnected, lọc hàng tồn thấp, đếm sản phẩm theo nhóm và cập nhật tồn kho.
- `ReportsController` chạy `sp_GetRevenueByCashier`, báo cáo theo khoảng ngày và top 5 sản phẩm bán chạy.
- POS dùng `POST /api/checkout`: một `SqlTransaction` ghi hóa đơn/chi tiết, trừ tồn kho và cộng điểm; lỗi thiếu hàng kích hoạt rollback toàn bộ.
- `DELETE /api/checkout/cancel/{orderId}` (Admin) hủy hóa đơn, hoàn kho và điều chỉnh điểm trong transaction.
- Không thêm bảng mới; Buổi 5 dùng lại sáu bảng của Buổi 4. Ba stored procedure đã được thêm vào `ThienAnSupermarketDb.sql`.

## Cấu hình và khởi chạy

1. Cài .NET 8 SDK và SQL Server hoặc SQL Server Express.
2. Mặc định `MiniSuperMarket.API/appsettings.json` dùng SQL Server LocalDB (`(localdb)\\MSSQLLocalDB`). Nếu dùng SQL Server khác, sửa `ConnectionStrings:DefaultConnection`, ví dụ `Server=localhost;Database=ThienAnSupermarketDb;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true` hoặc thay `localhost` bằng `.` cho SQL Express.
3. Đặt khóa JWT dài ít nhất 32 byte trong user secrets hoặc biến môi trường `JwtSettings__Secret`. Dự án đã có `UserSecretsId`; tại thư mục API, chạy:

   ```powershell
   dotnet user-secrets set "JwtSettings:Secret" "ThayBangChuoiBiMatDaiHon32ByteCuaBan"
   ```

4. Mở Package Manager Console trong Visual Studio, chọn `MiniSuperMarket.API` làm Default project rồi chạy:

   ```powershell
   Update-Database
   ```

   Hoặc dùng CLI từ thư mục solution:

   ```powershell
   dotnet ef database update --project MiniSuperMarket.API
   ```

   Migration Buổi 4 là migration khởi tạo toàn bộ sáu bảng. Nếu database Buổi 3 đã có các bảng `Categories`, `Products` hoặc `Customers` mà chưa có migration history tương ứng, hãy sao lưu dữ liệu trước và tạo database mới/đổi tên database trong connection string trước khi áp dụng migration này. Không chạy migration khởi tạo trên database đang có bảng cùng tên.

5. Mở solution trong Visual Studio và đặt API cùng WinForms chạy trong Multiple Startup Projects. API dùng HTTPS `https://localhost:7123`.
6. Đăng nhập bằng một tài khoản mẫu ở trên.

## API đáng chú ý

- `POST /api/auth/login`
- `GET /api/products/with-category`
- `GET /api/products/barcode/{barcode}`
- `POST /api/orders` (Admin/Cashier)
- `GET /api/ado-products/barcode/{barcode}` và `/disconnected-cache` (ADO.NET)
- `GET /api/ado-products/filter-stock?minStock=50` và `/category-counts`
- `POST /api/checkout` (Admin/Cashier, transaction)
- `DELETE /api/checkout/cancel/{orderId}` (Admin)
- `GET /api/reports/quick?date=2026-10-01` (Admin)
- `GET /api/reports/revenue-by-cashier` and `/revenue-by-cashier/range` (Stored Procedure, Admin)
- `GET /api/reports/top-selling-products` (Stored Procedure, Admin)
- `GET/POST /api/users` (Admin)

Swagger chỉ bật trong môi trường Development.

## Chủ đề và tác giả

Lê Minh Trường — MSSV 2124110110 — lớp CCQ2411D.
