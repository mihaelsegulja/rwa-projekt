CREATE TABLE [UserRole] (
	[Id] int PRIMARY KEY,
	[Name] nvarchar(50) NOT NULL
)
GO

CREATE TABLE [User] (
	[Id] int IDENTITY(1,1) PRIMARY KEY,
	[FirstName] nvarchar(255) NOT NULL,
	[LastName] nvarchar(255) NOT NULL,
	[Username] nvarchar(50) NOT NULL,
	[Email] nvarchar(255) NOT NULL,
	[PasswordHash] nvarchar(255) NOT NULL,
	[PasswordSalt] nvarchar(255) NOT NULL,
	[Phone] nvarchar(255) NULL,
	[ProfilePicture] nvarchar(max) NULL,
	[SecurityToken] nvarchar(255) NULL,
	[IsActive] bit NOT NULL DEFAULT 1,
    [DateCreated] datetime2 NOT NULL DEFAULT GETUTCDATE(),
    [DateDeleted] datetime2 NULL,
	[UserRoleId] int NOT NULL DEFAULT 10
	FOREIGN KEY REFERENCES [UserRole]([Id])
)
GO

CREATE TABLE [Topic] (
	[Id] int IDENTITY(1,1) PRIMARY KEY,
	[Name] nvarchar(255) NOT NULL
)
GO

CREATE TABLE [DifficultyLevel] (
    [Id] int PRIMARY KEY,
    [Name] nvarchar(50) NOT NULL
)
GO

CREATE TABLE [Project] (
	[Id] int IDENTITY(1,1) PRIMARY KEY,
	[Title] nvarchar(255) NOT NULL,
	[DateCreated] datetime2 NOT NULL DEFAULT GETUTCDATE(),
	[DateModified] datetime2 NOT NULL DEFAULT GETUTCDATE(),
	[Description] nvarchar(255) NOT NULL,
	[Content] nvarchar(max) NOT NULL,
	[TopicId] int NOT NULL
	FOREIGN KEY REFERENCES [Topic]([Id]),
	[UserId] int NOT NULL
	FOREIGN KEY REFERENCES [User]([Id]),
	[DifficultyLevelId] int NOT NULL
	FOREIGN KEY REFERENCES [DifficultyLevel]([Id])
)
GO

CREATE TABLE [Image] (
	[Id] int IDENTITY(1,1) PRIMARY KEY,
	[ImageData] nvarchar(max) NOT NULL,
	[Description] nvarchar(255) NULL,
	[DateAdded] datetime2 NOT NULL DEFAULT GETUTCDATE(),
)
GO

CREATE TABLE [ProjectImage] (
	[Id] int IDENTITY(1,1) PRIMARY KEY,
	[ProjectId] int NOT NULL
	FOREIGN KEY REFERENCES [Project]([Id]),
	[ImageId] int NOT NULL
	FOREIGN KEY REFERENCES [Image]([Id]),
	[IsMainImage] bit NOT NULL DEFAULT 0
)
GO

CREATE TABLE [Material] (
	[Id] int IDENTITY(1,1) PRIMARY KEY,
	[Name] nvarchar(255) NOT NULL
)
GO

CREATE TABLE [ProjectMaterial] (
	[Id] int IDENTITY(1,1) PRIMARY KEY,
	[ProjectId] int NOT NULL
	FOREIGN KEY REFERENCES [Project]([Id]),
	[MaterialId] int NOT NULL
	FOREIGN KEY REFERENCES [Material]([Id])
)
GO

CREATE TABLE [ProjectStatusType] (
	[Id] int PRIMARY KEY,
	[Name] nvarchar(50) NOT NULL
)
GO

CREATE TABLE [ProjectStatus] (
	[Id] int IDENTITY(1,1) PRIMARY KEY,
	[ProjectId] int NOT NULL
	FOREIGN KEY REFERENCES [Project]([Id]),
	[StatusTypeId] int NOT NULL DEFAULT 10
	FOREIGN KEY REFERENCES [ProjectStatusType]([Id]),
	[DateModified] datetime2 NOT NULL DEFAULT GETUTCDATE(),
	[ApproverId] int NULL
	FOREIGN KEY REFERENCES [User]([Id])
)
GO

CREATE TABLE [Comment] (
	[Id] int IDENTITY(1,1) PRIMARY KEY,
	[DateCreated] datetime2 NOT NULL DEFAULT GETUTCDATE(),
	[Content] nvarchar(max) NOT NULL,
	[UserId] int NOT NULL
	FOREIGN KEY REFERENCES [User]([Id]),
	[ProjectId] int NOT NULL
	FOREIGN KEY REFERENCES [Project]([Id]),
	[ParentCommentId] int NULL
	FOREIGN KEY REFERENCES [Comment]([Id])
)
GO

CREATE TABLE [Log] (
	[Id] int IDENTITY(1,1) PRIMARY KEY,
	[Timestamp] datetime2 NOT NULL DEFAULT GETUTCDATE(),
	[Level] nvarchar(50) NOT NULL,
	[Message] nvarchar(max) NOT NULL
)
GO

INSERT INTO [UserRole]([Id], [Name]) VALUES
	(10, 'User'),
	(100, 'Admin')
GO

INSERT INTO [DifficultyLevel]([Id], [Name]) VALUES
	(10, 'Beginner'),
	(20, 'Intermediate'),
	(30, 'Advanced'),
	(40, 'Expert')
GO

INSERT INTO [ProjectStatusType]([Id], [Name]) VALUES
	(10, 'Pending'),
	(20, 'Approved'),
	(30, 'Rejected'),
	(40, 'Deleted')
GO

INSERT INTO [User]
           ([FirstName]
           ,[LastName]
           ,[Username]
           ,[Email]
           ,[PasswordHash]
           ,[PasswordSalt]
           ,[IsActive]
           ,[DateCreated]
           ,[UserRoleId])
     VALUES
           ('Admin'
           ,'Adminić'
           ,'Admin0000'
           ,'example@notadomain.net'
           ,'Sq0823kG7LqdQv6BX67fFsjVuYAUffPz5HNlzTrsalU='
           ,'dKhqCOUFbfqXSA4Mrxi2mg=='
           ,1
           ,GETUTCDATE()
           ,100)
GO
