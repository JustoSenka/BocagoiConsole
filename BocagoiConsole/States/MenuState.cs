using BocagoiConsole.Core;
using System;
using System.Collections.Generic;

namespace BocagoiConsole.States
{
    public class MenuState : BaseState
    {
        private Action<PracticeSettings> m_ActionBeforePrint;
        private Action<PracticeSettings> m_ActionAfterPrint;
        private Action<PracticeSettings, int> m_ActionAfterUserInput;
        private Func<PracticeSettings, int, State> m_FuncNextState;

        private string m_TextToPrint;
        private Dictionary<int, State> m_Transitions;
        public MenuState(Action<PracticeSettings> actionBeforePrint = null, Action<PracticeSettings> actionAfterPrint = null,
            Action<PracticeSettings, int> actionAfterUserInput = null, Func<PracticeSettings, int, State> funcNextState = null,
            string textToPrint = "", Dictionary<int, State> transitions = null)
        {
            m_ActionBeforePrint = actionBeforePrint;
            m_ActionAfterPrint = actionAfterPrint;
            m_ActionAfterUserInput = actionAfterUserInput;
            m_FuncNextState = funcNextState;

            m_TextToPrint = textToPrint;
            m_Transitions = transitions;
        }

        public State Run(PracticeSettings pr)
        {
            Console.Clear();

            m_ActionBeforePrint?.Invoke(pr);
            Console.Write(m_TextToPrint);
            m_ActionAfterPrint?.Invoke(pr);

            int op = -1;
            while (!int.TryParse(Console.ReadLine(), out op)) { }

            m_ActionAfterUserInput?.Invoke(pr, op);

            // return next state
            if (m_Transitions != null && m_Transitions.ContainsKey(op))
                return m_Transitions[op];

            if (m_FuncNextState != null)
                return m_FuncNextState.Invoke(pr, op);

            return State.None;
        }
    }
}
