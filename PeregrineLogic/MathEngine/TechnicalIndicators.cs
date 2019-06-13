using ResearchLibrary.SQL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeregrineConsole.MathEngine
{
    public class TechnicalIndicators
    {
        public double FiftyDayAvg(string stock, Transactions transactions)
        {
            double SumOfPast50 = transactions.NiftyFiftyQuery(stock).GetRange(0, 50).Sum();

            double PrettyAverage = Math.Round(SumOfPast50 / 50, 2);

            return PrettyAverage;
        }



    }
}
