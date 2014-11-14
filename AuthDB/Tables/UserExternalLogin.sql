create table [dbo].[UserExternalLogin]
(
	[UserId]					int				not null,
	[ExternalLoginProviderId]	tinyint			not null,
	[ProviderKey]				nvarchar(255)	not null,

	primary key ([ExternalLoginProviderId], [ProviderKey]),

	constraint [AK_UserExtLogin] unique ([UserId], [ExternalLoginProviderId]),
	constraint [FK_UserExtLogin_UserId_To_User_Id] foreign key ([UserId]) references [User]([Id]),
	constraint [FK_UserExtLogin_ExtLogProviderId_To_ExtLogProvider_Id] foreign key ([ExternalLoginProviderId]) references [ExternalLoginProvider]([Id])
)