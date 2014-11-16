--drop procedure [dbo].[spCreateUser]

create procedure [dbo].[spCreateUser]
(
	@email				nvarchar(100),
	@password			nvarchar(100), 
    @fullName			nvarchar(100),
    @createdDate		datetime2, 
	@updatedDate		datetime2,
	@verifyEmailCode	uniqueidentifier
)
as

begin
	
	insert into [dbo].[User] ([Email], [Password], [FullName], [CreatedDate], [UpdatedDate], [VerifyEmailCode])
	values (@email, @password, @fullName, @createdDate, @updatedDate, @verifyEmailCode)

	declare @userId int = SCOPE_IDENTITY()

	exec [dbo].[spGetUserById] @userId
end


