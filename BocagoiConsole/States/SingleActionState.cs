using System;

namespace BocagoiConsole.States;

public class SingleActionState : IBaseState
{
    private readonly Action m_Action;
    private readonly Func<StateID> m_FuncNextState;

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
