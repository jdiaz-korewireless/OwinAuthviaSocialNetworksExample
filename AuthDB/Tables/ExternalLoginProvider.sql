create table [dbo].[ExternalLoginProvider]
(
	[Id]	tinyint			not null	primary key identity(1,1), 
    [Name]	nvarchar(50)	not null
)
