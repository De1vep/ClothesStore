-- Tạo cơ sở dữ liệu
CREATE DATABASE ClothesStore;
GO

USE ClothesStore;
GO

-- Sử dụng IDENTITY để tạo ID tự tăng cho các bảng
CREATE TABLE Category (
  CategoryID INT PRIMARY KEY IDENTITY(1,1),
  CategoryName VARCHAR(50) NOT NULL
);

CREATE TABLE Product (
  ProductID INT PRIMARY KEY IDENTITY(1,1),
  ProductName VARCHAR(100) NOT NULL,
  ProductDescription VARCHAR(500),
  Price DECIMAL(10,2) NOT NULL,
  Quantity INT NOT NULL,
  CategoryID INT NOT NULL,
  ProductImage VARCHAR(500),
  FOREIGN KEY (CategoryID) REFERENCES Category(CategoryID)
);

CREATE TABLE Users (
  UserID INT PRIMARY KEY IDENTITY(1,1),
  Fullname VARCHAR(50) NOT NULL,
  Email VARCHAR(100) NOT NULL,
  Password VARCHAR(100) NOT NULL,
  Role VARCHAR(10) NOT NULL
);

CREATE TABLE Orders (
  OrderID INT PRIMARY KEY IDENTITY(1,1),
  UserID INT NOT NULL,
  OrderDate DATE NOT NULL,
  ShippingAddress VARCHAR(200) NOT NULL,
  PhoneNumber VARCHAR(20) NOT NULL,
  TotalAmount DECIMAL(10,2) NOT NULL,
  OrderStatus VARCHAR(20) NOT NULL,
  FOREIGN KEY (UserID) REFERENCES Users(UserID)
);

CREATE TABLE OrderDetails (
  OrderDetailID INT PRIMARY KEY IDENTITY(1,1),
  OrderID INT NOT NULL,
  ProductID INT NOT NULL,
  Quantity INT NOT NULL,
  UnitPrice DECIMAL(10,2) NOT NULL,
  FOREIGN KEY (OrderID) REFERENCES Orders(OrderID),
  FOREIGN KEY (ProductID) REFERENCES Product(ProductID)
);
CREATE TABLE Cart (
  CartID INT PRIMARY KEY IDENTITY(1,1),
  UserID INT NOT NULL,
  ProductID INT NOT NULL,
  Quantity INT NOT NULL,
  FOREIGN KEY (UserID) REFERENCES Users(UserID),
  FOREIGN KEY (ProductID) REFERENCES Product(ProductID)
);
CREATE TABLE Review (
  ReviewID INT PRIMARY KEY IDENTITY(1,1),
  UserID INT NOT NULL,
  ProductID INT NOT NULL,
  Rating INT NOT NULL CHECK (Rating >= 1 AND Rating <= 5),
  ReviewText VARCHAR(1000),
  ReviewDate DATETIME NOT NULL DEFAULT GETDATE(),
  FOREIGN KEY (UserID) REFERENCES Users(UserID),
  FOREIGN KEY (ProductID) REFERENCES Product(ProductID)
);
-- Thêm dữ liệu vào bảng Danh Mục
INSERT INTO Category (CategoryName)
VALUES
  ('Điện thoại'),
  ('Máy tính'),
  ('Thiết bị gia dụng');

-- Thêm dữ liệu vào bảng Sản Phẩm
INSERT INTO Product (ProductName, ProductDescription, Price, Quantity, CategoryID, ProductImage)
VALUES
  ('iPhone 12', 'Điện thoại thông minh', 19999000, 50, 1, 'ao1.jpg'),
  ('Samsung Galaxy S21', 'Điện thoại thông minh', 17999000, 30, 1, 'ao1.jpg'),
  ('MacBook Pro', 'Máy tính xách tay', 34999000, 20, 2, 'ao1.jpg'),
  ('HP Pavilion', 'Máy tính xách tay', 15999000, 25, 2, 'ao2.jpg'),
  ('Tủ lạnh LG', 'Thiết bị gia dụng', 8999000, 15, 3, 'ao2.jpg'),
  ('Máy giặt Electrolux', 'Thiết bị gia dụng', 6999000, 10, 3, 'ao2.jpg');

-- Thêm dữ liệu vào bảng Người Dùng
INSERT INTO Users (Fullname, Email, Password, Role)
VALUES
('Thang', 'thangs@mail.com', '12345678', 'customer'),
('Thang', 'thang@mail.com', '12345678', 'admin'),
  ('John Doe', 'john.doe@example.com', 'password123', 'admin'),
  ('Jane Smith', 'jane.smith@example.com', 'abc456', 'customer'),
  ('Bob Johnson', 'bob.johnson@example.com', 'xyz789', 'customer');

-- Thêm dữ liệu vào bảng Đơn Hàng
INSERT INTO Orders (UserID, OrderDate, ShippingAddress, PhoneNumber, TotalAmount, OrderStatus)
VALUES
  (2, '2024-06-01', '123 Main St, Anytown USA', '555-1234', 19999000, 'Delivered'),
  (3, '2024-06-15', '456 Oak Rd, Somewhere City', '555-5678', 34999000, 'Shipped'),
  (2, '2024-07-01', '789 Elm Ave, Othertown', '555-9012', 15999000, 'Pending');

-- Thêm dữ liệu vào bảng Chi Tiết Đơn Hàng
INSERT INTO OrderDetails (OrderID, ProductID, Quantity, UnitPrice)
VALUES
  (1, 1, 1, 19999000),
  (2, 3, 1, 34999000),
  (3, 4, 1, 15999000);
  INSERT INTO Review (UserID, ProductID, Rating, ReviewText) VALUES 
(1, 1, 5, 'Excellent product, highly recommend!'),
(2, 2, 4, 'Very good, but could be improved.'),
(3, 3, 3, 'Average product, not bad but not great.'),
(4, 4, 2, 'Below expectations, would not buy again.'),
(5, 5, 1, 'Terrible product, do not buy!'),
(1, 6, 5, 'Fantastic quality, will buy again.'),
(2, 1, 4, 'Good value for the price.'),
(3, 2, 3, 'It’s okay, nothing special.'),
(4, 3, 2, 'Not very satisfied with this purchase.'),
(5, 4, 1, 'Very disappointed, poor quality.');