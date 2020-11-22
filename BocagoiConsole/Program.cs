using BocagoiConsole.Core;

namespace BocagoiConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var boc = new Bocagoi();
            var machine = new StateMachine();
            var stateMap = machine.Generate(boc);

            var walker = new ConsoleStateMachineWalker(stateMap);

            walker.Start(boc.PracticeSettings);
        }
    }
}
