IF OBJECT_ID('dbo.Users', 'U') IS NOT NULL 
    DROP TABLE dbo.Users;

CREATE TABLE [dbo].[Users] (
    [UserId]       INT            IDENTITY (1, 1) NOT NULL,
    [Username]     NVARCHAR (50)  NOT NULL,
    [PasswordHash] NVARCHAR (256) NOT NULL,
    [FullName]     NVARCHAR (100) NULL,
    PRIMARY KEY CLUSTERED ([UserId] ASC)
);