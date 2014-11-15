--drop procedure [dbo].[spDeleteUserWithDependencies]

create procedure [dbo].[spDeleteUserWithDependencies]
(
	@userId	int	
)
as

delete from [dbo].[UserExternalLogin] where [UserId] = @userId
delete from [dbo].[UserAvatar] where [UserId] = @userId
delete from [dbo].[User] where [Id] = @userId

return 0