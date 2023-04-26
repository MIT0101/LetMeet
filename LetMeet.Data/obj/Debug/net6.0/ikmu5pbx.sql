IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

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
    [isPresent] bit NOT NULL,
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

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20230424080139_MainAppMigrations', N'6.0.13');
GO

COMMIT;
GO

