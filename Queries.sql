INSERT INTO Products (ProductName, Description, Price, QuantityInStock)
VALUES ('Смартфон Galaxy X', 'Флагманский смартфон с 6.7" экраном', 79999.00, 25);

UPDATE Products
SET Price = 74999.00
WHERE ProductID = 1;

SELECT o.OrderID, o.OrderDate, o.Status
FROM Orders o
JOIN Users u ON u.UserID = o.UserID
WHERE u.UserID = 1
ORDER BY o.OrderDate DESC;

SELECT od.OrderID,
       SUM(od.TotalCost) AS OrderTotal
FROM OrderDetails od
WHERE od.OrderID = 1
GROUP BY od.OrderID;

SELECT SUM(QuantityInStock) AS TotalItemsInStock
FROM Products;

SELECT ProductID, ProductName, Price
FROM Products
ORDER BY Price DESC
LIMIT 5;

SELECT ProductID, ProductName, QuantityInStock
FROM Products
WHERE QuantityInStock < 5
ORDER BY QuantityInStock ASC;