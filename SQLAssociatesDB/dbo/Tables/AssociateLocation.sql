CREATE TABLE [dbo].[AssociateLocation]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [AssociateId] INT NOT NULL, 
    [LocationId] INT NOT NULL
)
