using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BocagoiConsole.Files
{
    public class RedBox
    {
        public const string RedBoxFile = "RedBox.txt";

        public IDictionary<string, Word> Words { get; private set; } = new Dictionary<string, Word>();

        private string m_FilePath;

        public void Fail((string, string) word)
        {
            if (!Words.ContainsKey(word.Item1))
                Words[word.Item1] = new Word(word);

            Words[word.Item1].Fails++;
            Words[word.Item1].Correct--;
        }

        public void Succeed((string, string) word)
        {
            if (!Words.ContainsKey(word.Item1))
                Words[word.Item1] = new Word(word);

            Words[word.Item1].Correct++;
        }

        public Task LoadFromFile(string filePath)
        {
            m_FilePath = filePath;

            if (File.Exists(filePath))
                return RepeatUntilSuccess(() => LoadFromText(File.ReadAllLines(filePath)), 5);

            using var file = File.Create(filePath);
            file.Close();
            return Task.CompletedTask;
        }

        private void LoadFromText(string[] lines)
        {
            Words.Clear();
            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                var word = new Word(line);
                Words[word.Left] = word;
            }
        }

        public Task Save()
        {
            if (string.IsNullOrEmpty(m_FilePath))
                throw new Exception("Cannot save Red Box to a file if it was not loaded from one");

            return Save(m_FilePath);
        }

        private Task Save(string filePath)
        {
            if (Words.Count == 0)
                throw new Exception("Not saving empty Red Box file due to safety reasons");

            m_FilePath = filePath;
            var lines = Words.Values.Select(word => word.ToString());

            return RepeatUntilSuccess(() => File.WriteAllLines(filePath, lines), 5);
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

    public class Word
    {
        public string Left { get; set; }
        public string Right { get; set; }
        public int Fails { get; set; }
        public int Correct { get; set; }

        public Word((string, string) pair)
        {
            Left = pair.Item1;
            Right = pair.Item2;
            Fails = 0;
            Correct = 0;
        }

        public Word(string line)
        {
            var parts = line.Split('-');
            Left = parts[0].Trim();
            Right = parts[1].Trim();
            Fails = int.Parse(parts[2].Trim());
            Correct = int.Parse(parts[3].Trim());
        }

        public override string ToString()
        {
            return $"{Left} - {Right} - {Fails} - {Correct}";
        }
    }
}
