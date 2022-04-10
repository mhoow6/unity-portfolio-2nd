using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ScriptableObject", menuName = "Configuration", order = 1)]
public class Configuration : ScriptableObject
{
    public int DownloadDataCount;
    public string SaveFilePath;
    public float GameVerison;
}
