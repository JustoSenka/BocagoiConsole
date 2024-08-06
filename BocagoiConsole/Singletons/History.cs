using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BocagoiConsole.Singletons;

public class History
{
    public static void Init() { Instance = new History(); }
    public static History Instance { get; private set; }

    public IList<Run> Runs { get; }

    private const string m_FilePath = "History.json";
    private const string m_FilePathBackup = "History-Backup.json";

    private History()
    {
        if (!File.Exists(m_FilePath))
        {
            using var file = File.Create(m_FilePath);
            file.Close();
            return;
        }

        var text = File.ReadAllText(m_FilePath);
        Runs = JsonConvert.DeserializeObject<IList<Run>>(text) ?? new List<Run>();

        // Backwards compatability with number based box serialization instead of file name based.
        if (int.TryParse(Runs.First().Box, out _))
        {
            text = text.Replace("\"Box\": 1,", "\"Box\": \"Top_Nouns.txt\",");
            text = text.Replace("\"Box\": 2,", "\"Box\": \"Top_Verbs.txt\",");
            text = text.Replace("\"Box\": 3,", "\"Box\": \"Top_Adjectives.txt\",");
            text = text.Replace("\"Box\": 4,", "\"Box\": \"Book_1.txt\",");
            text = text.Replace("\"Box\": 5,", "\"Box\": \"Book_2.txt\",");
            text = text.Replace("\"Box\": 101,", "\"Box\": \"MostFailed\",");
            text = text.Replace("\"Box\": 102,", "\"Box\": \"LeastPracticed\",");

            Runs = JsonConvert.DeserializeObject<IList<Run>>(text) ?? new List<Run>();

            File.Copy(m_FilePath, m_FilePath.Replace(".json", "-old.json"));
            Save();
        }

        // TODO: make sure to not override Runs
    }

    public void Save()
    {
        if (Runs.Count == 0)
            throw new Exception("Not saving empty history file due to safety reasons");

        var text = JsonConvert.SerializeObject(Runs, Formatting.Indented);
        TrySaveBackup(text);

        File.WriteAllText(m_FilePath, text);
    }

    private void TrySaveBackup(string text)
    {
        try
        {
            var oldText = File.Exists(m_FilePathBackup) ? File.ReadAllText(m_FilePathBackup) : string.Empty;

            if (oldText.Length <= text.Length)
                File.WriteAllText(m_FilePathBackup, text);
        }
        catch { }
    }
}

public struct Run
{
    public string Box { get; set; }
    public int Score { get; set; }
    public int From { get; set; }
    public int To { get; set; }
    public DateTime Time { get; set; }
    public TimeSpan Duration { get; set; }
    public PracticeSettings.PracticeMode Mode { get; set; }

    public override string ToString()
    {
        var boxName = Box.Replace(".txt", "");
        return string.Format("{0, -16} | {1, -9} | {2,4} - {3,-4} | {4,3} mins | {5,1} | Score: {6,3}%",
            Time.ToString("yyyy-MM-dd HH:mm"),
            boxName[..Math.Min(boxName.Length, 9)],
            From,
            To,
            (int)Math.Round(Duration.TotalMinutes),
            Mode.ToString()[0],
            Score);
    }
}
