using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Game Development Settings", menuName = "Game Development Settings", order = 1)]
public sealed class GameDevelopSettings : ScriptableObject
{
    public int DownloadDataCount => JsonDataCount + CSVDataCount;
    [Header("# 자동 기입")]
    public int CSVDataCount;
    public int JsonDataCount;
    public string SaveFilePath;
    
    public string GameVerison => m_GameVerison;

    // ----------------------------------------------------------------------

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
    public string SoundPath => m_SoundPath;
    [SerializeField] string m_SoundPath;

    // ----------------------------------------------------------------------

    public int UILayermask => m_UILayermask;
    [SerializeField] int m_UILayermask;
    public int TerrainLayermask => m_TerrainLayermask;
    [SerializeField] int m_TerrainLayermask;
    public int BaseObjectLayermask => m_BaseObjectLayermask;
    [SerializeField] int m_BaseObjectLayermask;
    public int PostProcessLayermask => m_PostProcessLayermask;
    [SerializeField] int m_PostProcessLayermask;

    // ----------------------------------------------------------------------

    public List<ObjectCode> NotImplementedCharacters => m_NotImplementedCharacters;
    [SerializeField] List<ObjectCode> m_NotImplementedCharacters;
}
