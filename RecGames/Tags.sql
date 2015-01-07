CREATE TABLE [dbo].[Tags] (
    [Id]  INT          NOT NULL,
    [Tag] VARCHAR (40) NOT NULL UNIQUE,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

