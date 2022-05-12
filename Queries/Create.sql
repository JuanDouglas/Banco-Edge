USE [master];

DROP DATABASE IF EXISTS [Banco Edge];

CREATE DATABASE [Banco Edge]

ALTER DATABASE [Banco Edge] 
SET SINGLE_USER WITH ROLLBACK IMMEDIATE
GO

USE [Banco Edge];

CREATE TABLE [Cliente](
[Id] INTEGER IDENTITY PRIMARY KEY NOT NULL,
[Nome] VARCHAR(100) NOT NULL,
[Email] VARCHAR(500) UNIQUE NOT NULL,
[Telefone] VARCHAR(15) NOT NULL,
[CpfOuCnpj] VARCHAR(14) UNIQUE NOT NULL
);

CREATE TABLE [Conta](
[Id] INTEGER IDENTITY PRIMARY KEY NOT NULL,
[Dono] INTEGER NOT NULL,
[Saldo] MONEY NULL DEFAULT 0.0,
[Tipo] TINYINT NOT NULL DEFAULT 1,
[Criacao] DATETIME2 NOT NULL,
FOREIGN KEY ([Dono]) REFERENCES [Cliente]([ID])
);