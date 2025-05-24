
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

INSERT INTO medidores (id, nome, data_ultimo_registro) VALUES
(1, 'Medidor Alpha', CURRENT_TIMESTAMP),
(2, 'Medidor Beta', NULL),
(3, 'Medidor Gama', '2025-04-06 08:45:19');

INSERT INTO estufas (id, nome, descricao, temperatura_min, temperatura_max, id_medidor) VALUES
(1, 'Estufa Norte', 'Estufa principal da unidade norte', 18.00, 30.00, 1),
(2, 'Estufa Sul', 'Controle de temperatura média', 20.00, 28.00, 2),
(3, 'Estufa Experimental', 'Ambiente para testes e simulações', 15.50, 32.00, 3);




-- PROCEDURES
-------------------------------------------------

DROP PROCEDURE IF EXISTS spInsereUsuario
GO
CREATE PROCEDURE spInsere_usuarios
(
	@id INTEGER,
	@nome VARCHAR(100),
	@email VARCHAR(100),
	@senha CHAR(60),
	@idPerfil INTEGER
)
AS
BEGIN
	INSERT INTO usuarios (id, nome, email, senha, id_perfil_acesso)
		VALUES (@id, @nome, @email, @senha, @idPerfil);
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
	@idPerfil INTEGER
)
AS
BEGIN
	UPDATE usuarios SET
		nome = @nome,
		email = @email,
		senha = @senha,
		id_perfil_acesso = @idPerfil
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
	@idPerfil INTEGER = null
)
AS
BEGIN
	DECLARE @query VARCHAR(MAX) = 'SELECT u.id idUsuario, u.nome nomeUsuario, u.email emailUsuario, '
	    + 'u.senha senhaUsuario, pa.id idPerfilAcesso FROM usuarios u '
		+ 'INNER JOIN perfisAcesso pa ON pa.id = u.id_perfil_acesso WHERE 1=1';

	IF ISNULL(@id, 0) <> 0
		SET @query = @query + ' AND u.id = ' + CAST(@id as VARCHAR(MAX));

	IF @nome IS NOT NULL
		SET @query = @query + ' AND u.nome = ''' + @nome + '''';

	IF @email IS NOT NULL
		SET @query = @query + ' AND u.email = ''' + @email + '''';

	IF ISNULL(@idPerfil, 0) <> 0
		SET @query = @query + ' AND pa.id = ' + CAST(@idPerfil as VARCHAR(MAX));
	
	EXEC(@query);
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
	@idMedidor INTEGER
)
AS
BEGIN
	INSERT INTO estufas (id, nome, descricao, temperatura_min, temperatura_max, id_medidor)
		VALUES (@id, @nome, @descricao, @temperaturaMin, @temperaturaMax, @idMedidor);
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
	@idMedidor INTEGER
)
AS
BEGIN
	UPDATE estufas SET
		nome = @nome,
		descricao = @descricao,
		temperatura_min = @temperaturaMin,
		temperatura_max = @temperaturaMax,
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
	DECLARE @query VARCHAR(MAX) = 'SELECT e.id idEstufa, e.nome nomeEstufa, e.descricao descricaoEstufa, '
	    + 'e.temperatura_min tempMinEstufa, e.temperatura_max tempMaxEstufa, m.id idMedidor, m.nome nomeMedidor FROM estufas e '
		+ 'INNER JOIN medidores m ON m.id = e.id_medidor WHERE 1=1 ';

	IF ISNULL(@id, 0) <> 0
		SET @query = @query + ' AND e.id = ' + CAST(@id as VARCHAR(MAX));

	IF @nome IS NOT NULL
		SET @query = @query + ' AND e.nome = ''' + @nome + '''';

	IF @descricao IS NOT NULL
		SET @query = @query + ' AND e.descricao = ''' + @descricao + '''';
	
	EXEC(@query);
END
GO

--

DROP PROCEDURE IF EXISTS spInsere_medidores
GO
CREATE PROCEDURE spInsere_medidores
(
	@id INTEGER,
	@nome VARCHAR(45)
)
AS
BEGIN
	INSERT INTO medidores (id, nome)
		VALUES (@id, @nome);
END
GO

DROP PROCEDURE IF EXISTS spAltera_medidores
GO
CREATE PROCEDURE spAltera_medidores
(
	@id INTEGER,
	@nome VARCHAR(45)
)
AS
BEGIN
	UPDATE medidores SET nome = @nome
		WHERE id = @id;
END
GO

DROP PROCEDURE IF EXISTS spConsulta_medidores
GO
CREATE PROCEDURE spConsulta_medidores
(
	@id INTEGER = null,
	@nome VARCHAR(MAX) = null
)
AS
BEGIN
	DECLARE @query VARCHAR(MAX) = 'SELECT m.id idMedidor, m.nome nomeMedidor, m.data_ultimo_registro ultimoRegistroMedidor FROM medidores m WHERE 1=1';

	IF ISNULL(@id, 0) <> 0
		SET @query = @query + ' AND m.id = ' + CAST(@id as VARCHAR(MAX));

	IF @nome IS NOT NULL
		SET @query = @query + ' AND m.nome = ''' + @nome + '''';
	
	EXEC(@query);
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