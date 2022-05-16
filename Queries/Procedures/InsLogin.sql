USE [Banco Edge];
GO

CREATE PROC InserirLogin 
@Token VARCHAR(96),
@IP VARBINARY(6),
@ClienteId INTEGER
AS 
BEGIN 
    INSERT INTO [Login]([Token], [IP], [ClienteId], [Data])
    VALUES (@Token, @IP, @ClienteId, GETDATE())
END