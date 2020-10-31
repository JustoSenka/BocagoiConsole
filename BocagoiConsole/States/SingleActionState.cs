using BocagoiConsole.Core;
using System;

namespace BocagoiConsole.States
{
    public class SingleActionState : BaseState
    {
        private Action<PracticeSettings> m_Action;
        private Func<PracticeSettings, State> m_FuncNextState;

        public SingleActionState(
            Action<PracticeSettings> action = null, 
            Func<PracticeSettings, State> funcNextState = null)
        {
            m_Action = action;
            m_FuncNextState = funcNextState;
        }

        public State Run(PracticeSettings pr)
        {
            Console.Clear();
            m_Action?.Invoke(pr);
            return m_FuncNextState != null ? m_FuncNextState.Invoke(pr) : State.None;
        }
    }
}
