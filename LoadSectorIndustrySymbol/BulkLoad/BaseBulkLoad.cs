using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

using Core.Contracts;

namespace LoadSectorIndustrySymbol.BulkLoad
{
    public class BaseBulkLoad
    {
        protected readonly ILogger logger;
        private string[] ColumnNames;

        public BaseBulkLoad(string[] columnNames)
        {
            if (columnNames.Length == 0)
                throw new ArgumentNullException("columnNames", "Bulk Load must have column names");

            ColumnNames = columnNames;
            // this.logger = logger;
        }

        public DataTable ConfigureDataTable()
        {
            var dt = new DataTable();

            for (int i = 0; i < ColumnNames.Length; i++)
            {
                dt.Columns.Add(new DataColumn());
                dt.Columns[i].ColumnName = ColumnNames[i];
            }
            return dt;
        }

        public void BulkCopy<T>(DataTable dt)
        {
            string connString = ConfigurationManager.ConnectionStrings["SymbolContext"].ConnectionString;

            string tableName = typeof(T).Name;

            using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connString))
            {
                for (int i = 0; i < ColumnNames.Length; i++)
                    bulkCopy.ColumnMappings.Add(i, ColumnNames[i]);

                bulkCopy.BulkCopyTimeout = 60; // in seconds 
                bulkCopy.DestinationTableName = tableName;
                try
                {
                    bulkCopy.WriteToServer(dt);
                }
                catch (Exception ex)
                {
                    //Log.WriteLog(new LogEvent("BulkLoadSector - BulkCopy<" + tableName + ">", "Bulk load error: " + ex.Message));
                }
                bulkCopy.Close();
            }
        }

        public static void UpdateData<T>(DataTable dt, string SQL)
        {
            string tableName = typeof(T).Name;

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["SymbolContext"].ConnectionString))
            {
                using (SqlCommand command = new SqlCommand("", conn))
                {
                    try
                    {
                        conn.Open();

                        //Creating temp table on database
                        command.CommandText = "CREATE TABLE #TmpTable( [Id] [int] IDENTITY(1,1) NOT NULL, [Date] [datetime] NOT NULL," 
                            + " [EtfName] [nvarchar](200) NOT NULL, [ExchangeId] int NULL, [Exchange] [nvarchar](60) NULL, "
                            + " [Ticker] [nvarchar](30) NOT NULL)";
                        command.ExecuteNonQuery();

                        //Bulk insert into temp table
                        using (SqlBulkCopy bulkcopy = new SqlBulkCopy(conn))
                        {
                            bulkcopy.BulkCopyTimeout = 660;
                            bulkcopy.DestinationTableName = tableName;
                            bulkcopy.WriteToServer(dt);
                            bulkcopy.Close();
                        }

                        // Updating destination table, and dropping temp table
                        command.CommandTimeout = 300;
                        command.CommandText = "UPDATE T SET [Date], [EtfName], [ExchangeId], [Exchange], [Ticker]  FROM " + tableName + " T INNER JOIN #TmpTable Temp ON ...; DROP TABLE #TmpTable;";
                        command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        // Handle exception properly
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
        }
    }
}