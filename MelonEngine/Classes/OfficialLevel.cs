namespace MelonEngine;

public class OfficialLevel
{
    public int ChapterNumber { get; set; }
    public string Filename { get; set; }

    public OfficialLevel(string filename, int chapterNumber)
    {
        ChapterNumber = chapterNumber;
        Filename = filename;
    }
}
