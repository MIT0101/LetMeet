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

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230220114137_MainAppMigration')
BEGIN
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
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230220114137_MainAppMigration')
BEGIN
    CREATE TABLE [DayFrees] (
        [id] int NOT NULL IDENTITY,
        [day] int NOT NULL,
        [startHour] int NOT NULL,
        [endHour] int NOT NULL,
        [UserInfoid] uniqueidentifier NULL,
        CONSTRAINT [PK_DayFrees] PRIMARY KEY ([id]),
        CONSTRAINT [FK_DayFrees_UserInfos_UserInfoid] FOREIGN KEY ([UserInfoid]) REFERENCES [UserInfos] ([id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230220114137_MainAppMigration')
BEGIN
    CREATE TABLE [Meetings] (
        [id] int NOT NULL IDENTITY,
        [studentid] uniqueidentifier NOT NULL,
        [supervisorid] uniqueidentifier NOT NULL,
        [totalTimeHours] real NOT NULL,
        [remindingTimeHours] real NOT NULL,
        [startFrom] datetime2 NOT NULL,
        [description] nvarchar(500) NULL,
        CONSTRAINT [PK_Meetings] PRIMARY KEY ([id]),
        CONSTRAINT [FK_Meetings_UserInfos_studentid] FOREIGN KEY ([studentid]) REFERENCES [UserInfos] ([id]),
        CONSTRAINT [FK_Meetings_UserInfos_supervisorid] FOREIGN KEY ([supervisorid]) REFERENCES [UserInfos] ([id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230220114137_MainAppMigration')
BEGIN
    CREATE TABLE [StudentSupervisors] (
        [id] int NOT NULL IDENTITY,
        [studentid] uniqueidentifier NOT NULL,
        [supervisorid] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_StudentSupervisors] PRIMARY KEY ([id]),
        CONSTRAINT [FK_StudentSupervisors_UserInfos_studentid] FOREIGN KEY ([studentid]) REFERENCES [UserInfos] ([id]),
        CONSTRAINT [FK_StudentSupervisors_UserInfos_supervisorid] FOREIGN KEY ([supervisorid]) REFERENCES [UserInfos] ([id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230220114137_MainAppMigration')
BEGIN
    CREATE TABLE [SubMeetings] (
        [id] int NOT NULL IDENTITY,
        [totalTimeHoure] int NOT NULL,
        [date] datetime2 NOT NULL,
        [startHour] int NOT NULL,
        [endHour] int NOT NULL,
        [description] nvarchar(500) NULL,
        [Meetingid] int NULL,
        CONSTRAINT [PK_SubMeetings] PRIMARY KEY ([id]),
        CONSTRAINT [FK_SubMeetings_Meetings_Meetingid] FOREIGN KEY ([Meetingid]) REFERENCES [Meetings] ([id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230220114137_MainAppMigration')
BEGIN
    CREATE TABLE [MeetingTasks] (
        [id] int NOT NULL IDENTITY,
        [title] nvarchar(500) NOT NULL,
        [decription] nvarchar(500) NOT NULL,
        [isCompleted] bit NOT NULL,
        [SubMeetingid] int NULL,
        CONSTRAINT [PK_MeetingTasks] PRIMARY KEY ([id]),
        CONSTRAINT [FK_MeetingTasks_SubMeetings_SubMeetingid] FOREIGN KEY ([SubMeetingid]) REFERENCES [SubMeetings] ([id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230220114137_MainAppMigration')
BEGIN
    CREATE INDEX [IX_DayFrees_UserInfoid] ON [DayFrees] ([UserInfoid]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230220114137_MainAppMigration')
BEGIN
    CREATE INDEX [IX_Meetings_studentid] ON [Meetings] ([studentid]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230220114137_MainAppMigration')
BEGIN
    CREATE INDEX [IX_Meetings_supervisorid] ON [Meetings] ([supervisorid]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230220114137_MainAppMigration')
BEGIN
    CREATE INDEX [IX_MeetingTasks_SubMeetingid] ON [MeetingTasks] ([SubMeetingid]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230220114137_MainAppMigration')
BEGIN
    CREATE INDEX [IX_StudentSupervisors_studentid] ON [StudentSupervisors] ([studentid]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230220114137_MainAppMigration')
BEGIN
    CREATE INDEX [IX_StudentSupervisors_supervisorid] ON [StudentSupervisors] ([supervisorid]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230220114137_MainAppMigration')
BEGIN
    CREATE INDEX [IX_SubMeetings_Meetingid] ON [SubMeetings] ([Meetingid]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230220114137_MainAppMigration')
BEGIN
    CREATE UNIQUE INDEX [IX_UserInfos_emailAddress] ON [UserInfos] ([emailAddress]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230220114137_MainAppMigration')
BEGIN
    CREATE UNIQUE INDEX [IX_UserInfos_identityId] ON [UserInfos] ([identityId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230220114137_MainAppMigration')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20230220114137_MainAppMigration', N'6.0.13');
END;
GO

COMMIT;
GO

