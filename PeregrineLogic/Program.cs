using PeregrineConsole.Operations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeregrineLogic
{
    class Program
    {
        static void Main(string[] args)
        {
            PeregrineOperation peregrine = new PeregrineOperation();

            peregrine.Run();
        }
    }

    // Check if there is a value for the EMA and Signal
    // If yes: check date of value
    // calc ema based on close value
    // save to Results
    // If no: use current Calc engine to start process
}
