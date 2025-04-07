USE [master]
GO

IF NOT EXISTS( SELECT * FROM sys.sysdatabases where name = 'Rommanel' ) 
  EXEC('CREATE DATABASE Rommanel');
GO

IF NOT EXISTS( SELECT * FROM SYS.syslogins WHERE name = 'user_api_romanel' )
  EXEC('CREATE LOGIN [user_api_romanel] WITH PASSWORD=N''12345678'' MUST_CHANGE, DEFAULT_DATABASE=[Rommanel], CHECK_EXPIRATION=ON, CHECK_POLICY=ON');
GO

USE [Rommanel]
GO
IF NOT EXISTS( SELECT * FROM SYS.sysusers WHERE name = 'user_api_romanel' )
BEGIN
  EXEC('CREATE USER [user_api_romanel] FOR LOGIN [user_api_romanel]')
  EXEC('ALTER ROLE [db_owner] ADD MEMBER [user_api_romanel]')
END

IF EXISTS( SELECT * FROM sys.sysobjects o WHERE o.name = 'Clientes' ) 
  EXEC('DROP TABLE dbo.Clientes');
GO

CREATE TABLE Clientes
(
	[Id] INT NOT NULL IDENTITY(1,1) PRIMARY KEY, 
	[Tipo] INT NOT NULL,
	[Nome] VARCHAR(200) NOT NULL,
	[CPFCNPJ] VARCHAR(14) NOT NULL,
	[IE] VARCHAR(12) NULL,
	[DataNascimento] DATE NULL,
	[Telefone] VARCHAR(20) NULL,
	[Email] VARCHAR(200) NULL,
	[CEP] VARCHAR(8) NULL,
	[Logradouro] VARCHAR(100) NULL,
	[Numero] VARCHAR(10) NULL,
	[Complemento] VARCHAR(50) NULL,
	[Bairro] VARCHAR(100) NULL,
	[Cidade] VARCHAR(100) NULL,
	[Estado] VARCHAR(2) NULL
);
GO

select * from clientes