using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ScriptableObject", menuName = "Configuration", order = 1)]
public sealed class Configuration : ScriptableObject
{
    public int DownloadDataCount => JsonDataCount + CSVDataCount;
    public int CSVDataCount;
    public int JsonDataCount;
    public string SaveFilePath;
    public string GameVerison;
}
