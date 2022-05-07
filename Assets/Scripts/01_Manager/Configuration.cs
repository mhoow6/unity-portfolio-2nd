using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ScriptableObject", menuName = "Configuration", order = 1)]
public sealed class Configuration : ScriptableObject
{
    public int DownloadDataCount => JsonDataCount + CSVDataCount;
    [Header("자동 기입")]
    public int CSVDataCount;
    public int JsonDataCount;
    public string SaveFilePath;
    [Header("수동 기입")]
    public string GameVerison;
    public string MaterialResourcePath;
    public string TextureResourcePath;
    public string TerrainResourcePath;
    public string CharacterResourcePath;
    public string InteractableResourcePath;
    public string UIResourcePath;
    public string ProjectileResourcePath;
    public string EffectResourcePath;
    public string AnimationControllerResourcePath;
    public string DatabasePath;
}
