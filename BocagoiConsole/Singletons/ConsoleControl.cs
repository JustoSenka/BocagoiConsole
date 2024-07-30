using Newtonsoft.Json;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace BocagoiConsole.Singletons;

public class ConsoleControl : IDisposable
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

    private ConsoleSettings m_Settings;

    private ConsoleControl()
    {
        if (!File.Exists(m_FilePath))
        {
            using var file = File.Create(m_FilePath);
            file.Close();
            return;
        }

        var text = File.ReadAllText(m_FilePath);
        m_Settings = JsonConvert.DeserializeObject<ConsoleSettings>(text);
        UpdateTitle();

        if (m_Settings != null)
            RestoreSize();
    }

    public void Dispose()
    {
        CollectCurrentSettings();

        var text = JsonConvert.SerializeObject(m_Settings);
        File.WriteAllText(m_FilePath, text);
    }

    private void CollectCurrentSettings()
    {
        m_Settings ??= new ConsoleSettings();

        try
        {
            m_Settings.WindowTop = Console.WindowTop;
            m_Settings.WindowLeft = Console.WindowLeft;
            m_Settings.WindowWidth = Console.WindowWidth;
            m_Settings.WindowHeight = Console.WindowHeight;

            var fontSize = GetFontSize();
            m_Settings.FontWidth = fontSize.Item1;
            m_Settings.FontHeight = fontSize.Item2;
        }
        catch { }
    }

    public void RestoreSize()
    {
#pragma warning disable CA1416 // Validate platform compatibility
        try
        {
            SetFontSize(m_Settings.FontWidth, m_Settings.FontHeight);
            Console.SetWindowSize(m_Settings.WindowWidth, m_Settings.WindowHeight);
            Console.SetWindowPosition(m_Settings.WindowLeft, m_Settings.WindowTop);
        }
        catch
        {
            CollectCurrentSettings();
        }
#pragma warning restore CA1416 // Validate platform compatibility
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

    [DllImport("kernel32.dll", SetLastError = true)]
    static extern IntPtr GetStdHandle(int nStdHandle);

    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    static extern bool GetCurrentConsoleFontEx(
           IntPtr consoleOutput,
           bool maximumWindow,
           ref CONSOLE_FONT_INFO_EX lpConsoleCurrentFontEx);

    [DllImport("kernel32.dll", SetLastError = true)]
    static extern bool SetCurrentConsoleFontEx(
           IntPtr consoleOutput,
           bool maximumWindow,
           CONSOLE_FONT_INFO_EX consoleCurrentFontEx);

    private const int STD_OUTPUT_HANDLE = -11;
    private static IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);

    public static unsafe (short, short) GetFontSize()
    {
        var hnd = GetStdHandle(STD_OUTPUT_HANDLE);
        if (hnd == INVALID_HANDLE_VALUE)
            return (8, 16);

        var info = new CONSOLE_FONT_INFO_EX();
        info.cbSize = (uint)Marshal.SizeOf(info);

        if (!GetCurrentConsoleFontEx(hnd, false, ref info))
            return (8, 16);

        return (info.dwFontSize.X, info.dwFontSize.Y);
    }

    public static unsafe void SetFontSize(short x, short y)
    {
        var hnd = GetStdHandle(STD_OUTPUT_HANDLE);
        if (hnd == INVALID_HANDLE_VALUE)
            return;

        var info = new CONSOLE_FONT_INFO_EX();
        info.cbSize = (uint)Marshal.SizeOf(info);

        if (!GetCurrentConsoleFontEx(hnd, false, ref info))
            return;

        info.dwFontSize = new COORD(x, y);
        SetCurrentConsoleFontEx(hnd, false, info);
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct COORD
    {
        internal short X;
        internal short Y;

        internal COORD(short x, short y)
        {
            X = x;
            Y = y;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct CONSOLE_FONT_INFO_EX
    {
        internal uint cbSize;
        internal uint nFont;
        internal COORD dwFontSize;
        internal int FontFamily;
        internal int FontWeight;
        internal fixed char FaceName[32];
    }

}

public class ConsoleSettings
{
    public short FontWidth { get; set; }
    public short FontHeight { get; set; }
    public int WindowWidth { get; set; }
    public int WindowHeight { get; set; }
    public int WindowLeft { get; set; }
    public int WindowTop { get; set; }
}
