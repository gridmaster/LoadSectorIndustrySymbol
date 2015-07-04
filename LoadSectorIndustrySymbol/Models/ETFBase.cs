// -----------------------------------------------------------------------
// <copyright file="EtfBase.cs" company="Magic FireFly">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace LoadSectorIndustrySymbol.Models
{
    public abstract class EtfBase
    {
        public int Id { get; set; }

        public DateTime Date { get; set; }

        // ETF NAME
        public string EtfName { get; set; }

        //TICKER
        public string Ticker { get; set; }

        public int ExchangeId { get; set; }

        public string Exchange { get; set; }

        public abstract T LoadRow<T>(string[] rows) where T : class;

        public abstract string GetURI();
    }
}
