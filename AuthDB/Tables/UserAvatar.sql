create table [dbo].[UserAvatar]
(
	[UserId]	int		not null primary key,
    [Avatar]	image	not null,

    constraint [FK_UserAvatar_UserId_To_User_Id] foreign key ([UserId]) references [User]([Id])
)
