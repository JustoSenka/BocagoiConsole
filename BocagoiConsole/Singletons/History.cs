using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace BocagoiConsole.Singletons;

public class History
{
    public static void Init() { Instance = new History(); }
    public static History Instance { get; private set; } 

    public IList<Run> Runs { get; }

    private const string m_FilePath = "History.json";

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
        // TODO: make sure to not override Runs
    }

    public void Save()
    {
        if (Runs.Count == 0)
            throw new Exception("Not saving empty history file due to safety reasons");

        var text = JsonConvert.SerializeObject(Runs);
        File.WriteAllText(m_FilePath, text);
    }
}

public struct Run
{
    public int Box { get; set; }
    public int Score { get; set; }
    public int From { get; set; }
    public int To { get; set; }
    public DateTime Time { get; set; }
    public TimeSpan Duration { get; set; }
    public PracticeSettings.PracticeMode Mode { get; set; }

    public override string ToString()
    {
        return string.Format("{0, -17} | WBox{1, -3} | From: {2,4}, To: {3,4} | Duration: {4,3} mins | Mode: {5,7} | Score: {6,3}%",
            Time.ToString("yyyy-MM-dd HH:mm"),
            Box,
            From,
            To,
            (int)Duration.TotalMinutes,
            Mode.ToString(),
            Score);
    }
}
