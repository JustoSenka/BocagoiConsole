using BocagoiConsole.Core;
using BocagoiConsole.Singletons;
using BocagoiConsole.States;
using System;

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
            ConsoleControl.Init();

            ConsoleControl.Instance.UpdateTitle();

            var stateMap = StateMachine.Generate();
            var walker = new ConsoleStateMachineWalker(stateMap);
            walker.Start();
        }
    }
}
