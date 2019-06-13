using System;
using System.Collections.Generic;
using System.Text;
using PeregrineConsole.MathEngine;

namespace ResearchLibrary.Objects
{
    /// <summary>
    /// Represents the results of the technical indicators.
    /// </summary>
    public class Results
    {
        public DateTime Date { get; set; }
        public int CIK { get; set; } // Central Index Key used by the Security and Exchange Commission (SEC) in the EDGAR database.
        public string Symbol { get; set; }
        public double LastClose { get; set; }
        public double FiftyDayAvg { get; set; }
        public double mACD { get; set; }
        public double MACDSignal { get; set; }
        public double EMA12 { get; set; }
        public double EMA26 { get; set; }
        public double StochasticFast { get; set; }
        public double StochasticSlow { get; set; }
        public double StochasticSignal { get; set; }
    }
}
