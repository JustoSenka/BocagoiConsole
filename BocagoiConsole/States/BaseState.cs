using BocagoiConsole.Core;

namespace BocagoiConsole.States
{
    public interface BaseState
    {
        public State Run(PracticeSettings pr);
    }

    public enum State
    {
        None,
        Menu,
        PracticeSelectBox, PracticeSelectWords, PracticeSelectMode, RunPractice,
        AddWords,
        CreateBox,
        History,
        Settings,
        Exit,
    }
}
