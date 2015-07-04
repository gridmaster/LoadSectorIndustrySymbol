SELECT d.[Symbol]
      ,c.Name
      ,c.DivYieldPercent
      ,d.[Date]
      ,[Amount]
  FROM [Markets].[dbo].[Dividends] d
  JOIN [Markets].[dbo].[Companies] c ON d.[Symbol] = c.Symbol  
  WHERE c.Date = '2015-06-26' AND d.Date > '2014-12-30'
  ORDER BY DivYieldPercent DESC