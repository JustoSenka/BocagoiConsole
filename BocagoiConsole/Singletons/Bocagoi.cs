using BocagoiConsole.Common;
using BocagoiConsole.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace BocagoiConsole.Singletons;

public class Bocagoi
{
    public static void Init() { Instance = new Bocagoi(); }
    public static Bocagoi Instance { get; private set; } 

    public PracticeSettings Settings { get; }

    private Bocagoi()
    {
        Settings = new PracticeSettings();
    }

    public Run RunGame()
    {
        var totalWordsForPractice = Boxes.Instance.GetWords(Settings.Box)
            .Skip(Settings.WordsMin - 1)
            .Take(Settings.WordsMax - Settings.WordsMin + 1)
            .ToList();

        var rand = new UniqueRandom();
        var score = new Score();

        var startTime = DateTime.Now;

        while (totalWordsForPractice.Count > 0)
        {
            var wordsLeft = totalWordsForPractice.PartitionListElements(20, rand);

            RunGameWithPartitionedWords(rand, score, wordsLeft);
        }

        var result = CreateAndPrintResults(score, startTime, endTime: DateTime.Now);

        SaveRedBox(score);

        Console.WriteLine("Press enter to continue...");
        Console.ReadLine();

        ConsoleControl.Instance.ResetTitle();

        return result;
    }

    private void RunGameWithPartitionedWords(Random rand, Score score, IList<(string, string)> wordsLeft)
    {
        while (wordsLeft.Count > 0)
        {
            UpdateTitle(score);

            var word = SelectRandomWord(rand, wordsLeft);
            var answer = AskToInputAnswer(word);

            VerifyAnswer(score, wordsLeft, word, answer);

            Console.WriteLine("Press enter to continue...");
            Console.ReadLine();
            Console.Clear();
        }
    }

    private void UpdateTitle(Score score)
    {
        ConsoleControl.Instance.TitlePracticeWordsFrom = Settings.WordsMin;
        ConsoleControl.Instance.TitlePracticeWordsTo = Settings.WordsMax;
        ConsoleControl.Instance.TitlePracticeBoxName = Boxes.Instance.GetBoxName(Settings.Box);
        ConsoleControl.Instance.TitlePracticeWordsCompleted = score.CorrectWords.Count;
        ConsoleControl.Instance.TitlePracticeWordsFailed = score.Mistakes.Count;
        ConsoleControl.Instance.SetPracticeTitle();
    }

    public void VerifyAnswer(Score score, IList<(string, string)> wordsLeft, (string, string) word, string answer)
    {
        if (IsAnswerCorrect(word, answer))
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
            Console.WriteLine($"Incorrect! It was: {word.Right(Settings.Mode)}");
            Console.WriteLine();
        }
    }

    private string AskToInputAnswer((string, string) word)
    {
        Console.Write($"{word.Left(Settings.Mode)} - ");
        var answer = Console.ReadLine();
        return answer;
    }

    private (string, string) SelectRandomWord(Random rand, IList<(string, string)> wordsLeft)
    {
        var nextWordIndex = rand.Next(0, wordsLeft.Count - 1);
        return wordsLeft[nextWordIndex % wordsLeft.Count];
    }

    public bool IsAnswerCorrect((string, string) word, string answer)
    {
        return string.Equals(answer, word.Right(Settings.Mode), StringComparison.InvariantCultureIgnoreCase);
    }

    public Run CreateAndPrintResults(Score score, DateTime startTime, DateTime endTime)
    {
        Console.WriteLine($"Final score: {score.DecimalScore()}");
        Console.WriteLine();
        Console.WriteLine("Words in which mistakes were made:");
        Console.WriteLine();

        foreach (var word in score.Mistakes)
            Console.WriteLine($"{word.Left(Settings.Mode)} - {word.Right(Settings.Mode)}");

        Console.WriteLine();

        return new Run()
        {
            Box = Settings.Box,
            From = Settings.WordsMin,
            To = Settings.WordsMax,
            Score = score.DecimalScore(),
            Mode = Settings.Mode,
            Time = startTime,
            Duration = endTime - startTime
        }; ;
    }

    public void SaveRedBox(Score score)
    {
        if (score.DecimalScore() < 50)
            return;

        foreach (var correct in score.CorrectWords)
            RedBox.Instance.Succeed(correct);

        foreach (var miss in score.Mistakes)
            RedBox.Instance.Fail(miss);

        RedBox.Instance.Save();
    }

    public void TryOpenBox(int num)
    {
        try
        {
            Process.Start(new ProcessStartInfo("cmd", $"/c start {Boxes.Instance.GetBoxName(num)}"));
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }

    public void GetDictionaryBoundsForPracticing(int wordCount, out int from, out int to)
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

    private bool IsWordSelectionInBounds(int wordCount, int from, int to)
    {
        return from < to && from >= 0 && to <= wordCount;
    }
}

public class PracticeSettings
{
    public int Box { get; set; }
    public char RedBoxMode { get; set; }
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
