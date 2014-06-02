USE [master]
GO
/****** Object:  Database [PTFScan]    Script Date: 02/20/2013 17:15:59 ******/
CREATE DATABASE [PTFScan] ON  PRIMARY 
( NAME = N'PTFScan', FILENAME = N'e:\DATA\PTFScan.mdf' , SIZE = 789504KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'PTFScan_log', FILENAME = N'e:\DATA\PTFScan_log.ldf' , SIZE = 470144KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
GO
ALTER DATABASE [PTFScan] SET COMPATIBILITY_LEVEL = 100
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [PTFScan].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [PTFScan] SET ANSI_NULL_DEFAULT OFF
GO
ALTER DATABASE [PTFScan] SET ANSI_NULLS OFF
GO
ALTER DATABASE [PTFScan] SET ANSI_PADDING OFF
GO
ALTER DATABASE [PTFScan] SET ANSI_WARNINGS OFF
GO
ALTER DATABASE [PTFScan] SET ARITHABORT OFF
GO
ALTER DATABASE [PTFScan] SET AUTO_CLOSE OFF
GO
ALTER DATABASE [PTFScan] SET AUTO_CREATE_STATISTICS ON
GO
ALTER DATABASE [PTFScan] SET AUTO_SHRINK OFF
GO
ALTER DATABASE [PTFScan] SET AUTO_UPDATE_STATISTICS ON
GO
ALTER DATABASE [PTFScan] SET CURSOR_CLOSE_ON_COMMIT OFF
GO
ALTER DATABASE [PTFScan] SET CURSOR_DEFAULT  GLOBAL
GO
ALTER DATABASE [PTFScan] SET CONCAT_NULL_YIELDS_NULL OFF
GO
ALTER DATABASE [PTFScan] SET NUMERIC_ROUNDABORT OFF
GO
ALTER DATABASE [PTFScan] SET QUOTED_IDENTIFIER OFF
GO
ALTER DATABASE [PTFScan] SET RECURSIVE_TRIGGERS OFF
GO
ALTER DATABASE [PTFScan] SET  ENABLE_BROKER
GO
ALTER DATABASE [PTFScan] SET AUTO_UPDATE_STATISTICS_ASYNC OFF
GO
ALTER DATABASE [PTFScan] SET DATE_CORRELATION_OPTIMIZATION OFF
GO
ALTER DATABASE [PTFScan] SET TRUSTWORTHY OFF
GO
ALTER DATABASE [PTFScan] SET ALLOW_SNAPSHOT_ISOLATION OFF
GO
ALTER DATABASE [PTFScan] SET PARAMETERIZATION SIMPLE
GO
ALTER DATABASE [PTFScan] SET READ_COMMITTED_SNAPSHOT OFF
GO
ALTER DATABASE [PTFScan] SET HONOR_BROKER_PRIORITY OFF
GO
ALTER DATABASE [PTFScan] SET  READ_WRITE
GO
ALTER DATABASE [PTFScan] SET RECOVERY SIMPLE
GO
ALTER DATABASE [PTFScan] SET  MULTI_USER
GO
ALTER DATABASE [PTFScan] SET PAGE_VERIFY CHECKSUM
GO
ALTER DATABASE [PTFScan] SET DB_CHAINING OFF
GO
EXEC sys.sp_db_vardecimal_storage_format N'PTFScan', N'ON'
GO
USE [PTFScan]
GO
/****** Object:  User [FINTRAX\GG_TRS_DBAs]    Script Date: 02/20/2013 17:15:59 ******/
CREATE USER [FINTRAX\GG_TRS_DBAs] FOR LOGIN [FINTRAX\GG_TRS_DBAs]
GO
/****** Object:  User [FINTRAX\GG_SQL_Admins_ALL]    Script Date: 02/20/2013 17:15:59 ******/
CREATE USER [FINTRAX\GG_SQL_Admins_ALL] FOR LOGIN [FINTRAX\GG_SQL_Admins_ALL]
GO
/****** Object:  Table [dbo].[CONFIG]    Script Date: 02/20/2013 17:16:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CONFIG](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[SourceID] [int] NOT NULL,
	[Key] [nvarchar](100) NOT NULL,
	[Value] [sql_variant] NULL,
 CONSTRAINT [PK_CONFIG] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CLIENTS]    Script Date: 02/20/2013 17:16:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CLIENTS](
	[ClientID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[IP] [nvarchar](100) NOT NULL,
	[Enabled] [bit] NOT NULL,
	[DateCreated] [datetime] NOT NULL,
	[DateLastVisit] [datetime] NOT NULL,
 CONSTRAINT [PK_CLIENTS] PRIMARY KEY CLUSTERED 
(
	[ClientID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MESSAGETYPES]    Script Date: 02/20/2013 17:16:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MESSAGETYPES](
	[ID] [int] NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_MESSAGETYPES] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MESSAGESOURCES]    Script Date: 02/20/2013 17:16:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MESSAGESOURCES](
	[ID] [int] NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_MESSAGESOURCES] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  StoredProcedure [dbo].[spCreateClient]    Script Date: 02/20/2013 17:17:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[spCreateClient]
	(
	@Name		nvarchar(100),
	@IP			nvarchar(100),
	@ID			int OUTPUT
	)
AS
BEGIN
	SET NOCOUNT ON
	--INSERT INTO CLIENTS([Name], IP) VALUES (@Name, @IP);
	
	MERGE CLIENTS AS target
    USING (SELECT @Name, @IP) AS source (Name, IP)
    ON (target.IP = source.IP)
    WHEN MATCHED THEN 
        UPDATE SET Name = source.Name
	WHEN NOT MATCHED THEN	
	    INSERT (Name, IP)
	    VALUES (source.Name, source.IP);
	    
	SELECT @ID = ClientID FROM CLIENTS WHERE Name = @Name AND IP = @IP;
END
GO
/****** Object:  Table [dbo].[MESSAGES]    Script Date: 02/20/2013 17:17:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MESSAGES](
	[MessageID] [int] IDENTITY(1,1) NOT NULL,
	[ClientID] [int] NOT NULL,
	[Message] [nvarchar](1024) NOT NULL,
	[Type] [int] NOT NULL,
	[SourceID] [int] NOT NULL,
	[StackTrace] [nvarchar](1024) NULL,
	[DateGenerated] [datetime] NOT NULL,
	[DateInserted] [datetime] NOT NULL,
 CONSTRAINT [PK_MESSAGES] PRIMARY KEY CLUSTERED 
(
	[MessageID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[FILES]    Script Date: 02/20/2013 17:17:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[FILES](
	[FileID] [int] IDENTITY(1,1) NOT NULL,
	[ClientID] [int] NOT NULL,
	[CountryID] [int] NULL,
	[RetailerID] [int] NULL,
	[VoucherID] [varchar](30) NULL,
	[SiteCode] [varchar](30) NULL,
	[Comment] [nvarchar](256) NULL,
	[VoucherImage] [varbinary](max) NULL,
	[BarCodeImage] [varbinary](max) NULL,
	[DateAllocated] [datetime] NOT NULL,
	[DateScanned] [datetime] NULL,
	[DateInserted] [datetime] NULL,
 CONSTRAINT [PK_FILES] PRIMARY KEY CLUSTERED 
(
	[FileID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'PrimaryKey' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FILES', @level2type=N'COLUMN',@level2name=N'FileID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'ClientID from CLIENTS' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FILES', @level2type=N'COLUMN',@level2name=N'ClientID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Country code from TRS database' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FILES', @level2type=N'COLUMN',@level2name=N'CountryID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'RetailerID written on voucher' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FILES', @level2type=N'COLUMN',@level2name=N'RetailerID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'VoucherID written on voucher' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FILES', @level2type=N'COLUMN',@level2name=N'VoucherID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'SiteCode from TRS database' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FILES', @level2type=N'COLUMN',@level2name=N'SiteCode'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Additional comment added to the scan operation' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FILES', @level2type=N'COLUMN',@level2name=N'Comment'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'VoucherImage ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FILES', @level2type=N'COLUMN',@level2name=N'VoucherImage'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'BarcodeImage' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FILES', @level2type=N'COLUMN',@level2name=N'BarCodeImage'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Date this record has been allocated' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FILES', @level2type=N'COLUMN',@level2name=N'DateAllocated'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Date scan was done' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FILES', @level2type=N'COLUMN',@level2name=N'DateScanned'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Date this record was been inserted back' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FILES', @level2type=N'COLUMN',@level2name=N'DateInserted'
GO
/****** Object:  Table [dbo].[STATIONS_CFG]    Script Date: 02/20/2013 17:17:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[STATIONS_CFG](
	[StationCfgID] [int] IDENTITY(1,1) NOT NULL,
	[ClientID] [int] NOT NULL,
	[Key] [nvarchar](100) NOT NULL,
	[Value] [nvarchar](1024) NOT NULL,
	[DateLastUpdated] [datetime] NOT NULL,
 CONSTRAINT [PK_CLIENT_CONFIGURATION_DATAS] PRIMARY KEY CLUSTERED 
(
	[StationCfgID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  StoredProcedure [dbo].[spSelectClients]    Script Date: 02/20/2013 17:17:43 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[spSelectClients]
AS
	SELECT IP FROM CLIENTS WHERE Enabled = 1;
GO
/****** Object:  StoredProcedure [dbo].[spInsertMessage]    Script Date: 02/20/2013 17:17:43 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[spInsertMessage]
	(
	@ClientID	int,
	@Message	varchar(1024),
	@Type		int,
	@SourceID	int,
	@StackTrace	varchar(1024),
	@DateGenerated datetime
	)
AS
BEGIN
	SET NOCOUNT ON 
	INSERT INTO MESSAGES(ClientID, Message, Type, SourceID, StackTrace, DateGenerated) VALUES
						(@ClientID, @Message, @Type, @SourceID, @StackTrace, @DateGenerated);
END
GO
/****** Object:  StoredProcedure [dbo].[spInsertFile]    Script Date: 02/20/2013 17:17:43 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[spInsertFile]
	(
	@FileID int,
	@ClientID int,
	@BatchID uniqueidentifier,
	@FileName nvarchar(100),
	@CountryCode int,
	@RetailerID int,
	@VoucherID int,
	
	@SiteCode varchar(20),
	@RelatedID int,
	@Comment nchar(100),
	
	 
	@VoucherImage varbinary(MAX),
	@BarCodeImage varbinary(MAX) = NULL,
	@VoucherData varbinary(MAX) = NULL,
	@DateScanned datetime
	)
AS
BEGIN
	SET NOCOUNT ON
	UPDATE FILES SET
	 ClientID = @ClientID, 
	 BatchID = @BatchID, 
	 [FileName] = @FileName, 
	 CountryCode = @CountryCode, 
	 RetailerID = @RetailerID,
	 VoucherID = @VoucherID, 
	 SiteCode = @SiteCode,	  
	 RelatedID = @RelatedID, 
	 Comment = @Comment,	 
	 VoucherImage = @VoucherImage, 
	 BarCodeImage = @BarCodeImage,
	 VoucherData = @VoucherData, 
	 DateScanned = @DateScanned,
	 DateInserted = getdate()
	WHERE
	FileID = @FileID;
	
	if @@rowcount <> 1
	  RAISERROR ('None or many results found', -- Message text.
               16, -- Severity.
               1 -- State.
               );
	
END
GO
/****** Object:  StoredProcedure [dbo].[spGenerateAuditIDs]    Script Date: 02/20/2013 17:17:44 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[spGenerateAuditIDs]
(
	@ClientID int,
	@Count int,
	@AuditIDFrom int OUTPUT,
	@AuditIDTo int OUTPUT
)
AS
BEGIN

BEGIN TRAN;

	SELECT @AuditIDFrom = ISNULL(MAX(FileID),0) + 1 FROM FILES;
	
	DECLARE @ID INT;
	SET @ID = 1;
	WHILE (@ID < @Count)
	BEGIN
		INSERT INTO FILES(ClientID, DateAllocated) VALUES (@ClientID, getdate());
		SET @ID = @ID + 1;
	END
	
	SELECT @AuditIDTo = MAX(FileID) FROM FILES;

COMMIT TRAN;
END
GO
/****** Object:  Default [DF_CLIENTS_Enabled]    Script Date: 02/20/2013 17:16:03 ******/
ALTER TABLE [dbo].[CLIENTS] ADD  CONSTRAINT [DF_CLIENTS_Enabled]  DEFAULT ((0)) FOR [Enabled]
GO
/****** Object:  Default [DF_CLIENTS_DateCreated]    Script Date: 02/20/2013 17:16:03 ******/
ALTER TABLE [dbo].[CLIENTS] ADD  CONSTRAINT [DF_CLIENTS_DateCreated]  DEFAULT (getdate()) FOR [DateCreated]
GO
/****** Object:  Default [DF_CLIENTS_DateLastVisited]    Script Date: 02/20/2013 17:16:03 ******/
ALTER TABLE [dbo].[CLIENTS] ADD  CONSTRAINT [DF_CLIENTS_DateLastVisited]  DEFAULT (getdate()) FOR [DateLastVisit]
GO
/****** Object:  Default [DF_MESSAGES_DateInserted]    Script Date: 02/20/2013 17:17:42 ******/
ALTER TABLE [dbo].[MESSAGES] ADD  CONSTRAINT [DF_MESSAGES_DateInserted]  DEFAULT (getdate()) FOR [DateInserted]
GO
/****** Object:  Default [DF_CLIENT_CONFIGURATION_DATAS_DateInserted]    Script Date: 02/20/2013 17:17:42 ******/
ALTER TABLE [dbo].[STATIONS_CFG] ADD  CONSTRAINT [DF_CLIENT_CONFIGURATION_DATAS_DateInserted]  DEFAULT (getdate()) FOR [DateLastUpdated]
GO
/****** Object:  ForeignKey [FK_MESSAGES_CLIENTS]    Script Date: 02/20/2013 17:17:42 ******/
ALTER TABLE [dbo].[MESSAGES]  WITH CHECK ADD  CONSTRAINT [FK_MESSAGES_CLIENTS] FOREIGN KEY([ClientID])
REFERENCES [dbo].[CLIENTS] ([ClientID])
GO
ALTER TABLE [dbo].[MESSAGES] CHECK CONSTRAINT [FK_MESSAGES_CLIENTS]
GO
/****** Object:  ForeignKey [FK_MESSAGES_MESSAGESOURCES]    Script Date: 02/20/2013 17:17:42 ******/
ALTER TABLE [dbo].[MESSAGES]  WITH CHECK ADD  CONSTRAINT [FK_MESSAGES_MESSAGESOURCES] FOREIGN KEY([SourceID])
REFERENCES [dbo].[MESSAGESOURCES] ([ID])
GO
ALTER TABLE [dbo].[MESSAGES] CHECK CONSTRAINT [FK_MESSAGES_MESSAGESOURCES]
GO
/****** Object:  ForeignKey [FK_MESSAGES_MESSAGETYPES]    Script Date: 02/20/2013 17:17:42 ******/
ALTER TABLE [dbo].[MESSAGES]  WITH CHECK ADD  CONSTRAINT [FK_MESSAGES_MESSAGETYPES] FOREIGN KEY([Type])
REFERENCES [dbo].[MESSAGETYPES] ([ID])
GO
ALTER TABLE [dbo].[MESSAGES] CHECK CONSTRAINT [FK_MESSAGES_MESSAGETYPES]
GO
/****** Object:  ForeignKey [FK_FILES_CLIENTS]    Script Date: 02/20/2013 17:17:42 ******/
ALTER TABLE [dbo].[FILES]  WITH CHECK ADD  CONSTRAINT [FK_FILES_CLIENTS] FOREIGN KEY([ClientID])
REFERENCES [dbo].[CLIENTS] ([ClientID])
GO
ALTER TABLE [dbo].[FILES] CHECK CONSTRAINT [FK_FILES_CLIENTS]
GO
/****** Object:  ForeignKey [FK_STATIONS_CFG_CLIENTS]    Script Date: 02/20/2013 17:17:42 ******/
ALTER TABLE [dbo].[STATIONS_CFG]  WITH CHECK ADD  CONSTRAINT [FK_STATIONS_CFG_CLIENTS] FOREIGN KEY([ClientID])
REFERENCES [dbo].[CLIENTS] ([ClientID])
GO
ALTER TABLE [dbo].[STATIONS_CFG] CHECK CONSTRAINT [FK_STATIONS_CFG_CLIENTS]
GO
