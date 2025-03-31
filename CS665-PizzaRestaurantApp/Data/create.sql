-- Create Customers table
CREATE TABLE CustomerModels (
    CustomerID INTEGER PRIMARY KEY AUTOINCREMENT,
    Name TEXT NOT NULL,
    Phone TEXT NOT NULL,
    Email TEXT NOT NULL,
    Address TEXT NOT NULL
);

-- Create Menu Items table
CREATE TABLE MenuItemModels (
    ItemID INTEGER PRIMARY KEY AUTOINCREMENT,
    Name TEXT NOT NULL,
    Price DECIMAL(10,2) NOT NULL,
    Description TEXT NOT NULL
);

-- Create Orders table
CREATE TABLE OrderModels (
    OrderID INTEGER PRIMARY KEY AUTOINCREMENT,
    CustomerID INTEGER NOT NULL,
    OrderDate TEXT NOT NULL,
    TotalAmount DECIMAL(10,2) NOT NULL,
    FOREIGN KEY (CustomerID) REFERENCES CustomerModels(CustomerID) ON DELETE CASCADE
);

-- Create Order Details table
CREATE TABLE OrderDetailModels (
    OrderDetailID INTEGER PRIMARY KEY AUTOINCREMENT,
    OrderID INTEGER NOT NULL,
    ItemID INTEGER NOT NULL,
    Quantity INTEGER NOT NULL,
    UnitPrice DECIMAL(10,2) NOT NULL,
    FOREIGN KEY (OrderID) REFERENCES OrderModels(OrderID) ON DELETE CASCADE,
    FOREIGN KEY (ItemID) REFERENCES MenuItemModels(ItemID) ON DELETE CASCADE
);
