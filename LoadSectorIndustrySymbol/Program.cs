using System;
using System.Data;
using System.Data.Entity;  
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using Ninject;
using LoadSectorIndustrySymbol.BulkLoad;
using LoadSectorIndustrySymbol.DIModule;
using LoadSectorIndustrySymbol.Models;
using LoadSectorIndustrySymbol.Web;
using DIContainer;

namespace LoadSectorIndustrySymbol
{
    class Program 
    {
        #region tunable resources
        public static int multipliar = 100;
        public static decimal maxLoss = -0.04m;
        public static decimal maxPain = 0.92m;
        public static decimal profitSellStop = 0.99m;
        public static decimal splitTest = 200m;
        public static int numberOfMonths = -1;
        public static int maxAge = 30;

        public static DateTime startDate = new DateTime(2013, 11, 01);
        #endregion tunable resources

        #region initialize DI container

        private static void InitializeDiContainer()
        {
            NinjectSettings settings = new NinjectSettings
            {
                LoadExtensions = false
            };

            // change DesignTimeModule to run other implementation ProductionModule || DebugModule
            IOCContainer.Instance.Initialize(settings, new DebugModule());
        }

        #endregion Initialize DI Container

        static void Main(string[] args)
        {
            bool writeETFExchanges = false;
            bool writeOutputFiles = false;
            bool writeBulkFiles = false;
            bool loadExchanges = false;
            bool writeDividends = false;
            bool writeHistory = true;

            string uri = WebPage.GetDividendUri("PG", DateTime.Now);
            Database.SetInitializer<SymbolContext>(null);

            Console.WriteLine("Start: {0}", DateTime.Now);

            if (writeHistory)
            {
                Console.WriteLine("Get History: {0}", DateTime.Now);

                GetHistory();
            }

            if (writeDividends)
            {
                Console.WriteLine("Get Dividends: {0}", DateTime.Now);

                Dividends dividends = GetDividends();

                using (BulkLoadDividends bls = new BulkLoadDividends())
                {
                    var dt = bls.ConfigureDataTable();
                    dt = bls.LoadDataTableWithDividends(dividends, dt);
                    bls.BulkCopy<Dividends>(dt);
                }
            }

            if (writeETFExchanges)
            {
                Console.WriteLine("Get ETFs: {0}", DateTime.Now);
                ETFs etfBase = LoadETFExchanges();

                using (BulkLoadETFBase bls = new BulkLoadETFBase())
                {
                    var dt = bls.ConfigureDataTable();
                    dt = bls.LoadDataTableWithBasicETF(etfBase, dt);
                    bls.BulkCopy<ETFs>(dt);
                }
                Console.WriteLine("Got ETFs: {0}", DateTime.Now);
            }

            #region os
            //========> Get Sectors
            Sectors sectors = GetSectors();

            if (writeBulkFiles)
            {
                using (BulkLoadSector bls = new BulkLoadSector())
                {
                    var dt = bls.ConfigureDataTable();
                    dt = bls.LoadDataTableWithSectors(sectors, dt);
                    bls.BulkCopy<Sectors>(dt);
                }
            }
            Console.WriteLine("Got Sectors: {0}", DateTime.Now);

            //========> Get Industries
            Industries industries = GetIndustries(sectors);

            if (writeBulkFiles)
            {
                using (BulkLoadIndustry bli = new BulkLoadIndustry())
                {
                    var dti = bli.ConfigureDataTable();
                    dti = bli.LoadDataTableWithIndustries(industries, dti);
                    bli.BulkCopy<Industries>(dti);
                }
            }
            Console.WriteLine("Got Industries: {0}", DateTime.Now);

            //========> Get Companies
            Companies companies = GetCompanies(industries);
            Console.WriteLine("Got Companies: {0}", DateTime.Now);

            if (loadExchanges)
            {
                LoadCompanyExchanges(companies);
                Console.WriteLine("Got Exchanges: {0}", DateTime.Now);
            }

            if (writeBulkFiles)
            {
                using (BulkLoadCompany blc = new BulkLoadCompany())
                {
                    var dtc = blc.ConfigureDataTable();
                    dtc = blc.LoadDataTableWithIndustries(companies, dtc);
                    blc.BulkCopy<Companies>(dtc);

                }
            }

            #region write output
            if (writeOutputFiles)
            {
                Console.WriteLine("Write Output files: {0}", DateTime.Now);
                string json = JsonConvert.SerializeObject(companies);
                using (StreamWriter sw = new StreamWriter("C:/Temp/companies.txt"))
                {
                    sw.Write(json);
                }

                json = JsonConvert.SerializeObject(companies.Where(c => c.Exchange != null));
                using (StreamWriter sw = new StreamWriter("C:/Temp/companiesWExchanges.txt"))
                {
                    sw.Write(json);
                }

                json = JsonConvert.SerializeObject(sectors);
                using (StreamWriter sw = new StreamWriter("C:/Temp/Sectors.txt"))
                {
                    sw.Write(json);
                }

                json = JsonConvert.SerializeObject(industries);
                using (StreamWriter sw = new StreamWriter("C:/Temp/Industries.txt"))
                {
                    sw.Write(json);
                }
                Console.WriteLine("Wrote Output files: {0}", DateTime.Now);
            }
            #endregion write output

            #endregion os

            Console.WriteLine("End: {0}", DateTime.Now);
            Console.ReadKey();
        }

        #region private methods
        private static void GetHistory()
        {
            Company company = GetMaxDate();

            using (SymbolContext db = new SymbolContext())
            {
                Dailys dailys = new Dailys();

                var companeez = from c in db.Companies
                                where c.Date.Equals(company.Date)
                                select c;

                foreach (Company item in companeez)
                {
                    if (item.Exchange.IndexOf("NYS") == -1 && item.Exchange.IndexOf("Nasd") == -1) continue;

                    string uriString = WebPage.GetHistoryUri(item.Symbol, DateTime.Now);

                    string sPage = WebPage.Get(uriString);
                    //http://real-chart.finance.yahoo.com/table.csv?s=PIH&a=1&b=1&c=1900&d=d&a=7&e=4&f=2015&g=v&ignore=.csv
                    string[] dailyArray = sPage.Split('\n');
                    for (int i = 1; i < dailyArray.Length; i++)
                    {
                        if (dailyArray[i].Length == 0) continue;
                        string date = dailyArray[i].Split(',')[0];
                        string open = dailyArray[i].Split(',')[1];
                        string high = dailyArray[i].Split(',')[2];
                        string low = dailyArray[i].Split(',')[3];
                        string close = dailyArray[i].Split(',')[4];
                        string volume = dailyArray[i].Split(',')[5];
                        string adjClose = dailyArray[i].Split(',')[6];

                        Daily daily = new Daily()
                        {
                            Symbol = item.Symbol,
                            Date = Convert.ToDateTime(date),
                            Open = Convert.ToDecimal(open),
                            High = Convert.ToDecimal(high),
                            Low = Convert.ToDecimal(low),
                            Close = Convert.ToDecimal(close),
                            Volume = Convert.ToInt32(volume),
                            AdjClose = Convert.ToDecimal(adjClose),
                        };
                        dailys.Add(daily);
                    }

                    using (BulkLoadDailys bls = new BulkLoadDailys())
                    {
                        var dt = bls.ConfigureDataTable();
                        dt = bls.LoadDataTableWithDailys(dailys, dt);
                        bls.BulkCopy<Dailys>(dt);
                    }
                }
            }
        }

        private static Dividends GetDividends()
        {
            var date = DateTime.Now;

            Dividends dividends = new Dividends();
            Company company = GetMaxDate();

            using (SymbolContext db = new SymbolContext())
            {

                var companeez = from c in db.Companies
                                where c.Date.Equals(company.Date)
                                select c;
                foreach (Company item in companeez)
                {

                    if (item.Exchange.IndexOf("NYS") == -1 && item.Exchange.IndexOf("Nasd") == -1) continue;

                    string uriString = WebPage.GetDividendUri(item.Symbol, DateTime.Now);

                    string sPage = WebPage.Get(uriString);

                    string[] dailyArray = sPage.Split('\n');
                    for (int i = 1; i < dailyArray.Length; i++)
                    {
                        if (dailyArray[i].Length == 0) continue;
                        string divDate = dailyArray[i].Split(',')[0];
                        string amount = dailyArray[i].Split(',')[1];
                        Dividend div = new Dividend()
                        {
                            Symbol = item.Symbol,
                            Amount = Convert.ToDecimal(amount),
                            Date = Convert.ToDateTime(divDate)
                        };
                        dividends.Add(div);
                    }
                }
            }
            return dividends;
        }

        private static Company GetMaxDate()
        {
            int selected = 0;
            using (var db = new SymbolContext())
            {
                Company company = new Company();
                try
                {
                    selected = (from c in db.Companies
                                select c.id).Max();
                    company = db.Companies.Where(c => c.id == selected).SingleOrDefault();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Program.cs - GetMaxDate Error:", ex.ToString());
                }

                return company;
            }
        }


        private static Companies LoadCompanyExchanges(Companies companies)
        {
            int Count = 0;

            for (int i = 0; i < companies.Count(); i++)
            {
                string sPage = WebPage.Get(companies[i].URI);
                sPage = sPage.Replace("\r\n", " ").Replace("\r", " ").Replace("\n", " ");

                if (sPage == "N/A") continue;

                int index = sPage.IndexOf("class=\"title\"");
                if (index == -1) continue;

                string testIt = sPage.Substring(index, 200);

                string[] stringSeparators = new string[] { "<td", "<h2>", "<span" };

                var columns = testIt.Split(stringSeparators, StringSplitOptions.None);

                if (columns[3].IndexOf("</span") == -1)
                {
                    continue;
                }

                stringSeparators[0] = "</span";
                string getit = columns[3].Substring(columns[3].IndexOf("</span")).Replace("</span>", "").Trim();

                companies[i].Exchange = getit;
                Count++;
            }

            Console.WriteLine("Found {0} Exchanges", Count);

            return companies;
        }

        private static ETFs LoadETFExchanges()
        {
            ETFs etfBase = new ETFs();
            ETFContext db = new ETFContext();

            var yesterday = DateTime.Now.AddDays(-1);
            var data = db.EtfTradingVolumes.Where(e => e.Date > yesterday);

            int Count = 0;

            foreach (var item in data)
            {
                string sPage = WebPage.Get(string.Format(Models.EtfUris.uriSymbol, item.Ticker));
                sPage = sPage.Replace("\r\n", " ").Replace("\r", " ").Replace("\n", " ");

                if (sPage == "N/A") continue;

                int index = sPage.IndexOf("class=\"title\"");
                if (index == -1) continue;

                string testIt = sPage.Substring(index, 200);

                string[] stringSeparators = new string[] { "<td", "<h2>", "<span" };

                var columns = testIt.Split(stringSeparators, StringSplitOptions.None);

                if (columns[3].IndexOf("</span") == -1)
                {
                    continue;
                }

                stringSeparators[0] = "</span";
                string exchange = columns[3].Substring(columns[3].IndexOf("</span")).Replace("</span>", "").Trim();

                ETFBaseData etfbd = new ETFBaseData()
                {
                    Date = item.Date,
                    EtfName = item.EtfName,
                    ExchangeId = item.ExchangeId,
                    Exchange = exchange,
                    Ticker = item.Ticker
                };

                etfBase.Add(etfbd);

                Count++;
            }

            Console.WriteLine("Found {0} Exchanges", Count);

            return etfBase;
        }

        private static Companies GetCompanies(Industries industries)
        {
            string path = "ErrorLog.txt";
            Companies companies = new Companies();

            if (File.Exists(path)) File.Delete(path);

            using (StreamWriter sw = new StreamWriter(path, true))
            {
                using (Company company = new Company())
                {
                    // for each industry, retreve company data
                    for (int i = 0; i < industries.Count(); i++)
                    {
                        sw.WriteLine("Industry: " + industries[i].Name);
                        string[] rows = GetRows(industries[i].URI);

                        Companies companiesByIndustries = company.GetCompanies(industries[i].Sector, industries[i].Name, rows, sw);

                        companies.AddRange(companiesByIndustries);
                    }
                }
            }
            return companies;
        }

        private static Industries GetIndustries(Sectors sectors)
        {
            Industries industries = new Industries();
            using (Industry industry = new Industry())
            {
                // for each sector, retreve industry data
                for (int i = 0; i < sectors.Count(); i++)
                {
                    string[] rows = GetRows(sectors[i].URI);

                    Industries industriesBySector = industry.GetIndustries(sectors[i].Name, rows);

                    industries.AddRange(industriesBySector);
                }
            }
            return industries;
        }

        private static Sectors GetSectors()
        {
            string[] rows = GetRows("http://biz.yahoo.com/p/s_conameu.html");

            Sector sector = new Sector();

            Sectors sectors = sector.GetSectors(rows);

            return sectors;
        }

        private static string[] GetRows(string uri) {
            string sPage = WebPage.Get(uri);

            sPage = sPage.Replace("\r\n", " ").Replace("\r", " ").Replace("\n", " ");

            string table = sPage.Substring(sPage.LastIndexOf("<table"));
            table = table.Substring(0, table.IndexOf("</table") + "</table".Length);
            string[] stringSeparators = new string[] { "<tr" };
            string[] rows = table.Split(stringSeparators, StringSplitOptions.None);
            return rows;
        }
        #endregion private methods

        #region possible future use
        //private static string GetOutPutFile(string symbol, string name)
        //{ 
        //    string today = DateTime.Now.Date.ToString().Substring(0, DateTime.Now.ToString().IndexOf(' '));
        //    return string.Format(@"C:\Users\Jim\Documents\Visual Studio 2012\Projects\ScanVXX\Files\{0} {1} - {2}.csv", name, symbol.ToUpper(), today.Replace('/', '-'));
        //}

        ////example of complete uri
        ////"http://real-chart.finance.yahoo.com/table.csv?s=WFC&a=05&b=1&c=1972&d=04&e=29&f=2015&g=v&ignore=.csv"
        //private static string GetDividendUri(string symbol, DateTime date)
        //{
        //    return string.Format("{0}{1}&a={2}&b={3}&c={4}&d={5}&e={6}&f={7}&g=v&ignore=.csv",
        //              @"http://real-chart.finance.yahoo.com/table.csv?s=", symbol, 
        //              date.Month, date.Day, date.Year,
        //              date.AddMonths(numberOfMonths - 1).Month, date.AddDays(-1).AddMonths(numberOfMonths).Day, date.AddMonths(numberOfMonths).Year);
        //}
        #endregion possible future use
    }
}
