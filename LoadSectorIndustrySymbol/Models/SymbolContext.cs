using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;

namespace LoadSectorIndustrySymbol.Models
{
    class SymbolContext : DbContext
    {
        public DbSet<ETFBaseData> ETFBaseData { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<Dividend> Dividends { get; set; }
    }
}
