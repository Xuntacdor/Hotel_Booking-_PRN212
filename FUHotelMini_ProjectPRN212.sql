use [master]
go
/*******************************************************************************
   Drop database if it exists
********************************************************************************/
IF EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = N'FUMiniHotel_ProjectPRN212')
BEGIN
	ALTER DATABASE [FUMiniHotel_ProjectPRN212] SET OFFLINE WITH ROLLBACK IMMEDIATE;
	ALTER DATABASE [FUMiniHotel_ProjectPRN212] SET ONLINE;
	DROP DATABASE [FUMiniHotel_ProjectPRN212];
END

GO
create database [FUMiniHotel_ProjectPRN212]
go
use [FUMiniHotel_ProjectPRN212]
go
CREATE TABLE [Users](
    UserID INT PRIMARY KEY IDENTITY(1,1), 
    Email NVARCHAR(255) UNIQUE NOT NULL, 
    Password NVARCHAR(255) NOT NULL, 
    Role NVARCHAR(50) NOT NULL,
    Status NVARCHAR(50) NOT NULL
);
CREATE TABLE [Customers](
    CustomerID INT PRIMARY KEY IDENTITY(1,1),
    UserID INT FOREIGN KEY REFERENCES [Users](UserID), 
    FullName NVARCHAR(255) NOT NULL, 
    Telephone NVARCHAR(20), 
    Email NVARCHAR(255), 
    Birthday DATE, 
    Status NVARCHAR(50) NOT NULL 
);

CREATE TABLE [Admin] (
    AdminID INT PRIMARY KEY IDENTITY(1,1), 
    UserID INT FOREIGN KEY REFERENCES [Users](UserID), 
    FullName NVARCHAR(255) NOT NULL, 
    Telephone NVARCHAR(20),
    Email NVARCHAR(255)
);
CREATE TABLE [EmployeeRoles] (
    RoleID INT PRIMARY KEY IDENTITY(1,1), -- Khóa chính, tự động tăng
    RoleName NVARCHAR(255) NOT NULL, -- Tên vai trò (ví dụ: Quản lý, Lễ tân)
    Description NVARCHAR(MAX) -- Mô tả vai trò
);
CREATE TABLE [Employee] (
    EmployeeID INT PRIMARY KEY IDENTITY(1,1), -- Khóa chính, tự động tăng
    UserID INT FOREIGN KEY REFERENCES [Users](UserID), -- Liên kết với bảng User
    RoleID INT FOREIGN KEY REFERENCES [EmployeeRoles](RoleID), -- Liên kết với bảng EmployeeRole
    FullName NVARCHAR(255) NOT NULL, -- Tên đầy đủ
    Telephone NVARCHAR(20), -- Số điện thoại
    Email NVARCHAR(255), -- Email
    HireDate DATE NOT NULL, -- Ngày bắt đầu làm việc
    Salary DECIMAL(18, 2), -- Lương
    Status NVARCHAR(50) NOT NULL -- Trạng thái (Active, Inactive)
);

CREATE TABLE [RoomType] (
    RoomTypeID INT PRIMARY KEY IDENTITY(1,1), -- Khóa chính, tự động tăng
    TypeName NVARCHAR(255) NOT NULL, -- Tên loại phòng
    Description NVARCHAR(MAX), -- Mô tả loại phòng
    Note NVARCHAR(MAX) -- Ghi chú
);
CREATE TABLE [Rooms] (
    RoomID INT PRIMARY KEY IDENTITY(1,1), -- Khóa chính, tự động tăng
    RoomNumber NVARCHAR(50) NOT NULL, -- Số phòng
    Description NVARCHAR(MAX), -- Mô tả phòng
    MaxCapacity INT NOT NULL, -- Sức chứa tối đa
    RoomTypeID INT FOREIGN KEY REFERENCES RoomType(RoomTypeID), -- Liên kết với bảng RoomType
    Status NVARCHAR(50) NOT NULL, -- Trạng thái (Available, Booked, Under Maintenance)
    PricePerDay DECIMAL(18, 2) NOT NULL -- Giá phòng mỗi ngày
);
CREATE TABLE [Bookings] (
    BookingID INT PRIMARY KEY IDENTITY(1,1), -- Khóa chính, tự động tăng
    CustomerID INT FOREIGN KEY REFERENCES [Customers](CustomerID), -- Liên kết với bảng Customer
    BookingDate DATETIME NOT NULL, -- Ngày đặt phòng
    TotalPrice DECIMAL(18, 2) NOT NULL, -- Tổng giá tiền
    Status NVARCHAR(50) NOT NULL -- Trạng thái (Pending, Confirmed, Cancelled)
);
CREATE TABLE [BookingDetails] (
    BookingDetailID INT PRIMARY KEY IDENTITY(1,1), -- Khóa chính, tự động tăng
    BookingID INT FOREIGN KEY REFERENCES [Bookings](BookingID), -- Liên kết với bảng Booking
    RoomID INT FOREIGN KEY REFERENCES [Rooms](RoomID), -- Liên kết với bảng Room
    StartDate DATE NOT NULL, -- Ngày bắt đầu
    EndDate DATE NOT NULL, -- Ngày kết thúc
    ActualPrice DECIMAL(18, 2) NOT NULL -- Giá thực tế
);
CREATE TABLE [Services] (
    ServiceID INT PRIMARY KEY IDENTITY(1,1), -- Khóa chính, tự động tăng
    ServiceName NVARCHAR(255) NOT NULL, -- Tên dịch vụ
    Description NVARCHAR(MAX), -- Mô tả dịch vụ
    Price DECIMAL(18, 2) NOT NULL -- Giá dịch vụ
);
CREATE TABLE BookingService (
    BookingServiceID INT PRIMARY KEY IDENTITY(1,1), -- Khóa chính, tự động tăng
    BookingID INT FOREIGN KEY REFERENCES [Bookings](BookingID), -- Liên kết với bảng Booking
    ServiceID INT FOREIGN KEY REFERENCES [Services](ServiceID), -- Liên kết với bảng Service
    Quantity INT NOT NULL, -- Số lượng
    TotalPrice DECIMAL(18, 2) NOT NULL -- Tổng giá tiền
);
CREATE TABLE [Payment] (
    PaymentID INT PRIMARY KEY IDENTITY(1,1), -- Khóa chính, tự động tăng
    BookingID INT FOREIGN KEY REFERENCES [Bookings](BookingID), -- Liên kết với bảng Booking
    PaymentDate DATETIME NOT NULL, -- Ngày thanh toán
    Amount DECIMAL(18, 2) NOT NULL, -- Số tiền thanh toán
    PaymentMethod NVARCHAR(50) NOT NULL -- Phương thức thanh toán (Cash, Credit Card)
);
CREATE TABLE [Feedbacks] (
    FeedbackID INT PRIMARY KEY IDENTITY(1,1), -- Khóa chính, tự động tăng
    CustomerID INT FOREIGN KEY REFERENCES [Customers](CustomerID), -- Liên kết với bảng Customer
    BookingID INT FOREIGN KEY REFERENCES [Bookings](BookingID), -- Liên kết với bảng Booking
    FeedbackDate DATETIME NOT NULL, -- Ngày phản hồi
    Rating INT NOT NULL, -- Đánh giá (1-5)
    Comment NVARCHAR(MAX) -- Bình luận
);
CREATE TABLE Invoices (
    InvoiceID INT PRIMARY KEY IDENTITY(1,1), -- Khóa chính, tự động tăng
    BookingID INT FOREIGN KEY REFERENCES [Bookings](BookingID), -- Liên kết với bảng Booking
    CustomerID INT FOREIGN KEY REFERENCES [Customers](CustomerID), -- Liên kết với bảng Customer
    InvoiceDate DATETIME NOT NULL, -- Ngày tạo hóa đơn	
    TotalAmount DECIMAL(18, 2) NOT NULL, -- Tổng số tiền
    PaymentMethod NVARCHAR(50) NOT NULL, -- Phương thức thanh toán
    PaymentStatus NVARCHAR(50) NOT NULL, -- Trạng thái thanh toán (Paid, Unpaid)
    Notes NVARCHAR(MAX) -- Ghi chú
);

INSERT INTO [Users] ([Email], [Password], [Role], [Status])
VALUES 
('admin@fumini.com', 'admin123', 'Admin', 'Active'),
('nkl123@gmail.com', 'customer123', 'Customer', 'Active'),
('nta123@gmail.com', 'customer123', 'Customer', 'Active'),
('nht123@gmail.com', 'customer123', 'Customer', 'Active'),
('nlv123@gmail.com', 'employee123', 'Employee', 'Active'),
('nmk123@gmail.com', 'employee123', 'Employee', 'Active'),
('ntt123@gmail.com', 'employee123', 'Employee', 'Active'),
('dtp123@gmail.com', 'employee123', 'Employee', 'Active');
INSERT INTO [Customers] ([UserID], [FullName], [Telephone], [Email], [Birthday], [Status])
VALUES 
(2, N'Nguyễn Khánh Linh', '0912345678', 'nkl123@gmail.com', '2003-05-24', 'Active'),
(3, N'Nguyễn Tiến Anh ', '0987654323', 'nta123@gmail.com', '2003-10-19', 'Active'),
(4, N'Nguyễn Hữu Tiến ', '0987654324', 'nht123@gmail.com', '2003-12-04', 'Active');
INSERT INTO [Admin] (UserID, FullName, Telephone, Email)
VALUES 
(1, N'Phạm Ngọc Hà', '0909123456', 'admin@fumini.com');
INSERT INTO [EmployeeRoles] ([RoleName], [Description])
VALUES 
(N'Quản lý', N'Quản lý toàn bộ hoạt động của khách sạn'),
(N'Lễ tân', N'Tiếp nhận và hỗ trợ khách hàng'),
(N'Nhân viên dọn dẹp', N'Dọn dẹp vệ sinh');
INSERT INTO [Employee] ([UserID], [RoleID], [FullName], [Telephone], [Email], [HireDate], [Salary], [Status])
VALUES 
(5, 3, N'Nguyễn Long Vũ', '0987654325', 'nlv123@gmail.com', '2024-09-20', 10000000 , 'Active'),
(6, 3, N'Nguyễn Minh Khoa ', '0987654326', 'nmk123@gmail.com', '2023-04-16', 1000000, 'Active'),
(7, 1, N'Nguễn Thùy Trang', '0987654327', 'ntt123@gmail.com', '2024-10-20', 20000000, 'Active'),
(8, 2, N'Đỗ Thị Phượng ', '0987654328', 'dtp123@gmail.com', '2025-01-16', 15000000, 'Active');
INSERT INTO [RoomType] ([TypeName], [Description],[Note])
VALUES 
(N'Phòng Đơn', N'Phòng dành cho 1 người', N'Phòng nhỏ, tiện nghi cơ bản'),
(N'Phòng Đôi', N'Phòng dành cho 2 người', N'Phòng rộng rãi, tiện nghi đầy đủ'),
(N'Phòng VIP', N'Phòng cao cấp', N'Phòng sang trọng, tiện nghi cao cấp');
INSERT INTO [Rooms] ([RoomNumber], [Description], [MaxCapacity], [RoomTypeID], [Status], [PricePerDay])
VALUES 
('101', N'Phòng đơn view biển', 1, 1, 'Available', 500000),
('102', N'Phòng đôi view biển', 2, 2, 'Available', 800000),
('103', N'Phòng đơn thường', 1, 1, 'Available', 300000),
('201', N'Phòng VIP view biển', 2, 3, 'Available', 1200000),
('202', N'Phòng đôi view biển', 2, 2, 'Available', 800000),
('203', N'Phòng đôi thường', 2, 2, 'Available', 700000);
INSERT INTO [Bookings] (CustomerID, BookingDate, TotalPrice, Status)
VALUES 
(1, '2023-10-01 10:00:00', 1300000, 'Confirmed'),
(2, '2023-10-02 11:00:00', 2700000, 'Confirmed');
INSERT INTO [BookingDetails] (BookingID, RoomID, StartDate, EndDate, ActualPrice)
VALUES 
(1, 1, '2023-10-05', '2023-10-07', 1000000),
(2, 2, '2023-10-06', '2023-10-09', 2400000);
INSERT INTO [Services] (ServiceName, Description, Price)
VALUES 
(N'Dịch vụ ăn sáng', N'Bữa sáng buffet', 150000),
(N'Dịch vụ giặt ủi', N'Giặt ủi quần áo', 100000),
(N'Dịch vụ spa', N'Massage thư giãn', 300000);
INSERT INTO [BookingService] (BookingID, ServiceID, Quantity, TotalPrice)
VALUES 
(1, 1, 2, 300000),
(2, 3, 1, 300000);
INSERT INTO [Payment] (BookingID, PaymentDate, Amount, PaymentMethod)
VALUES 
(1, '2023-10-01 10:30:00', 1300000, 'Credit Card'),
(2, '2023-10-02 11:30:00', 2700000, 'Cash');
INSERT INTO [Feedbacks] (CustomerID, BookingID, FeedbackDate, Rating, Comment)
VALUES 
(1, 1, '2023-10-07 12:00:00', 5, N'Phòng sạch sẽ, dịch vụ tốt'),
(2, 2, '2023-10-09 13:00:00', 4, N'Phòng đẹp, nhân viên thân thiện');
INSERT INTO [Invoices] (BookingID, CustomerID, InvoiceDate, TotalAmount, PaymentMethod, PaymentStatus, Notes)
VALUES 
(1, 1, '2023-10-07 12:30:00', 1300000, 'Credit Card', 'Paid', N'Thanh toán đầy đủ'),
(2, 2, '2023-10-09 13:30:00', 2700000, 'Cash', 'Paid', N'Thanh toán đầy đủ');