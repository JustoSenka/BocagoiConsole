using BocagoiConsole.States;
using System;
using System.Collections.Generic;
using System.IO;

namespace BocagoiConsole.Core
{
    public class StateMachine
    {
        public Dictionary<State, BaseState> Generate(Bocagoi bocagoi)
        {
            return new Dictionary<State, BaseState>
            {
                {
                    State.Menu,  new MenuState(
                        textToPrint: Strings.Menu,
                        transitions: new Dictionary<int, State>
                    {
                        {1,  State.PracticeSelectBox },
                        {2,  State.AddWords },
                        {3,  State.CreateBox},
                        {4,  State.History},
                        {5,  State.Settings},
                        {0,  State.Exit },
                    })
                },
                {
                    State.PracticeSelectBox,  new MenuState(actionBeforePrint: (pr) =>
                    {
                        var availableBoxes = Bocagoi.ComputeAvailableBoxesString();
                        Console.Write(string.Format(Strings.PracticeSelectBox, availableBoxes));

                    }, actionAfterUserInput: (pr, op) =>
                    {
                        pr.Box = op;

                    }, transitions: new Dictionary<int, State>
                    {
                        {0,  State.Exit },
                    },
                        funcNextState: (_, __) => State.PracticeSelectWords)
                },
                {
                    State.PracticeSelectWords,  new SingleActionState(action: (pr) =>
                    {
                        var wordCount = File.ReadAllLines(string.Format(Bocagoi.WordBoxFileName, pr.Box)).Length;
                        Console.Write(string.Format(Strings.PracticeSelectWords1, wordCount));

                        Bocagoi.GetDictionaryBoundsForPracticing(wordCount, out int from, out int to);

                        pr.WordsMax = to;
                        pr.WordsMin = from;

                    }, funcNextState: _ => State.PracticeSelectMode)
                },
                {
                    State.PracticeSelectMode,  new MenuState(textToPrint: Strings.PracticeSelectMode,
                        actionAfterUserInput: (pr, op) =>
                    {
                        pr.Mode = op == 1 ? PracticeSettings.PracticeMode.Normal : PracticeSettings.PracticeMode.Reverse;

                    }, transitions: new Dictionary<int, State>
                    {
                        {1,  State.RunPractice },
                        {2,  State.RunPractice },
                        {0,  State.Exit },
                    })
                },
                {
                    State.RunPractice,  new SingleActionState(
                        action: Bocagoi.RunGame,
                        funcNextState: _ => State.Menu)
                },
                {
                    State.AddWords,  new MenuState(actionBeforePrint: (pr) =>
                    {
                        var availableBoxes = Bocagoi.ComputeAvailableBoxesString();
                        Console.Write(string.Format(Strings.AddWordsToBox, availableBoxes));

                    }, actionAfterUserInput: (pr, op) =>
                    {
                        try
                        {
                            Bocagoi.OpenBox(op);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                        }

                    }, funcNextState: (_, __) => State.Menu)
                },
            };
        }
    }
}
