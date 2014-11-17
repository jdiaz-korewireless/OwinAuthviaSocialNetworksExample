--drop procedure [dbo].[spUpdateUser]

create procedure [dbo].[spUpdateUser]
(
	@userId				int,
	@password			nvarchar(100), 
    @fullName			nvarchar(100),
	@updatedDate		datetime2,
	@verifyEmailCode	uniqueidentifier
)
as
begin
	update [dbo].[User] set
		[Password] = @password,
		[FullName] = @fullName,
		[UpdatedDate] = @updatedDate,
		[VerifyEmailCode] = @verifyEmailCode
	where [Id] = @userId

	exec [dbo].[spGetUserById] @userId
end