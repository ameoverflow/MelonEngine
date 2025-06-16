namespace MelonEngine;

public static class I18N
{
    private static Dictionary<string, string> currentLangStrings = new();

    public static void LoadLanguage(string file)
    {
        foreach (var line in file.Split("\n"))
        {
            string[] translation = line.Split("=", 2);
            currentLangStrings.Add(translation[0], translation[1]);
        }
    }

    public static string GetString(string key) => currentLangStrings[key].Replace("\\n", Environment.NewLine);
}