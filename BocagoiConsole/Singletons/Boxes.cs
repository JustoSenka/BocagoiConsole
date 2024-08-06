using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BocagoiConsole.Singletons;

public class Boxes
{
    public static void Init() { Instance = new Boxes(); }
    public static Boxes Instance { get; private set; }

    public IDictionary<int, Box> BoxList { get; }

    public const string ResourceDir = "Resources";

    private Boxes()
    {
        BoxList = new Dictionary<int, Box>();
        ReloadWords();
    }

    private void ReloadWords()
    {
        BoxList.Clear();

        int i = 1;
        foreach (var boxName in GetFilesInResourceFolder())
        {
            var pairs = File.ReadAllLines($"{ResourceDir}/{boxName}")
                .Where(line => !string.IsNullOrWhiteSpace(line)) // Skipping whitespace
                .Select(line => line.Trim(' ', '\r', '\t').Split('_', '-', '–'))
                .Where(words => words.Length > 1)
                .Select(words => (words[0].Trim(), words[1].Trim()))
                .ToList();

            BoxList[i] = new Box
            {
                Index = i,
                Name = boxName,
                Words = pairs
            };

            i++;
        }
    }

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

        return BoxList[boxIndex].Words;
    }

    public string GetBoxName(int boxIndex)
    {
        if (boxIndex == 101)
            return "MostFailed";

        if (boxIndex == 102)
            return "LeastPracticed";

        return BoxList[boxIndex].Name;
    }

    public IList<string> GetFilesInResourceFolder()
    {
        var resourcesDir = new DirectoryInfo(ResourceDir);
        return resourcesDir.GetFiles("*.txt", SearchOption.TopDirectoryOnly)
            .Select(file => file.Name)
            .OrderBy(s => s)
            .ToList();
    }

    public string BuildBoxNameListWithWordCount()
        => string.Join(Environment.NewLine, BoxList.Keys.Select(index => $"{index}. {BoxList[index].Name} ({BoxList[index].Words.Count})"));

    public string BuildBoxNameList()
        => string.Join(Environment.NewLine, BoxList.Keys.Select(index => $"{index}. {BoxList[index].Name}"));
}

public class Box
{
    public int Index { get; set; }
    public string Name { get; set; }
    public string Path => $"{Boxes.ResourceDir}/{Name}";
    public List<(string Left, string Right)> Words { get; set; }
}
