using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class FileHelper
{
    public static List<string> GetLinesFromTableTextAsset(string filePath)
    {
        TextAsset txtAsset = Resources.Load<TextAsset>(filePath);
        if (txtAsset == null)
            return null;

        char[] option = { '\r', '\n' };
        string[] _lines = txtAsset.text.Split(option);
        List<string> lines = new List<string>();

        foreach (string line in _lines)
        {
            if (string.IsNullOrEmpty(line))
                continue;

            lines.Add(line);
        }

        return lines;
    }

    public static List<string> GetLinesFromTableFileStream(string filePath)
    {
        string line = string.Empty;
        List<string> lines = new List<string>();

        using (FileStream f = new FileStream(filePath, FileMode.Open, FileAccess.Read))
        {
            using (StreamReader sr = new StreamReader(f, System.Text.Encoding.UTF8))
            {
                while ((line = sr.ReadLine()) != null)
                {
                    lines.Add(line);
                }
            }
        }

        return lines;
    }

    public static string GetLinesWithFileStream(string filePath)
    {
        string line = string.Empty;
        string lines = string.Empty;

        using (FileStream f = new FileStream(filePath, FileMode.Open, FileAccess.Read))
        {
            using (StreamReader sr = new StreamReader(f, System.Text.Encoding.UTF8))
            {
                while ((line = sr.ReadLine()) != null)
                {
                    lines += line;
                }
            }
        }

        return lines;
    }

    public static bool MakeDirectory(string directory)
    {
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
            return true;
        }
        return false;
    }

    public static void DirectoryCheck(string directory, string notFoundMessage)
    {
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
            AssetDatabase.Refresh();
            throw new DirectoryNotFoundException(notFoundMessage);
        }
    }
}
