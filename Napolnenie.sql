INSERT INTO Users (UserName, Email, RegistrationDate) VALUES
('Иван Петров',  'ivan@example.com',  '2024-01-15'),
('Мария Сидорова','maria@example.com','2024-02-20');

INSERT INTO Products (ProductName, Description, Price, QuantityInStock) VALUES
('Ноутбук Pro 15',   'Игровой ноутбук',       120000.00, 10),
('Наушники Air',     'Беспроводные наушники',   8990.00,  3),
('Клавиатура Mech',  'Механическая клавиатура', 6500.00, 15),
('Мышка Wireless',   'Беспроводная мышь',       2500.00,  2),
('Монитор 27"',      'IPS монитор 144 Гц',     25000.00,  7),
('Кабель USB-C',     'Кабель 2 м',               590.00, 50);

INSERT INTO Orders (UserID, OrderDate, Status) VALUES
(1, '2024-03-01', 'Completed'),
(1, '2024-03-10', 'Processing'),
(2, '2024-03-12', 'New');

INSERT INTO OrderDetails (OrderID, ProductID, Quantity, TotalCost) VALUES
(1, 1, 1, 120000.00),
(1, 3, 2,  13000.00),
(2, 5, 1,  25000.00),
(3, 2, 1,   8990.00),
(3, 6, 3,   1770.00);