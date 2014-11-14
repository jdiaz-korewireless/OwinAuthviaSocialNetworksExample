create table [dbo].[User]
(
	[Id]				int					not null	primary key identity(1,1),
	[Email]				nvarchar(100)		not null,
	[Password]			nvarchar(100)		null, 
    [FullName]			nvarchar(100)		null,
    [CreatedDate]		datetime2			not null, 
	[UpdatedDate]		datetime2			not null,
	[VerifyEmailCode]	uniqueidentifier	null,

	constraint [AK_User_Email] unique ([Email])
)
