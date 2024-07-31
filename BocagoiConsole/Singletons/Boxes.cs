using BocagoiConsole.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BocagoiConsole.Singletons;

public class Boxes
{
    public static void Init() { Instance = new Boxes(); }
    public static Boxes Instance { get; private set; }

    private IDictionary<int, List<(string Left, string Right)>> Words { get; }

    public string GetBoxName(int index) => string.Format("WBox{0}.txt", index);

    private Boxes()
    {
        CreateBoxIfNotExist(1);
        CreateBoxIfNotExist(2);

        Words = new Dictionary<int, List<(string, string)>>();
        ReloadWords();
    }

    public IDictionary<int, List<(string Left, string Right)>> GetAllWords() => Words;

    public List<(string Left, string Right)> GetWords(int boxIndex)
    {
        if (boxIndex == 101) // Most failed
            return RedBox.Instance.Words.Values
                .OrderByDescending(word => word.Fails - word.Correct)
                .Select(word => (word.Left, word.Right))
                .ToList();

        if (boxIndex == 102) // Least practiced
            return RedBox.Instance.Words.Values
                .OrderBy(word => word.Correct + word.Fails)
                .Select(word => (word.Left, word.Right))
                .ToList();

        return Words[boxIndex];
    }

    private void CreateBoxIfNotExist(int index)
    {
        var fileName = GetBoxName(index);
        if (!File.Exists(fileName))
            File.Create(fileName);
    }

    private void ReloadWords()
    {
        Words.Clear();

        int i = 1;
        foreach (var boxName in GetAllBoxNames())
        {
            var pairs = File.ReadAllLines(boxName)
                .Where(line => !string.IsNullOrWhiteSpace(line)) // Skipping whitespace
                .Select(line => line.Trim(' ', '\r', '\t').Split('_', '-', '–'))
                .Where(words => words.Length > 1)
                .Select(words => (words[0].Trim(), words[1].Trim()))
                .ToList();

            Words[i] = pairs;
            i++;
        }
    }

    public IEnumerable<string> GetAllBoxNames()
    {
        int i = 1;
        var boxName = GetBoxName(i);

        while (File.Exists(boxName))
        {
            yield return boxName;
            boxName = GetBoxName(++i);
        }
    }

    public int CreateNewBox()
    {
        var index = GetAllBoxNames().Count() + 1;
        File.WriteAllText(GetBoxName(index), Strings.AddingWordsToBoxExample);
        ReloadWords();
        return index;
    }

    public string BuildBoxNameListWithWordCount()
        => string.Join(Environment.NewLine, Words.Keys.Select(index => $"{index}. {GetBoxName(index)} ({Words[index].Count})"));

    public string BuildBoxNameList()
        => string.Join(Environment.NewLine, Words.Keys.Select(index => $"{index}. {GetBoxName(index)}"));
}
