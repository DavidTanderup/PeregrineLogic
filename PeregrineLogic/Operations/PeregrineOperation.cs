using PeregrineConsole.MathEngine;
using PeregrineConsole.Objects;
using ResearchLibrary.DataMove;
using ResearchLibrary.Locations;
using ResearchLibrary.Objects;
using ResearchLibrary.SQL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace PeregrineConsole.Operations
{
    public class PeregrineOperation
    {
        private string database = "PeregrineResearchDatabase";
        private string cleandata = "Clean Data";
        private string results = "Results";
        private string peregrineresults = "Peregrine Results";
        public string Results
        {
            get
            {
                return results;
            }
        }
        public string Clean
        {
            get
            {
                return cleandata;
            }
        }
        private string stocks = "Stocks";
        public string Database
        {
            get
            {
                return database; // ensures the correct database is used everytime
            }

        }

        public void Run()
        {
            DateTime date = DateTime.Now;
            DownloadEngine download = new DownloadEngine();
            Formatting formatting = new Formatting();
            PeregrineOperation peregrine = new PeregrineOperation();
            MACD macd = new MACD();
            Stochastic stochastic = new Stochastic();
            TechnicalIndicators indicators = new TechnicalIndicators();
            Transactions transactions = new Transactions();
            WebClient web = new WebClient(); // provides the ability to download from the internet
            WebURIs uRIs = new WebURIs(); // refers to an instance of the Wall Street Journal URL


            Download(date, download, formatting, peregrine, macd, stochastic, indicators, transactions, web, uRIs, Database);

        }


        /// Download and save to Database
        private void Download(DateTime date, DownloadEngine download, Formatting formatting, PeregrineOperation peregrine, MACD macd, Stochastic stochastic, TechnicalIndicators indicators, Transactions transactions, WebClient web, WebURIs uRIs, string Database)
        {
            //List<string> Symbols = transactions.DatabaseSymbolListQuery(stocks);
            //transactions.TruncateStockTable("Clean Data"); // clears data from the clean data table.
            int count = 1;
            //foreach (var stock in Symbols)
            //{
            //    Console.Clear();
            //    Console.WriteLine($"Downloading {stock} || {count} of {Symbols.Count}");
            //    download.ToDatabase(date, formatting, peregrine, transactions, web, uRIs, stock, Database);
            //    count++;
            //}
            //Console.WriteLine("I'm Done");
            //count = 1;
            //calls the list of stocks that have been data verified
            List<string> CleanData = transactions.DatabaseSymbolListQuery(stocks);
            foreach (var stock in CleanData)
            {
                Console.Clear();
                Console.WriteLine($"Doing Math {stock} || {count} of {CleanData.Count}");
                List<Results> results = MathPredictsTheFuture(download, macd, stochastic, transactions, indicators, stock);
                transactions.SaveToTableResult(Database, results); // saves the calculations to the Results table in the database
                count++;
            }


            // calls the stocks from the results list
            List<PeregrineResults> todaysResults = transactions.DailyResultsQuery();
            transactions.TruncateStockTable(peregrineresults); // clears the web results table. This table feeds the model for the ASP.Net interface.
            transactions.SaveToTableWebResult(todaysResults);
            Console.WriteLine("Peregrine Results Updated");

        }

        /// Prediction Algo via Technical Indicators
        public List<Results> MathPredictsTheFuture(DownloadEngine download, MACD macd, Stochastic stochastic,
                                                   Transactions transactions, TechnicalIndicators indicators, string stock)
        {
            DateTime date = transactions.DatabaseDateQuery(stock)[0];
            // 50 Day Average - queries database and calculates the 50 day average
            double FiftyDayAverage = indicators.FiftyDayAvg(stock, transactions);

            // MACD - determines if a stock is bearish or bullish. Uses 12 and 26 period exponental moving average
            List<MACD> todaysMACD = macd.TodaysMACD(stock, transactions);
            // Stochastic
            List<Stochastic> todayStoch = stochastic.StochasticToday(Database, stock);

            List<Results> results = new List<Results>()
            {
                new Results
                {
                    Date = date,
                    CIK = download.DatabaseCIKQuery(stock),
                    Symbol = stock,
                    LastClose = transactions.CloseQuery(stock),
                    FiftyDayAvg = FiftyDayAverage,
                    mACD = todaysMACD[0].MACDValue,
                    MACDSignal = todaysMACD[0].MACDSignal,
                    EMA12 = todaysMACD[0].EMA12,
                    EMA26 = todaysMACD[0].EMA26,
                    StochasticFast = todayStoch[0].Fast,
                    StochasticSlow = todayStoch[0].Slow,
                    StochasticSignal = todayStoch[0].Signal
                }
            };
            return results;

        }





    }
}
