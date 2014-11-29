/* Index to search user by email/password */
create unique nonclustered index IX_UserEmailPassSearch on [dbo].[User]
(
	[Email],
	[Password]
)
where [Password] is not null