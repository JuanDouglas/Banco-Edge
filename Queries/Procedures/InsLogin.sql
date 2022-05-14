USE [Banco Edge];
GO

CREATE PROC InserirLogin 
@Token VARCHAR(150),
@IP VARBINARY(4),
@ContaId INTEGER
AS 
BEGIN 
    INSERT INTO [Cliente]([Token], [IP], [ContaId])
    VALUES (@Token, @IP, @ContaId)
END