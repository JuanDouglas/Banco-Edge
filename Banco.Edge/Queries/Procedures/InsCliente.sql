USE [Banco Edge];
GO

CREATE PROC InsereCliente
@Nome VARCHAR(100),
@Email VARCHAR(500),
@CpfOrCnpj VARCHAR(14)
AS
BEGIN
	INSERT INTO [Cliente]([Nome],[Email],[CpfOrCnpj]) 
	VALUES (@Nome,@Email,@CpfOrCnpj)
END