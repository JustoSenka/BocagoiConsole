using System;

namespace BocagoiConsole.Common
{
    /// <summary>
    /// Random which will not return the same integer value twice in a row.
    /// </summary>
    public class UniqueRandom : Random
    {
        private int m_LastInt;

        public UniqueRandom() : base() { }
        public UniqueRandom(int Seed) : base(Seed) { }

        public override int Next()
        {
            int newInt = m_LastInt;
            for (int i = 0; i <= 10 && m_LastInt == newInt; i++)
            {
                newInt = base.Next();
            }
            return m_LastInt = newInt;
        }

        public override int Next(int maxValue)
        {
            int newInt = m_LastInt;
            for (int i = 0; i <= 10 && m_LastInt == newInt; i++)
            {
                newInt = base.Next(maxValue);
            }
            return m_LastInt = newInt;
        }

        public override int Next(int minValue, int maxValue)
        {
            int newInt = m_LastInt;
            for (int i = 0; i <= 10 && m_LastInt == newInt; i++)
            {
                newInt = base.Next(minValue, maxValue);
            }
            return m_LastInt = newInt;
        }
    }
}
