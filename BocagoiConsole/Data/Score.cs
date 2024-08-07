using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BocagoiConsole.Core;

public class Score
{
    private int m_Correct = 0;
    private int m_Incorrect = 0;

    public ISet<(string Left, string Right)> CorrectWords = new HashSet<(string Left, string Right)>();
    public ISet<(string Left, string Right)> Mistakes = new HashSet<(string Left, string Right)>();

    public void Correct() => m_Correct++;
    public void Incorrect() => m_Incorrect++;

    private const string LastRunFile1 = "Resources/zLastRunMistakes1.txt";
    private const string LastRunFile2 = "Resources/zlastRunMistakes2.txt";
    private const string LastRunFile3 = "Resources/zlastRunMistakes3.txt";

    public int DecimalScore()
    {
        var score = (int)(100f * (m_Correct) / ((m_Correct + m_Incorrect) * 1f));
        return Math.Max(0, Math.Min(100, score));
    }

    public void SaveMistakesToFile()
    {
        if (Mistakes.Count <= 5)
            return;

        if (File.Exists(LastRunFile2))
            File.Copy(LastRunFile2, LastRunFile3, true);

        if (File.Exists(LastRunFile1))
            File.Copy(LastRunFile1, LastRunFile2, true);

        var words = string.Join(Environment.NewLine, Mistakes.Select(pair => $"{pair.Left} - {pair.Right}"));
        File.WriteAllLines(LastRunFile1, Mistakes.Select(pair => $"{pair.Left} - {pair.Right}"));
    }
}
