--drop procedure [dbo].[spGetUserAvatar]

create procedure [dbo].[spGetUserAvatar]
(
	@userId	int
)
as

select	[Avatar]
from [dbo].[UserAvatar]
where	[UserId] = @userId
