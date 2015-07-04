using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.InteropServices;
using LoadSectorIndustrySymbol.Models;

namespace LoadSectorIndustrySymbol.BulkLoad
{
    class BulkLoadDailys : BaseBulkLoad, IDisposable
    {
        public string Symbol { get; set; }
        public DateTime Date { get; set; }
        public decimal Open { get; set; }
        public decimal High { get; set; }
        public decimal Low { get; set; }
        public decimal Close { get; set; }
        public int Volume { get; set; }
        public decimal AdjClose { get; set; }

        private static readonly string[] ColumnNames = new string[]
            {
                "Symbol", "Date", "Open", "High", "Low", "Close", "Volume", "AdjClose"
            };

        public BulkLoadDailys() : base(ColumnNames)
        {

        }

        public DataTable LoadDataTableWithDailys(IEnumerable<Daily> dStats, DataTable dt)
        {
            foreach (var value in dStats)
            {
                var sValue = value.Symbol + "^" + value.Date + "^" + Open + "^" + value.High + "^" + value.Low 
                                + "^" + value.Close + "^" + value.Volume + "^" + value.AdjClose;

                DataRow row = dt.NewRow();

                row.ItemArray = sValue.Split('^');

                dt.Rows.Add(row);
            }

            return dt;
        }

        #region Implement IDisposable
        private IntPtr nativeResource = Marshal.AllocHGlobal(100);
        //More Info

        // Dispose() calls Dispose(true)
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // NOTE: Leave out the finalizer altogether if this class doesn't 
        // own unmanaged resources itself, but leave the other methods
        // exactly as they are. 
        ~BulkLoadDailys()
        {
            // Finalizer calls Dispose(false)
            Dispose(false);
        }

        // The bulk of the clean-up code is implemented in Dispose(bool)
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // free managed resources
            }
            // free native resources if there are any.
            if (nativeResource != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(nativeResource);
                nativeResource = IntPtr.Zero;
            }
        }
        #endregion Implement IDisposable
    }
}
