
-- CRIAÇÃO DA ESTRUTURA
-------------------------------------------------
USE master;

DROP DATABASE IF EXISTS weathuinodb;
GO

CREATE DATABASE weathuinodb;
GO

USE weathuinodb;



-- CRIAÇÃO DAS TABELAS
-------------------------------------------------

-- Create table: perfis_acesso
CREATE TABLE perfisAcesso (
    id INT IDENTITY(1,1) PRIMARY KEY,
    nome VARCHAR(50) NOT NULL
);
GO

-- Create table: usuarios
CREATE TABLE usuarios (
    id INT PRIMARY KEY,
    nome VARCHAR(100) NOT NULL,
    email VARCHAR(100) NOT NULL,
    senha CHAR(60) NOT NULL,
    id_perfil_acesso INT NOT NULL,
    FOREIGN KEY (id_perfil_acesso) REFERENCES perfisAcesso(id)
);
GO

-- Create table: medidores
CREATE TABLE medidores (
    id INT PRIMARY KEY,
	device_id_fiware VARCHAR(45) NOT NULL,
    nome VARCHAR(45) NOT NULL,
    data_ultimo_registro DATETIME
);
GO

-- Create table: estufas
CREATE TABLE estufas (
    id INT PRIMARY KEY,
    nome VARCHAR(100) NOT NULL,
    descricao VARCHAR(200),
    temperatura_min DECIMAL(5, 2),
    temperatura_max DECIMAL(5, 2),
	imagem VARBINARY(MAX),
    id_medidor INT NOT NULL,
    FOREIGN KEY (id_medidor) REFERENCES medidores(id)
);
GO


-- SEEDS
-------------------------------------------------
INSERT INTO perfisAcesso (nome) VALUES
('COMUM'),
('ADMIN');

INSERT INTO usuarios (id, nome, email, senha, id_perfil_acesso) VALUES
(1, 'Alice Admin', 'alice@admin.com', '$2a$11$ZRgwlDqXssWQT3vwlQmdyO6R2yv1xpCPgqJvqDO/8GUAXFiF.uZAC', 1), -- senha: admin123
(2, 'Bob', 'bob@user.com', '$2a$11$ZRgwlDqXssWQT3vwlQmdyO6R2yv1xpCPgqJvqDO/8GUAXFiF.uZAC', 2),  -- senha: user123
(3, 'Carol', 'carol@guest.com', '$2a$11$ZRgwlDqXssWQT3vwlQmdyO6R2yv1xpCPgqJvqDO/8GUAXFiF.uZAC', 2); -- senha: visit123


-- PROCEDURES
-------------------------------------------------

DROP PROCEDURE IF EXISTS spInsere_usuarios
GO
CREATE PROCEDURE spInsere_usuarios
(
	@id INTEGER,
	@nome VARCHAR(100),
	@email VARCHAR(100),
	@senha CHAR(60),
	@perfilUsuario INTEGER
)
AS
BEGIN
	INSERT INTO usuarios (id, nome, email, senha, id_perfil_acesso)
		VALUES (@id, @nome, @email, @senha, @perfilUsuario);
END
GO

DROP PROCEDURE IF EXISTS spAltera_usuarios
GO
CREATE PROCEDURE spAltera_usuarios
(
	@id INTEGER,
	@nome VARCHAR(100),
	@email VARCHAR(100),
	@senha CHAR(60),
	@perfilUsuario INTEGER
)
AS
BEGIN
	UPDATE usuarios SET
		nome = @nome,
		email = @email,
		senha = @senha,
		id_perfil_acesso = @perfilUsuario
	WHERE id = @id
END
GO

DROP PROCEDURE IF EXISTS spConsulta_usuarios
GO
CREATE PROCEDURE spConsulta_usuarios
(
	@id INTEGER = null,
	@email VARCHAR(MAX) = null,
	@nome VARCHAR(MAX) = null,
	@perfilUsuario INTEGER = null
)
AS
BEGIN
	SELECT u.id idUsuario, u.nome nomeUsuario, u.email emailUsuario,
		   u.senha senhaUsuario, pa.id idPerfilAcesso
	FROM usuarios u
	INNER JOIN perfisAcesso pa ON pa.id = u.id_perfil_acesso
	WHERE
		(@id IS NULL OR u.id = @id) AND
		(@nome IS NULL OR u.nome like '%' + @nome + '%') AND
		(@email IS NULL OR u.email like '%' + @email + '%') AND
		(@perfilUsuario IS NULL OR pa.id = @perfilUsuario);
END
GO

--

DROP PROCEDURE IF EXISTS spInsere_estufas
GO
CREATE PROCEDURE spInsere_estufas
(
	@id INTEGER,
	@nome VARCHAR(100),
	@descricao VARCHAR(200),
	@temperaturaMin DECIMAL(5,2) = NULL,
	@temperaturaMax DECIMAL(5,2) = NULL,
	@imagem VARBINARY(MAX) = NULL,
	@idMedidor INTEGER
)
AS
BEGIN
	INSERT INTO estufas (id, nome, descricao, temperatura_min, temperatura_max, imagem, id_medidor)
		VALUES (@id, @nome, @descricao, @temperaturaMin, @temperaturaMax, @imagem, @idMedidor);
END
GO

DROP PROCEDURE IF EXISTS spAltera_estufas
GO
CREATE PROCEDURE spAltera_estufas
(
	@id INTEGER,
	@nome VARCHAR(100),
	@descricao VARCHAR(200),
	@temperaturaMin DECIMAL(5,2) = NULL,
	@temperaturaMax DECIMAL(5,2) = NULL,
	@imagem VARBINARY(MAX) = NULL,
	@idMedidor INTEGER
)
AS
BEGIN
	UPDATE estufas SET
		nome = @nome,
		descricao = @descricao,
		temperatura_min = @temperaturaMin,
		temperatura_max = @temperaturaMax,
		imagem = @imagem,
		id_medidor = @idMedidor
	WHERE id = @id
END
GO

DROP PROCEDURE IF EXISTS spConsulta_estufas
GO
CREATE PROCEDURE spConsulta_estufas
(
	@id INTEGER = NULL,
	@nome VARCHAR(MAX) = NULL,
	@descricao VARCHAR(MAX) = null
)
AS
BEGIN
	SELECT e.id idEstufa, e.nome nomeEstufa, e.descricao descricaoEstufa,
	       e.temperatura_min tempMinEstufa, e.temperatura_max tempMaxEstufa,
		   e.imagem imagemEstufa, m.id idMedidor, m.nome nomeMedidor
	FROM estufas e 
	INNER JOIN medidores m ON m.id = e.id_medidor
	WHERE
		(@id IS NULL OR e.id = @id) AND
		(@nome IS NULL OR e.nome like '%' + @nome + '%') AND
		(@descricao IS NULL OR e.descricao like '%' + @descricao + '%');
END
GO

--

DROP PROCEDURE IF EXISTS spInsere_medidores
GO
CREATE PROCEDURE spInsere_medidores
(
	@id INTEGER,
	@nome VARCHAR(45),
	@deviceIdFiware VARCHAR(45)
)
AS
BEGIN
	INSERT INTO medidores (id, device_id_fiware, nome)
		VALUES (@id, @deviceIdFiware, @nome);
END
GO

DROP PROCEDURE IF EXISTS spAltera_medidores
GO
CREATE PROCEDURE spAltera_medidores
(
	@id INTEGER,
	@nome VARCHAR(45),
	@deviceIdFiware VARCHAR(45)
)
AS
BEGIN
	UPDATE medidores SET
		nome = @nome,
		device_id_fiware = @deviceIdFiware
		WHERE id = @id;
END
GO

DROP PROCEDURE IF EXISTS spConsulta_medidores
GO
CREATE PROCEDURE spConsulta_medidores
(
	@id INTEGER = null,
	@nome VARCHAR(MAX) = null,
	@deviceID VARCHAR(MAX) = null
)
AS
BEGIN
	SELECT m.id idMedidor, m.device_id_fiware deviceIdFiware,  m.nome nomeMedidor, m.data_ultimo_registro ultimoRegistroMedidor
		FROM medidores m
		WHERE
			(@id IS NULL OR m.id = @id) AND
			(@nome IS NULL OR m.nome like '%' + @nome + '%') AND
			(@deviceID IS NULL OR m.device_id_fiware like '%' + @deviceID + '%');
END
GO

DROP PROCEDURE IF EXISTS spVerificaUsoMedidor
GO
CREATE PROCEDURE spVerificaUsoMedidor
(
	@id INTEGER
)
AS
BEGIN
	DECLARE @id_estufa INTEGER = (SELECT TOP 1 id FROM estufas WHERE id_medidor = @id);
	DECLARE @estaEmUso TINYINT;

	IF ISNULL(@id_estufa, 0) <> 0
		SET @estaEmUso = 1;
	ELSE
		SET @estaEmUso = 0;

	SELECT @estaEmUso estaEmUso;
END
GO

--

CREATE PROCEDURE spDelete 
( 
  @id INTEGER,
  @tabela VARCHAR(MAX)
) 
AS 
BEGIN
   DECLARE @sql VARCHAR(MAX); 
   SET @sql = ' DELETE ' + @tabela +  
       ' WHERE id = ' + CAST(@id AS VARCHAR(MAX)) 
   EXEC(@sql) 
END 
GO

CREATE PROCEDURE spProximoId 
(@tabela  VARCHAR(MAX)) 
AS 
BEGIN
    EXEC('SELECT ISNULL(MAX(id) + 1, 1) AS MAIOR FROM ' + @tabela) 
END 
GO