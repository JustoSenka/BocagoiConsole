using BocagoiConsole.Core;
using BocagoiConsole.Singletons;

namespace BocagoiConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Boxes.Init();
            History.Init();
            RedBox.Init();
            Bocagoi.Init();

            var stateMap = StateMachine.Generate();
            var walker = new ConsoleStateMachineWalker(stateMap);
            walker.Start();
        }
    }
}
