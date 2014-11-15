--drop procedure [dbo].[spCreateUserAvatar]

create procedure [dbo].[spCreateUserAvatar]
(
	@userId	int,
	@avatar	image
)
as

insert into [dbo].[UserAvatar] ([UserId], [Avatar])
values (@userId, @avatar)	
