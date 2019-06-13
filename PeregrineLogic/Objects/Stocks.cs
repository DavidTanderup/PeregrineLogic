using System;
using System.Collections.Generic;
using System.Text;

namespace ResearchLibrary.Objects
{
    public class Stock
    {
        public int CIK { get; set; }
        public string Symbol { get; set; }
        public DateTime Date { get; set; }
        public double OpenPrice { get; set; }
        public double HighPrice { get; set; }
        public double LowPrice { get; set; }
        public double ClosePrice { get; set; }
        public uint Volume { get; set; }
    }
}
