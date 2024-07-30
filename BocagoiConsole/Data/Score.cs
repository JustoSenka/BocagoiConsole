using System;
using System.Collections.Generic;

namespace BocagoiConsole.Core;

public class Score
{
    public int m_Correct = 0;
    public int m_Incorrect = 0;

    public ISet<(string Left, string Right)> CorrectWords = new HashSet<(string Left, string Right)>();
    public ISet<(string Left, string Right)> Mistakes = new HashSet<(string Left, string Right)>();

    public void Correct() => m_Correct++;
    public void Incorrect() => m_Incorrect++;

    public int DecimalScore()
    {
        var score = (int)(100f * m_Correct / (m_Correct + m_Incorrect));
        return Math.Max(0, Math.Min(100, score));
    }
}
