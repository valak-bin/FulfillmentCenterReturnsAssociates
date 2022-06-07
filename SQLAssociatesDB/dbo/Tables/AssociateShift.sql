CREATE TABLE [dbo].[AssociateShift]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [AssociateId] INT NOT NULL, 
    [ShiftId] INT NOT NULL
)
