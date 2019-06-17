using PeregrineConsole.Objects;
using ResearchLibrary.Locations;
using ResearchLibrary.Objects;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;

namespace ResearchLibrary.SQL
{
    public class Transactions
    {
        public void SaveToTableStockPrices(List<Stock> stock)
        {
            FileLocations locations = new FileLocations();

            var connect = locations.DatabaseConnectionString(); // returns Database connectionstring for SQLConnection

            using (SqlConnection connection = new SqlConnection(connect.ConnectionString))
            {
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                SqlTransaction transaction;

                for (int i = 0; i < stock.Count; i++)
                {
                    transaction = connection.BeginTransaction($"Transaction {i}");
                    command.Connection = connection;
                    command.Transaction = transaction;
                    command.CommandText = $"Insert into Stocks ([StockID],[Symbol],[Date],[Open],[High],[Low],[Close],[Volume]) " +
                                          $"Values ({stock[i].CIK}, '{stock[i].Symbol.Trim()}', Convert(Date,'{stock[i].Date}'), {stock[i].OpenPrice}, " +
                                          $"{stock[i].HighPrice}, {stock[i].LowPrice},{stock[i].ClosePrice}, {stock[i].Volume});";

                    command.ExecuteNonQuery();

                    transaction.Commit();
                }

            }

        }

        public void TruncateStockTable(string tablename)
        {
            FileLocations locations = new FileLocations();

            var connect = locations.DatabaseConnectionString(); // returns Database connectionstring for SQLConnection

            using (SqlConnection connection = new SqlConnection(connect.ConnectionString))
            {
                connection.Open(); // opens connection to SQL Database

                SqlCommand command = connection.CreateCommand(); // translates text to SQL commands for the database

                SqlTransaction transaction;

                transaction = connection.BeginTransaction("Truncate Table"); // transaction must have a name

                command.Connection = connection;
                command.Transaction = transaction;


                command.CommandText = $"TRUNCATE TABLE [{tablename}];";
                command.ExecuteNonQuery();
                transaction.Commit();

            }
        }

        public void SaveToCleanData(string stock)
        {
            FileLocations locations = new FileLocations();

            var connect = locations.DatabaseConnectionString(); // returns Database connectionstring for SQLConnection

            using (SqlConnection connection = new SqlConnection(connect.ConnectionString))
            {
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                SqlTransaction transaction;

                transaction = connection.BeginTransaction($"Transaction {stock}");
                command.Connection = connection;
                command.Transaction = transaction;
                command.CommandText = $"Insert into [Clean Data] ([Symbol]) " +
                                      $"Values ('{stock.Trim()}');";

                command.ExecuteNonQuery();

                transaction.Commit();
            }

        }



        /// <summary>
        /// Returns the closing price for a set date period from the database.
        /// </summary>
        /// <param name="database"></param>
        /// <param name="stock"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public List<double> DatabasePriceQuery(string stock, string priceSet) /// TODO: make this method able to retrieve any price.
        {
            FileLocations locations = new FileLocations();

            var connect = locations.DatabaseConnectionString(); // returns Database connectionstring for SQLConnection

            List<double> Close = new List<double>();

            using (SqlConnection connection = new SqlConnection(connect.ConnectionString))
            {
                string queryString = $"select S.[{priceSet.Trim()}]  from Stocks as S  where S.Symbol = '{stock}' order by S.Date ASC";

                SqlCommand command = new SqlCommand(queryString, connection); //

                command.Parameters.AddWithValue($"{stock}", stock);
                connection.Open(); // opens the connection to the database

                using (SqlDataReader reader = command.ExecuteReader()) // opens the datareader to retrieve data with query
                {
                    while (reader.Read())
                    {
                        Close.Add(Convert.ToDouble(reader[$"{priceSet.Trim()}"]));

                    }

                }

                connection.Close();
            }
            return Close;
        }

        /// <summary>
        /// Returns a list of distinct stock ticker symbols for the specified table.
        /// </summary>
        /// <param name="database"></param>
        /// <param name="table"></param>
        /// <returns></returns>
        public List<string> DatabaseSymbolListQuery(string table)
        {
            FileLocations locations = new FileLocations();

            var connect = locations.DatabaseConnectionString(); // returns Database connectionstring for SQLConnection

            List<string> Symbol = new List<string>();

            using (SqlConnection connection = new SqlConnection(connect.ConnectionString))
            {
                string queryString = $"SELECT DISTINCT S.[Symbol] FROM [{table}] AS S ORDER BY S.Symbol ASC";

                SqlCommand command = new SqlCommand(queryString, connection); //

                command.Parameters.AddWithValue($"Symbol", "Symbol");
                connection.Open(); // opens the connection to the database

                using (SqlDataReader reader = command.ExecuteReader()) // opens the datareader to retrieve data with query
                {
                    while (reader.Read())
                    {
                        Symbol.Add(Convert.ToString(reader["Symbol"]));

                    }

                }

                connection.Close();
            }
            return Symbol;
        }

        /// <summary>
        /// Returns the 50 most recent closing prices from the selected symbol.
        /// </summary>
        /// <param name="database"></param>
        /// <param name="stock"></param>
        /// <returns></returns>
        public List<double> NiftyFiftyQuery(string stock) /// TODO: make this method able to retrieve any price.
        {
            FileLocations locations = new FileLocations();

            var connect = locations.DatabaseConnectionString(); // returns Database connectionstring for SQLConnection

            List<double> Close = new List<double>();

            using (SqlConnection connection = new SqlConnection(connect.ConnectionString))
            {
                string queryString = $"SELECT TOP (50)(S.[Close] ) FROM Stocks AS S  WHERE S.[Symbol] = '{stock}'ORDER BY S.[Date] desc;";

                SqlCommand command = new SqlCommand(queryString, connection); //

                command.Parameters.AddWithValue($"{stock}", stock);
                connection.Open(); // opens the connection to the database

                using (SqlDataReader reader = command.ExecuteReader()) // opens the datareader to retrieve data with query
                {
                    while (reader.Read())
                    {
                        Close.Add(Convert.ToDouble(reader["Close"]));

                    }

                }

                connection.Close();
            }
            return Close;
        }

        public double CloseQuery(string stock) /// TODO: make this method able to retrieve any price.
        {
            FileLocations locations = new FileLocations();

            var connect = locations.DatabaseConnectionString(); // returns Database connectionstring for SQLConnection

            double Close = 0;

            using (SqlConnection connection = new SqlConnection(connect.ConnectionString))
            {
                string queryString = $"select S.[Close]  from Stocks as S  where S.Symbol = '{stock}' order by S.Date ASC";

                SqlCommand command = new SqlCommand(queryString, connection); //

                command.Parameters.AddWithValue($"{stock}", stock);
                connection.Open(); // opens the connection to the database

                using (SqlDataReader reader = command.ExecuteReader()) // opens the datareader to retrieve data with query
                {
                    while (reader.Read())
                    {
                        Close = Convert.ToDouble(reader[$"Close"]);

                    }

                }

                connection.Close();
            }
            return Close;
        }


        //TODO: Complete the query. Check the Table.
        public void SaveToTableResult(string database, List<Results> results)
        {
            FileLocations locations = new FileLocations();

            var connect = locations.DatabaseConnectionString(); // returns Database connectionstring for SQLConnection

            using (SqlConnection connection = new SqlConnection(connect.ConnectionString))
            {
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                SqlTransaction transaction;

                for (int i = 0; i < results.Count; i++)
                {
                    transaction = connection.BeginTransaction($"Transaction {i}");
                    command.Connection = connection;
                    command.Transaction = transaction;
                    command.CommandText = $"Insert into Results ([Date],[CIK],[Symbol],[Last Close],[50 Day Avg],[MACD],[MACD Signal],[EMA 12],[EMA 26],[Stochastic Fast],[Stochastic Slow],[Stochastic Signal]) " +
                                          $"Values (Convert(Date,'{results[i].Date}'),{results[i].CIK}, '{results[i].Symbol.Trim()}',{results[i].LastClose}," +
                                          $"{results[i].FiftyDayAvg}, {results[i].mACD},{results[i].MACDSignal}, {results[i].EMA12},{results[i].EMA26}," +
                                          $"{results[i].StochasticFast},{results[i].StochasticSlow},{results[i].StochasticSignal});";

                    command.ExecuteNonQuery();

                    transaction.Commit();
                }

            }

        }

        // gets the most recent stock date
        public List<DateTime> DatabaseDateQuery(string stock)
        {
            FileLocations locations = new FileLocations();

            var connect = locations.DatabaseConnectionString(); // returns Database connectionstring for SQLConnection

            List<DateTime> Date = new List<DateTime>();

            using (SqlConnection connection = new SqlConnection(connect.ConnectionString))
            {
                string queryString = $"select S.[Date]  from Stocks as S  where S.Symbol = '{stock}' order by S.Date DESC";

                SqlCommand command = new SqlCommand(queryString, connection); //

                command.Parameters.AddWithValue($"{stock}", stock);
                connection.Open(); // opens the connection to the database

                using (SqlDataReader reader = command.ExecuteReader()) // opens the datareader to retrieve data with query
                {
                    while (reader.Read())
                    {
                        Date.Add(Convert.ToDateTime(reader[$"Date"]));

                    }

                }

                connection.Close();
            }
            return Date;
        }

        public List<DateTime> DatabaseDateQuery()
        {
            FileLocations locations = new FileLocations();

            var connect = locations.DatabaseConnectionString(); // returns Database connectionstring for SQLConnection

            List<DateTime> Date = new List<DateTime>();

            using (SqlConnection connection = new SqlConnection(connect.ConnectionString))
            {
                string queryString = $"select S.[Date]  from Stocks as S   order by S.Date DESC";

                SqlCommand command = new SqlCommand(queryString, connection); //

                command.Parameters.AddWithValue($"Date", "Date");
                connection.Open(); // opens the connection to the database

                using (SqlDataReader reader = command.ExecuteReader()) // opens the datareader to retrieve data with query
                {
                    while (reader.Read())
                    {
                        Date.Add(Convert.ToDateTime(reader[$"Date"]));

                    }

                }

                connection.Close();
            }
            return Date;
        }


        public void SaveToTableWebResult(List<PeregrineResults> results)
        {
            FileLocations locations = new FileLocations();

            var connect = locations.DatabaseConnectionString(); // returns Database connectionstring for SQLConnection
            try
            {
                using (SqlConnection connection = new SqlConnection(connect.ConnectionString))
                {
                    connection.Open();
                    SqlCommand command = connection.CreateCommand();
                    SqlTransaction transaction;

                    for (int i = 0; i < results.Count; i++)
                    {
                        transaction = connection.BeginTransaction($"Transaction {i}");
                        command.Connection = connection;
                        command.Transaction = transaction;
                        command.CommandText = $"Insert into [Peregrine Results] ([Date],[CIK],[Symbol],[Last Close],[50 Day Avg], [Name], [Address 1], [Address 2]," +
                                              $"[City / State], [Phone], [SIC], [Industry], [MACD], [MACD Signal], [Stochastic Slow], [Stochastic Signal]) " +
                                              $"Values (Convert(Date,'{results[i].Date}'),{results[i].CIK}, '{results[i].Symbol.Trim()}',{results[i].LastClose}," +
                                              $"{results[i].FiftyDay}, '{results[i].Name.Trim()}', '{results[i].Address1.Trim()}', '{results[i].Address2.Trim()}', '{results[i].CityState.Trim()}'," +
                                              $"'{results[i].Phone.Trim()}', {results[i].SIC}, '{results[i].Industry.Trim()}',{results[i].MACD},{results[i].MACDSignal}," +
                                              $"{results[i].Stochastic}, {results[i].StochasticSignal});";

                        command.ExecuteNonQuery();

                        transaction.Commit();
                    }

                }
            }
            // If Address 2 is Null the null ref ex is caught and the insert is tried without reference to address 2 allowing it to enter the table as a null ref.
            catch (NullReferenceException)
            {
                using (SqlConnection connection = new SqlConnection(connect.ConnectionString))
                {
                    connection.Open();
                    SqlCommand command = connection.CreateCommand();
                    SqlTransaction transaction;

                    for (int i = 0; i < results.Count; i++)
                    {
                        transaction = connection.BeginTransaction($"Transaction {i}");
                        command.Connection = connection;
                        command.Transaction = transaction;
                        command.CommandText = $"Insert into [Peregrine Results] ([Date],[CIK],[Symbol],[Last Close],[50 Day Avg], [Name], [Address 1]," +
                                              $"[City / State], [Phone], [SIC], [Industry_Title], [MACD], [MACD Signal], [Stochastic Slow], [Stochastic Signal]) " +
                                              $"Values (Convert(Date,'{results[i].Date}'),{results[i].CIK}, '{results[i].Symbol.Trim()}',{results[i].LastClose}," +
                                              $"{results[i].FiftyDay}, '{results[i].Name.Trim()}', '{results[i].Address1.Trim()}', '{results[i].CityState.Trim()}'," +
                                              $"'{results[i].Phone.Trim()}', {results[i].SIC}, '{results[i].Industry.Trim()}',{results[i].MACD},{results[i].MACDSignal}," +
                                              $"{results[i].Stochastic}, {results[i].StochasticSignal});";

                        command.ExecuteNonQuery();

                        transaction.Commit();
                    }

                }

            }


        }

        /// <summary>
        /// Sends a stored procedure query to the database and records the results.
        /// </summary>
        /// <param name="database"></param>
        /// <returns></returns>
        public List<PeregrineResults> DailyResultsQuery() /// TODO: reapply date as string
        {
            FileLocations locations = new FileLocations();

            var connect = locations.DatabaseConnectionString(); // returns Database connectionstring for SQLConnection

            List<PeregrineResults> todayresults = new List<PeregrineResults>();

            using (SqlConnection connection = new SqlConnection(connect.ConnectionString))
            {
                string queryString = $"EXEC PeregrineResults";

                SqlCommand command = new SqlCommand(queryString, connection); //

                command.Parameters.AddWithValue($"Symbol", "Symbol");
                connection.Open(); // opens the connection to the database

                using (SqlDataReader reader = command.ExecuteReader()) // opens the datareader to retrieve data with query
                {
                    while (reader.Read())
                    {
                        todayresults.Add(new PeregrineResults
                        {
                            Date = Convert.ToDateTime(reader["Date"]).Date,
                            CIK = Convert.ToInt32(reader["CIK"]),
                            Symbol = Convert.ToString(reader["Symbol"]),
                            LastClose = Convert.ToDouble(reader["Last Close"]),
                            FiftyDay = Convert.ToDouble(reader["50 Day Avg"]),
                            Name = Convert.ToString(reader["Name"]),
                            Address1 = Convert.ToString(reader["Address 1"]),
                            Address2 = Convert.ToString(reader["Address 2"]),
                            CityState = Convert.ToString(reader["City / State"]),
                            Phone = Convert.ToString(reader["Phone"]),
                            SIC = Convert.ToInt32(reader["SIC Code"]),
                            Industry = Convert.ToString(reader["Industry Title"]),
                            MACD = Convert.ToDouble(reader["MACD"]),
                            MACDSignal = Convert.ToDouble(reader["MACD Signal"]),
                            Stochastic = Convert.ToDouble(reader["Stochastic Slow"]),
                            StochasticSignal = Convert.ToDouble(reader["Stochastic Signal"])
                        });

                    }

                }

                connection.Close();
            }
            return todayresults;
        }

    }
}