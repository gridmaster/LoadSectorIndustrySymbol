/****** Script for SelectTopNRows command from SSMS  ******/
SELECT d.[Id]
      ,[SymbolId]
      ,d.[Symbol]
      ,c.Name
      ,c.DivYieldPercent
      ,d.[Date]
      ,[Amount]
      ,[timestamp]
  FROM [Markets].[dbo].[Dividends] d
  JOIN [Markets].[dbo].[Companies] c ON d.[Symbol] = c.Symbol  
  WHERE d.Symbol = 'DOM' AND c.Date = '2015-06-26'
  --ORDER BY Id
  
 -- TRUNCATE TABLE [Markets].[dbo].[Dividends]