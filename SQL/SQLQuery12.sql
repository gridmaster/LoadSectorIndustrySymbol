SELECT COUNT([Symbol])
      ,[Symbol]
  FROM [Markets].[dbo].[Dividends]
  GROUP BY [Symbol]
  ORDER BY 1 DESC