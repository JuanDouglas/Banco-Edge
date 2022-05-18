USE [Banco Edge];
GO

CREATE PROC InserirConta
@Tipo TINYINT = NULL,
@DonoId INTEGER
AS 
BEGIN 
    INSERT INTO [Conta]([Dono], [Tipo], [Criacao])
    VALUES (@DonoId, @Tipo, GETDATE())
END