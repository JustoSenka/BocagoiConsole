using System;

namespace BocagoiConsole.Common
{
    /// <summary>
    /// Random which will not return the same integer value twice in a row.
    /// </summary>
    public class UniqueRandom : Random
    {
        private int m_LastNext;
        private int m_LastNextMax;
        private int m_LastNextMinMax;

        public UniqueRandom() : base() { }
        public UniqueRandom(int Seed) : base(Seed) { }

        public override int Next()
        {
            int newInt = m_LastNext;
            for (int i = 0; i <= 10 && m_LastNext == newInt; i++)
            {
                newInt = base.Next();
            }
            return m_LastNext = newInt;
        }

        public override int Next(int maxValue)
        {
            int newInt = m_LastNextMax;
            for (int i = 0; i <= 10 && m_LastNextMax == newInt; i++)
            {
                newInt = base.Next(maxValue);
            }
            return m_LastNextMax = newInt;
        }

        public override int Next(int minValue, int maxValue)
        {
            int newInt = m_LastNextMinMax;
            for (int i = 0; i <= 10 && m_LastNextMinMax == newInt; i++)
            {
                newInt = base.Next(minValue, maxValue);
            }
            return m_LastNextMinMax = newInt;
        }
    }
}
