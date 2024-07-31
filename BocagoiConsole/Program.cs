using BocagoiConsole.Core;
using BocagoiConsole.Singletons;
using BocagoiConsole.States;
using System;
using System.Runtime.InteropServices;

namespace BocagoiConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            GlobalSettings.Init();

            Boxes.Init();
            History.Init();
            RedBox.Init();
            Bocagoi.Init();

            ConsoleControl.Init();
            ConsoleControl.Instance.UpdateTitle();

            if (GlobalSettings.Instance.Data.RememberConsoleFontSize)
            {
                handler = new ConsoleEventDelegate(ConsoleEventCallback);
                SetConsoleCtrlHandler(handler, true);
            }

            var stateMap = StateMachine.Generate();
            var walker = new ConsoleStateMachineWalker(stateMap);
            walker.Start();

            ConsoleControl.Instance.Dispose();
        }

        static bool ConsoleEventCallback(int eventType)
        {
            ConsoleControl.Instance.Dispose();
            Environment.Exit(-1);
            return false;
        }

        static ConsoleEventDelegate handler;   // Keeps it from getting garbage collected

        private delegate bool ConsoleEventDelegate(int eventType);
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetConsoleCtrlHandler(ConsoleEventDelegate callback, bool add);
    }
}
