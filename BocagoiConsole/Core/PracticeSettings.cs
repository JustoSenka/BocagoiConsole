namespace BocagoiConsole.Core
{
    public class PracticeSettings
    {
        public int Box { get; set; }
        public int WordsMin { get; set; }
        public int WordsMax { get; set; }
        public PracticeMode Mode { get; set; }

        public enum PracticeMode { Normal, Reverse }
    }

    public static class PracticeModeExtension
    {
        public static string Left(this (string, string) pair, PracticeSettings.PracticeMode pr)
        {
            return pr switch
            {
                PracticeSettings.PracticeMode.Normal => pair.Item2,
                PracticeSettings.PracticeMode.Reverse => pair.Item1,
                _ => null,
            };
        }

        public static string Right(this (string, string) pair, PracticeSettings.PracticeMode pr)
        {
            return pr switch
            {
                PracticeSettings.PracticeMode.Normal => pair.Item1,
                PracticeSettings.PracticeMode.Reverse => pair.Item2,
                _ => null,
            };
        }
    }
}
