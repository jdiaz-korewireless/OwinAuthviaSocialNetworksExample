--drop procedure [dbo].[spGetUserById]

create procedure [dbo].[spGetUserById]
(
	@userId	int
)
as

select	[User].*
from [dbo].[User]
where	[Id] = @userId
