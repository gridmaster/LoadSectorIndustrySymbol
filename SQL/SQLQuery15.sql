SELECT COUNT(d.[Symbol]) AS 'Count'
      ,d.[Symbol]
            ,c.Name
      ,c.DivYieldPercent
  FROM [Markets].[dbo].[Dividends] d
  JOIN [Markets].[dbo].[Companies] c ON d.[Symbol] = c.Symbol  
  WHERE c.Date = '2015-06-26' --AND d.Date > '2014-12-30'
  GROUP BY d.[Symbol], c.Name, c.DivYieldPercent
HAVING c.DivYieldPercent > 4.999 AND COUNT(d.[Symbol]) > 20
  ORDER BY 4 DESC -- c.DivYieldPercent DESC