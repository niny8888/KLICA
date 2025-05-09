using System.IO;
using System.Text.Json;

public static class SaveManager
{
    private static string SaveFilePath => "savegame.json";

    public static void Save(GameData data)
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = true
        };
        options.Converters.Add(new Vector2Converter());

        var json = JsonSerializer.Serialize(data, options);
        File.WriteAllText(SaveFilePath, json);
    }


    public static GameData Load()
    {
        if (!File.Exists(SaveFilePath)) return null;

        var options = new JsonSerializerOptions();
        options.Converters.Add(new Vector2Converter());

        var json = File.ReadAllText(SaveFilePath);
        return JsonSerializer.Deserialize<GameData>(json, options);
    }

    public static void Reset()
    {
        var defaultData = new GameData(); // all defaults
        Save(defaultData);
    }

}
