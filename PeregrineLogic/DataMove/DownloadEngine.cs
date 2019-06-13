using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Data.SqlTypes;
using ResearchLibrary.Locations;
using System.Data.SqlClient;
using ResearchLibrary.SQL;
using PeregrineConsole.Operations;

namespace ResearchLibrary.DataMove
{
    /// <summary>
    /// Download data from the internet.
    /// </summary>
    public class DownloadEngine
    {
        /// <summary>
        /// Downloads a single stock, verifies contents. If there is valid data the data is saved to the data base 
        /// and the table containing the Clean Data list is updated.
        /// </summary>
        /// <param name="numberDays"></param>
        /// <param name="stock"></param>
        public void ToDatabase(DateTime date, Formatting formatting, PeregrineOperation peregrine, Transactions transactions, WebClient web, WebURIs uRIs, string stock, string Database)
        {


            string startDate = DateSelect(date, transactions, peregrine, stock); // customizes start date to the last date downloaded defaults with 100 days of data

            //int benchMark = tradingDays(startDate, web, uRIs, date); // number of rows that should be in a valid data set.


            string stockData = HistoricalDataDownload(stock, startDate, web, uRIs, date, peregrine, transactions); // downloads historical data as a string.
                                                                                                                   // int actualData = cleanData(stockData); // passes data to check the quantity of data.
            var DailyStockData = formatting.CSVFileParse(stockData, stock);

            transactions.SaveToTableStockPrices(DailyStockData);

            transactions.SaveToCleanData(stock); // write clean list to Table


        }

        /// <summary>
        /// Downloads daily historical stock data.
        /// </summary>
        public string HistoricalDataDownload(string stock, string startDate, WebClient web, WebURIs uRIs, DateTime date, PeregrineOperation peregrine, Transactions transactions)
        {

            string endDate = date.ToShortDateString();

            string stockData = web.DownloadString(uRIs.WSJHistorical(stock, startDate, endDate)); // method call to retrieve the data                           

            return stockData;

        }

        private int cleanData(string stock)
        {
            int data = stock.Split('\n').ToArray().Count();
            return data;
        }

        /// <summary>
        /// The Trading Days method allows the program to validate if the file being downloaded holds the correct amount of data. I am using five of the largest
        /// stocks which are heavily tracked. It is unlikely that there will not be historical data on these five stocks, making them a good bench mark.
        /// </summary>
        /// <param name="numberDays"></param>
        /// <returns></returns>
        private int tradingDays(string startDate, WebClient web, WebURIs uRIs, DateTime date)
        {

            List<string> FAANG = new List<string> { "FB", "AAPL", "AMZN", "NFLX", "GOOGL" }; // Top Tech stocks: Facebook, Apple, Amazon, NetFlix, Google

            int count = 0; // count of the total trading days recorded for FAANG over the period.
            foreach (var item in FAANG)
            {
                //              (website location  (stock requested, startdate, enddate) splitting by the return symbol > places it into an array > counts the array elements
                int rows = web.DownloadString(uRIs.WSJHistorical(item, startDate, date.ToShortDateString())).Split('\n').ToArray().Count();
                count += rows; // count = count(current) + rows
            }
            int tradingDays = count / 5;

            return tradingDays;
        }

        public int DatabaseCIKQuery(string stock)
        {
            FileLocations locations = new FileLocations();

            var connect = locations.DatabaseConnectionString(); // returns Database connectionstring for SQLConnection

            int CIK = 0;

            using (SqlConnection connection = new SqlConnection(connect.ConnectionString))
            {
                string queryString = $"SELECT CIK FROM CentralIndexKey WHERE Symbol = '{stock}'"; // Query sent to SQL database

                SqlCommand command = new SqlCommand(queryString, connection); //

                command.Parameters.AddWithValue($"{stock}", stock);
                connection.Open(); // opens the connection to the database

                using (SqlDataReader reader = command.ExecuteReader()) // opens the datareader to retrieve data with query
                {
                    while (reader.Read())
                    {
                        CIK = Convert.ToInt32(reader["CIK"]);

                    }

                }

                connection.Close();
            }
            return CIK;
        }

        /// <summary>
        /// Checks the database for the last date downloaded and sets the start date string. This allows the program to be flexible to weekends and holidays.
        /// Additionally the method allows the program to retreive data that may not have been available previously.
        /// </summary>
        /// <param name="now"></param>
        /// <param name="transactions"></param>
        /// <param name="peregrine"></param>
        /// <param name="stock"></param>
        /// <returns></returns>
        public string DateSelect(DateTime date, Transactions transactions, PeregrineOperation peregrine, string stock)
        {
            List<DateTime> latestDate = transactions.DatabaseDateQuery(stock);

            try
            {
                string startDate;
                if (date.Date.AddDays(-100) > latestDate[0])
                {
                    startDate = date.AddDays(-100).ToShortDateString(); // if there is greater than 100 day gap the start is set to 100 days.
                }
                else if (latestDate == null)
                {
                    startDate = date.AddDays(-100).ToShortDateString();
                }
                else
                {
                    startDate = latestDate[0].AddDays(1).ToShortDateString();
                }
                return startDate;
            }
            catch (ArgumentOutOfRangeException)
            {
                Console.WriteLine($"There was a problem with {stock} it was skipped");
                string startDate = date.AddDays(-100).ToShortDateString();

                return startDate;
            }

            // Add Try - Catch to deal with stock not found.

        }


    }
}
