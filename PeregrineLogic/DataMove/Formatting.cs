using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ResearchLibrary.Objects;

namespace ResearchLibrary.DataMove
{
    public class Formatting
    {
        public List<Stock> CSVFileParse(string stockData, string stock)
        {
            DownloadEngine download = new DownloadEngine();
            List<Stock> stockList = new List<Stock>();

            string[] stockArray = stockData.Split('\n').Skip(1).ToArray();

            foreach (var item in stockArray)
            {
                string[] Row = item.Split(',').ToArray();


                stockList.Add(new Stock
                {
                    CIK = download.DatabaseCIKQuery(stock),
                    Symbol = stock,
                    Date = DateTime.Parse(Row[0]),
                    OpenPrice = Convert.ToDouble(Row[1]),
                    HighPrice = Convert.ToDouble(Row[2]),
                    LowPrice = Convert.ToDouble(Row[3]),
                    ClosePrice = Convert.ToDouble(Row[4]),
                    Volume = Convert.ToUInt32(Convert.ToDouble(Row[5]))
                });
            }

            return stockList;
        }

    }
}
