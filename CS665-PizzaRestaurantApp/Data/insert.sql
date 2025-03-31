-- Insert sample customers
INSERT INTO CustomerModels (Name, Phone, Email, Address) VALUES
('John Doe', '123-456-7890', 'johndoe@example.com', '123 Main St'),
('Jane Smith', '987-654-3210', 'janesmith@example.com', '456 Elm St');

-- Insert sample menu items
INSERT INTO MenuItemModels (Name, Price, Description) VALUES
('Pepperoni Pizza', 12.99, 'Classic pepperoni pizza with mozzarella cheese'),
('Cheese Pizza', 10.99, 'Plain cheese pizza with tomato sauce and mozzarella'),
('Veggie Pizza', 11.99, 'Vegetarian pizza with bell peppers, olives, and onions');

-- Insert sample orders
INSERT INTO OrderModels (CustomerID, OrderDate, TotalAmount) VALUES
(1, '2025-03-31', 25.98),
(2, '2025-03-31', 10.99);

-- Insert sample order details
INSERT INTO OrderDetailModels (OrderID, ItemID, Quantity, UnitPrice) VALUES
(1, 1, 1, 12.99),
(1, 3, 1, 12.99),
(2, 2, 1, 10.99);
