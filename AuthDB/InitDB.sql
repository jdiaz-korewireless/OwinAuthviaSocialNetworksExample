set identity_insert [dbo].[ExternalLoginProvider] on
insert into [dbo].[ExternalLoginProvider]
(
	Id,
	Name
)
values
(1, 'Google'),
(2, 'Facebook'),
(3, 'Microsoft')

set identity_insert [dbo].[ExternalLoginProvider] off