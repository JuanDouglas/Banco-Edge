USE [Banco Edge];
GO

CREATE PROC ExcluirCliente
@IdCliente INTEGER
AS 
BEGIN 
	DECLARE @Contas TABLE ([Id] INTEGER);

	INSERT INTO @Contas 
	SELECT [Id] FROM [Conta]
	WHERE [Dono] = @IdCliente

    DELETE FROM [Transacao]
    WHERE [De] IN(SELECT [Id] FROM @Contas) OR 
		  [Para] IN(SELECT [Id] FROM @Contas)  

	DELETE FROM [Conta]
	WHERE [Dono] = @IdCliente

	DELETE FROM [Cliente]
	WHERE [Id] = @IdCliente
END