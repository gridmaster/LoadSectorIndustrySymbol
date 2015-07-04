// -----------------------------------------------------------------------
// <copyright file="EtfUris.cs" company="Magic FireFly">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace LoadSectorIndustrySymbol.Models
{
    public class EtfUris
    {
        public const string uriReturn =
            "http://finance.yahoo.com/etf/lists/?mod_id=mediaquotesetf&tab=tab1&scol=imkt&stype=desc&rcnt={0}&page={1}";

        public const string uriReturnNav =
            "http://finance.yahoo.com/etf/lists/?mod_id=mediaquotesetf&tab=tab2&scol=nav3m&stype=desc&rcnt={0}&page={1}";

        public const string uriTradingVolume =
            "http://finance.yahoo.com/etf/lists/?mod_id=mediaquotesetf&tab=tab3&scol=volint&stype=desc&rcnt={0}&page={1}";

        public const string uriHoldings =
            "http://finance.yahoo.com/etf/lists/?mod_id=mediaquotesetf&tab=tab4&scol=avgcap&stype=desc&rcnt={0}&page={1}";

        public const string uriRisk =
            "http://finance.yahoo.com/etf/lists/?mod_id=mediaquotesetf&tab=tab5&scol=riskb&stype=desc&rcnt={0}&page={1}";

        public const string uriOperations =
            "http://finance.yahoo.com/etf/lists/?mod_id=mediaquotesetf&tab=tab6&scol=nasset&stype=desc&rcnt={0}&page={1}";

        public const string uriSymbol = 
            "http://finance.yahoo.com/q?s={0}";

    }
}
