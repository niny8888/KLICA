using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;


public static class SaveManager
{
    // private static string SaveFilePath => "savegame.json";

    private static string SaveFilePath
    {
        get
        {
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string saveDir = Path.Combine(appData, "Klica"); // Use your game's name here
            Directory.CreateDirectory(saveDir); // Ensures the directory exists
            return Path.Combine(saveDir, "savegame.json");
        }
    }

    public static void Save(GameData data)
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            Converters = { new JsonStringEnumConverter(), new Vector2Converter() }
        };

        var json = JsonSerializer.Serialize(data, options);
        File.WriteAllText(SaveFilePath, json);
    }


    public static GameData Load()
    {
        var options = new JsonSerializerOptions
        {
            Converters = { new JsonStringEnumConverter() }
        };

        // If the file doesn't exist, create it with default values
        if (!File.Exists(SaveFilePath))
        {
            var defaultData = new GameData(); // Default: level 1, empty traits
            Save(defaultData);               // Create the file
            return defaultData;
        }

        var json = File.ReadAllText(SaveFilePath);
        return JsonSerializer.Deserialize<GameData>(json, options);
    }


    public static void Reset()
    {
        var defaultData = new GameData(); // all defaults
        Save(defaultData);
    }

}
