USE [Banco Edge];
GO

CREATE PROC ListarClientes
@IdInicial INTEGER,
@Maximo INTEGER = 200
AS
BEGIN
	SELECT TOP (@Maximo) * FROM [Cliente]
	WHERE [Id] >= @IdInicial
END 