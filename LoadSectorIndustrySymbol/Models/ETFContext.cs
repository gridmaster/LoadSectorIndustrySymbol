using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;

namespace LoadSectorIndustrySymbol.Models
{
    class ETFContext : DbContext
    {
        public DbSet<EtfTradingVolumes> EtfTradingVolumes { get; set; }
    }
}
