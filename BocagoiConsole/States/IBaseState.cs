namespace BocagoiConsole.States;

public interface IBaseState
{
    public StateID Run();
}

public enum StateID
{
    None,
    Menu,
    PracticeSelectBox, PracticeSelectWords, PracticeSelectMode, RunPractice,
    AddWords,
    CreateBox,
    Search,
    History,
    MostPracticedWords,
    MostFailedWords,
    Settings,
    Exit,
}
