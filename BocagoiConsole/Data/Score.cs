using System.Collections.Generic;

namespace BocagoiConsole.Core
{
    public class Score
    {
        public int Correct = 0;
        public int Incorrect = 0;

        public ISet<(string, string)> Mistakes = new HashSet<(string, string)>();

        public int DecimalScore()
        {
            return Correct / (Correct + Incorrect);
        }
    }
}
