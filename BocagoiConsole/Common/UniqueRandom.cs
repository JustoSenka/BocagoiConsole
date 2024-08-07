using System;

namespace BocagoiConsole.Common;

/// <summary>
/// Random which will not return the same integer value twice in a row.
/// </summary>
public class UniqueRandom
{
    private int m_LastNext;
    private int m_LastNextMax;
    private int m_LastNextMinMax;
    
    private Random m_Random;

    public UniqueRandom()
    {
        m_Random = new Random();
    }

    public int Next()
    {
        int newInt = m_LastNext;
        for (int i = 0; i <= 10 && m_LastNext == newInt; i++)
        {
            newInt = m_Random.Next();
        }
        return m_LastNext = newInt;
    }

    public int Next(int maxValue)
    {
        int newInt = m_LastNextMax;
        for (int i = 0; i <= 10 && m_LastNextMax == newInt; i++)
        {
            newInt = m_Random.Next() % (maxValue + 1);
        }
        return m_LastNextMax = newInt;
    }

    public int Next(int minValue, int maxValue)
    {
        int newInt = m_LastNextMinMax;
        for (int i = 0; i <= 10 && m_LastNextMinMax == newInt; i++)
        {
            newInt = minValue + (m_Random.Next() % (maxValue - minValue + 1));
        }
        return m_LastNextMinMax = newInt;
    }
}
