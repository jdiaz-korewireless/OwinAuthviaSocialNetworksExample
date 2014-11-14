--drop procedure [dbo].[spGetExternalUser]

create procedure [dbo].[spGetExternalUser]
(
	@providerId		tinyint,
	@providerKey	nvarchar(255)
)
as

select	[User].*

from [dbo].[UserExternalLogin] as [ExtLogin]
inner join [dbo].[User] as [User] on [User].[Id] = [ExtLogin].[UserId]

where	[ExtLogin].[ExternalLoginProviderId] = @providerId
	and	[ExtLogin].[ProviderKey] = @providerKey