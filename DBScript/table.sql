USE [ProductScanDB]
GO

/****** Object:  Table [dbo].[ProductDetails]    Script Date: 11-06-2019 2.33.55 PM ******/
DROP TABLE [dbo].[ProductDetails]
GO

/****** Object:  Table [dbo].[ProductDetails]    Script Date: 11-06-2019 2.33.55 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ProductDetails](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](200) NULL,
	[Price] [decimal](5, 2) NULL,
	[Description] [nvarchar](200) NULL,
	[Datetime] [datetime] NULL,
 CONSTRAINT [PK_Product] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


