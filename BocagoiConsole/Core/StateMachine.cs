using BocagoiConsole.States;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BocagoiConsole.Core
{
    public class StateMachine
    {
        public Dictionary<StateID, BaseState> Generate(Bocagoi bocagoi)
        {
            return new Dictionary<StateID, BaseState>
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
                        {6,  StateID.Settings},
                        {0,  StateID.Exit },
                    })
                },
                {
                    StateID.PracticeSelectBox,  new MenuState(actionBeforePrint: (pr) =>
                    {
                        var availableBoxes = Bocagoi.ComputeAvailableBoxesString();
                        Console.Write(string.Format(Strings.PracticeSelectBox, availableBoxes));

                    }, actionAfterUserInput: (pr, op) =>
                    {
                        pr.Box = op;

                    }, transitions: new Dictionary<int, StateID>
                    {
                        {0,  StateID.Exit },
                    },
                        funcNextState: (_, __) => StateID.PracticeSelectWords)
                },
                {
                    StateID.PracticeSelectWords,  new SingleActionState(action: (pr) =>
                    {
                        var wordCount = File.ReadAllLines(string.Format(Bocagoi.WordBoxFileName, pr.Box)).Length;
                        Console.Write(string.Format(Strings.PracticeSelectWords1, wordCount));

                        Bocagoi.GetDictionaryBoundsForPracticing(wordCount, out int from, out int to);

                        pr.WordsMax = to;
                        pr.WordsMin = from;

                    }, funcNextState: pr => pr.WordsMin == -1 ? StateID.Exit : StateID.PracticeSelectMode)
                },
                {
                    StateID.PracticeSelectMode,  new MenuState(textToPrint: Strings.PracticeSelectMode,
                        actionAfterUserInput: (pr, op) =>
                    {
                        pr.Mode = op == 1 ? PracticeSettings.PracticeMode.Normal : PracticeSettings.PracticeMode.Reverse;

                    }, transitions: new Dictionary<int, StateID>
                    {
                        {1,  StateID.RunPractice },
                        {2,  StateID.RunPractice },
                        {0,  StateID.Exit },
                    })
                },
                {
                    StateID.RunPractice,  new SingleActionState(
                        action: (pr) =>
                        {
                            var run = Bocagoi.RunGame(pr);
                            Bocagoi.AppendHistory(run);
                        },
                        funcNextState: _ => StateID.Menu)
                },
                {
                    StateID.AddWords,  new MenuState(actionBeforePrint: (pr) =>
                    {
                        var availableBoxes = Bocagoi.ComputeAvailableBoxesString();
                        Console.Write(string.Format(Strings.AddWordsToBox, availableBoxes));

                    }, actionAfterUserInput: (pr, op) =>
                    {
                        Bocagoi.TryOpenBox(op);
                    }, funcNextState: (_, __) => StateID.Menu)
                },
                {
                    StateID.CreateBox,  new MenuState(actionBeforePrint: (pr) =>
                    {
                        var newBoxNumber = Bocagoi.GetAllBoxNames().Count + 1;
                        File.WriteAllText(string.Format(Bocagoi.WordBoxFileName, newBoxNumber), Strings.AddingWordsToBoxExample);
                        Bocagoi.TryOpenBox(newBoxNumber);

                    }, funcNextState: (_, __) => StateID.Menu)
                },
                {
                    StateID.Search,  new SingleActionState(action: (pr) =>
                    {
                        Console.WriteLine(Strings.SearchWords);
                        var str = Console.ReadLine();
                        Console.WriteLine();

                        // TODO: Slow
                        var wordBoxesMap = Bocagoi.LoadAllWords()
                            .Select(pairarray => (pairarray.Key, Words: pairarray.Value
                            .Where(pair => pair.Item1.Contains(str) || pair.Item2.Contains(str))
                            .ToList()));

                        var sb = new StringBuilder();
                        foreach(var (box, wordsFound) in wordBoxesMap)
                        {
                            if (wordsFound.Count == 0)
                                continue;

                            sb.AppendLine(string.Format(Bocagoi.WordBoxFileName, box) + ":");
                            sb.AppendLine();
                            foreach(var word in wordsFound)
                                sb.AppendLine(word.Item1 + " - " + word.Item2);

                            sb.AppendLine();
                        }

                        Console.WriteLine(sb.ToString());
                        Console.WriteLine("Press enter to continue...");
                        Console.ReadLine();

                    }, funcNextState: _ => StateID.Menu)
                },
                {
                    StateID.History,  new SingleActionState(action: (pr) =>
                    {
                        var history = new History();
                        history.LoadFromFile(History.DefaultHistoryFile).ContinueWith(task =>
                        {
                            var entries = string.Join(Environment.NewLine, history.Runs.Select(r => r.ToString()));
                            var str = string.Format(Strings.History, entries);

                            Console.WriteLine(str);
                        });

                        Console.ReadLine();
                    }, funcNextState: _ => StateID.Menu)
                },
            };
        }
    }
}
