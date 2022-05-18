USE [Banco Edge];
GO

CREATE PROC BuscarContas
@DonoId INTEGER,
@Take INTEGER = 3,
@Tipo TINYINT = NULL
AS 
BEGIN 
    IF ISNULL(@Tipo, 0) = 0
        BEGIN 
            SELECT TOP (@Take) * FROM [Conta]
            WHERE [Dono] = @DonoId
        END 
    ELSE 
        BEGIN 
            SELECT TOP (@Take) * FROM [Conta]
            WHERE [Dono] = @DonoId AND
                  [Tipo] = @Tipo
        END
END