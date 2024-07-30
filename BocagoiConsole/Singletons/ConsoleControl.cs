using System;

namespace BocagoiConsole.Singletons;

public class ConsoleControl
{
    public static void Init() { Instance = new ConsoleControl(); }
    public static ConsoleControl Instance { get; private set; }

    private const string m_FilePath = "ConsoleControl.json";
    private const string m_DefaultTitle = "BocagoiConsole";

    private bool m_PracticeTitle = false;
    public string TitlePracticeBoxName { get; set; }
    public int TitlePracticeWordsFrom { get; set; }
    public int TitlePracticeWordsTo { get; set; }
    public int TitlePracticeWordsCompleted { get; set; }
    public int TitlePracticeWordsFailed { get; set; }

    private ConsoleControl()
    {
        UpdateTitle();
    }

    public void UpdateTitle()
    {
        var title = m_DefaultTitle;

        if (m_PracticeTitle)
            title = $"{m_DefaultTitle} | {TitlePracticeBoxName} | {TitlePracticeWordsFrom} - {TitlePracticeWordsTo} | {TitlePracticeWordsCompleted} / {TitlePracticeWordsTo - TitlePracticeWordsFrom + 1} | Misstakes: {TitlePracticeWordsFailed}";

        Console.Title = title;
    }

    public void ResetTitle()
    {
        m_PracticeTitle = false;
        UpdateTitle();
    }

    public void SetPracticeTitle()
    {
        m_PracticeTitle = true;
        UpdateTitle();
    }
}