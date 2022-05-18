USE [Banco Edge];
GO

CREATE PROC InserirConta
@Tipo TINYINT = NULL,
@ClienteId INTEGER
AS 
BEGIN 
    INSERT INTO [Conta]([Dono],[Tipo],[Criacao])
    VALUES (@ClienteId,@Tipo,GETDATE())
END