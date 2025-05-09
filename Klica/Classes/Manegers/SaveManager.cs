using System.IO;
using System.Text.Json;

public static class SaveManager
{
    private static string SaveFilePath => "savegame.json";

    public static void Save(GameData data)
    {
        var json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(SaveFilePath, json);
    }

    public static GameData Load()
    {
        if (!File.Exists(SaveFilePath)) return null;
        var json = File.ReadAllText(SaveFilePath);
        return JsonSerializer.Deserialize<GameData>(json);
    }
}
