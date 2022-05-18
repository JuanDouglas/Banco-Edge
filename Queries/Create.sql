USE [master];

DROP DATABASE IF EXISTS [Banco Edge];
CREATE DATABASE [Banco Edge];

GO

USE [Banco Edge];
GO

CREATE TABLE [Cliente](
    [Id] INTEGER IDENTITY PRIMARY KEY NOT NULL,
    [Nome] VARCHAR(100) NOT NULL,
    [Email] VARCHAR(500) UNIQUE NOT NULL,
    [Telefone] VARCHAR(15) NOT NULL,
    [CpfOuCnpj] VARCHAR(14) UNIQUE NOT NULL,
    [Senha] VARCHAR(90) NOT NULL,
    [Chave] VARCHAR(96) UNIQUE NOT NULL,
    [Cadastro] DATETIME2 NOT NULL
);

CREATE TABLE [Login](
   [Id] INTEGER IDENTITY PRIMARY KEY NOT NULL,
   [Token] VARCHAR(96) NOT NULL,
   /* Suporta IPV6 armazenando em 6 bytes o IP informado!*/
   [IP] VARBINARY(6) NOT NULL,
   [ClienteId] INTEGER NOT NULL,
   [Data] DATETIME2 NOT NULL,
   FOREIGN KEY ([ClienteId]) REFERENCES [Cliente]([Id])
);

CREATE TABLE [Conta](
    [Id] INTEGER IDENTITY PRIMARY KEY NOT NULL,
    [Dono] INTEGER NOT NULL,
    [Saldo] MONEY NULL DEFAULT 0.0,
    [Tipo] TINYINT NOT NULL DEFAULT 1,
    [Criacao] DATETIME2 NOT NULL,
    FOREIGN KEY ([Dono]) REFERENCES [Cliente]([ID])
);

/* Transações acontecem quando um cliente efetua saque, deposito ou transferência. */
CREATE TABLE [Transacao](
    [Id] INTEGER IDENTITY PRIMARY KEY NOT NULL,
    [Data] DATETIME2 NOT NULL,
    [Tipo] TINYINT NOT NULL,
    [Valor] MONEY NOT NULL,
    [De] INTEGER NULL,
    [Para] INTEGER NOT NULL,
    [Descricao] VARCHAR(250) DEFAULT 'Operação não definida!',
    [Referencia] INTEGER NULL,
    FOREIGN KEY ([De]) REFERENCES [Conta]([Id]),
    FOREIGN KEY ([Para]) REFERENCES [Conta]([Id]),
    FOREIGN KEY ([Referencia]) REFERENCES [Transacao]([Id])
);