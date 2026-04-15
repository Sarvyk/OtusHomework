-- Все задачи пользователя с id = 1
SELECT * FROM "ToDoItem" tdi WHERE tdi."UserId" = 1;

-- Невыполненные задачи (ItemState = 0) пользователя с id = 2
SELECT * FROM "ToDoItem" tdi WHERE tdi."UserId" = 2 AND tdi."ItemState" = 0;

-- Выполненные задачи (ItemState = 1) пользователя с id = 1
SELECT * FROM "ToDoItem" tdi WHERE tdi."UserId" = 1 AND tdi."ItemState" = 1;

-- Задача по id
SELECT * FROM "ToDoItem" tdi WHERE tdi.id = 1;

-- Задача по имени
SELECT * FROM "ToDoItem" tdi WHERE tdi."ItemName" = 'testItem';

-- Количество невыполненных задач пользователя с id = 2
SELECT COUNT(*) FROM "ToDoItem" tdi WHERE tdi."UserId" = 2 AND tdi."ItemState" = 0;