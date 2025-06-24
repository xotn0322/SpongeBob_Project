using System.IO;
using System;
using UnityEngine;

public class IOUtil
{
    public static string CombinePath(string path1, string path2)
    {
        return Path.Combine(path1, path2);
    }
    public static bool DirectoryExist(string directoryPath)
{
    try
    {
        return Directory.Exists(directoryPath);
    }
    catch (Exception e)
    {
        Debug.LogError($"IoUtil : DirectoryExist failed. filePath={directoryPath}. e={e}");
    }

    return false;
}
}