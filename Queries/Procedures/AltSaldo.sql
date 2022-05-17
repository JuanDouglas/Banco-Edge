USE [Banco Edge];
GO

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
    /*
        Caso for uma operação de saque, transferência ou 
        reembolso verifica se o valor em conta é igual o 
        menor ao valor em conta.
    */
    IF @Tipo = 1 OR @Tipo = 2 OR @Tipo = 3
        BEGIN    
            DECLARE @Saldo MONEY;
    
            SELECT @Saldo = [Saldo] FROM [Conta]
            WHERE [Id] = @De;

            IF @Saldo < @Valor
                BEGIN
                    RAISERROR ('O valor de transferência deve ser maior que o valor saldo na conta!',10,1);
                END

            /*
                Caso seja um reembolso verifica se o valor 
                dessa transação é igual ao valor da transação 
                que será reembolsada.
            */    
            IF @Tipo = 3
                BEGIN 
                    DECLARE @ValorAnterior MONEY;

                    SELECT @ValorAnterior = [Valor] FROM [Transacao]
                    WHERE [Id] = @Referencia AND
                        [Para] = @De

					IF @ValorAnterior = @Valor
                        BEGIN
                            RAISERROR('O valor de reembolso deve ser o valor da transação anterior',10,1);
                        END
                END 
        END
    BEGIN TRAN
        INSERT INTO [Transacao]([Data], [Valor],[Tipo], [De], [Para], [Referencia])
        VALUES (GETDATE(), @Valor, @Tipo, @De, @Para, @Referencia)
        
        BEGIN TRY
            /* Operação de deposito */
            IF @Tipo = 0
                UPDATE [Conta] 
                SET [Saldo] += @Valor
                WHERE [Id] = @Para

            /* Operação de saque */
            ELSE IF @Tipo = 1
                UPDATE [Conta]
                SET [Saldo] -= @Valor
                WHERE [Id] = @De  

            /* Operação de transferência ou reembolso*/
            ELSE IF @Tipo = 2 OR @Tipo = 3
                BEGIN
                    UPDATE [Conta]
                    SET [Saldo] -= @Valor
                    WHERE [Id] = @De 

                    UPDATE [Conta]
                    SET [Saldo] += @Valor
                    WHERE [Id] = @Para 
                END

			END TRY
        BEGIN CATCH
            IF @@TRANCOUNT > 0
				ROLLBACK
        END CATCH
    COMMIT
END