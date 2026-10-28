using Microsoft.EntityFrameworkCore;
using MiniSuperMarket.API.Models;

namespace MiniSuperMarket.API.Data
{
    public class SupermarketDbContext : DbContext
    {
        public SupermarketDbContext(DbContextOptions<SupermarketDbContext> options) : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Product> Products => Set<Product>();
        public DbSet<Customer> Customers => Set<Customer>();
        public DbSet<Order> Orders => Set<Order>();
        public DbSet<OrderDetail> OrderDetails => Set<OrderDetail>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>().HasIndex(x => x.Username).IsUnique();
            modelBuilder.Entity<Category>().HasIndex(x => x.CategoryName).IsUnique();
            modelBuilder.Entity<Product>().HasIndex(x => x.Barcode).IsUnique();
            modelBuilder.Entity<Customer>().HasIndex(x => x.PhoneNumber).IsUnique();
            modelBuilder.Entity<Order>().HasIndex(x => x.OrderCode).IsUnique();

            modelBuilder.Entity<Product>()
                .HasOne(x => x.Category).WithMany(x => x.Products)
                .HasForeignKey(x => x.CategoryId).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Order>()
                .HasOne(x => x.Customer).WithMany(x => x.Orders)
                .HasForeignKey(x => x.CustomerId).OnDelete(DeleteBehavior.SetNull);
            modelBuilder.Entity<OrderDetail>()
                .HasOne(x => x.Order).WithMany(x => x.OrderDetails)
                .HasForeignKey(x => x.OrderId).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<OrderDetail>()
                .HasOne(x => x.Product).WithMany(x => x.OrderDetails)
                .HasForeignKey(x => x.ProductId).OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>().HasData(Seed.Users);
            modelBuilder.Entity<Category>().HasData(Seed.Categories);
            modelBuilder.Entity<Product>().HasData(Seed.Products);
            modelBuilder.Entity<Customer>().HasData(Seed.Customers);
            modelBuilder.Entity<Order>().HasData(Seed.Orders);
            modelBuilder.Entity<OrderDetail>().HasData(Seed.OrderDetails);
        }

        private static class Seed
        {
            // Password for all demo accounts: 123456 (ASP.NET Core Identity PasswordHasher format).
            private const string DemoPasswordHash = "AQAAAAIAAYagAAAAECIYHg9epqjH8ZXhTRnHUxb5Ks7F9B6PV3aP8JfxDc2fDxnjdyW04TH5wDIy1RopEg==";
            private static User Staff(int id, string username, string fullName, string role) =>
                new() { UserId = id, Username = username, FullName = fullName, Role = role, PasswordHash = DemoPasswordHash, IsActive = true };

            public static readonly User[] Users =
            [
                Staff(1, "admin01", "Nguyễn Quản Trị", "Admin"),
                Staff(2, "admin02", "Trần Giám Đốc", "Admin"),
                Staff(3, "cashier01", "Lê Thu Ngân", "Cashier"),
                Staff(4, "cashier02", "Phạm Bán Hàng", "Cashier"),
                Staff(5, "cashier03", "Hoàng Thu Ngân", "Cashier"),
                Staff(6, "cashier04", "Vũ Thị Quầy", "Cashier"),
                Staff(7, "cashier05", "Đỗ Bán Lẻ", "Cashier"),
                Staff(8, "ware01", "Ngô Quản Kho", "Warehouse"),
                Staff(9, "ware02", "Bùi Kiểm Kê", "Warehouse"),
                Staff(10, "ware03", "Dương Thủ Kho", "Warehouse"),
                Staff(11, "ware04", "Lý Nhập Hàng", "Warehouse"),
                Staff(12, "admin_backup", "Đặng Hỗ Trợ", "Admin"),
                Staff(13, "cashier06", "Hồ Ca Chiều", "Cashier"),
                Staff(14, "ware05", "Trương Vận Chuyển", "Warehouse"),
                Staff(15, "supervisor", "Mai Giám Sát", "Admin")
            ];

            public static readonly Category[] Categories =
            [
                new() { CategoryId=1, CategoryName="Bánh kẹo & Đồ ăn vặt", Description="Snack, bánh quy, kẹo trái cây" },
                new() { CategoryId=2, CategoryName="Nước giải khát & Trà", Description="Nước ngọt, nước khoáng, trà đóng chai" },
                new() { CategoryId=3, CategoryName="Sữa & Sản phẩm từ sữa", Description="Sữa tươi, sữa chua, phô mai" },
                new() { CategoryId=4, CategoryName="Mì gói & Thực phẩm ăn liền", Description="Mì ăn liền, phở khô, cháo gói" },
                new() { CategoryId=5, CategoryName="Gia vị & Dầu ăn", Description="Nước mắm, hạt nêm, dầu thực vật" },
                new() { CategoryId=6, CategoryName="Đồ hộp & Chế biến sẵn", Description="Cá hộp, thịt hộp, xúc xích" },
                new() { CategoryId=7, CategoryName="Hóa phẩm tẩy rửa", Description="Nước giặt, nước lau sàn, nước rửa chén" },
                new() { CategoryId=8, CategoryName="Chăm sóc cá nhân", Description="Dầu gội, sữa tắm, kem đánh răng" },
                new() { CategoryId=9, CategoryName="Đồ gia dụng tiện ích", Description="Khăn giấy, túi rác, pin" },
                new() { CategoryId=10, CategoryName="Rau củ tươi sạch", Description="Rau củ theo mùa" },
                new() { CategoryId=11, CategoryName="Thịt tươi & Thủy sản", Description="Thịt, cá và thủy sản" },
                new() { CategoryId=12, CategoryName="Thực phẩm đông lạnh", Description="Thực phẩm đông lạnh" },
                new() { CategoryId=13, CategoryName="Cà phê & Ngũ cốc", Description="Cà phê, yến mạch, ngũ cốc" },
                new() { CategoryId=14, CategoryName="Văn phòng phẩm mini", Description="Bút, vở và dụng cụ học tập" },
                new() { CategoryId=15, CategoryName="Vật tư đóng gói POS", Description="Túi, giấy in hóa đơn và tem" }
            ];

            private static readonly string[] ProductNames =
            [
                "Snack khoai tây O'Star 60g", "Bánh quy bơ Danisa 454g", "Kẹo dẻo trái cây 90g", "Bánh Chocopie hộp 12 cái",
                "Nước ngọt Coca Cola 320ml", "Nước khoáng Lavie 500ml", "Trà xanh Không Độ 455ml", "Nước tăng lực Red Bull 250ml",
                "Sữa tươi Vinamilk 1L", "Phô mai con bò cười hộp 8 miếng", "Sữa chua Vinamilk lốc 4", "Sữa đặc Ông Thọ 380g",
                "Mì Hảo Hảo chua cay 75g", "Phở bò Đệ Nhất 68g", "Mì trộn tương đen 115g", "Dầu ăn Simply 2L",
                "Nước mắm Nam Ngư 900ml", "Hạt nêm Knorr 400g", "Cá nục sốt cà hộp 155g", "Nước rửa chén Sunlight 750g"
            ];
            private static readonly decimal[] Prices = [12500m,135000m,15000m,54000m,10000m,6000m,11000m,14500m,34500m,39000m,28000m,26000m,4500m,8500m,14000m,125000m,30000m,36000m,19000m,29000m];
            public static readonly Product[] Products = ProductNames.Select((name, i) => new Product
            {
                ProductId = i + 1, Barcode = $"893456789{i + 1:000}", ProductName = name,
                Price = Prices[i], StockQuantity = 40 + ((i * 37) % 260), CategoryId = ((i / 3) % 15) + 1
            }).ToArray();

            private static readonly (string Name, string Phone, string Address, int Points, string Rank)[] CustomerRows =
            [
                ("Trần Văn Hoàng","0903123456","123 Lê Lợi, Quận 1",120,"Bạc"),
                ("Nguyễn Thị Kim Loan","0918234567","45 Điện Biên Phủ, Bình Thạnh",350,"Vàng"),
                ("Lê Minh Trí","0977345678","88 Nam Kỳ Khởi Nghĩa, Quận 3",45,"Chuẩn"),
                ("Phạm Hồng Nhung","0988456789","102 Quang Trung, Gò Vấp",520,"Bạch Kim"),
                ("Võ Thành Nam","0909567890","15 Phan Xích Long, Phú Nhuận",80,"Chuẩn"),
                ("Đỗ Thị Mỹ Duyên","0933678901","234 Hoàng Văn Thụ, Tân Bình",210,"Bạc"),
                ("Hoàng Quốc Bảo","0944789012","312 Ba Tháng Hai, Quận 10",15,"Chuẩn"),
                ("Bùi Lan Hương","0966890123","56 Nguyễn Thị Thập, Quận 7",410,"Vàng"),
                ("Đặng Tuấn Kiệt","0922901234","79 Kinh Dương Vương, Bình Tân",95,"Chuẩn"),
                ("Ngô Thanh Vân","0971012345","18 Hoàng Diệu, Quận 4",650,"Bạch Kim"),
                ("Trương Đình Trọng","0982123456","405 Lê Văn Khương, Quận 12",30,"Chuẩn"),
                ("Lý Mỹ Lệ","0904234567","123 Long Thuận, Thủ Đức",180,"Bạc"),
                ("Vũ Đức Thịnh","0915345678","67 Lũy Bán Bích, Tân Phú",290,"Bạc"),
                ("Phan Bích Thủy","0936456789","142 Phạm Thế Hiển, Quận 8",480,"Vàng"),
                ("Dương Gia Huy","0977567890","89 An Dương Vương, Quận 5",0,"Chuẩn")
            ];
            public static readonly Customer[] Customers = CustomerRows.Select((x, i) => new Customer
            {
                CustomerId = i + 1, CustomerName = x.Name, PhoneNumber = x.Phone,
                Address = x.Address, RewardPoints = x.Points, MembershipRank = x.Rank
            }).ToArray();

            public static readonly Order[] Orders = Enumerable.Range(1, 15).Select(i =>
            {
                int firstProduct = ((i - 1) % 20) + 1;
                int secondProduct = (firstProduct % 20) + 1;
                decimal total = Prices[firstProduct - 1] + Prices[secondProduct - 1];
                return new Order
                {
                    OrderId = i, OrderCode = $"HD202610{i:00}-001", CreatedDate = new DateTime(2026, 10, 1).AddDays(i - 1).AddHours(9),
                    CustomerId = i, CashierUsername = $"cashier0{((i - 1) % 5) + 1}", TotalItems = 2,
                    FinalAmount = total, RewardPoints = (int)(total / 10000m)
                };
            }).ToArray();

            public static readonly OrderDetail[] OrderDetails = Enumerable.Range(1, 15)
                .SelectMany(orderId =>
                {
                    int firstProduct = ((orderId - 1) % 20) + 1;
                    int secondProduct = (firstProduct % 20) + 1;
                    return new[]
                    {
                        new OrderDetail { OrderDetailId = orderId * 2 - 1, OrderId = orderId, ProductId = firstProduct, Quantity = 1, UnitPrice = Prices[firstProduct - 1], LineTotal = Prices[firstProduct - 1] },
                        new OrderDetail { OrderDetailId = orderId * 2, OrderId = orderId, ProductId = secondProduct, Quantity = 1, UnitPrice = Prices[secondProduct - 1], LineTotal = Prices[secondProduct - 1] }
                    };
                }).ToArray();
        }
    }
}
