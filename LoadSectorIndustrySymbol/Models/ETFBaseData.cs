// -----------------------------------------------------------------------
// <copyright file="EtfTradingVolume.cs" company="Magic FireFly">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace LoadSectorIndustrySymbol.Models
{
    public class ETFBaseData : EtfBase
    {
        public override T LoadRow<T>(string[] rows)
        {
            if (rows.Length < 3)
                throw new ArgumentException("requires 3 rows to be passed.");

            Date = DateTime.Now;
            EtfName = rows[1];
            Ticker = rows[2];
            ExchangeId = 0;
            Exchange = rows[3];
            return this as T;
        }

        public override string GetURI()
        {
            return EtfUris.uriTradingVolume;
        }
    }
}
