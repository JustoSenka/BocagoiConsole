using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BocagoiConsole.Singletons
{
    public class Boxes
    {
        public static void Init() { Instance = new Boxes(); }
        public static Boxes Instance { get; private set; }

        public IDictionary<int, List<(string Left, string Right)>> Words { get; }

        public string GetBoxName(int index) => string.Format("WBox{0}.txt", index);

        private Boxes()
        {
            CreateBoxIfNotExist(1);
            CreateBoxIfNotExist(2);

            Words = LoadWords();
        }

        private void CreateBoxIfNotExist(int index)
        {
            var fileName = GetBoxName(index);
            if (!File.Exists(fileName))
                File.Create(fileName);
        }

        private IDictionary<int, List<(string, string)>> LoadWords()
        {
            var words = new Dictionary<int, List<(string, string)>>();

            int i = 1;
            foreach (var boxName in GetAllBoxNames())
            {
                var pairs = File.ReadAllLines(boxName)
                    .Where(line => !string.IsNullOrWhiteSpace(line)) // Skipping whitespace
                    .Select(line => line.Trim(' ', '\r', '\t').Split('_', '-', '–'))
                    .Where(words => words.Length > 1)
                    .Select(words => (words[0].Trim(), words[1].Trim()))
                    .ToList();

                words[i] = pairs;
                i++;
            }

            return words;
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

        public override string ToString()
        {
            var sb = new StringBuilder();
            int i = 1;
            foreach (var boxName in GetAllBoxNames())
                sb.AppendLine($"{i++}. {boxName}");

            return sb.ToString();
        }
    }
}
