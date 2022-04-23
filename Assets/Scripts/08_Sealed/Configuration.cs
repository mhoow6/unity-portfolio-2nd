using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ScriptableObject", menuName = "Configuration", order = 1)]
public sealed class Configuration : ScriptableObject
{
    public int DownloadDataCount;
    public string SaveFilePath;
    public string GameVerison;
    public int JsonDataCount;
}
