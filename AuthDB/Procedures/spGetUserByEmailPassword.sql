--drop procedure [dbo].[spGetUserByEmailPassword]

create procedure [dbo].[spGetUserByEmailPassword]
(
	@email		nvarchar(100),
	@password	nvarchar(100)		
)
as

select	[User].*
from [dbo].[User]
where	[Email] = @email and [Password] = @password