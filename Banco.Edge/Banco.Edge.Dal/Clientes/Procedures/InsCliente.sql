CREATE PROC InsertCliente
@Nome VARCHAR(100) NOT NULL,
@Email VARCHAR(500) NOT NULL,
@CpfOrCnpj VARCHAR(14) NOT NULL
AS
BEGIN
	INSERT INTO [Cliente]([Nome],[Email],[CpfOrCnpj]) 
	VALUES (@Nome,@Email,@CpfOrCnpj)
END