using BocagoiConsole.Common;
using BocagoiConsole.Files;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BocagoiConsole.Core
{
    public class Bocagoi
    {
        public PracticeSettings PracticeSettings { get; }

        public const string WordBoxFileName = "WBox{0}.txt";

        public Bocagoi()
        {
            PracticeSettings = new PracticeSettings();

            var box1 = string.Format(WordBoxFileName, 1);
            var box2 = string.Format(WordBoxFileName, 2);

            if (!File.Exists(box1))
                File.Create(box1);

            if (!File.Exists(box2))
                File.Create(box2);
        }

        public static Run RunGame(PracticeSettings pr)
        {
            var words = LoadAllWords();
            var totalWordsForPractice = words[pr.Box].Skip(pr.WordsMin - 1).Take(pr.WordsMax - pr.WordsMin + 1).ToList();

            var rand = new UniqueRandom();
            var score = new Score();
            var redBox = new RedBox();
            redBox.LoadFromFile(RedBox.RedBoxFile);

            var startTime = DateTime.Now;

            while (totalWordsForPractice.Count > 0)
            {
                var wordsLeft = totalWordsForPractice.PartitionListElements(20, rand);

                RunGameWithPartitionedWords(pr, rand, score, redBox, wordsLeft);
            }

            var result = CreateAndPrintResults(pr, score, startTime, endTime: DateTime.Now);

            SaveRedBox(redBox, score);

            Console.WriteLine("Press enter to continue...");
            Console.ReadLine();
            return result;
        }

        private static void RunGameWithPartitionedWords(PracticeSettings pr, Random rand, Score score, RedBox redBox, IList<(string, string)> wordsLeft)
        {
            while (wordsLeft.Count > 0)
            {
                var word = SelectRandomWord(rand, wordsLeft);
                var answer = AskToInputAnswer(pr, word);

                VerifyAnswer(pr, score, redBox, wordsLeft, word, answer);

                Console.WriteLine("Press enter to continue...");
                Console.ReadLine();
                Console.Clear();
            }
        }

        public static void VerifyAnswer(PracticeSettings pr, Score score, RedBox redBox, IList<(string, string)> wordsLeft, (string, string) word, string answer)
        {
            if (IsAnswerCorrect(pr, word, answer))
            {
                wordsLeft.Remove(word);
                score.Correct();
                score.CorrectWords.Add(word);

                Console.WriteLine();
                Console.WriteLine("Correct!");
                Console.WriteLine();
            }
            else
            {
                score.Incorrect();
                score.Mistakes.Add(word);

                Console.WriteLine();
                Console.WriteLine($"Incorrect! It was: {word.Right(pr.Mode)}");
                Console.WriteLine();
            }
        }

        private static string AskToInputAnswer(PracticeSettings pr, (string, string) word)
        {
            Console.Write($"{word.Left(pr.Mode)} - ");
            var answer = Console.ReadLine();
            return answer;
        }

        private static (string, string) SelectRandomWord(Random rand, IList<(string, string)> wordsLeft)
        {
            var nextWordIndex = rand.Next(0, wordsLeft.Count - 1);
            return wordsLeft[nextWordIndex % wordsLeft.Count];
        }

        public static bool IsAnswerCorrect(PracticeSettings pr, (string, string) word, string answer)
        {
            return string.Equals(answer, word.Right(pr.Mode), StringComparison.InvariantCultureIgnoreCase);
        }

        public static Run CreateAndPrintResults(PracticeSettings pr, Score score, DateTime startTime, DateTime endTime)
        {
            Console.WriteLine($"Final score: {score.DecimalScore()}");
            Console.WriteLine();
            Console.WriteLine("Words in which mistakes were made:");
            Console.WriteLine();

            foreach (var word in score.Mistakes)
                Console.WriteLine($"{word.Left(pr.Mode)} - {word.Right(pr.Mode)}");

            Console.WriteLine();

            return new Run()
            {
                Box = pr.Box,
                From = pr.WordsMin,
                To = pr.WordsMax,
                Score = score.DecimalScore(),
                Mode = pr.Mode,
                Time = startTime,
                Duration = endTime - startTime
            }; ;
        }

        public static Task AppendHistory(Run run)
        {
            // TODO: Reads history file every time, would be best to read on startup and reuse
            // for it we need to make this class non static
            var h = new History();
            return h.LoadFromFile(History.DefaultHistoryFile).ContinueWith(t =>
            {
                h.Runs.Add(run);
                h.Save();
            });
        }

        public static void SaveRedBox(RedBox redBox, Score score)
        {
            if (score.DecimalScore() < 50)
                return;

            foreach (var miss in score.Mistakes)
                redBox.Fail(miss);

            foreach (var correct in score.CorrectWords)
                redBox.Succeed(correct);

            redBox.Save();
        }

        public static void TryOpenBox(int num)
        {
            try
            {
                Process.Start(new ProcessStartInfo("cmd", $"/c start {string.Format(WordBoxFileName, num)}"));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public static void GetDictionaryBoundsForPracticing(int wordCount, out int from, out int to)
        {
            from = 0;
            to = 0;
            while (!IsWordSelectionInBounds(wordCount, from, to))
            {
                Console.Write(Strings.PracticeSelectWords2);
                int.TryParse(Console.ReadLine(), out from);

                Console.Write(Strings.PracticeSelectWords3);
                int.TryParse(Console.ReadLine(), out to);

                if (!IsWordSelectionInBounds(wordCount, from, to))
                {
                    Console.WriteLine("\nWords selection is incorrect. Word selection should not exceed word count and at 'from' should be lower than 'to'.");
                    Console.WriteLine("\nPress enter to continue or enter 'q' to return back to menu.\n");

                    var answer = Console.ReadLine();
                    if ("q".Equals(answer, StringComparison.InvariantCultureIgnoreCase))
                    {
                        from = -1;
                        to = -1;
                        return;
                    }
                }
            }
        }

        private static bool IsWordSelectionInBounds(int wordCount, int from, int to)
        {
            return from < to && from >= 0 && to <= wordCount;
        }

        public static string ComputeAvailableBoxesString()
        {
            var sb = new StringBuilder();
            int i = 1;
            foreach (var boxName in GetAllBoxNames())
                sb.AppendLine($"{i++}. {boxName}");

            return sb.ToString();
        }

        public static IList<string> GetAllBoxNames()
        {
            var boxes = new List<string>();
            int i = 1;
            while (true)
            {
                var boxName = string.Format(WordBoxFileName, i++);

                if (File.Exists(boxName))
                    boxes.Add(boxName);
                else
                    break;
            }
            return boxes;
        }

        public static Dictionary<int, (string, string)[]> LoadAllWords()
        {
            var Words = new Dictionary<int, (string, string)[]>();

            int i = 1;
            foreach (var boxName in GetAllBoxNames())
            {
                var pairs = File.ReadAllLines(boxName)
                    .Select(line => line.Trim(' ', '\r', '\t').Split('_', '-', '–'))
                    .Where(words => words.Length > 1) // Just in case no separator, so it works
                    .Select(words => (words[0].Trim(), words[1].Trim()))
                    .ToArray();

                Words[i] = pairs;
                i++;
            }

            return Words;
        }
    }
}
