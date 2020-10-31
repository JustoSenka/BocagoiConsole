using BocagoiConsole.States;
using System.Collections.Generic;

namespace BocagoiConsole.Core
{
    public class ConsoleStateMachineWalker
    {
        private Dictionary<State, BaseState> m_StateMap;
        private Stack<State> m_StateStack = new Stack<State>();

        public ConsoleStateMachineWalker(Bocagoi Bocagoi, StateMachine StateMachine)
        {
            m_StateMap = StateMachine.Generate(Bocagoi);
        }

        public void Start()
        {
            var pr = new PracticeSettings();

            m_StateStack.Push(State.Menu);

            while (true)
            {
                if (m_StateStack.Count == 0)
                    break;

                var currentState = m_StateStack.Peek();
                var stateAction = m_StateMap[currentState];
                var newState = stateAction.Run(pr);

                if (newState == State.Exit)
                    m_StateStack.Pop();
                else if (newState == State.None)
                    continue;
                else
                    m_StateStack.Push(newState);
            }
        }
    }
}
