--drop procedure [dbo].[spGetUserByEmail]

create procedure [dbo].[spGetUserByEmail]
(
	@email	nvarchar(100)		
)
as

select	[User].*
from [dbo].[User]
where	[Email] = @email
