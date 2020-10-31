using BocagoiConsole.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace BocagoiConsole.Core
{
    public class Bocagoi : IBocagoi
    {
        public const string WordBoxFileName = "WBox{0}.txt";

        public Bocagoi()
        {
            var box1 = string.Format(WordBoxFileName, 1);
            var box2 = string.Format(WordBoxFileName, 2);

            if (!File.Exists(box1))
                File.Create(box1);

            if (!File.Exists(box2))
                File.Create(box2);
        }

        public static void RunGame(PracticeSettings pr)
        {
            var words = LoadAllWords();
            var wordsLeft = words[pr.Box].Skip(pr.WordsMin - 1).Take(pr.WordsMax - pr.WordsMin + 1).ToList();

            var rand = new Random();
            var score = new Score();

            while (wordsLeft.Count > 0)
            {
                var word = wordsLeft[rand.Next() % wordsLeft.Count];

                Console.Write($"{word.Left(pr.Mode)} - ");
                var answer = Console.ReadLine();

                if (answer == word.Right(pr.Mode))
                {
                    wordsLeft.Remove(word);
                    score.Correct++;
                    Console.WriteLine("Correct!");
                }
                else
                {
                    score.Incorrect--;
                    score.Mistakes.Add(word);

                    Console.WriteLine($"Incorrect! It was: {word.Right(pr.Mode)}");

                }

                Console.WriteLine("Press enter to continue...");
                Console.ReadLine();
                Console.Clear();
            }

            Console.WriteLine("Final score: " + score.DecimalScore());
            Console.WriteLine();
            Console.WriteLine("Words in which mistakes were made:");
            Console.WriteLine();

            foreach (var word in score.Mistakes)
                Console.WriteLine($"{word.Left(pr.Mode)} - {word.Right(pr.Mode)}");

            Console.WriteLine();
            Console.WriteLine("Press enter to continue...");
            Console.ReadLine();
        }

        public static void OpenBox(int num)
        {
            Process.Start(new ProcessStartInfo("cmd", $"/c start {string.Format(WordBoxFileName, num)}"));
        }

        public static void GetDictionaryBoundsForPracticing(int wordCount, out int from, out int to)
        {
            from = 0;
            to = 0;
            while (!(from < to && from >= 0 && to <= wordCount))
            {
                Console.Write(Strings.PracticeSelectWords2);
                int.TryParse(Console.ReadLine(), out from);

                Console.Write(Strings.PracticeSelectWords3);
                int.TryParse(Console.ReadLine(), out to);
            }
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
                    .Select(line => line.Trim(' ', '\r', '\t').Split('_', '-'))
                    .Select(els => (els[0].Trim(), els[1].Trim()))
                    .ToArray();

                Words[i++] = pairs;
            }

            return Words;
        }
    }
}
