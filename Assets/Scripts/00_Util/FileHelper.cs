using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class FileHelper
{
    public static List<string> GetStringsFromByCSVFormat(string filePath)
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

    public static List<string> GetStringsFrom(string filePath)
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

    public static string GetStringFrom(string filePath)
    {
        string line = string.Empty;
        string lines = string.Empty;

        try
        {
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
        }
        catch (FileNotFoundException)
        {
            Debug.LogError($"{filePath}을 찾을 수 없습니다.");
        }

        return lines;
    }

    public static void WriteFile(string filePath, string content, Action onExceptionCB = null)
    {
        try
        {
            FileStream fs = new FileStream(filePath, FileMode.Create);
            using (StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.UTF8))
            {
                sw.WriteLine(content);
                sw.Close();
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
            onExceptionCB?.Invoke();
        }
    }

    public static void DeleteFile(string filePath)
    {
        File.Delete(filePath);
    }

    public static void WriteSceneData(GameObject parent, StreamWriter streamWriter, bool hasChild = true)
    {
        if (hasChild)
        {
            streamWriter.WriteLine(parent.name);
            for (int i = 0; i < parent.transform.childCount; i++)
            {
                streamWriter.WriteLine(
                    EraseBracketInName(parent.transform.GetChild(i).name) + "," +
                    parent.transform.GetChild(i).position.x + "," +
                    parent.transform.GetChild(i).position.y + "," +
                    parent.transform.GetChild(i).position.z + "," +
                    parent.transform.GetChild(i).rotation.eulerAngles.x + "," +
                    parent.transform.GetChild(i).rotation.eulerAngles.y + "," +
                    parent.transform.GetChild(i).rotation.eulerAngles.z + "," +
                    parent.transform.GetChild(i).localScale.x + "," +
                    parent.transform.GetChild(i).localScale.y + "," +
                    parent.transform.GetChild(i).localScale.z
                    );
            }
        }
        else
        {
            streamWriter.WriteLine(
                    EraseBracketInName(parent.name) + "," +
                    parent.transform.position.x + "," +
                    parent.transform.position.y + "," +
                    parent.transform.position.z + "," +
                    parent.transform.rotation.eulerAngles.x + "," +
                    parent.transform.rotation.eulerAngles.y + "," +
                    parent.transform.rotation.eulerAngles.z + "," +
                    parent.transform.localScale.x + "," +
                    parent.transform.localScale.y + "," +
                    parent.transform.localScale.z
                    );
        }
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

    public static bool DirectoryCheck(string directory, string notFoundMessage)
    {
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
            AssetDatabase.Refresh();
            Debug.LogError($"{notFoundMessage}");
            return false;
        }
        return true;
    }

    public static string EraseBracketInName(string name)
    {
        string mobNameWithNoSpace = name.Replace(" ", "");
        int index = mobNameWithNoSpace.IndexOf('(');

        if (index == -1)
            return name;

        return mobNameWithNoSpace.Remove(index);
    }
}

