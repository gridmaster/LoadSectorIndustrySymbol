﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.InteropServices;
using LoadSectorIndustrySymbol.Models;
using Core.Contracts;


namespace LoadSectorIndustrySymbol.BulkLoad
{
    class BulkLoadDividends : BaseBulkLoad, IDisposable
    {
        private static readonly string[] ColumnNames = new string[] { "SymbolId", "Symbol", "Date", "Amount"};

        public BulkLoadDividends() : base(ColumnNames)
        {
            
        }

        public DataTable LoadDataTableWithDividends(IEnumerable<Dividend> dStats, DataTable dt)
        {
            foreach (var value in dStats)
            {
                var sValue = value.SymbolId + "^" + value.Symbol + "^" + value.Date + "^" + value.Amount;

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
        ~BulkLoadDividends()
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
