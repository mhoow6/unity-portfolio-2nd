using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DatabaseSystem;
using System;
using Cinemachine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public static bool Initialized;

    // Data
    public static PlayerData PlayerData => Instance.m_PlayerData;
    PlayerData m_PlayerData;

    public static SceneCode SceneCode
    {
        get
        {
            return Instance.m_SceneCode;
        } 
        set
        {
            Instance.m_SceneCode = value;
        }
    }
    SceneCode m_SceneCode;

    // System
    public static UISystem UISystem => Instance.m_UISystem;
    [SerializeField] UISystem m_UISystem;

    EnergyRecoverySystem m_EnergyRecoverySystem;

    public static AchievementSystem AchievementSystem => Instance.m_AchievementSystem;
    public AchievementSystem m_AchievementSystem { get; private set; }

    public static InputSystem InputSystem => Instance.m_InputSystem;
    [SerializeField] InputSystem m_InputSystem;

    // Update Handler
    Action m_Update;
    Action m_FixedUpdate;

    [Header("# ���� ����")]
    [SerializeField] GameDevelopSettings m_GameDevelopSettings;
    public static GameDevelopSettings GameDevelopSettings => Instance.m_GameDevelopSettings;
    [SerializeField] GameSettings m_GameSettings;
    public static GameSettings GameSettings => Instance.m_GameSettings;

    [Header("# ������ �ɼ�")]
    [Rename("���� ����")] public string GameVerison;
    [Rename("�г��� ���� ��ŵ")] public bool AskForNickNameSkip;
    [Rename("���� �� ���̺� ���� ����")] public bool NoAutoSavePlayerData;
    [Rename("�ΰ��� �׽�Ʈ ȯ��")] public bool IsTestZone;
    [Rename("����Ͽ��� �α� �����ֱ�")] public bool DebugLog;

    private void Awake()
    {
        DontDestroyOnLoad(this);
        Instance = this;
        m_Update = null;
        m_FixedUpdate = null;

        // Game Setting
        GameSettings gs = new GameSettings()
        {
            AutoTargeting = true,
            TargetFrameRate = 60
        };
        m_GameSettings = gs;

        Application.targetFrameRate = GameSettings.TargetFrameRate;
        Screen.sleepTimeout = SleepTimeout.SystemSetting;

#if UNITY_EDITOR
        Debug.unityLogger.logEnabled = true;
#else
        Debug.unityLogger.logEnabled = DebugLog ? true : false;
#endif

        // Load Database
        if (!m_GameDevelopSettings)
            m_GameDevelopSettings = Resources.Load<GameDevelopSettings>("GameDevelopSettings");
        m_GameDevelopSettings.SaveFilePath = $"{Application.persistentDataPath}/PlayerData.json";
        GameVerison = GameDevelopSettings.GameVerison;

        // PlayerData
        m_PlayerData = PlayerData.GetData(GameDevelopSettings.SaveFilePath);

        // System Init
        TableManager.Instance.LoadTable();
        JsonManager.Instance.LoadJson();

        if (m_UISystem != null)
            m_UISystem.Init();

        m_EnergyRecoverySystem = new EnergyRecoverySystem();
        m_EnergyRecoverySystem.Init();

        m_AchievementSystem = new AchievementSystem();
        AchievementSystem.Init(JsonManager.Instance);

        if (InputSystem != null)
            InputSystem.Init();

        // Update
        if (m_UISystem != null)
            m_Update += m_UISystem.Tick;
        if (InputSystem != null)
            m_Update += InputSystem.Tick;

        // FixedUpdate
        m_FixedUpdate += m_EnergyRecoverySystem.Tick;

        // PlayerData Init
        AddRewardForNewbie();
        AddDefaultStagePartyPreset();
    }

    private void Start()
    {
        m_UISystem.PushToast<LogoUI>(ToastType.Logo);
    }

    private void FixedUpdate()
    {
        m_FixedUpdate?.Invoke();
    }

    private void Update()
    {
        m_Update?.Invoke();
    }

    void OnApplicationQuit()
    {
        if (!NoAutoSavePlayerData)
            PlayerData.Save();
    }

    #region �÷��̾� ������
    /// <summary> ���� �ý��ۿ� ��ϵ� ���� ����� �÷��̾� �����Ϳ� ������Ʈ </summary>
	public void UpdatePlayerAchievementRecords()
    {
        foreach (var map in m_AchievementSystem.QuestRecords)
        {
            var questRecord = map.Value;
            var exist = PlayerData.QuestRecords.Find(r => r.QuestIdx == questRecord.QuestIdx);
            if (exist == null)
                PlayerData.QuestRecords.Add(questRecord);
            else
            {
                exist.SuccessCount = questRecord.SuccessCount;
                exist.Clear = questRecord.Clear;
            }
        }
    }

    /// <summary> characterCode�� �ش��ϴ� ĳ���͸� ����ϴ�. </summary>
    public void AddPlayerCharacter(ObjectCode characterCode)
    {
        var exist = PlayerData.CharacterDatas.Find(c => c.Code == characterCode);
        if (exist == null)
        {
            var data = TableManager.Instance.CharacterTable.Find(c => c.Code == characterCode);
            var newData = new CharacterData()
            {
                Code = characterCode,
                Hp = data.BaseHp,
                Sp = data.BaseSp,
                Speed = data.BaseSpeed,
                Level = 1,
                Critical = data.BaseCritical,
                Damage = data.BaseDamage,
                Defense = data.BaseDefense,
                EquipWeaponData = new WeaponData()
                {
                    Code = ObjectCode.NONE,
                    Critical = 0,
                    Damage = 0
                }
            };
        }
    }

    /// <summary> characterCode�� �ش��ϴ� ĳ���͸� ���۴ϴ�. </summary>
    public void RemovePlayerCharacter(ObjectCode characterCode)
    {
        var exist = PlayerData.CharacterDatas.Find(c => c.Code == characterCode);
        if (exist != null)
            PlayerData.CharacterDatas.Remove(exist);
    }

    /// <summary> ó������ ���ӿ� ���� �÷��̾�� ������ ���� </summary>
    void AddRewardForNewbie()
    {
        // �⺻ ĳ���� ����
        if (m_PlayerData.CharacterDatas.Find(character => character.Code == ObjectCode.CHAR_Sparcher) == null)
        {
            m_PlayerData.CharacterDatas.Add(new CharacterRecordData()
            {
                Code = ObjectCode.CHAR_Sparcher,
                Level = 1,
                EquipWeaponCode = ObjectCode.NONE,
            });
        }

        // ������ ����
        if (m_PlayerData.LastEnergyUpdateTime == DateTime.MinValue)
            m_EnergyRecoverySystem.AddEnergy(99);

    }

    /// <summary> �������� ��Ƽ ������ �ʱ� ���� </summary>
    void AddDefaultStagePartyPreset()
    {
        // �׽�Ʈ���� ���� ���� ������
        if (m_PlayerData.StageRecords.Find(record => record.WorldIdx == 0 && record.StageIdx == 0) == null)
        {
            m_PlayerData.StageRecords.Add(new StageRecordData()
            {
                WorldIdx = 0,
                StageIdx = 0,
                CharacterLeader = ObjectCode.CHAR_Sparcher,
                CharacterSecond = ObjectCode.NONE,
                CharacterThird = ObjectCode.NONE,
                Clear = false
            });
        }

        // �������� 1-1 �׽�Ʈ �뵵
        if (m_PlayerData.StageRecords.Find(record => record.WorldIdx == 1 && record.StageIdx == 1) == null)
        {
            m_PlayerData.StageRecords.Add(new StageRecordData()
            {
                WorldIdx = 1,
                StageIdx = 1,
                CharacterLeader = ObjectCode.CHAR_Sparcher,
                CharacterSecond = ObjectCode.NONE,
                CharacterThird = ObjectCode.NONE,
                Clear = false
            });
        }
    }
    #endregion

    #region �� �ε�
    public void LoadScene(SceneCode scene, Action onPrevSceneLoading = null, Action onSceneLoading = null, Action onSceneLoaded = null)
    {
        if (scene == SceneCode.Logo)
        {
            Debug.LogError("������ �����ϰ� ���� �ΰ�� �ٽ� ���ư� �� �����ϴ�.");
            return;
        }    

        StartCoroutine(LoadSceneCoroutine(scene, onPrevSceneLoading, onSceneLoading, onSceneLoaded));
    }

    IEnumerator LoadSceneCoroutine(SceneCode scene, Action onPrevSceneLoading = null, Action onSceneLoading = null, Action onSceneLoaded = null)
    {
        AsyncOperation async = SceneManager.LoadSceneAsync((int)scene, LoadSceneMode.Single);

        onPrevSceneLoading?.Invoke();
        while (!async.isDone)
        {
            Debug.Log($"async progress: {async.progress}%\nasync.allowSceneActivation = {async.allowSceneActivation}");

            // ���� �ε�Ǹ�
            if (async.progress >= 0.9f)
            {
                onSceneLoading?.Invoke();
                async.allowSceneActivation = true;
            }
            yield return null;
        }
        onSceneLoaded?.Invoke();
    }
    #endregion

#if UNITY_EDITOR
    [ContextMenu("# Get Attached System")]
    void GetAttachedSystem()
    {
        // UI System
        var uisys = GetComponent<UISystem>();
        if (uisys != null)
        {
            m_UISystem = uisys;
        }

        // InputSystem
        var inputsys = GetComponent<InputSystem>();
        if (inputsys != null)
        {
            m_InputSystem = inputsys;
        }
    }
#endif
}

[Serializable]
public struct GameSettings
{
    public bool AutoTargeting;
    public int TargetFrameRate;
    public bool OneShotKill;
}