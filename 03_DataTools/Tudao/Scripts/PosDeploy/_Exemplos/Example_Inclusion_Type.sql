/*
=================================================================================================================
-- Criação da Tabela temporária e as informações que devem ser inseridas na tabela real
=================================================================================================================
*/

DECLARE @TableTipoExemplo TABLE
(
	[Id]		INT				NOT NULL,
	[Descricao]	VARCHAR (100)	NOT NULL
)

INSERT INTO @TableTipoExemplo ([Id], [Descricao])
	 VALUES (1, 'ValorA'),
			(2, 'ValorB')

/*
=================================================================================================================
-- Insere, Atualiza ou Deleta as informações na tabela real
=================================================================================================================
*/

--SET Identity_Insert [Tipo].[Exemplo] ON

MERGE [Tipo].[Exemplo]

USING @TableTipoExemplo as tmp

-- Condição
ON tmp.Id = [Tipo].[Exemplo].[Id] 

-- Caso a condição seja verdadeira
WHEN MATCHED THEN UPDATE SET 
	[Tipo].[Exemplo].[Descricao] = tmp.[Descricao]

-- Quando a condição for false
WHEN NOT MATCHED THEN
	INSERT ([Descricao])
	VALUES (tmp.[Descricao])

-- Deleta todos os demais registros da tabela caso não esteja na tabela temporária
WHEN NOT MATCHED BY SOURCE THEN
    DELETE;

--SET Identity_Insert [Tipo].[Exemplo] OFF
GO
