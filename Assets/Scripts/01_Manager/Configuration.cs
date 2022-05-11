using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ScriptableObject", menuName = "Configuration", order = 1)]
public sealed class Configuration : ScriptableObject
{
    public int DownloadDataCount => JsonDataCount + CSVDataCount;
    [Header("# 자동 기입")]
    public int CSVDataCount;
    public int JsonDataCount;
    public string SaveFilePath;
    
    public string GameVerison => m_GameVerison;
    [Header("# 수동 기입")]
    [SerializeField] string m_GameVerison;
    public string MaterialResourcePath => m_MaterialResourcePath;
    [SerializeField] string m_MaterialResourcePath;
    public string TextureResourcePath => m_TextureResourcePath;
    [SerializeField] string m_TextureResourcePath;
    public string TerrainResourcePath => m_TerrainResourcePath;
    [SerializeField] string m_TerrainResourcePath;
    public string CharacterResourcePath => m_CharacterResourcePath;
    [SerializeField] string m_CharacterResourcePath;
    public string InteractableResourcePath => m_InteractableResourcePath;
    [SerializeField] string m_InteractableResourcePath;
    public string UIResourcePath => m_UIResourcePath;
    [SerializeField] string m_UIResourcePath;
    public string ProjectileResourcePath => m_ProjectileResourcePath;
    [SerializeField] string m_ProjectileResourcePath;
    public string EffectResourcePath => m_EffectResourcePath;
    [SerializeField] string m_EffectResourcePath;
    public string AnimationControllerResourcePath => m_AnimationControllerResourcePath;
    [SerializeField] string m_AnimationControllerResourcePath;
    public string DatabasePath => m_DatabasePath;
    [SerializeField] string m_DatabasePath;
    public string CinemachineBlendSettingsPath => m_CinemachineBlendSettingsPath;
    [SerializeField] string m_CinemachineBlendSettingsPath;
}
