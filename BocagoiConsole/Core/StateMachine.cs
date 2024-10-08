﻿using BocagoiConsole.Singletons;
using BocagoiConsole.States;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BocagoiConsole.Core;

public class StateMachine
{
    public static Dictionary<StateID, IBaseState> Generate()
    {
        return new Dictionary<StateID, IBaseState>
        {
            {
                StateID.Menu,  new MenuState(
                    textToPrint: Strings.Menu,
                    transitions: new Dictionary<int, StateID>
                {
                    {1,  StateID.PracticeSelectBox },
                    {2,  StateID.AddWords },
                    {3,  StateID.CreateBox},
                    {4,  StateID.Search},
                    {5,  StateID.History},
                    {6,  StateID.MostFailedWords},
                    {7,  StateID.LeastPracticedWords},
                    {8,  StateID.Settings},
                    {0,  StateID.Exit },
                })
            },
            {
                StateID.PracticeSelectBox, new MenuState(actionBeforePrint: () =>
                {
                    Boxes.Instance.ReloadWords();
                    var availableBoxes = Boxes.Instance.BuildBoxNameListWithWordCount();
                    Console.Write(string.Format(Strings.PracticeSelectBox, availableBoxes));

                }, actionAfterUserInput: (op) =>
                {
                    Bocagoi.Instance.Settings.Box = op;

                }, transitions: new Dictionary<int, StateID>
                {
                    {0,  StateID.Exit },
                },
                    funcNextState: _ => StateID.PracticeSelectWords)
            },
            {
                StateID.PracticeSelectWords, new SingleActionState(action: () =>
                {
                    var wordCount = Boxes.Instance.GetWords(Bocagoi.Instance.Settings.Box).Count;
                    Console.Write(string.Format(Strings.PracticeSelectWords1, wordCount));

                    Bocagoi.Instance.GetDictionaryBoundsForPracticing(wordCount, out int from, out int to);

                    Bocagoi.Instance.Settings.WordsMax = to;
                    Bocagoi.Instance.Settings.WordsMin = from;

                }, funcNextState: () =>
                    Bocagoi.Instance.Settings.WordsMin <= 0 || Bocagoi.Instance.Settings.WordsMax <= 0
                        ? StateID.Exit
                        : StateID.PracticeSelectMode)
            },
            {
                StateID.PracticeSelectMode,
                new MenuState(actionAfterPrint: () =>
                {
                    var textToWrite = string.Format(Strings.PracticeSelectMode,
                        Bocagoi.Instance.GetWordsForPractice().Count,
                        (Bocagoi.Instance.Settings.WordsMax - Bocagoi.Instance.Settings.WordsMin + 1),
                        GlobalSettings.Instance.Data.Difficulty,
                        Bocagoi.Instance.Settings.Mode);

                    Console.Write(textToWrite);
                }, actionAfterUserInput: (op) =>
                {
                    if (op == 2)
                    {
                        GlobalSettings.Instance.Data.Difficulty -= 10;
                        GlobalSettings.Instance.Data.Difficulty = Math.Clamp(GlobalSettings.Instance.Data.Difficulty, 0, 100);
                        GlobalSettings.Instance.Save();
                    }
                    if (op == 3)
                    {
                        GlobalSettings.Instance.Data.Difficulty += 10;
                        GlobalSettings.Instance.Data.Difficulty = Math.Clamp(GlobalSettings.Instance.Data.Difficulty, 0, 100);
                        GlobalSettings.Instance.Save();
                    }
                    if (op == 5)
                    {
                        Bocagoi.Instance.Settings.Mode = Bocagoi.Instance.Settings.Mode == PracticeSettings.PracticeMode.Normal
                            ? PracticeSettings.PracticeMode.Reverse
                            : PracticeSettings.PracticeMode.Normal;
                    }

                }, transitions: new Dictionary<int, StateID>
                {
                    {1,  StateID.RunPractice },
                    {2,  StateID.PracticeSelectMode },
                    {3,  StateID.PracticeSelectMode },
                    {4,  StateID.PrintSelectedWords },
                    {5,  StateID.PracticeSelectMode },
                    {0,  StateID.Exit },
                })
            },
            {
                StateID.PrintSelectedWords, new MenuState(actionBeforePrint: () =>
                {
                    var words = string.Join(Environment.NewLine, 
                        Bocagoi.Instance.GetWordsForPractice()
                        .OrderByDescending(word => RedBox.Instance.Words.ContainsKey(word.Left) ? RedBox.Instance.Words[word.Left].SuccessScore : 0)
                        .Select(word => $"{word.Left} - {word.Right}"));

                    Console.Write(string.Format(Strings.PrintSelectedWords, words));

                }, funcNextState: _ => StateID.PracticeSelectMode)
            },
            {
                StateID.RunPractice, new SingleActionState(
                    action: () =>
                    {
                        Bocagoi.Instance.RunGame();
                    },
                    funcNextState: () => StateID.Menu)
            },
            {
                StateID.AddWords, new MenuState(actionBeforePrint: () =>
                {
                    var availableBoxes = Boxes.Instance.BuildBoxNameListWithWordCount();
                    Console.Write(string.Format(Strings.AddWordsToBox, availableBoxes));

                }, actionAfterUserInput: (op) =>
                {
                    if (op != 0)
                        Bocagoi.Instance.TryOpenBox(op);

                }, funcNextState: _ => StateID.Menu)
            },
            {
                StateID.CreateBox, new SingleActionState(action: () =>
                {
                    // TODO: Create new box
                    // var index = Boxes.Instance.CreateNewBox();
                    // Bocagoi.Instance.TryOpenBox(index);

                }, funcNextState: () => StateID.Menu)
            },
            {
                StateID.Search, new SingleActionState(action: () =>
                {
                    Console.WriteLine(Strings.SearchWords);
                    var str = Console.ReadLine();
                    Console.WriteLine();

                    var wordBoxesMap = Boxes.Instance.BoxList.Values.Select(box =>
                       (box.Index,
                        box.Name,
                        box.Words.Where(tuple => tuple.Left.Contains(str) || tuple.Right.Contains(str)).ToList()));

                    var sb = new StringBuilder();
                    foreach(var (index, boxName, wordsFound) in wordBoxesMap)
                    {
                        if (wordsFound.Count == 0)
                            continue;

                        sb.AppendLine(boxName + ":");
                        sb.AppendLine();
                        foreach(var (Left, Right) in wordsFound)
                            sb.AppendLine(Left + " - " + Right);

                        sb.AppendLine();
                    }

                    Console.WriteLine(sb.ToString());
                    Console.WriteLine("Press enter to continue...");
                    Console.ReadLine();

                }, funcNextState: () => StateID.Menu)
            },
            {
                StateID.History, new SingleActionState(action: () =>
                {
                    var entries = string.Join(Environment.NewLine, History.Instance.Runs.Select(run => run.ToString()));
                    var str = string.Format(Strings.History, entries);

                    Console.WriteLine(str);

                    Console.ReadLine();
                }, funcNextState: () => StateID.Menu)
            },
            {
                StateID.MostFailedWords, new SingleActionState(action: () =>
                {
                    var entries = string.Join(Environment.NewLine, RedBox.Instance.Words.Values
                        .OrderByDescending(word => word.Fails - word.Correct)
                        .Select(word => string.Format(" {0, -5} | {1, -5} | {2, -7} |  {3} - {4}",
                            word.Fails - word.Correct, word.Fails, word.Correct, word.Left, word.Right))
                        .Prepend(string.Format(" {0, -5} | {1, -5} | {2, -7} |  {3}", "score", "fails", "correct", "Word")));

                    var str = string.Format(Strings.MostFailedWords, entries);

                    Console.WriteLine(str);

                    Console.ReadLine();
                }, funcNextState: () => StateID.Menu)
            },
            {
                StateID.LeastPracticedWords, new SingleActionState(action: () =>
                {
                    var entries = string.Join(Environment.NewLine, RedBox.Instance.Words.Values
                        .OrderBy(word => word.Correct + word.Fails)
                        .Select(word => string.Format(" {0, -5} | {1, -5} | {2, -7} |  {3} - {4}",
                            word.Correct + word.Fails, word.Fails, word.Correct, word.Left, word.Right))
                        .Prepend(string.Format(" {0, -5} | {1, -5} | {2, -7} |  {3}", "score", "fails", "correct", "Word")));

                    var str = string.Format(Strings.LeastPracticedWords, entries);

                    Console.WriteLine(str);

                    Console.ReadLine();
                }, funcNextState: () => StateID.Menu)
            },
        };
    }
}
