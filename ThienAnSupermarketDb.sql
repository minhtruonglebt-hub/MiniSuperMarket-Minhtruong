/*
  CSDL: ThienAnSupermarketDb
  Sinh tu migration EF Core 8.0.11 cua du an Buoi 4.
  Chay trong SSMS ket noi SQL Server (LocalDB) hoac SQL Server cua ban.
  Script tao 6 bang va seed du lieu; co the chay lai neu migration da duoc ap dung.
*/
IF DB_ID(N'ThienAnSupermarketDb') IS NULL
BEGIN
    EXEC(N'CREATE DATABASE [ThienAnSupermarketDb]');
END;
GO
USE [ThienAnSupermarketDb];
GO

IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261021125549_InitBuoi4SixTablesAndSeedData'
)
BEGIN
    CREATE TABLE [Categories] (
        [CategoryId] int NOT NULL IDENTITY,
        [CategoryName] nvarchar(100) NOT NULL,
        [Description] nvarchar(255) NULL,
        CONSTRAINT [PK_Categories] PRIMARY KEY ([CategoryId])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261021125549_InitBuoi4SixTablesAndSeedData'
)
BEGIN
    CREATE TABLE [Customers] (
        [CustomerId] int NOT NULL IDENTITY,
        [CustomerName] nvarchar(100) NOT NULL,
        [PhoneNumber] varchar(15) NOT NULL,
        [Address] nvarchar(200) NULL,
        [RewardPoints] int NOT NULL,
        [MembershipRank] nvarchar(50) NOT NULL,
        CONSTRAINT [PK_Customers] PRIMARY KEY ([CustomerId])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261021125549_InitBuoi4SixTablesAndSeedData'
)
BEGIN
    CREATE TABLE [Users] (
        [UserId] int NOT NULL IDENTITY,
        [Username] nvarchar(50) NOT NULL,
        [PasswordHash] nvarchar(max) NOT NULL,
        [FullName] nvarchar(100) NOT NULL,
        [Role] nvarchar(50) NOT NULL,
        [IsActive] bit NOT NULL,
        CONSTRAINT [PK_Users] PRIMARY KEY ([UserId])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261021125549_InitBuoi4SixTablesAndSeedData'
)
BEGIN
    CREATE TABLE [Products] (
        [ProductId] int NOT NULL IDENTITY,
        [Barcode] nvarchar(50) NOT NULL,
        [ProductName] nvarchar(150) NOT NULL,
        [Price] decimal(18,2) NOT NULL,
        [StockQuantity] int NOT NULL,
        [CategoryId] int NOT NULL,
        CONSTRAINT [PK_Products] PRIMARY KEY ([ProductId]),
        CONSTRAINT [FK_Products_Categories_CategoryId] FOREIGN KEY ([CategoryId]) REFERENCES [Categories] ([CategoryId]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261021125549_InitBuoi4SixTablesAndSeedData'
)
BEGIN
    CREATE TABLE [Orders] (
        [OrderId] int NOT NULL IDENTITY,
        [OrderCode] nvarchar(50) NOT NULL,
        [CreatedDate] datetime2 NOT NULL,
        [CustomerId] int NULL,
        [CashierUsername] nvarchar(50) NOT NULL,
        [TotalItems] int NOT NULL,
        [FinalAmount] decimal(18,2) NOT NULL,
        [RewardPoints] int NOT NULL,
        CONSTRAINT [PK_Orders] PRIMARY KEY ([OrderId]),
        CONSTRAINT [FK_Orders_Customers_CustomerId] FOREIGN KEY ([CustomerId]) REFERENCES [Customers] ([CustomerId]) ON DELETE SET NULL
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261021125549_InitBuoi4SixTablesAndSeedData'
)
BEGIN
    CREATE TABLE [OrderDetails] (
        [OrderDetailId] int NOT NULL IDENTITY,
        [OrderId] int NOT NULL,
        [ProductId] int NOT NULL,
        [Quantity] int NOT NULL,
        [UnitPrice] decimal(18,2) NOT NULL,
        [LineTotal] decimal(18,2) NOT NULL,
        CONSTRAINT [PK_OrderDetails] PRIMARY KEY ([OrderDetailId]),
        CONSTRAINT [FK_OrderDetails_Orders_OrderId] FOREIGN KEY ([OrderId]) REFERENCES [Orders] ([OrderId]) ON DELETE CASCADE,
        CONSTRAINT [FK_OrderDetails_Products_ProductId] FOREIGN KEY ([ProductId]) REFERENCES [Products] ([ProductId]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261021125549_InitBuoi4SixTablesAndSeedData'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'CategoryId', N'CategoryName', N'Description') AND [object_id] = OBJECT_ID(N'[Categories]'))
        SET IDENTITY_INSERT [Categories] ON;
    EXEC(N'INSERT INTO [Categories] ([CategoryId], [CategoryName], [Description])
    VALUES (1, N''Bánh kẹo & Đồ ăn vặt'', N''Snack, bánh quy, kẹo trái cây''),
    (2, N''Nước giải khát & Trà'', N''Nước ngọt, nước khoáng, trà đóng chai''),
    (3, N''Sữa & Sản phẩm từ sữa'', N''Sữa tươi, sữa chua, phô mai''),
    (4, N''Mì gói & Thực phẩm ăn liền'', N''Mì ăn liền, phở khô, cháo gói''),
    (5, N''Gia vị & Dầu ăn'', N''Nước mắm, hạt nêm, dầu thực vật''),
    (6, N''Đồ hộp & Chế biến sẵn'', N''Cá hộp, thịt hộp, xúc xích''),
    (7, N''Hóa phẩm tẩy rửa'', N''Nước giặt, nước lau sàn, nước rửa chén''),
    (8, N''Chăm sóc cá nhân'', N''Dầu gội, sữa tắm, kem đánh răng''),
    (9, N''Đồ gia dụng tiện ích'', N''Khăn giấy, túi rác, pin''),
    (10, N''Rau củ tươi sạch'', N''Rau củ theo mùa''),
    (11, N''Thịt tươi & Thủy sản'', N''Thịt, cá và thủy sản''),
    (12, N''Thực phẩm đông lạnh'', N''Thực phẩm đông lạnh''),
    (13, N''Cà phê & Ngũ cốc'', N''Cà phê, yến mạch, ngũ cốc''),
    (14, N''Văn phòng phẩm mini'', N''Bút, vở và dụng cụ học tập''),
    (15, N''Vật tư đóng gói POS'', N''Túi, giấy in hóa đơn và tem'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'CategoryId', N'CategoryName', N'Description') AND [object_id] = OBJECT_ID(N'[Categories]'))
        SET IDENTITY_INSERT [Categories] OFF;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261021125549_InitBuoi4SixTablesAndSeedData'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'CustomerId', N'Address', N'CustomerName', N'MembershipRank', N'PhoneNumber', N'RewardPoints') AND [object_id] = OBJECT_ID(N'[Customers]'))
        SET IDENTITY_INSERT [Customers] ON;
    EXEC(N'INSERT INTO [Customers] ([CustomerId], [Address], [CustomerName], [MembershipRank], [PhoneNumber], [RewardPoints])
    VALUES (1, N''123 Lê Lợi, Quận 1'', N''Trần Văn Hoàng'', N''Bạc'', ''0903123456'', 120),
    (2, N''45 Điện Biên Phủ, Bình Thạnh'', N''Nguyễn Thị Kim Loan'', N''Vàng'', ''0918234567'', 350),
    (3, N''88 Nam Kỳ Khởi Nghĩa, Quận 3'', N''Lê Minh Trí'', N''Chuẩn'', ''0977345678'', 45),
    (4, N''102 Quang Trung, Gò Vấp'', N''Phạm Hồng Nhung'', N''Bạch Kim'', ''0988456789'', 520),
    (5, N''15 Phan Xích Long, Phú Nhuận'', N''Võ Thành Nam'', N''Chuẩn'', ''0909567890'', 80),
    (6, N''234 Hoàng Văn Thụ, Tân Bình'', N''Đỗ Thị Mỹ Duyên'', N''Bạc'', ''0933678901'', 210),
    (7, N''312 Ba Tháng Hai, Quận 10'', N''Hoàng Quốc Bảo'', N''Chuẩn'', ''0944789012'', 15),
    (8, N''56 Nguyễn Thị Thập, Quận 7'', N''Bùi Lan Hương'', N''Vàng'', ''0966890123'', 410),
    (9, N''79 Kinh Dương Vương, Bình Tân'', N''Đặng Tuấn Kiệt'', N''Chuẩn'', ''0922901234'', 95),
    (10, N''18 Hoàng Diệu, Quận 4'', N''Ngô Thanh Vân'', N''Bạch Kim'', ''0971012345'', 650),
    (11, N''405 Lê Văn Khương, Quận 12'', N''Trương Đình Trọng'', N''Chuẩn'', ''0982123456'', 30),
    (12, N''123 Long Thuận, Thủ Đức'', N''Lý Mỹ Lệ'', N''Bạc'', ''0904234567'', 180),
    (13, N''67 Lũy Bán Bích, Tân Phú'', N''Vũ Đức Thịnh'', N''Bạc'', ''0915345678'', 290),
    (14, N''142 Phạm Thế Hiển, Quận 8'', N''Phan Bích Thủy'', N''Vàng'', ''0936456789'', 480),
    (15, N''89 An Dương Vương, Quận 5'', N''Dương Gia Huy'', N''Chuẩn'', ''0977567890'', 0)');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'CustomerId', N'Address', N'CustomerName', N'MembershipRank', N'PhoneNumber', N'RewardPoints') AND [object_id] = OBJECT_ID(N'[Customers]'))
        SET IDENTITY_INSERT [Customers] OFF;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261021125549_InitBuoi4SixTablesAndSeedData'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'UserId', N'FullName', N'IsActive', N'PasswordHash', N'Role', N'Username') AND [object_id] = OBJECT_ID(N'[Users]'))
        SET IDENTITY_INSERT [Users] ON;
    EXEC(N'INSERT INTO [Users] ([UserId], [FullName], [IsActive], [PasswordHash], [Role], [Username])
    VALUES (1, N''Nguyễn Quản Trị'', CAST(1 AS bit), N''AQAAAAIAAYagAAAAECIYHg9epqjH8ZXhTRnHUxb5Ks7F9B6PV3aP8JfxDc2fDxnjdyW04TH5wDIy1RopEg=='', N''Admin'', N''admin01''),
    (2, N''Trần Giám Đốc'', CAST(1 AS bit), N''AQAAAAIAAYagAAAAECIYHg9epqjH8ZXhTRnHUxb5Ks7F9B6PV3aP8JfxDc2fDxnjdyW04TH5wDIy1RopEg=='', N''Admin'', N''admin02''),
    (3, N''Lê Thu Ngân'', CAST(1 AS bit), N''AQAAAAIAAYagAAAAECIYHg9epqjH8ZXhTRnHUxb5Ks7F9B6PV3aP8JfxDc2fDxnjdyW04TH5wDIy1RopEg=='', N''Cashier'', N''cashier01''),
    (4, N''Phạm Bán Hàng'', CAST(1 AS bit), N''AQAAAAIAAYagAAAAECIYHg9epqjH8ZXhTRnHUxb5Ks7F9B6PV3aP8JfxDc2fDxnjdyW04TH5wDIy1RopEg=='', N''Cashier'', N''cashier02''),
    (5, N''Hoàng Thu Ngân'', CAST(1 AS bit), N''AQAAAAIAAYagAAAAECIYHg9epqjH8ZXhTRnHUxb5Ks7F9B6PV3aP8JfxDc2fDxnjdyW04TH5wDIy1RopEg=='', N''Cashier'', N''cashier03''),
    (6, N''Vũ Thị Quầy'', CAST(1 AS bit), N''AQAAAAIAAYagAAAAECIYHg9epqjH8ZXhTRnHUxb5Ks7F9B6PV3aP8JfxDc2fDxnjdyW04TH5wDIy1RopEg=='', N''Cashier'', N''cashier04''),
    (7, N''Đỗ Bán Lẻ'', CAST(1 AS bit), N''AQAAAAIAAYagAAAAECIYHg9epqjH8ZXhTRnHUxb5Ks7F9B6PV3aP8JfxDc2fDxnjdyW04TH5wDIy1RopEg=='', N''Cashier'', N''cashier05''),
    (8, N''Ngô Quản Kho'', CAST(1 AS bit), N''AQAAAAIAAYagAAAAECIYHg9epqjH8ZXhTRnHUxb5Ks7F9B6PV3aP8JfxDc2fDxnjdyW04TH5wDIy1RopEg=='', N''Warehouse'', N''ware01''),
    (9, N''Bùi Kiểm Kê'', CAST(1 AS bit), N''AQAAAAIAAYagAAAAECIYHg9epqjH8ZXhTRnHUxb5Ks7F9B6PV3aP8JfxDc2fDxnjdyW04TH5wDIy1RopEg=='', N''Warehouse'', N''ware02''),
    (10, N''Dương Thủ Kho'', CAST(1 AS bit), N''AQAAAAIAAYagAAAAECIYHg9epqjH8ZXhTRnHUxb5Ks7F9B6PV3aP8JfxDc2fDxnjdyW04TH5wDIy1RopEg=='', N''Warehouse'', N''ware03''),
    (11, N''Lý Nhập Hàng'', CAST(1 AS bit), N''AQAAAAIAAYagAAAAECIYHg9epqjH8ZXhTRnHUxb5Ks7F9B6PV3aP8JfxDc2fDxnjdyW04TH5wDIy1RopEg=='', N''Warehouse'', N''ware04''),
    (12, N''Đặng Hỗ Trợ'', CAST(1 AS bit), N''AQAAAAIAAYagAAAAECIYHg9epqjH8ZXhTRnHUxb5Ks7F9B6PV3aP8JfxDc2fDxnjdyW04TH5wDIy1RopEg=='', N''Admin'', N''admin_backup''),
    (13, N''Hồ Ca Chiều'', CAST(1 AS bit), N''AQAAAAIAAYagAAAAECIYHg9epqjH8ZXhTRnHUxb5Ks7F9B6PV3aP8JfxDc2fDxnjdyW04TH5wDIy1RopEg=='', N''Cashier'', N''cashier06''),
    (14, N''Trương Vận Chuyển'', CAST(1 AS bit), N''AQAAAAIAAYagAAAAECIYHg9epqjH8ZXhTRnHUxb5Ks7F9B6PV3aP8JfxDc2fDxnjdyW04TH5wDIy1RopEg=='', N''Warehouse'', N''ware05''),
    (15, N''Mai Giám Sát'', CAST(1 AS bit), N''AQAAAAIAAYagAAAAECIYHg9epqjH8ZXhTRnHUxb5Ks7F9B6PV3aP8JfxDc2fDxnjdyW04TH5wDIy1RopEg=='', N''Admin'', N''supervisor'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'UserId', N'FullName', N'IsActive', N'PasswordHash', N'Role', N'Username') AND [object_id] = OBJECT_ID(N'[Users]'))
        SET IDENTITY_INSERT [Users] OFF;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261021125549_InitBuoi4SixTablesAndSeedData'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'OrderId', N'CashierUsername', N'CreatedDate', N'CustomerId', N'FinalAmount', N'OrderCode', N'RewardPoints', N'TotalItems') AND [object_id] = OBJECT_ID(N'[Orders]'))
        SET IDENTITY_INSERT [Orders] ON;
    EXEC(N'INSERT INTO [Orders] ([OrderId], [CashierUsername], [CreatedDate], [CustomerId], [FinalAmount], [OrderCode], [RewardPoints], [TotalItems])
    VALUES (1, N''cashier01'', ''2026-10-01T09:00:00.0000000'', 1, 147500.0, N''HD20261001-001'', 14, 2),
    (2, N''cashier02'', ''2026-10-02T09:00:00.0000000'', 2, 150000.0, N''HD20261002-001'', 15, 2),
    (3, N''cashier03'', ''2026-10-03T09:00:00.0000000'', 3, 69000.0, N''HD20261003-001'', 6, 2),
    (4, N''cashier04'', ''2026-10-04T09:00:00.0000000'', 4, 64000.0, N''HD20261004-001'', 6, 2),
    (5, N''cashier05'', ''2026-10-05T09:00:00.0000000'', 5, 16000.0, N''HD20261005-001'', 1, 2),
    (6, N''cashier01'', ''2026-10-06T09:00:00.0000000'', 6, 17000.0, N''HD20261006-001'', 1, 2),
    (7, N''cashier02'', ''2026-10-07T09:00:00.0000000'', 7, 25500.0, N''HD20261007-001'', 2, 2),
    (8, N''cashier03'', ''2026-10-08T09:00:00.0000000'', 8, 49000.0, N''HD20261008-001'', 4, 2),
    (9, N''cashier04'', ''2026-10-09T09:00:00.0000000'', 9, 73500.0, N''HD20261009-001'', 7, 2),
    (10, N''cashier05'', ''2026-10-10T09:00:00.0000000'', 10, 67000.0, N''HD20261010-001'', 6, 2),
    (11, N''cashier01'', ''2026-10-11T09:00:00.0000000'', 11, 54000.0, N''HD20261011-001'', 5, 2),
    (12, N''cashier02'', ''2026-10-12T09:00:00.0000000'', 12, 30500.0, N''HD20261012-001'', 3, 2),
    (13, N''cashier03'', ''2026-10-13T09:00:00.0000000'', 13, 13000.0, N''HD20261013-001'', 1, 2),
    (14, N''cashier04'', ''2026-10-14T09:00:00.0000000'', 14, 22500.0, N''HD20261014-001'', 2, 2),
    (15, N''cashier05'', ''2026-10-15T09:00:00.0000000'', 15, 139000.0, N''HD20261015-001'', 13, 2)');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'OrderId', N'CashierUsername', N'CreatedDate', N'CustomerId', N'FinalAmount', N'OrderCode', N'RewardPoints', N'TotalItems') AND [object_id] = OBJECT_ID(N'[Orders]'))
        SET IDENTITY_INSERT [Orders] OFF;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261021125549_InitBuoi4SixTablesAndSeedData'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'ProductId', N'Barcode', N'CategoryId', N'Price', N'ProductName', N'StockQuantity') AND [object_id] = OBJECT_ID(N'[Products]'))
        SET IDENTITY_INSERT [Products] ON;
    EXEC(N'INSERT INTO [Products] ([ProductId], [Barcode], [CategoryId], [Price], [ProductName], [StockQuantity])
    VALUES (1, N''893456789001'', 1, 12500.0, N''Snack khoai tây O''''Star 60g'', 40),
    (2, N''893456789002'', 1, 135000.0, N''Bánh quy bơ Danisa 454g'', 77),
    (3, N''893456789003'', 1, 15000.0, N''Kẹo dẻo trái cây 90g'', 114),
    (4, N''893456789004'', 2, 54000.0, N''Bánh Chocopie hộp 12 cái'', 151),
    (5, N''893456789005'', 2, 10000.0, N''Nước ngọt Coca Cola 320ml'', 188),
    (6, N''893456789006'', 2, 6000.0, N''Nước khoáng Lavie 500ml'', 225),
    (7, N''893456789007'', 3, 11000.0, N''Trà xanh Không Độ 455ml'', 262),
    (8, N''893456789008'', 3, 14500.0, N''Nước tăng lực Red Bull 250ml'', 299),
    (9, N''893456789009'', 3, 34500.0, N''Sữa tươi Vinamilk 1L'', 76),
    (10, N''893456789010'', 4, 39000.0, N''Phô mai con bò cười hộp 8 miếng'', 113),
    (11, N''893456789011'', 4, 28000.0, N''Sữa chua Vinamilk lốc 4'', 150),
    (12, N''893456789012'', 4, 26000.0, N''Sữa đặc Ông Thọ 380g'', 187),
    (13, N''893456789013'', 5, 4500.0, N''Mì Hảo Hảo chua cay 75g'', 224),
    (14, N''893456789014'', 5, 8500.0, N''Phở bò Đệ Nhất 68g'', 261),
    (15, N''893456789015'', 5, 14000.0, N''Mì trộn tương đen 115g'', 298),
    (16, N''893456789016'', 6, 125000.0, N''Dầu ăn Simply 2L'', 75),
    (17, N''893456789017'', 6, 30000.0, N''Nước mắm Nam Ngư 900ml'', 112),
    (18, N''893456789018'', 6, 36000.0, N''Hạt nêm Knorr 400g'', 149),
    (19, N''893456789019'', 7, 19000.0, N''Cá nục sốt cà hộp 155g'', 186),
    (20, N''893456789020'', 7, 29000.0, N''Nước rửa chén Sunlight 750g'', 223)');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'ProductId', N'Barcode', N'CategoryId', N'Price', N'ProductName', N'StockQuantity') AND [object_id] = OBJECT_ID(N'[Products]'))
        SET IDENTITY_INSERT [Products] OFF;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261021125549_InitBuoi4SixTablesAndSeedData'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'OrderDetailId', N'LineTotal', N'OrderId', N'ProductId', N'Quantity', N'UnitPrice') AND [object_id] = OBJECT_ID(N'[OrderDetails]'))
        SET IDENTITY_INSERT [OrderDetails] ON;
    EXEC(N'INSERT INTO [OrderDetails] ([OrderDetailId], [LineTotal], [OrderId], [ProductId], [Quantity], [UnitPrice])
    VALUES (1, 12500.0, 1, 1, 1, 12500.0),
    (2, 135000.0, 1, 2, 1, 135000.0),
    (3, 135000.0, 2, 2, 1, 135000.0),
    (4, 15000.0, 2, 3, 1, 15000.0),
    (5, 15000.0, 3, 3, 1, 15000.0),
    (6, 54000.0, 3, 4, 1, 54000.0),
    (7, 54000.0, 4, 4, 1, 54000.0),
    (8, 10000.0, 4, 5, 1, 10000.0),
    (9, 10000.0, 5, 5, 1, 10000.0),
    (10, 6000.0, 5, 6, 1, 6000.0),
    (11, 6000.0, 6, 6, 1, 6000.0),
    (12, 11000.0, 6, 7, 1, 11000.0),
    (13, 11000.0, 7, 7, 1, 11000.0),
    (14, 14500.0, 7, 8, 1, 14500.0),
    (15, 14500.0, 8, 8, 1, 14500.0),
    (16, 34500.0, 8, 9, 1, 34500.0),
    (17, 34500.0, 9, 9, 1, 34500.0),
    (18, 39000.0, 9, 10, 1, 39000.0),
    (19, 39000.0, 10, 10, 1, 39000.0),
    (20, 28000.0, 10, 11, 1, 28000.0),
    (21, 28000.0, 11, 11, 1, 28000.0),
    (22, 26000.0, 11, 12, 1, 26000.0),
    (23, 26000.0, 12, 12, 1, 26000.0),
    (24, 4500.0, 12, 13, 1, 4500.0),
    (25, 4500.0, 13, 13, 1, 4500.0),
    (26, 8500.0, 13, 14, 1, 8500.0),
    (27, 8500.0, 14, 14, 1, 8500.0),
    (28, 14000.0, 14, 15, 1, 14000.0),
    (29, 14000.0, 15, 15, 1, 14000.0),
    (30, 125000.0, 15, 16, 1, 125000.0)');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'OrderDetailId', N'LineTotal', N'OrderId', N'ProductId', N'Quantity', N'UnitPrice') AND [object_id] = OBJECT_ID(N'[OrderDetails]'))
        SET IDENTITY_INSERT [OrderDetails] OFF;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261021125549_InitBuoi4SixTablesAndSeedData'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Categories_CategoryName] ON [Categories] ([CategoryName]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261021125549_InitBuoi4SixTablesAndSeedData'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Customers_PhoneNumber] ON [Customers] ([PhoneNumber]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261021125549_InitBuoi4SixTablesAndSeedData'
)
BEGIN
    CREATE INDEX [IX_OrderDetails_OrderId] ON [OrderDetails] ([OrderId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261021125549_InitBuoi4SixTablesAndSeedData'
)
BEGIN
    CREATE INDEX [IX_OrderDetails_ProductId] ON [OrderDetails] ([ProductId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261021125549_InitBuoi4SixTablesAndSeedData'
)
BEGIN
    CREATE INDEX [IX_Orders_CustomerId] ON [Orders] ([CustomerId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261021125549_InitBuoi4SixTablesAndSeedData'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Orders_OrderCode] ON [Orders] ([OrderCode]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261021125549_InitBuoi4SixTablesAndSeedData'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Products_Barcode] ON [Products] ([Barcode]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261021125549_InitBuoi4SixTablesAndSeedData'
)
BEGIN
    CREATE INDEX [IX_Products_CategoryId] ON [Products] ([CategoryId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261021125549_InitBuoi4SixTablesAndSeedData'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Users_Username] ON [Users] ([Username]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261021125549_InitBuoi4SixTablesAndSeedData'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261021125549_InitBuoi4SixTablesAndSeedData', N'8.0.11');
END;
GO

COMMIT;
GO

-- BUOI 5: ADO.NET Stored Procedures cho bao cao doanh thu.
-- Chay ca file tren CSDL moi hoac CSDL Buoi 4; CREATE OR ALTER giup chay lai an toan.
CREATE OR ALTER PROCEDURE dbo.sp_GetRevenueByCashier
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        CashierUsername,
        COUNT(*) AS TotalOrders,
        COALESCE(SUM(FinalAmount), 0) AS TotalRevenue,
        COALESCE(SUM(RewardPoints), 0) AS TotalPointsAwarded
    FROM dbo.Orders
    GROUP BY CashierUsername
    ORDER BY TotalRevenue DESC, CashierUsername;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_GetRevenueByCashierByDateRange
    @StartDate datetime2,
    @EndDateExclusive datetime2
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        CashierUsername,
        COUNT(*) AS TotalOrders,
        COALESCE(SUM(FinalAmount), 0) AS TotalRevenue,
        COALESCE(SUM(RewardPoints), 0) AS TotalPointsAwarded
    FROM dbo.Orders
    WHERE CreatedDate >= @StartDate AND CreatedDate < @EndDateExclusive
    GROUP BY CashierUsername
    ORDER BY TotalRevenue DESC, CashierUsername;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_GetTopSellingProducts
AS
BEGIN
    SET NOCOUNT ON;
    SELECT TOP (5)
        p.ProductId,
        p.ProductName,
        SUM(d.Quantity) AS QuantitySold,
        COALESCE(SUM(d.LineTotal), 0) AS Revenue
    FROM dbo.OrderDetails AS d
    INNER JOIN dbo.Products AS p ON p.ProductId = d.ProductId
    GROUP BY p.ProductId, p.ProductName
    ORDER BY QuantitySold DESC, Revenue DESC, p.ProductName;
END;
GO

