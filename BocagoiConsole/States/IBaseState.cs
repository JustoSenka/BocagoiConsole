﻿namespace BocagoiConsole.States;

public interface IBaseState
{
    public StateID Run();
}

public enum StateID
{
    None,
    Menu,
    PracticeSelectBox, PracticeSelectWords, PracticeSelectMode, PrintSelectedWords, RunPractice,
    AddWords,
    CreateBox,
    Search,
    History,
    LeastPracticedWords,
    MostFailedWords,
    Settings,
    Exit,
}
