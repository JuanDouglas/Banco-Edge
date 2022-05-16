CREATE PROC BuscarLogin 
@Token VARCHAR(96),
@ChaveConta VARCHAR(96)
AS 
BEGIN 
  SELECT TOP(1) [log].Id, [log].Token, [log].[IP], [cli].Id, [log].[Data] FROM [Login] AS [log]
  LEFT JOIN [Cliente] AS cli
  ON cli.Id = [log].ClienteId
  WHERE [log].Token = @Token AND cli.Chave = @ChaveConta
END