using System;

namespace BocagoiConsole.States
{
    public class SingleActionState : BaseState
    {
        private Action m_Action;
        private Func<StateID> m_FuncNextState;

        public SingleActionState(
            Action action = null,
            Func<StateID> funcNextState = null)
        {
            m_Action = action;
            m_FuncNextState = funcNextState;
        }

        public StateID Run()
        {
            Console.Clear();
            m_Action?.Invoke();
            return m_FuncNextState != null ? m_FuncNextState.Invoke() : StateID.None;
        }
    }
}
