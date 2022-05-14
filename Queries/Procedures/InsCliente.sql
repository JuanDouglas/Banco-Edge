USE [Banco Edge];
GO

CREATE PROC InserirCliente
@Nome VARCHAR(100),
@Email VARCHAR(500),
@Telefone VARCHAR(15),
@CpfOuCnpj VARCHAR(14),
@Senha VARCHAR(90)
AS
BEGIN
	DECLARE @Id INTEGER;

	INSERT INTO [Cliente]([Nome], [Telefone], [Email], [CpfOuCnpj], [Senha]) 
	VALUES (@Nome, @Telefone, @Email, @CpfOuCnpj, @Senha)

	SET @Id = SCOPE_IDENTITY();
	
	INSERT INTO [Conta]([Dono], [Criacao], [Tipo])
	VALUES (@Id, GETDATE(), 1)
	
	SELECT @Id;
END