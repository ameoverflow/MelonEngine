using System.Runtime.InteropServices;

namespace MelonEngine;

public static class PathDetector
{
    public static bool IsPathAbsolute(string path)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            //system is windows, then check if 2nd and 3rd character are :\
            return path[1] == ':' && path[2] == '\\';
        }
        else
        {
            //system is unix like so check if path starts with /
            return path[0] == '/';
        }
    }
}