select * from "ToDoItem" tdi where tdi."UserId" = 1;
select * from "ToDoItem" tdi where tdi."UserId" = 2 and tdi."ItemState" = 0;
select * from "ToDoItem" tdi where tdi."UserId" = 1 and tdi."ItemState" = 1;
select * from "ToDoItem" tdi where tdi.id = 1;
select * from "ToDoItem" tdi where tdi."ItemName" = 'testItem'
select count(*) from "ToDoItem" tdi where tdi."UserId" = 2 and tdi."ItemState" = 0;