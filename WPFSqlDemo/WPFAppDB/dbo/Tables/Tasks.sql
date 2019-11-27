CREATE TABLE [dbo].[Tasks]
(
	[TaskId] INT NOT NULL PRIMARY KEY IDENTITY, 
    [Description] NVARCHAR(50) NULL, 
    [Done] BIT NULL, 
    [DateCompleted] DATE NULL, 
    [CategoryId] INT NULL, 
    [UserId] INT NULL, 
    CONSTRAINT [CategoryId] FOREIGN KEY ([CategoryId]) REFERENCES [Categories]([CategoryId]), 
    CONSTRAINT [UserId] FOREIGN KEY ([UserId]) REFERENCES [Users]([UserId]) 
)
