-- CREATE DATABASE dbDiyProjectPlatform;
-- GO

-- USE dbDiyProjectPlatform;
-- GO

CREATE TABLE [UserRole] (
	[Id] int PRIMARY KEY,
	[Name] nvarchar(50) NOT NULL
);
GO

CREATE TABLE [User] (
	[Id] int IDENTITY(1,1) PRIMARY KEY,
	[FirstName] nvarchar(255) NOT NULL,
	[LastName] nvarchar(255) NOT NULL,
	[Username] nvarchar(255) NOT NULL,
	[Email] nvarchar(255) NOT NULL,
	[PwdHash] nvarchar(255) NOT NULL,
	[PwdSalt] nvarchar(255) NOT NULL,
	[Phone] nvarchar(255) NULL,
	[ProfilePicture] nvarchar(max) NULL,
	[UserRoleId] int DEFAULT 10 NOT NULL
	FOREIGN KEY REFERENCES [UserRole]([Id])
);
GO

CREATE TABLE [Topic] (
	[Id] int IDENTITY(1,1) PRIMARY KEY,
	[Name] nvarchar(255) NOT NULL
);
GO

CREATE TABLE [DifficultyLevel] (
    [Id] int PRIMARY KEY,
    [Name] nvarchar(50) NOT NULL
);
GO

CREATE TABLE [Project] (
	[Id] int IDENTITY(1,1) PRIMARY KEY,
	[Title] nvarchar(255) NOT NULL,
	[DateCreated] datetime2 DEFAULT GETUTCDATE(),
	[Description] nvarchar(255) NOT NULL,
	[Content] nvarchar(max) NOT NULL,
	[TopicId] int NOT NULL
	FOREIGN KEY REFERENCES [Topic]([Id]),
	[UserId] int NOT NULL
	FOREIGN KEY REFERENCES [User]([Id]),
	[DifficultyLevelId] int NOT NULL
	FOREIGN KEY REFERENCES [DifficultyLevel]([Id])
);
GO

CREATE TABLE [ProjectImage] (
	[Id] int IDENTITY(1,1) PRIMARY KEY,
	[ProjectId] int NOT NULL
	FOREIGN KEY REFERENCES [Project]([Id]),
	[Image] nvarchar(max) NOT NULL
);
GO

CREATE TABLE [Material] (
	[Id] int IDENTITY(1,1) PRIMARY KEY,
	[Name] nvarchar(255) NOT NULL
);
GO

CREATE TABLE [ProjectMaterial] (
	[Id] int IDENTITY(1,1) PRIMARY KEY,
	[ProjectId] int NOT NULL
	FOREIGN KEY REFERENCES [Project]([Id]),
	[MaterialId] int NOT NULL
	FOREIGN KEY REFERENCES [Material]([Id])
);
GO

CREATE TABLE [ProjectStatusType] (
	[Id] int PRIMARY KEY,
	[Name] nvarchar(50) NOT NULL
);
GO

CREATE TABLE [ProjectStatus] (
	[Id] int IDENTITY(1,1) PRIMARY KEY,
	[ProjectId] int NOT NULL
	FOREIGN KEY REFERENCES [Project]([Id]),
	[StatusTypeId] int DEFAULT 10 NOT NULL
	FOREIGN KEY REFERENCES [ProjectStatusType]([Id]),
	[DateModified] datetime2 DEFAULT GETUTCDATE(),
	[ApproverId] int NULL
	FOREIGN KEY REFERENCES [User]([Id]),
	CONSTRAINT [CK_ProjectStatus_ApproverIsAdmin]
	CHECK (
		[ApproverId] IS NULL OR [ApproverId] IN (
			SELECT [Id] FROM [User] WHERE [UserRoleId] = 100
		)
	)
);
GO

CREATE TABLE [Comment] (
	[Id] int IDENTITY(1,1) PRIMARY KEY,
	[DateCreated] datetime2 DEFAULT GETUTCDATE(),
	[Content] nvarchar(max) NOT NULL,
	[UserId] int NOT NULL
	FOREIGN KEY REFERENCES [User]([Id]),
	[ProjectId] int NOT NULL
	FOREIGN KEY REFERENCES [Project]([Id]),
	[ParentCommentId] int NULL
	FOREIGN KEY REFERENCES [Comment]([Id])
);
GO

CREATE TABLE [Log] (
	[Id] int IDENTITY(1,1) PRIMARY KEY,
	[Timestamp] datetime2 DEFAULT GETUTCDATE(),
	[Level] nvarchar(50) NOT NULL,
	[Message] nvarchar(max) NOT NULL
);
GO

INSERT INTO [UserRole]([Id], [Name]) VALUES (10, 'User');
INSERT INTO [UserRole]([Id], [Name]) VALUES (100, 'Admin');
GO

INSERT INTO [DifficultyLevel]([Id], [Name]) VALUES (10, 'Beginner');
INSERT INTO [DifficultyLevel]([Id], [Name]) VALUES (20, 'Intermediate');
INSERT INTO [DifficultyLevel]([Id], [Name]) VALUES (30, 'Advanced');
INSERT INTO [DifficultyLevel]([Id], [Name]) VALUES (40, 'Expert');
GO

INSERT INTO [ProjectStatusType]([Id], [Name]) VALUES (10, 'Pending');
INSERT INTO [ProjectStatusType]([Id], [Name]) VALUES (20, 'Approved');
INSERT INTO [ProjectStatusType]([Id], [Name]) VALUES (30, 'Rejected');
GO
