BEGIN TRANSACTION;
GO

CREATE TABLE [UserInfos] (
    [id] uniqueidentifier NOT NULL,
    [fullName] nvarchar(100) NOT NULL,
    [emailAddress] nvarchar(450) NOT NULL,
    [phoneNumber] nvarchar(50) NULL,
    [stage] int NULL,
    [profileImage] nvarchar(max) NULL,
    [identityId] uniqueidentifier NOT NULL,
    [userRole] int NOT NULL,
    CONSTRAINT [PK_UserInfos] PRIMARY KEY ([id])
);
GO

CREATE TABLE [DayFrees] (
    [id] int NOT NULL IDENTITY,
    [day] int NOT NULL,
    [startHour] int NOT NULL,
    [endHour] int NOT NULL,
    [UserInfoid] uniqueidentifier NULL,
    CONSTRAINT [PK_DayFrees] PRIMARY KEY ([id]),
    CONSTRAINT [FK_DayFrees_UserInfos_UserInfoid] FOREIGN KEY ([UserInfoid]) REFERENCES [UserInfos] ([id])
);
GO

CREATE TABLE [SupervisionInfo] (
    [id] int NOT NULL IDENTITY,
    [studentid] uniqueidentifier NOT NULL,
    [supervisorid] uniqueidentifier NOT NULL,
    [startDate] datetime2 NOT NULL,
    [endDate] datetime2 NOT NULL,
    [extendTimes] int NOT NULL,
    CONSTRAINT [PK_SupervisionInfo] PRIMARY KEY ([id]),
    CONSTRAINT [FK_SupervisionInfo_UserInfos_studentid] FOREIGN KEY ([studentid]) REFERENCES [UserInfos] ([id]),
    CONSTRAINT [FK_SupervisionInfo_UserInfos_supervisorid] FOREIGN KEY ([supervisorid]) REFERENCES [UserInfos] ([id])
);
GO

CREATE TABLE [Meetings] (
    [id] int NOT NULL IDENTITY,
    [totalTimeHoure] int NOT NULL,
    [date] datetime2 NOT NULL,
    [startHour] int NOT NULL,
    [endHour] int NOT NULL,
    [description] nvarchar(max) NULL,
    [isStudentPresent] bit NOT NULL,
    [isSupervisorPresent] bit NOT NULL,
    [created] datetime2 NOT NULL,
    [SupervisionInfoid] int NOT NULL,
    CONSTRAINT [PK_Meetings] PRIMARY KEY ([id]),
    CONSTRAINT [FK_Meetings_SupervisionInfo_SupervisionInfoid] FOREIGN KEY ([SupervisionInfoid]) REFERENCES [SupervisionInfo] ([id])
);
GO

CREATE TABLE [MeetingTasks] (
    [id] int NOT NULL IDENTITY,
    [title] nvarchar(500) NOT NULL,
    [decription] nvarchar(500) NOT NULL,
    [isCompleted] bit NOT NULL,
    [Meetingid] int NULL,
    CONSTRAINT [PK_MeetingTasks] PRIMARY KEY ([id]),
    CONSTRAINT [FK_MeetingTasks_Meetings_Meetingid] FOREIGN KEY ([Meetingid]) REFERENCES [Meetings] ([id])
);
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'id', N'emailAddress', N'fullName', N'identityId', N'phoneNumber', N'profileImage', N'stage', N'userRole') AND [object_id] = OBJECT_ID(N'[UserInfos]'))
    SET IDENTITY_INSERT [UserInfos] ON;
INSERT INTO [UserInfos] ([id], [emailAddress], [fullName], [identityId], [phoneNumber], [profileImage], [stage], [userRole])
VALUES ('dc734be3-598f-4056-9df6-9d80a86e679b', N'default@user.com', N'Default User', '18376c6a-6c12-40a0-a6e9-66f769c05db4', N'07823947489', NULL, 3, 0);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'id', N'emailAddress', N'fullName', N'identityId', N'phoneNumber', N'profileImage', N'stage', N'userRole') AND [object_id] = OBJECT_ID(N'[UserInfos]'))
    SET IDENTITY_INSERT [UserInfos] OFF;
GO

CREATE INDEX [IX_DayFrees_UserInfoid] ON [DayFrees] ([UserInfoid]);
GO

CREATE INDEX [IX_Meetings_SupervisionInfoid] ON [Meetings] ([SupervisionInfoid]);
GO

CREATE INDEX [IX_MeetingTasks_Meetingid] ON [MeetingTasks] ([Meetingid]);
GO

CREATE INDEX [IX_SupervisionInfo_studentid] ON [SupervisionInfo] ([studentid]);
GO

CREATE INDEX [IX_SupervisionInfo_supervisorid] ON [SupervisionInfo] ([supervisorid]);
GO

CREATE UNIQUE INDEX [IX_UserInfos_emailAddress] ON [UserInfos] ([emailAddress]);
GO

CREATE UNIQUE INDEX [IX_UserInfos_identityId] ON [UserInfos] ([identityId]);
GO

COMMIT;
GO

