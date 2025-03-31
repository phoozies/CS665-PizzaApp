-- Create Customers table
-- Functional Dependencies: 
-- CustomerID → Name, Phone, Email, Address 
-- (CustomerID uniquely determines all other attributes in the Customers table)
CREATE TABLE CustomerModels (
    CustomerID INTEGER PRIMARY KEY AUTOINCREMENT,
    Name TEXT NOT NULL,
    Phone TEXT NOT NULL,
    Email TEXT NOT NULL,
    Address TEXT NOT NULL
);

-- Create Menu Items table
-- Functional Dependencies: 
-- ItemID → Name, Price, Description 
-- (ItemID uniquely determines the menu item's name, price, and description)
CREATE TABLE MenuItemModels (
    ItemID INTEGER PRIMARY KEY AUTOINCREMENT,
    Name TEXT NOT NULL,
    Price DECIMAL(10,2) NOT NULL,
    Description TEXT NOT NULL
);

-- Create Orders table
-- Functional Dependencies: 
-- OrderID → CustomerID, OrderDate, TotalAmount 
-- (OrderID uniquely determines which customer placed the order, the date, and the total amount)
-- CustomerID → {OrderID} (1:N Relationship: A customer can have multiple orders)
CREATE TABLE OrderModels (
    OrderID INTEGER PRIMARY KEY AUTOINCREMENT,
    CustomerID INTEGER NOT NULL,
    OrderDate TEXT NOT NULL,
    TotalAmount DECIMAL(10,2) NOT NULL,
    FOREIGN KEY (CustomerID) REFERENCES CustomerModels(CustomerID) ON DELETE CASCADE
);

-- Create Order Details table (Junction Table for Many-to-Many Relationship between Orders and Menu Items)
-- Functional Dependencies: 
-- OrderDetailID → OrderID, ItemID, Quantity, UnitPrice 
-- (Each OrderDetailID uniquely identifies the associated order, menu item, quantity, and unit price)
-- (OrderID, ItemID) → Quantity, UnitPrice 
-- (An OrderID and ItemID together determine the quantity and unit price)
CREATE TABLE OrderDetailModels (
    OrderDetailID INTEGER PRIMARY KEY AUTOINCREMENT,
    OrderID INTEGER NOT NULL,
    ItemID INTEGER NOT NULL,
    Quantity INTEGER NOT NULL,
    UnitPrice DECIMAL(10,2) NOT NULL,
    FOREIGN KEY (OrderID) REFERENCES OrderModels(OrderID) ON DELETE CASCADE,
    FOREIGN KEY (ItemID) REFERENCES MenuItemModels(ItemID) ON DELETE CASCADE
);
