CREATE TABLE "ToDoUser" (
    id SERIAL PRIMARY KEY,
    "external_id" UUID NOT NULL DEFAULT gen_random_uuid(),
    "Telegram_UserId" BIGINT NOT NULL,
    "Telegram_UserName" VARCHAR NOT NULL,
    "Registered_At" TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Таблица ToDoList
CREATE TABLE "ToDoList" (
    id SERIAL PRIMARY KEY,
    "external_id" UUID NOT NULL DEFAULT gen_random_uuid(),
    "ListName" VARCHAR NOT NULL,
    "CreatedAt" TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    "UserId" INTEGER NOT NULL REFERENCES "ToDoUser"(id)
);

-- Таблица ToDoItem
CREATE TABLE "ToDoItem" (
    id SERIAL PRIMARY KEY,
    "external_id" UUID NOT NULL DEFAULT gen_random_uuid(),
    "ItemName" VARCHAR NOT NULL,
    "CreatedAt" TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    "ItemState" INTEGER NOT NULL,
    "DeadLine" TIMESTAMP NOT NULL,
    "StateChangedAt" TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    "UserId" INTEGER NOT NULL REFERENCES "ToDoUser"(id),
    "ToDoListId" INTEGER NULL REFERENCES "ToDoList"(id)
);

--Таблица Notification
CREATE TABLE "Notification" (
    id SERIAL PRIMARY KEY,
    "External_id" UUID NOT NULL DEFAULT gen_random_uuid(),
    "User_id" INTEGER NOT NULL REFERENCES "ToDoUser"(id),
    "Type" VARCHAR NOT NULL,
    "Text" VARCHAR NOT NULL,
    "Scheduled_at" TIMESTAMP NOT NULL,
    "Is_notified" BOOLEAN NOT NULL DEFAULT FALSE,
    "Notified_at" TIMESTAMP NULL
);

-- Индексы
CREATE INDEX "idx_ToDoList_userid" ON "ToDoList"("UserId");
CREATE INDEX "idx_ToDoItem_userid" ON "ToDoItem"("UserId");
CREATE INDEX "idx_ToDoItem_ToDoList" ON "ToDoItem"("ToDoListId");
CREATE INDEX idx_notification_scheduled_at ON "Notification"("Scheduled_at") WHERE "Is_notified" = FALSE;
CREATE INDEX idx_notification_user_unnotified ON "Notification"("User_id", "Scheduled_at") WHERE "Is_notified" = FALSE;

-- Уникальные индексы
CREATE UNIQUE INDEX "uq_ToDoUser_telegramuserid" ON "ToDoUser"("Telegram_UserId");
CREATE UNIQUE INDEX "uq_ToDoUser_external_id" ON "ToDoUser"("external_id");
CREATE UNIQUE INDEX "uq_ToDoList_external_id" ON "ToDoList"("external_id");
CREATE UNIQUE INDEX "uq_ToDoItem_external_id" ON "ToDoItem"("external_id");