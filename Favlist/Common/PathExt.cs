using System;
using System.IO;

namespace Favlist.Common;

internal static class PathExt
{
    public static string UniqueCombine(string path1, string path2) => 
        GetUniqueFilePath(Path.Combine(path1, path2));
    
    public static string GetUniqueFilePath(string fullPath)
    {
        string result = fullPath;
        string directoryName = Path.GetDirectoryName(fullPath) ?? throw new ArgumentException("Invalid path");
        string fileName = Path.GetFileNameWithoutExtension(fullPath);
        string extension = Path.GetExtension(fullPath);
        int count = 2;
        while (File.Exists(result))
        {
            result = $"{Path.Combine(directoryName, fileName)} ({count++}){extension}";
        }
        return result;
    } 
}