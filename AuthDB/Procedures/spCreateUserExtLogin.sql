--drop procedure [dbo].[spCreateUserExtLogin]

create procedure [dbo].[spCreateUserExtLogin]
(
	@userId			int,
	@providerId		tinyint,
	@providerKey	nvarchar(255)
)
as

insert into [dbo].[UserExternalLogin] ([UserId], [ExternalLoginProviderId], [ProviderKey])
values (@userId, @providerId, @providerKey)	
