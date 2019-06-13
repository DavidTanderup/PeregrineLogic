using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeregrineConsole.Objects
{
    public class PeregrineResults
    {
        public DateTime Date { get; set; }
        public int CIK { get; set; }
        public string Symbol { get; set; }
        public string Name { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string CityState { get; set; }
        public string Phone { get; set; }
        public int SIC { get; set; }
        public string Industry { get; set; }
        public double LastClose { get; set; }
        public double FiftyDay { get; set; }
        public double MACD { get; set; }
        public double MACDSignal { get; set; }
        public double Stochastic { get; set; }
        public double StochasticSignal { get; set; }

    }
}
