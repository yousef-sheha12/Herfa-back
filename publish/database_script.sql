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

CREATE TABLE [BlacklistedTokens] (
    [Id] int NOT NULL IDENTITY,
    [Jti] nvarchar(450) NOT NULL,
    [ExpiresAt] datetime2 NOT NULL,
    [BlacklistedAt] datetime2 NOT NULL DEFAULT (GETUTCDATE()),
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NOT NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_BlacklistedTokens] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Categories] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(100) NOT NULL,
    [IconUrl] nvarchar(max) NULL,
    [Description] nvarchar(max) NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NOT NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_Categories] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [PasswordResetTokens] (
    [Id] int NOT NULL IDENTITY,
    [Email] nvarchar(max) NOT NULL,
    [Token] nvarchar(450) NOT NULL,
    [ExpiresAt] datetime2 NOT NULL,
    [IsUsed] bit NOT NULL,
    [CreatedAt] datetime2 NOT NULL DEFAULT (GETUTCDATE()),
    [UpdatedAt] datetime2 NOT NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_PasswordResetTokens] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [RefreshTokens] (
    [Id] int NOT NULL IDENTITY,
    [UserId] int NOT NULL,
    [Token] nvarchar(450) NOT NULL,
    [ExpiresAt] datetime2 NOT NULL,
    [IsRevoked] bit NOT NULL,
    [CreatedAt] datetime2 NOT NULL DEFAULT (GETUTCDATE()),
    [UpdatedAt] datetime2 NOT NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_RefreshTokens] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Users] (
    [Id] int NOT NULL IDENTITY,
    [Username] nvarchar(max) NOT NULL,
    [Email] nvarchar(450) NOT NULL,
    [PasswordHash] nvarchar(max) NOT NULL,
    [AvatarUrl] nvarchar(max) NULL,
    [Role] nvarchar(max) NOT NULL,
    [CreatedAt] datetime2 NOT NULL DEFAULT (GETUTCDATE()),
    [UpdatedAt] datetime2 NOT NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_Users] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [ArtisanProfiles] (
    [Id] int NOT NULL IDENTITY,
    [UserId] int NOT NULL,
    [CategoryId] int NOT NULL,
    [NationalId] nvarchar(15) NOT NULL,
    [Bio] nvarchar(max) NULL,
    [City] nvarchar(450) NULL,
    [Rating] real NOT NULL,
    [TotalReviews] int NOT NULL,
    [IsAvailable] bit NOT NULL,
    [IsVerified] bit NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NOT NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_ArtisanProfiles] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_ArtisanProfiles_Categories_CategoryId] FOREIGN KEY ([CategoryId]) REFERENCES [Categories] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_ArtisanProfiles_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [Notifications] (
    [Id] int NOT NULL IDENTITY,
    [UserId] int NOT NULL,
    [Title] nvarchar(200) NOT NULL,
    [Message] nvarchar(1000) NOT NULL,
    [Type] nvarchar(max) NOT NULL,
    [IsRead] bit NOT NULL DEFAULT CAST(0 AS bit),
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NOT NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_Notifications] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Notifications_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [ServiceRequests] (
    [Id] int NOT NULL IDENTITY,
    [CategoryId] int NOT NULL,
    [Title] nvarchar(200) NOT NULL,
    [Description] nvarchar(max) NOT NULL,
    [Address] nvarchar(500) NOT NULL,
    [ImageUrl] nvarchar(max) NULL,
    [Status] nvarchar(max) NOT NULL,
    [ClientId] int NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NOT NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_ServiceRequests] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_ServiceRequests_Categories_CategoryId] FOREIGN KEY ([CategoryId]) REFERENCES [Categories] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_ServiceRequests_Users_ClientId] FOREIGN KEY ([ClientId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [Jobs] (
    [Id] int NOT NULL IDENTITY,
    [ServiceRequestId] int NOT NULL,
    [ArtisanId] int NOT NULL,
    [ClientId] int NOT NULL,
    [Status] nvarchar(max) NOT NULL,
    [StartedAt] datetime2 NOT NULL DEFAULT (GETUTCDATE()),
    [CompletedAt] datetime2 NULL,
    [CreatedAt] datetime2 NOT NULL DEFAULT (GETUTCDATE()),
    [UpdatedAt] datetime2 NOT NULL DEFAULT (GETUTCDATE()),
    [IsDeleted] bit NOT NULL DEFAULT CAST(0 AS bit),
    CONSTRAINT [PK_Jobs] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Jobs_ArtisanProfiles_ArtisanId] FOREIGN KEY ([ArtisanId]) REFERENCES [ArtisanProfiles] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_Jobs_ServiceRequests_ServiceRequestId] FOREIGN KEY ([ServiceRequestId]) REFERENCES [ServiceRequests] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Jobs_Users_ClientId] FOREIGN KEY ([ClientId]) REFERENCES [Users] ([Id])
);
GO

CREATE TABLE [ServiceOffers] (
    [Id] int NOT NULL IDENTITY,
    [ServiceRequestId] int NOT NULL,
    [ArtisanId] int NOT NULL,
    [Message] nvarchar(1000) NOT NULL,
    [Price] decimal(18,2) NOT NULL,
    [Status] nvarchar(max) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NOT NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_ServiceOffers] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_ServiceOffers_ArtisanProfiles_ArtisanId] FOREIGN KEY ([ArtisanId]) REFERENCES [ArtisanProfiles] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_ServiceOffers_ServiceRequests_ServiceRequestId] FOREIGN KEY ([ServiceRequestId]) REFERENCES [ServiceRequests] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [Reviews] (
    [Id] int NOT NULL IDENTITY,
    [JobId] int NOT NULL,
    [ClientId] int NOT NULL,
    [ArtisanId] int NOT NULL,
    [Rating] int NOT NULL,
    [Comment] nvarchar(1000) NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NOT NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_Reviews] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Reviews_ArtisanProfiles_ArtisanId] FOREIGN KEY ([ArtisanId]) REFERENCES [ArtisanProfiles] ([Id]),
    CONSTRAINT [FK_Reviews_Jobs_JobId] FOREIGN KEY ([JobId]) REFERENCES [Jobs] ([Id]),
    CONSTRAINT [FK_Reviews_Users_ClientId] FOREIGN KEY ([ClientId]) REFERENCES [Users] ([Id])
);
GO

CREATE INDEX [IX_ArtisanProfiles_CategoryId] ON [ArtisanProfiles] ([CategoryId]);
GO

CREATE INDEX [IX_ArtisanProfiles_City] ON [ArtisanProfiles] ([City]);
GO

CREATE UNIQUE INDEX [IX_ArtisanProfiles_UserId] ON [ArtisanProfiles] ([UserId]);
GO

CREATE UNIQUE INDEX [IX_BlacklistedTokens_Jti] ON [BlacklistedTokens] ([Jti]);
GO

CREATE INDEX [IX_Jobs_ArtisanId] ON [Jobs] ([ArtisanId]);
GO

CREATE INDEX [IX_Jobs_ClientId] ON [Jobs] ([ClientId]);
GO

CREATE UNIQUE INDEX [IX_Jobs_ServiceRequestId] ON [Jobs] ([ServiceRequestId]);
GO

CREATE INDEX [IX_Notifications_UserId_IsRead] ON [Notifications] ([UserId], [IsRead]);
GO

CREATE UNIQUE INDEX [IX_PasswordResetTokens_Token] ON [PasswordResetTokens] ([Token]);
GO

CREATE UNIQUE INDEX [IX_RefreshTokens_Token] ON [RefreshTokens] ([Token]);
GO

CREATE INDEX [IX_Reviews_ArtisanId] ON [Reviews] ([ArtisanId]);
GO

CREATE INDEX [IX_Reviews_ClientId] ON [Reviews] ([ClientId]);
GO

CREATE UNIQUE INDEX [IX_Reviews_JobId] ON [Reviews] ([JobId]);
GO

CREATE INDEX [IX_ServiceOffers_ArtisanId] ON [ServiceOffers] ([ArtisanId]);
GO

CREATE UNIQUE INDEX [IX_ServiceOffers_ServiceRequestId_ArtisanId] ON [ServiceOffers] ([ServiceRequestId], [ArtisanId]);
GO

CREATE INDEX [IX_ServiceRequests_CategoryId] ON [ServiceRequests] ([CategoryId]);
GO

CREATE INDEX [IX_ServiceRequests_ClientId] ON [ServiceRequests] ([ClientId]);
GO

CREATE UNIQUE INDEX [IX_Users_Email] ON [Users] ([Email]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260701224715_AddReviewsAndNotifications', N'8.0.0');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [Notifications] ADD [ReferenceId] int NULL;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260702022518_AddReferenceIdToNotification', N'8.0.0');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [Notifications] ADD [Comment] nvarchar(max) NULL;
GO

ALTER TABLE [Notifications] ADD [Price] decimal(18,2) NULL;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260702022847_AddPriceAndCommentToNotification', N'8.0.0');
GO

COMMIT;
GO

