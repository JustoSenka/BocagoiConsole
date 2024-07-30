using BocagoiConsole.States;
using System;
using System.Collections.Generic;

namespace BocagoiConsole.Core
{
    public class ConsoleStateMachineWalker
    {
        private readonly Dictionary<StateID, BaseState> m_StateMap;

        public ConsoleStateMachineWalker(Dictionary<StateID, BaseState> stateMap)
        {
            m_StateMap = stateMap;
        }

        public void Start()
        {
            var stateStack = new Stack<StateID>();
            stateStack.Push(StateID.Menu);

            while (true)
            {
                if (stateStack.Count == 0)
                    break;

                var currentState = stateStack.Peek();
                var stateAction = m_StateMap[currentState];
                var newState = stateAction.Run();

                if (newState == StateID.Exit)
                    stateStack.Pop();
                
                else if (newState == StateID.None)
                    continue;
                
                else if (m_StateMap.ContainsKey(newState) && !stateStack.Contains(newState))
                    stateStack.Push(newState);

                else if (m_StateMap.ContainsKey(newState) && stateStack.Contains(newState))
                {
                    while (stateStack.Peek() != newState)
                        stateStack.Pop();
                }

                else // does not contain
                {
                    Console.WriteLine("Action not yet supported");
                    Console.WriteLine("Press enter to continue...");
                    Console.ReadLine();
                }
            }
        }
    }
}
