using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LoadSectorIndustrySymbol.Models
{
    public class Dividend
    {
        public int Id { get; set; }
        public int SymbolId { get; set; }
        public string Symbol { get; set; }
        public DateTime Date { get; set; }
        public Decimal Amount { get; set; }

        public Dividends GetDividends(string companys)
        {
            Dividends divs = new Dividends();

            return divs;
        }
    }
}
//http://real-chart.finance.yahoo.com/table.csv?s=PG&a=01&b=8&c=2000&d=06&e=1&f=2015&g=v&ignore=.csv