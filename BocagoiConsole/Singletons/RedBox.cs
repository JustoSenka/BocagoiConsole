using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BocagoiConsole.Singletons;

public class RedBox
{
    public static void Init() { Instance = new RedBox(); }
    public static RedBox Instance { get; private set; }

    public IDictionary<string, Word> Words { get; }

    private const string m_FilePath = "RedBox.txt";
    private const string m_FilePathBackup = "RedBox-Backup.txt";

    public static bool ForceSaveAfterMigration = false;

    private RedBox()
    {
        if (!File.Exists(m_FilePath))
        {
            using var file = File.Create(m_FilePath);
            file.Close();
            return;
        }

        Words = new Dictionary<string, Word>();
        foreach (var line in File.ReadAllLines(m_FilePath))
        {
            if (string.IsNullOrWhiteSpace(line))
                continue;

            var word = new Word(line);
            Words[word.Left] = word;
        }

        if (ForceSaveAfterMigration)
            Save();
    }

    public void Fail((string, string) word)
    {
        if (!Words.ContainsKey(word.Item1))
            Words[word.Item1] = new Word(word);

        Words[word.Item1].Fails++;
        Words[word.Item1].Correct--;

        Words[word.Item1].LastPracticed = DateOnly.FromDateTime(DateTime.Now);
    }

    public void Succeed((string, string) word)
    {
        if (!Words.ContainsKey(word.Item1))
            Words[word.Item1] = new Word(word);

        Words[word.Item1].Correct++;

        Words[word.Item1].LastPracticed = DateOnly.FromDateTime(DateTime.Now);
    }

    public void Save()
    {
        if (Words.Count == 0)
            throw new Exception("Not saving empty Red Box file due to safety reasons");

        var lines = Words.Values.Select(word => word.ToString());
        TrySaveBackup(lines);

        File.WriteAllLines(m_FilePath, lines);
    }

    private void TrySaveBackup(IEnumerable<string> lines)
    {
        try
        {
            var oldLines = File.Exists(m_FilePathBackup) ? File.ReadAllLines(m_FilePathBackup) : Array.Empty<string>();

            if (oldLines.Count() <= lines.Count())
                File.WriteAllLines(m_FilePathBackup, lines);
        }
        catch { }
    }
}

public class Word
{
    public string Left { get; set; }
    public string Right { get; set; }
    public int Fails { get; set; }
    public int Correct { get; set; }
    public DateOnly LastPracticed { get; set; }

    /// Gives score between 0-100, 100 means no failures, 0 means failed every time practiced or never practiced
    public int SuccessScore => Correct + Fails == 0 ? 0 : (int) Math.Round(Correct * 100f / (Correct + Fails));

    public Word((string, string) pair)
    {
        Left = pair.Item1;
        Right = pair.Item2;
        Fails = 0;
        Correct = 0;
        LastPracticed = DateOnly.FromDateTime(DateTime.Now);
    }

    public Word(string line)
    {
        var parts = line.Split('-');
        Left = parts[0].Trim();
        Right = parts[1].Trim();
        Fails = int.Parse(parts[2].Trim());
        Correct = int.Parse(parts[3].Trim());

        // TODO: backwards compatibility to convert from old format. Delete once not needed
        if (parts.Length < 5)
        {
            RedBox.ForceSaveAfterMigration = true;

            // This can happen if words were deleted in boxes but not in red box. In that case we do not care about timestamp
            if (!Boxes.Instance.WordToBoxIndex.ContainsKey(Left))
            {
                LastPracticed = DateOnly.FromDateTime(DateTime.Now).AddDays(-30);
                return;
            }

            var boxes = Boxes.Instance.WordToBoxIndex[Left];

            var lastRunForThisWord = History.Instance.Runs.Reverse()
                .Select(run => (run, box: Boxes.Instance.BoxList.Values.FirstOrDefault(box => box.Name.Trim() == run.Box)))
                .Where(tuple => tuple.box != default)
                .FirstOrDefault(tuple => boxes.Contains(tuple.box.Index)).run;

            if (string.IsNullOrEmpty(lastRunForThisWord.Box))
            {
                LastPracticed = DateOnly.FromDateTime(DateTime.Now).AddDays(-30);
                return;
            }

            LastPracticed = DateOnly.FromDateTime(lastRunForThisWord.Time);
        }

        else
        {
            LastPracticed = DateOnly.ParseExact(parts[4].Trim(), "yyyy_MM_dd");
        }
    }

    public override string ToString()
    {
        return $"{Left} - {Right} - {Fails} - {Correct} - {LastPracticed:yyyy_MM_dd}";
    }
}
