using ResearchLibrary.SQL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeregrineConsole.MathEngine
{
    public class Stochastic
    {
        public double Signal { get; set; }
        public double Fast { get; set; }
        public double Slow { get; set; }

        /// <summary>
        /// Fast moving Stochastic. It is very volatile and is not recommended to trade on. Can serve as a good early warning system.
        /// </summary>
        /// <param name="highest"></param>
        /// <param name="lowest"></param>
        /// <returns></returns>
        private List<double> FastStochastic(List<double> highest, List<double> lowest) // set
        {
            List<double> Fast = new List<double>();

            for (int i = 0; i < highest.Count; i++)
            {
                Fast.Add((lowest[i] / highest[i]) * 100);
            }

            return Fast;
        }

        /// <summary>
        /// This is the 'Slow Average' or %K it is the more commonly used average because it does not react as sharply to every price change.
        /// </summary>
        /// <param name="highest"></param>
        /// <param name="lowest"></param>
        private List<double> SlowStochastic(List<double> highest, List<double> lowest) // set ish
        {
            List<double> Slow = new List<double>();

            for (int i = 0; i < highest.Count - 2; i++)
            {
                Slow.Add((lowest.GetRange(i, 3).Sum() / highest.GetRange(i, 3).Sum()) * 100);
                //Console.WriteLine($"Low {i}: {Math.Round(lowest.GetRange(i, 3).Sum(), 2)} High: {Math.Round(highest.GetRange(i, 3).Sum(), 2)}");
                //foreach (var item in highest.GetRange(i, 3))
                //{
                //    Console.WriteLine(item);
                //}

            }
            return Slow;
        }

        private List<double> StochasticSignal(List<double> slow) // set
        {
            List<double> Signal = new List<double>();

            for (int i = 0; i < slow.Count - 2; i++)
            {
                Signal.Add(slow.GetRange(i, 3).Average());
            }
            return Signal;
        }

        /// <summary>
        /// Collects the distance of the highest high from the lowest low in a set period of time
        /// </summary>
        /// <param name="database"></param>
        /// <param name="stock"></param>
        /// <param name="transactions"></param>
        /// <returns></returns>
        private List<double> HighestHigh(string database, string stock, Transactions transactions) // set
        {
            List<double> Highest = new List<double>();

            // Low price Data
            List<double> Low = transactions.DatabasePriceQuery(stock, "Low"); // SQL query calls low price from database

            List<double> High = transactions.DatabasePriceQuery(stock, "High"); // High price

            // Max of 14 period high - Lowest from 14 period low

            for (int i = 0; i < High.Count - 13; i++)
            {
                Highest.Add(High.GetRange(i, 14).Max<double>() - Low.GetRange(i, 14).Min<double>());
            }

            return Highest;
        }

        /// <summary>
        /// Collects the distance of the lowest low for a set period from the current close.
        /// </summary>
        /// <param name="database"></param>
        /// <param name="stock"></param>
        /// <param name="transactions"></param>
        /// <returns></returns>
        private List<double> LowestLow(string database, string stock, Transactions transactions) // set
        {
            List<double> Lowest = new List<double>();

            // Close price Data
            List<double> Close = transactions.DatabasePriceQuery(stock, "Close");

            // Low price Data
            List<double> Low = transactions.DatabasePriceQuery(stock, "Low");

            int lowIndex = 0; // starts the low range finder at the zero index

            for (int i = 13; i < Low.Count; i++)
            {
                Lowest.Add(Math.Round(Close[i], 2) - Math.Round(Low.GetRange(lowIndex, 14).Min<double>(), 2));

                lowIndex++;
            }

            return Lowest;
        }

        // TODO: Create method that returns most recent Stochastic indications.
        public List<Stochastic> StochasticToday(string database, string stock)
        {
            Transactions transactions = new Transactions();

            List<double> Fast = FastStochastic(HighestHigh(database, stock, transactions), LowestLow(database, stock, transactions));
            List<double> Slow = SlowStochastic(HighestHigh(database, stock, transactions), LowestLow(database, stock, transactions));
            List<double> Signal = StochasticSignal(Slow);

            List<Stochastic> stochastics = new List<Stochastic>()
            {
                new Stochastic
                {
                    Slow = Slow[Slow.Count-1],
                    Fast = Fast[Fast.Count-1],
                    Signal = Signal[Signal.Count-1]
                }
            };
            return stochastics;
        }


    }

}

