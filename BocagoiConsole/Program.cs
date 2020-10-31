using BocagoiConsole.Core;
using System;

namespace BocagoiConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var boc = new Bocagoi();
            var machine = new StateMachine();

            var walker = new ConsoleStateMachineWalker(boc, machine);
            walker.Start();
        }
    }
}
