CREATE TABLE Products (
    ProductID       SERIAL PRIMARY KEY,
    ProductName     VARCHAR(255) NOT NULL,
    Description     TEXT,
    Price           NUMERIC(10, 2) NOT NULL CHECK (Price >= 0),
    QuantityInStock INTEGER NOT NULL DEFAULT 0 CHECK (QuantityInStock >= 0)
);

CREATE TABLE Users (
    UserID           SERIAL PRIMARY KEY,
    UserName         VARCHAR(100) NOT NULL,
    Email            VARCHAR(255) NOT NULL UNIQUE,
    RegistrationDate DATE NOT NULL DEFAULT CURRENT_DATE
);

CREATE TABLE Orders (
    OrderID   SERIAL PRIMARY KEY,
    UserID    INTEGER NOT NULL,
    OrderDate DATE NOT NULL DEFAULT CURRENT_DATE,
    Status    VARCHAR(50) NOT NULL DEFAULT 'New',
    CONSTRAINT fk_orders_user
        FOREIGN KEY (UserID) REFERENCES Users(UserID)
        ON DELETE RESTRICT
        ON UPDATE CASCADE
);

CREATE TABLE OrderDetails (
    OrderDetailID SERIAL PRIMARY KEY,
    OrderID       INTEGER NOT NULL,
    ProductID     INTEGER NOT NULL,
    Quantity      INTEGER NOT NULL CHECK (Quantity > 0),
    TotalCost     NUMERIC(10, 2) NOT NULL CHECK (TotalCost >= 0),
    CONSTRAINT fk_orderdetails_order
        FOREIGN KEY (OrderID) REFERENCES Orders(OrderID)
        ON DELETE RESTRICT
        ON UPDATE CASCADE,
    CONSTRAINT fk_orderdetails_product
        FOREIGN KEY (ProductID) REFERENCES Products(ProductID)
        ON DELETE RESTRICT
        ON UPDATE CASCADE
);