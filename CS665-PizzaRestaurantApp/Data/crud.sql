-- Read: Get all customers
SELECT * FROM CustomerModels;

-- Read: Get all orders with customer information
SELECT o.OrderID, c.Name, o.OrderDate, o.TotalAmount
FROM OrderModels o
JOIN CustomerModels c ON o.CustomerID = c.CustomerID;

-- Read: Get order details with item names
SELECT od.OrderDetailID, o.OrderID, m.Name, od.Quantity, od.UnitPrice
FROM OrderDetailModels od
JOIN OrderModels o ON od.OrderID = o.OrderID
JOIN MenuItemModels m ON od.ItemID = m.ItemID;

-- Update: Change a customer's phone number
UPDATE CustomerModels
SET Phone = '555-555-5555'
WHERE Name = 'John Doe';

-- Update: Change an order's total amount
UPDATE OrderModels
SET TotalAmount = 30.99
WHERE OrderID = 1;

-- Delete: Remove a specific order (deletes related order details due to cascade)
DELETE FROM OrderModels WHERE OrderID = 2;
