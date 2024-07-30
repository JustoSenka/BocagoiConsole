using System;
using System.Collections.Generic;

namespace BocagoiConsole.States;

public class MenuState : IBaseState
{
    private readonly Action m_ActionBeforePrint;
    private readonly Action m_ActionAfterPrint;
    private readonly Action<int> m_ActionAfterUserInput;
    private readonly Func<int, StateID> m_FuncNextState;

    private readonly string m_TextToPrint;
    private readonly Dictionary<int, StateID> m_Transitions;
    public MenuState(
        Action actionBeforePrint = null, 
        Action actionAfterPrint = null,
        Action<int> actionAfterUserInput = null, 
        Func<int, StateID> funcNextState = null,
        string textToPrint = "", 
        Dictionary<int, StateID> transitions = null)
    {
        m_ActionBeforePrint = actionBeforePrint;
        m_ActionAfterPrint = actionAfterPrint;
        m_ActionAfterUserInput = actionAfterUserInput;
        m_FuncNextState = funcNextState;

        m_TextToPrint = textToPrint;
        m_Transitions = transitions;
    }

    public StateID Run()
    {
        Console.Clear();

        m_ActionBeforePrint?.Invoke();
        Console.Write(m_TextToPrint);
        m_ActionAfterPrint?.Invoke();

        int op = -1;
        while (!int.TryParse(Console.ReadLine(), out op)) { }

        m_ActionAfterUserInput?.Invoke(op);

        // return next state
        if (m_Transitions != null && m_Transitions.ContainsKey(op))
            return m_Transitions[op];

        if (m_FuncNextState != null)
            return m_FuncNextState.Invoke(op);

        return StateID.None;
    }
}
