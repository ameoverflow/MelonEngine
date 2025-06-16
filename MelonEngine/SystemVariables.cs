using System.Reflection;

namespace MelonEngine;

public static class SystemVariables
{
    public static string Branch
    {
        get
        {
            return Assembly.GetExecutingAssembly().GetCustomAttribute<GitInfo>().Branch;
        }
    }

    public static string Version
    {
        get
        {
            Version? version = Assembly.GetExecutingAssembly().GetName().Version;
            return version + "." + Branch;
        }
    }

    public static List<OfficialLevel> Chapters = new()
    {
        new OfficialLevel("levels/map01.tmx", 1)
    };
}

[AttributeUsage(AttributeTargets.Assembly)]
public class GitInfo : Attribute
{
    public string Branch { get; set; }
    public GitInfo(string value)
    {
        Branch = value;
    }
}
