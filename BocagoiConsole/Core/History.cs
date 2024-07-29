using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace BocagoiConsole.Core
{
    public class History
    {
        public const string DefaultHistoryFile = "History.json";

        public IList<Run> Runs { get; private set; } = new List<Run>();

        private string m_FilePath;

        public Task LoadFromFile(string filePath)
        {
            m_FilePath = filePath;

            if (File.Exists(filePath))
                return RepeatUntilSuccess(() => LoadFromJson(File.ReadAllText(filePath)), 5);

            var file = File.Create(filePath);
            file.Close();
            return Task.CompletedTask;
        }

        public void LoadFromJson(string jsonContents)
        {
            Runs = JsonConvert.DeserializeObject<IList<Run>>(jsonContents) ?? Runs;
        }

        public Task Save()
        {
            if (string.IsNullOrEmpty(m_FilePath))
                throw new Exception("Cannot save history to a file if it was not loaded from one");

            return Save(m_FilePath);
        }

        private Task Save(string filePath)
        {
            if (Runs.Count == 0)
                throw new Exception("Not saving empty history file due to safety reasons");

            var str = JsonConvert.SerializeObject(Runs);
            m_FilePath = filePath;

            return RepeatUntilSuccess(() => File.WriteAllText(filePath, str), 5);
        }

        private static Task RepeatUntilSuccess(Action ac, int timesToRepeat)
        {
            return Task.Run(() =>
            {
                for (int i = 1; i <= timesToRepeat; ++i)
                {
                    try
                    {
                        ac.Invoke();
                        break;
                    }
                    catch when (i <= timesToRepeat)
                    {
                        Task.Delay(50);
                    }
                }
            });
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
            return string.Format("{0, -17} | WBox{1} | From: {2,4}, To: {3,4} | Duration: {4,3} mins | Mode: {5,7} | Score: {6,3}",
                Time.ToString("yyyy-MM-dd HH:mm"),
                Box,
                From,
                To,
                (int)Duration.TotalMinutes,
                Mode.ToString(),
                Score);
        }
    }
}
