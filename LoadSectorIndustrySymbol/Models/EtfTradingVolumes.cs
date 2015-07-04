using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoadSectorIndustrySymbol.Models
{
    public class EtfTradingVolumes : EtfBase
    {
        //VOLUME (INTRADAY)
        public string IntradayVolume { get; set; }

        //VOLUME (3-MO AVG)
        public string ThreeMoAveragevolume { get; set; }

        //LAST TRADE
        public string LastTrade { get; set; }

        //52-WEEK HIGH	
        public string WeekHigh52 { get; set; }

        //52-WEEK LOW
        public string WeekLow52 { get; set; }

        public override T LoadRow<T>(string[] rows)
        {
            if (rows.Length < 10)
                throw new ArgumentException("requires 10 rows to be passed.");

            Date = DateTime.Now;
            EtfName = rows[1];
            Ticker = rows[2];
       //     Category = rows[3];
       //     FundFamily = rows[4];
            IntradayVolume = rows[5];
            ThreeMoAveragevolume = rows[6];
            LastTrade = rows[7];
            WeekHigh52 = rows[8];
            WeekLow52 = rows[9];

            return this as T;
        }

        public override string GetURI()
        {
            return EtfUris.uriTradingVolume;
        }
    }
}
