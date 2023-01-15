/*
Created by: Gursharan Tatla
*/

USE [GameShoppingDB]
GO
/****** Object:  Table [dbo].[Customers]    Script Date: 14-Apr-21 4:12:45 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Customers](
	[CustomerId] [int] IDENTITY(1,1) NOT NULL,
	[CustomerName] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_Customers] PRIMARY KEY CLUSTERED 
(
	[CustomerId] ASC
))
GO
/****** Object:  Table [dbo].[Games]    Script Date: 14-Apr-21 4:12:45 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Games](
	[GameId] [int] IDENTITY(1,1) NOT NULL,
	[GameName] [nvarchar](max) NOT NULL,
	[Price] [float] NOT NULL,
	[Stock] [int] NOT NULL,
 CONSTRAINT [PK_Games] PRIMARY KEY CLUSTERED 
(
	[GameId] ASC
))
GO
/****** Object:  Table [dbo].[Orders]    Script Date: 14-Apr-21 4:12:45 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Orders](
	[OrderId] [int] IDENTITY(1,1) NOT NULL,
	[Date] [date] NOT NULL,
	[CustomerId] [int] NOT NULL,
	[GameId] [int] NOT NULL,
	[Quantity] [int] NOT NULL,
	[Discount] [float] NOT NULL,
 CONSTRAINT [PK_Orders] PRIMARY KEY CLUSTERED 
(
	[OrderId] ASC
))
GO

SET IDENTITY_INSERT [dbo].[Games] ON 

INSERT [dbo].[Games] ([GameId], [GameName], [Price], [Stock]) VALUES (1, N'FIFA', 79.9, 10)
INSERT [dbo].[Games] ([GameId], [GameName], [Price], [Stock]) VALUES (2, N'Call of Duty', 65.9, 5)
INSERT [dbo].[Games] ([GameId], [GameName], [Price], [Stock]) VALUES (3, N'Minecraft', 5.99, 1)
INSERT [dbo].[Games] ([GameId], [GameName], [Price], [Stock]) VALUES (4, N'Need for Speed', 9.99, 0)
INSERT [dbo].[Games] ([GameId], [GameName], [Price], [Stock]) VALUES (5, N'GTA V', 72.9, 2)
SET IDENTITY_INSERT [dbo].[Games] OFF
GO

ALTER TABLE [dbo].[Orders]  WITH CHECK ADD  CONSTRAINT [FK_CustomerOrder] FOREIGN KEY([CustomerId])
REFERENCES [dbo].[Customers] ([CustomerId])
GO
ALTER TABLE [dbo].[Orders] CHECK CONSTRAINT [FK_CustomerOrder]
GO
ALTER TABLE [dbo].[Orders]  WITH CHECK ADD  CONSTRAINT [FK_GameOrder] FOREIGN KEY([GameId])
REFERENCES [dbo].[Games] ([GameId])
GO
ALTER TABLE [dbo].[Orders] CHECK CONSTRAINT [FK_GameOrder]
GO
USE [master]
GO
ALTER DATABASE [GameShoppingDB] SET  READ_WRITE 
GO
