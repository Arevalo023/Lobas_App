USE [LobasOrdersApp]
GO

IF COL_LENGTH('dbo.Users', 'LastNameChangedAt') IS NULL
BEGIN
    ALTER TABLE [dbo].[Users]
    ADD [LastNameChangedAt] [datetime] NULL;
END
GO
