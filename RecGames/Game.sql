CREATE TABLE Game
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [Name] VARCHAR(50) NOT NULL, 
    [Controller_support] VARCHAR(50) NULL, 
    [Platforms] VARCHAR(50) NULL, 
    [Developers] VARCHAR(50) NULL, 
    [Publishers] VARCHAR(50) NULL, 
    [Recommendations] INT NULL, 
    [Metacritic] INT NULL, 
    [Tags_id] NCHAR(10) NULL, 
    [Id_price] INT NULL,
	CONSTRAINT [FK_Price_Game] FOREIGN KEY (Id_price) REFERENCES Price(Id)
)
