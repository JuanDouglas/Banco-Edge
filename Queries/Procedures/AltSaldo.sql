CREATE PROC NovaTransacao
/*
0 - Deposito
1 - Saque
2 - Transferencia 
3 - Reembolso
*/
@Tipo TINYINT,
@Valor MONEY,
@De INTEGER = NULL,
@Para INTEGER,
@Referencia INTEGER = NULL
AS 
BEGIN  
    DECLARE @SaldoAtual MONEY;
    
    IF @Tipo = 1 OR @Tipo = 2
        BEGIN
            SELECT @SaldoAtual = @Saldo FROM [Conta]
            WHERE [Id] = @De
        END

    INSERT INTO [Transacao]([Data], [Valor], [De], [Para], [Referencia])
    VALUES (GETDATE(), @Valor, @Tipo, @De, @Para, @Referencia)

    /* Operação de deposito */
    IF @Tipo = 0
        UPDATE [Cliente] 
        SET [Saldo] += @Valor
        WHERE [Id] = @De

    /* Operação de saque */
    ELSE IF @Tipo = 1
        UPDATE [Cliente]
        SET [Saldo] -= @Valor
        WHERE [Id] = @De  

    /* Operação de transferência ou reembolso*/
    ELSE IF @Tipo = 2 OR @Tipo = 3
        BEGIN
            UPDATE [Cliente]
            SET [Saldo] -= @Valor
            WHERE [Id] = @De 

            UPDATE [Cliente]
            SET [Saldo] += @Valor
            WHERE [Id] = @Para 
        END
END