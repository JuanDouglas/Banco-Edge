USE [Banco Edge];
GO

CREATE PROC BuscaCliente
@Skip INTEGER,
@Take INTEGER,
@Id INT = NULL,
@Email VARCHAR(100) = NULL,
@CpfOuCnpj VARCHAR(14) = NULL
AS
IF ISNULL(@Id, 0) != 0 OR @CpfOuCnpj IS NOT NULL OR @Email IS NOT NULL 
	BEGIN
		IF ISNULL(@Id, 0) != 0 
			SELECT TOP(@Take) * FROM [Cliente]
			WHERE [Id] = @Id;

		IF @Email IS NOT NULL 
			SELECT TOP(@Take) * FROM [Cliente]
			WHERE [Email] = @Email;

		IF @CpfOuCnpj IS NOT NULL 
			SELECT TOP(@Take) * FROM [Cliente]
			WHERE [CpfOuCnpj] = @CpfOuCnpj;
	END
ELSE 
	BEGIN 
		SELECT * FROM [Cliente]
		ORDER BY [Id]
		OFFSET (@Skip) ROWS FETCH NEXT (@Take) ROWS ONLY;
	END