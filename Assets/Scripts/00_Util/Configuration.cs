using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ScriptableObject", menuName = "Configuration", order = 1)]
public class Configuration : ScriptableObject
{
    public int DownloadDataCount;
    public string SaveFilePath;
    public int EnergyRecoveryMinute = 10;
    public float UIScaleTweeningSpeed = 0.2f;
}
