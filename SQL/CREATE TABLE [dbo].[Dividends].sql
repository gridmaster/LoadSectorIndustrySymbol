USE [Markets]
GO

/****** Object:  Table [dbo].[Dividends]    Script Date: 07/03/2015 15:00:36 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Dailys] (
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[SymbolId] [int] NULL,
	[Symbol] [nvarchar](40) NOT NULL,
	[Date] [datetime] NOT NULL,
	[Open] [decimal](18, 4) NOT NULL,
	[High] [decimal](18, 4) NOT NULL,
	[Low] [decimal](18, 4) NOT NULL,
	[Close] [decimal](18, 4) NOT NULL,
	[Volume] [int] NOT NULL,
	[AdjClose] [decimal](18, 4) NOT NULL,					
	[timestamp] [timestamp] NULL,
PRIMARY KEY CLUSTERED 
(
	[Symbol] ASC,
	[Date] ASC,
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


