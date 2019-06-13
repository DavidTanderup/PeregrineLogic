using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Text;

namespace ResearchLibrary.Locations
{
    public class FileLocations
    {
        /// <summary>
        /// Returns the location of the historical stock data.
        /// </summary>
        /// <param name="stock"></param>
        /// <returns></returns>
        public string StockData(string stock)
        {
            string location = @"C:\Peregrine Research\Stock Data\" + stock + ".csv";
            return location;
        }

        public SqlConnectionStringBuilder DatabaseConnectionString()
        {
            var connection = new SqlConnectionStringBuilder();
            connection.DataSource = "peregrineresearch.database.windows.net";
            connection.UserID = "DavidTanderup";
            connection.Password = "Yankee$5";
            connection.InitialCatalog = "PeregrineResearchDatabase";

            return connection;
        }
    }
}
