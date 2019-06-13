using PeregrineConsole.Operations;
using ResearchLibrary.SQL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeregrineConsole.MathEngine
{
    public class MACD
    {
        public double MACDValue { get; set; }
        public double MACDSignal { get; set; }
        public double EMA12 { get; set; }
        public double EMA26 { get; set; }

        public List<MACD> TodaysMACD(string stock, Transactions transactions)
        {
            PeregrineOperation peregrine = new PeregrineOperation();
            List<double> Close = transactions.DatabasePriceQuery(stock, "Close"); // returns data with most recent date at Nth index
            List<double> EMA12 = EMA(Close, 12);
            List<double> EMA26 = EMA(Close, 26);
            List<double> macd = StarterMACD(stock, EMA12, EMA26, Close, transactions);
            List<double> signal = MACD_Signal(macd);


            // List creating the MACD object
            List<MACD> MACDList = new List<MACD>()
            {
                new MACD{MACDValue = macd[0], MACDSignal = signal[signal.Count-1],
                         EMA12 = EMA12[0], EMA26 = EMA26[0]}
            };

            return MACDList;
        }

        private List<double> StarterMACD(string stock, List<double> EMA12, List<double> EMA26, List<double> Close, Transactions transactions)
        {
            List<double> MACD = new List<double>();


            for (int i = 0; i < EMA26.Count; i++)
            {
                MACD.Add(Math.Round(EMA12[i] - EMA26[i], 4));
            }

            return MACD;
        }

        private List<double> MACD_Signal(List<double> macdList)
        {
            double periodLength = 9;

            List<double> macd = new List<double>();
            int index = macdList.Count - 1;

            foreach (var item in macdList)
            {
                macd.Add(macdList[index]);
                index--;
            }


            List<double> EMA = new List<double>(); // holds the list of calculated EMA

            double todaySmooth = (2 / (periodLength + 1));
            double yesterdaySmooth = (1 - (2 / (periodLength + 1)));

            int closeIndex = (int)periodLength;
            int emaIndex = 0;
            EMA.Add(Math.Round((macd[closeIndex] * todaySmooth) + ((macd.GetRange(0, Convert.ToInt32(periodLength)).Sum() / periodLength) * yesterdaySmooth), 4));
            closeIndex++;

            for (int i = closeIndex; i < macd.Count; i++)
            {
                EMA.Add(Math.Round((macd[i] * todaySmooth) + (EMA[emaIndex] * yesterdaySmooth), 4));
                emaIndex++;
            }
            // reverses the order of the EMA list


            return EMA;
        }

        /// <summary>
        /// creates the weighted average that is more reactive to changes that are more recent.
        /// </summary>
        /// <param name="close"></param>
        /// <param name="periodLength"></param>
        /// <returns></returns>
        private List<double> EMA(List<double> close, double periodLength)
        {
            List<double> EMA = new List<double>(); // holds the list of calculated EMA
            List<double> rEMA = new List<double>();

            double todaySmooth = (2 / (periodLength + 1));
            double yesterdaySmooth = (1 - (2 / (periodLength + 1)));

            int closeIndex = (int)periodLength;
            int emaIndex = 0;
            EMA.Add(Math.Round((close[closeIndex] * todaySmooth) + ((close.GetRange(0, Convert.ToInt32(periodLength)).Sum() / periodLength) * yesterdaySmooth), 4));

            closeIndex++;

            for (int i = closeIndex; i < close.Count; i++)
            {
                EMA.Add(Math.Round((close[i] * todaySmooth) + (EMA[emaIndex] * yesterdaySmooth), 4));

                emaIndex++;
            }

            int Index = EMA.Count - 1;


            // reverses the order of the EMA list
            foreach (var item in EMA)
            {
                rEMA.Add(EMA[Index]);
                Index--;
            }

            return rEMA;
        }


    }
}
