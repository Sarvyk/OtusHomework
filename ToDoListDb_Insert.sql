INSERT INTO "ToDoUser" ("Telegram_UserId", "Telegram_UserName") VALUES (2,'test'), (3,'test3');
INSERT INTO "ToDoList" ("ListName", "UserId") VALUES ('testList', 1),('listTest2', 2);
INSERT INTO "ToDoItem" ("ItemName", "ItemState", "DeadLine", "UserId", "ToDoList") 
VALUES ('testItem',1, '2026-04-05',1,1), ('testItem2',0, '2026-04-15',2,2);