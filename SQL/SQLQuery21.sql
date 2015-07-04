/****** Script for SelectTopNRows command from SSMS  ******/
SELECT  [Id] 
      ,[SymbolId]
      ,[Symbol]
      ,[Date]
      ,[Open]
      ,[High]
      ,[Low]
      ,[Close]
      ,[Volume]
      ,[AdjClose]
      ,[timestamp]
  FROM [Markets].[dbo].[Dailys]
  WHERE [Id] = 100
   