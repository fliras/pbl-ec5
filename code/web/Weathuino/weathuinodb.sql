
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
CREATE TABLE perfis_acesso (
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
    FOREIGN KEY (id_perfil_acesso) REFERENCES perfis_acesso(id)
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
INSERT INTO perfis_acesso (nome) VALUES
('ADMIN'),
('COMUM');

INSERT INTO usuarios (id, nome, email, senha, id_perfil_acesso) VALUES
(1, 'Alice Admin', 'alice@admin.com', '$2b$12$e0NRSCpq4u1fE0E0eAa0euMtD57yQrbAYBsbzYhS.f61QIQhQKOwa', 1), -- senha: admin123
(2, 'Bob', 'bob@user.com', '$2b$12$JQkFqD1UpWlNgdo7AKzleuYq/XuzM1ORC9cfyP7Kx6E6l8VUkghzK', 2),  -- senha: user123
(3, 'Carol', 'carol@guest.com', '$2b$12$9gZByVXz2W2GT7KUYGNeWeqK0RSgS.y.AK2jlBDmF5SC2a2A9hvCi', 2); -- senha: visit123

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
CREATE PROCEDURE spInsereUsuario
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

DROP PROCEDURE IF EXISTS spAlteraUsuario
GO
CREATE PROCEDURE spAlteraUsuario
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

DROP PROCEDURE IF EXISTS spConsultaUsuarios
GO
CREATE PROCEDURE spConsultaUsuarios
(
	@id INTEGER
)
AS
BEGIN
	DECLARE @query VARCHAR(MAX) = 'SELECT u.id idUsuario, u.nome nomeUsuario, u.email emailUsuario, '
	    + 'u.senha senhaUsuario, pa.id idPerfilAcesso, pa.nome nomePerfilAcesso FROM usuarios u '
		+ 'INNER JOIN perfis_acesso pa ON pa.id = u.id_perfil_acesso ';

	IF ISNULL(@id, 0) <> 0
		SET @query = @query + 'WHERE u.id = ' + CAST(@id as VARCHAR(MAX));
	
	EXEC(@query);
END
GO

--

DROP PROCEDURE IF EXISTS spInsereEstufa
GO
CREATE PROCEDURE spInsereEstufa
(
	@id INTEGER,
	@nome VARCHAR(100),
	@descricao VARCHAR(200),
	@temperaturaMin DECIMAL(5,2),
	@temperaturaMax DECIMAL(5,2),
	@idMedidor INTEGER
)
AS
BEGIN
	INSERT INTO estufas (id, nome, descricao, temperatura_min, temperatura_max, id_medidor)
		VALUES (@id, @nome, @descricao, @temperaturaMin, @temperaturaMax, @idMedidor);
END
GO

DROP PROCEDURE IF EXISTS spAlteraEstufa
GO
CREATE PROCEDURE spAlteraEstufa
(
	@id INTEGER,
	@nome VARCHAR(100),
	@descricao VARCHAR(200),
	@temperaturaMin DECIMAL(5,2),
	@temperaturaMax DECIMAL(5,2),
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

DROP PROCEDURE IF EXISTS spConsultaEstufas
GO
CREATE PROCEDURE spConsultaEstufas
(
	@id INTEGER
)
AS
BEGIN
	DECLARE @query VARCHAR(MAX) = 'SELECT e.id idEstufa, e.nome nomeEstufa, e.descricao descricaoEstufa, '
	    + 'e.temperatura_min tempMinEstufa, e.temperatura_max tempMaxEstufa, m.id idMedidor, m.nome nomeMedidor FROM estufas e '
		+ 'INNER JOIN medidores m ON m.id = e.id_medidor ';

	IF ISNULL(@id, 0) <> 0
		SET @query = @query + 'WHERE e.id = ' + CAST(@id as VARCHAR(MAX));
	
	EXEC(@query);
END
GO

--

DROP PROCEDURE IF EXISTS spInsereMedidor
GO
CREATE PROCEDURE spInsereMedidor
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

DROP PROCEDURE IF EXISTS spAlteraMedidor
GO
CREATE PROCEDURE spAlteraMedidor
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

DROP PROCEDURE IF EXISTS spConsultaMedidores
GO
CREATE PROCEDURE spConsultaMedidores
(
	@id INTEGER
)
AS
BEGIN
	DECLARE @query VARCHAR(MAX) = 'SELECT m.id idMedidor, m.nome nomeMedidor, m.data_ultimo_registro ultimoRegistroMedidor FROM medidores m ';

	IF ISNULL(@id, 0) <> 0
		SET @query = @query + 'WHERE m.id = ' + CAST(@id as VARCHAR(MAX));
	
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

DROP PROCEDURE IF EXISTS spConsultaPerfisAcesso
GO
CREATE PROCEDURE spConsultaPerfisAcesso
(
	@id INTEGER
)
AS
BEGIN
	DECLARE @query VARCHAR(MAX) = 'SELECT pa.id idPerfilAcesso, pa.nome nomePerfilAcesso FROM perfis_acesso pa ';

	IF ISNULL(@id, 0) <> 0
		SET @query = @query + 'WHERE pa.id = ' + CAST(@id as VARCHAR(MAX));
	
	EXEC(@query);
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