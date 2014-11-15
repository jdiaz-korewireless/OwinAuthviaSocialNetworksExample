--drop procedure [dbo].[spUpdateUserAvatar]

create procedure [dbo].[spUpdateUserAvatar]
(
	@userId	int,
	@avatar	image
)
as

update [dbo].[UserAvatar] set [Avatar] = @avatar
where [UserId] = @userId
