CREATE TABLE [dbo].[Students] (
    [StudentId]  INT            IDENTITY (1, 1) NOT NULL,
    [Phone]      NVARCHAR (20)  NOT NULL,
    [BirthDate]  DATETIME       NOT NULL,
    [Gender]     NVARCHAR (10)  NOT NULL,
    [CourseId]   NVARCHAR (10)  NOT NULL,
    [CourseName] NVARCHAR (100) NOT NULL,
    [IsPaid]     BIT            NOT NULL,
    PRIMARY KEY CLUSTERED ([StudentId] ASC)
);