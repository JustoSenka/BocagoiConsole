using System;
using System.Collections.Generic;

namespace BocagoiConsole.Core;

public class Score
{
    private int m_Correct = 0;
    private int m_Incorrect = 0;

    public ISet<(string Left, string Right)> CorrectWords = new HashSet<(string Left, string Right)>();
    public ISet<(string Left, string Right)> Mistakes = new HashSet<(string Left, string Right)>();

    public void Correct() => m_Correct++;
    public void Incorrect() => m_Incorrect++;

    public int DecimalScore()
    {
        var score = (int)(100f * (m_Correct - m_Incorrect) / (m_Correct * 1f));
        return Math.Max(0, Math.Min(100, score));
    }
}
