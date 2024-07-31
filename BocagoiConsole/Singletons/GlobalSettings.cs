using Newtonsoft.Json;
using System.IO;

namespace BocagoiConsole.Singletons;

public class GlobalSettings
{
    public static void Init() { Instance = new GlobalSettings(); }
    public static GlobalSettings Instance { get; private set; }

    public DataInfo Data { get; }

    private const string m_FilePath = "Settings.json";

    private GlobalSettings()
    {
        if (!File.Exists(m_FilePath))
        {
            Data = new DataInfo();
            Save();
            return;
        }

        var text = File.ReadAllText(m_FilePath);
        Data = JsonConvert.DeserializeObject<DataInfo>(text) ?? new DataInfo();
    }

    public void Save()
    {
        var text = JsonConvert.SerializeObject(Data, Formatting.Indented);
        File.WriteAllText(m_FilePath, text);
    }

    public class DataInfo
    {
        public bool UseDoubleSpaceConsole { get; set; } = true;
        public bool RememberConsoleFontSize { get; set; } = true;
    }
}
