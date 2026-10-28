using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MiniSuperMarket.API.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitBuoi4SixTablesAndSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    CategoryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.CategoryId);
                });

            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    CustomerId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PhoneNumber = table.Column<string>(type: "varchar(15)", maxLength: 15, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    RewardPoints = table.Column<int>(type: "int", nullable: false),
                    MembershipRank = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.CustomerId);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Role = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    ProductId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Barcode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ProductName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    StockQuantity = table.Column<int>(type: "int", nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.ProductId);
                    table.ForeignKey(
                        name: "FK_Products_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "CategoryId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    OrderId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CustomerId = table.Column<int>(type: "int", nullable: true),
                    CashierUsername = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TotalItems = table.Column<int>(type: "int", nullable: false),
                    FinalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    RewardPoints = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.OrderId);
                    table.ForeignKey(
                        name: "FK_Orders_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "CustomerId",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "OrderDetails",
                columns: table => new
                {
                    OrderDetailId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    LineTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderDetails", x => x.OrderDetailId);
                    table.ForeignKey(
                        name: "FK_OrderDetails_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "OrderId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderDetails_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "ProductId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "CategoryId", "CategoryName", "Description" },
                values: new object[,]
                {
                    { 1, "Bánh kẹo & Đồ ăn vặt", "Snack, bánh quy, kẹo trái cây" },
                    { 2, "Nước giải khát & Trà", "Nước ngọt, nước khoáng, trà đóng chai" },
                    { 3, "Sữa & Sản phẩm từ sữa", "Sữa tươi, sữa chua, phô mai" },
                    { 4, "Mì gói & Thực phẩm ăn liền", "Mì ăn liền, phở khô, cháo gói" },
                    { 5, "Gia vị & Dầu ăn", "Nước mắm, hạt nêm, dầu thực vật" },
                    { 6, "Đồ hộp & Chế biến sẵn", "Cá hộp, thịt hộp, xúc xích" },
                    { 7, "Hóa phẩm tẩy rửa", "Nước giặt, nước lau sàn, nước rửa chén" },
                    { 8, "Chăm sóc cá nhân", "Dầu gội, sữa tắm, kem đánh răng" },
                    { 9, "Đồ gia dụng tiện ích", "Khăn giấy, túi rác, pin" },
                    { 10, "Rau củ tươi sạch", "Rau củ theo mùa" },
                    { 11, "Thịt tươi & Thủy sản", "Thịt, cá và thủy sản" },
                    { 12, "Thực phẩm đông lạnh", "Thực phẩm đông lạnh" },
                    { 13, "Cà phê & Ngũ cốc", "Cà phê, yến mạch, ngũ cốc" },
                    { 14, "Văn phòng phẩm mini", "Bút, vở và dụng cụ học tập" },
                    { 15, "Vật tư đóng gói POS", "Túi, giấy in hóa đơn và tem" }
                });

            migrationBuilder.InsertData(
                table: "Customers",
                columns: new[] { "CustomerId", "Address", "CustomerName", "MembershipRank", "PhoneNumber", "RewardPoints" },
                values: new object[,]
                {
                    { 1, "123 Lê Lợi, Quận 1", "Trần Văn Hoàng", "Bạc", "0903123456", 120 },
                    { 2, "45 Điện Biên Phủ, Bình Thạnh", "Nguyễn Thị Kim Loan", "Vàng", "0918234567", 350 },
                    { 3, "88 Nam Kỳ Khởi Nghĩa, Quận 3", "Lê Minh Trí", "Chuẩn", "0977345678", 45 },
                    { 4, "102 Quang Trung, Gò Vấp", "Phạm Hồng Nhung", "Bạch Kim", "0988456789", 520 },
                    { 5, "15 Phan Xích Long, Phú Nhuận", "Võ Thành Nam", "Chuẩn", "0909567890", 80 },
                    { 6, "234 Hoàng Văn Thụ, Tân Bình", "Đỗ Thị Mỹ Duyên", "Bạc", "0933678901", 210 },
                    { 7, "312 Ba Tháng Hai, Quận 10", "Hoàng Quốc Bảo", "Chuẩn", "0944789012", 15 },
                    { 8, "56 Nguyễn Thị Thập, Quận 7", "Bùi Lan Hương", "Vàng", "0966890123", 410 },
                    { 9, "79 Kinh Dương Vương, Bình Tân", "Đặng Tuấn Kiệt", "Chuẩn", "0922901234", 95 },
                    { 10, "18 Hoàng Diệu, Quận 4", "Ngô Thanh Vân", "Bạch Kim", "0971012345", 650 },
                    { 11, "405 Lê Văn Khương, Quận 12", "Trương Đình Trọng", "Chuẩn", "0982123456", 30 },
                    { 12, "123 Long Thuận, Thủ Đức", "Lý Mỹ Lệ", "Bạc", "0904234567", 180 },
                    { 13, "67 Lũy Bán Bích, Tân Phú", "Vũ Đức Thịnh", "Bạc", "0915345678", 290 },
                    { 14, "142 Phạm Thế Hiển, Quận 8", "Phan Bích Thủy", "Vàng", "0936456789", 480 },
                    { 15, "89 An Dương Vương, Quận 5", "Dương Gia Huy", "Chuẩn", "0977567890", 0 }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserId", "FullName", "IsActive", "PasswordHash", "Role", "Username" },
                values: new object[,]
                {
                    { 1, "Nguyễn Quản Trị", true, "AQAAAAIAAYagAAAAECIYHg9epqjH8ZXhTRnHUxb5Ks7F9B6PV3aP8JfxDc2fDxnjdyW04TH5wDIy1RopEg==", "Admin", "admin01" },
                    { 2, "Trần Giám Đốc", true, "AQAAAAIAAYagAAAAECIYHg9epqjH8ZXhTRnHUxb5Ks7F9B6PV3aP8JfxDc2fDxnjdyW04TH5wDIy1RopEg==", "Admin", "admin02" },
                    { 3, "Lê Thu Ngân", true, "AQAAAAIAAYagAAAAECIYHg9epqjH8ZXhTRnHUxb5Ks7F9B6PV3aP8JfxDc2fDxnjdyW04TH5wDIy1RopEg==", "Cashier", "cashier01" },
                    { 4, "Phạm Bán Hàng", true, "AQAAAAIAAYagAAAAECIYHg9epqjH8ZXhTRnHUxb5Ks7F9B6PV3aP8JfxDc2fDxnjdyW04TH5wDIy1RopEg==", "Cashier", "cashier02" },
                    { 5, "Hoàng Thu Ngân", true, "AQAAAAIAAYagAAAAECIYHg9epqjH8ZXhTRnHUxb5Ks7F9B6PV3aP8JfxDc2fDxnjdyW04TH5wDIy1RopEg==", "Cashier", "cashier03" },
                    { 6, "Vũ Thị Quầy", true, "AQAAAAIAAYagAAAAECIYHg9epqjH8ZXhTRnHUxb5Ks7F9B6PV3aP8JfxDc2fDxnjdyW04TH5wDIy1RopEg==", "Cashier", "cashier04" },
                    { 7, "Đỗ Bán Lẻ", true, "AQAAAAIAAYagAAAAECIYHg9epqjH8ZXhTRnHUxb5Ks7F9B6PV3aP8JfxDc2fDxnjdyW04TH5wDIy1RopEg==", "Cashier", "cashier05" },
                    { 8, "Ngô Quản Kho", true, "AQAAAAIAAYagAAAAECIYHg9epqjH8ZXhTRnHUxb5Ks7F9B6PV3aP8JfxDc2fDxnjdyW04TH5wDIy1RopEg==", "Warehouse", "ware01" },
                    { 9, "Bùi Kiểm Kê", true, "AQAAAAIAAYagAAAAECIYHg9epqjH8ZXhTRnHUxb5Ks7F9B6PV3aP8JfxDc2fDxnjdyW04TH5wDIy1RopEg==", "Warehouse", "ware02" },
                    { 10, "Dương Thủ Kho", true, "AQAAAAIAAYagAAAAECIYHg9epqjH8ZXhTRnHUxb5Ks7F9B6PV3aP8JfxDc2fDxnjdyW04TH5wDIy1RopEg==", "Warehouse", "ware03" },
                    { 11, "Lý Nhập Hàng", true, "AQAAAAIAAYagAAAAECIYHg9epqjH8ZXhTRnHUxb5Ks7F9B6PV3aP8JfxDc2fDxnjdyW04TH5wDIy1RopEg==", "Warehouse", "ware04" },
                    { 12, "Đặng Hỗ Trợ", true, "AQAAAAIAAYagAAAAECIYHg9epqjH8ZXhTRnHUxb5Ks7F9B6PV3aP8JfxDc2fDxnjdyW04TH5wDIy1RopEg==", "Admin", "admin_backup" },
                    { 13, "Hồ Ca Chiều", true, "AQAAAAIAAYagAAAAECIYHg9epqjH8ZXhTRnHUxb5Ks7F9B6PV3aP8JfxDc2fDxnjdyW04TH5wDIy1RopEg==", "Cashier", "cashier06" },
                    { 14, "Trương Vận Chuyển", true, "AQAAAAIAAYagAAAAECIYHg9epqjH8ZXhTRnHUxb5Ks7F9B6PV3aP8JfxDc2fDxnjdyW04TH5wDIy1RopEg==", "Warehouse", "ware05" },
                    { 15, "Mai Giám Sát", true, "AQAAAAIAAYagAAAAECIYHg9epqjH8ZXhTRnHUxb5Ks7F9B6PV3aP8JfxDc2fDxnjdyW04TH5wDIy1RopEg==", "Admin", "supervisor" }
                });

            migrationBuilder.InsertData(
                table: "Orders",
                columns: new[] { "OrderId", "CashierUsername", "CreatedDate", "CustomerId", "FinalAmount", "OrderCode", "RewardPoints", "TotalItems" },
                values: new object[,]
                {
                    { 1, "cashier01", new DateTime(2026, 10, 1, 9, 0, 0, 0, DateTimeKind.Unspecified), 1, 147500m, "HD20261001-001", 14, 2 },
                    { 2, "cashier02", new DateTime(2026, 10, 2, 9, 0, 0, 0, DateTimeKind.Unspecified), 2, 150000m, "HD20261002-001", 15, 2 },
                    { 3, "cashier03", new DateTime(2026, 10, 3, 9, 0, 0, 0, DateTimeKind.Unspecified), 3, 69000m, "HD20261003-001", 6, 2 },
                    { 4, "cashier04", new DateTime(2026, 10, 4, 9, 0, 0, 0, DateTimeKind.Unspecified), 4, 64000m, "HD20261004-001", 6, 2 },
                    { 5, "cashier05", new DateTime(2026, 10, 5, 9, 0, 0, 0, DateTimeKind.Unspecified), 5, 16000m, "HD20261005-001", 1, 2 },
                    { 6, "cashier01", new DateTime(2026, 10, 6, 9, 0, 0, 0, DateTimeKind.Unspecified), 6, 17000m, "HD20261006-001", 1, 2 },
                    { 7, "cashier02", new DateTime(2026, 10, 7, 9, 0, 0, 0, DateTimeKind.Unspecified), 7, 25500m, "HD20261007-001", 2, 2 },
                    { 8, "cashier03", new DateTime(2026, 10, 8, 9, 0, 0, 0, DateTimeKind.Unspecified), 8, 49000m, "HD20261008-001", 4, 2 },
                    { 9, "cashier04", new DateTime(2026, 10, 9, 9, 0, 0, 0, DateTimeKind.Unspecified), 9, 73500m, "HD20261009-001", 7, 2 },
                    { 10, "cashier05", new DateTime(2026, 10, 10, 9, 0, 0, 0, DateTimeKind.Unspecified), 10, 67000m, "HD20261010-001", 6, 2 },
                    { 11, "cashier01", new DateTime(2026, 10, 11, 9, 0, 0, 0, DateTimeKind.Unspecified), 11, 54000m, "HD20261011-001", 5, 2 },
                    { 12, "cashier02", new DateTime(2026, 10, 12, 9, 0, 0, 0, DateTimeKind.Unspecified), 12, 30500m, "HD20261012-001", 3, 2 },
                    { 13, "cashier03", new DateTime(2026, 10, 13, 9, 0, 0, 0, DateTimeKind.Unspecified), 13, 13000m, "HD20261013-001", 1, 2 },
                    { 14, "cashier04", new DateTime(2026, 10, 14, 9, 0, 0, 0, DateTimeKind.Unspecified), 14, 22500m, "HD20261014-001", 2, 2 },
                    { 15, "cashier05", new DateTime(2026, 10, 15, 9, 0, 0, 0, DateTimeKind.Unspecified), 15, 139000m, "HD20261015-001", 13, 2 }
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "ProductId", "Barcode", "CategoryId", "Price", "ProductName", "StockQuantity" },
                values: new object[,]
                {
                    { 1, "893456789001", 1, 12500m, "Snack khoai tây O'Star 60g", 40 },
                    { 2, "893456789002", 1, 135000m, "Bánh quy bơ Danisa 454g", 77 },
                    { 3, "893456789003", 1, 15000m, "Kẹo dẻo trái cây 90g", 114 },
                    { 4, "893456789004", 2, 54000m, "Bánh Chocopie hộp 12 cái", 151 },
                    { 5, "893456789005", 2, 10000m, "Nước ngọt Coca Cola 320ml", 188 },
                    { 6, "893456789006", 2, 6000m, "Nước khoáng Lavie 500ml", 225 },
                    { 7, "893456789007", 3, 11000m, "Trà xanh Không Độ 455ml", 262 },
                    { 8, "893456789008", 3, 14500m, "Nước tăng lực Red Bull 250ml", 299 },
                    { 9, "893456789009", 3, 34500m, "Sữa tươi Vinamilk 1L", 76 },
                    { 10, "893456789010", 4, 39000m, "Phô mai con bò cười hộp 8 miếng", 113 },
                    { 11, "893456789011", 4, 28000m, "Sữa chua Vinamilk lốc 4", 150 },
                    { 12, "893456789012", 4, 26000m, "Sữa đặc Ông Thọ 380g", 187 },
                    { 13, "893456789013", 5, 4500m, "Mì Hảo Hảo chua cay 75g", 224 },
                    { 14, "893456789014", 5, 8500m, "Phở bò Đệ Nhất 68g", 261 },
                    { 15, "893456789015", 5, 14000m, "Mì trộn tương đen 115g", 298 },
                    { 16, "893456789016", 6, 125000m, "Dầu ăn Simply 2L", 75 },
                    { 17, "893456789017", 6, 30000m, "Nước mắm Nam Ngư 900ml", 112 },
                    { 18, "893456789018", 6, 36000m, "Hạt nêm Knorr 400g", 149 },
                    { 19, "893456789019", 7, 19000m, "Cá nục sốt cà hộp 155g", 186 },
                    { 20, "893456789020", 7, 29000m, "Nước rửa chén Sunlight 750g", 223 }
                });

            migrationBuilder.InsertData(
                table: "OrderDetails",
                columns: new[] { "OrderDetailId", "LineTotal", "OrderId", "ProductId", "Quantity", "UnitPrice" },
                values: new object[,]
                {
                    { 1, 12500m, 1, 1, 1, 12500m },
                    { 2, 135000m, 1, 2, 1, 135000m },
                    { 3, 135000m, 2, 2, 1, 135000m },
                    { 4, 15000m, 2, 3, 1, 15000m },
                    { 5, 15000m, 3, 3, 1, 15000m },
                    { 6, 54000m, 3, 4, 1, 54000m },
                    { 7, 54000m, 4, 4, 1, 54000m },
                    { 8, 10000m, 4, 5, 1, 10000m },
                    { 9, 10000m, 5, 5, 1, 10000m },
                    { 10, 6000m, 5, 6, 1, 6000m },
                    { 11, 6000m, 6, 6, 1, 6000m },
                    { 12, 11000m, 6, 7, 1, 11000m },
                    { 13, 11000m, 7, 7, 1, 11000m },
                    { 14, 14500m, 7, 8, 1, 14500m },
                    { 15, 14500m, 8, 8, 1, 14500m },
                    { 16, 34500m, 8, 9, 1, 34500m },
                    { 17, 34500m, 9, 9, 1, 34500m },
                    { 18, 39000m, 9, 10, 1, 39000m },
                    { 19, 39000m, 10, 10, 1, 39000m },
                    { 20, 28000m, 10, 11, 1, 28000m },
                    { 21, 28000m, 11, 11, 1, 28000m },
                    { 22, 26000m, 11, 12, 1, 26000m },
                    { 23, 26000m, 12, 12, 1, 26000m },
                    { 24, 4500m, 12, 13, 1, 4500m },
                    { 25, 4500m, 13, 13, 1, 4500m },
                    { 26, 8500m, 13, 14, 1, 8500m },
                    { 27, 8500m, 14, 14, 1, 8500m },
                    { 28, 14000m, 14, 15, 1, 14000m },
                    { 29, 14000m, 15, 15, 1, 14000m },
                    { 30, 125000m, 15, 16, 1, 125000m }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Categories_CategoryName",
                table: "Categories",
                column: "CategoryName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Customers_PhoneNumber",
                table: "Customers",
                column: "PhoneNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_OrderId",
                table: "OrderDetails",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_ProductId",
                table: "OrderDetails",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_CustomerId",
                table: "Orders",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_OrderCode",
                table: "Orders",
                column: "OrderCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Products_Barcode",
                table: "Products",
                column: "Barcode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Products_CategoryId",
                table: "Products",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrderDetails");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "Customers");

            migrationBuilder.DropTable(
                name: "Categories");
        }
    }
}
