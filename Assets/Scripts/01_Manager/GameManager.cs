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
    
    // Setting
    public static bool Initialized;
    public bool AutoTargeting;

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

    // Scriptable Object
    [Header("# ��ũ���ͺ� ������Ʈ")]
    [SerializeField] GameDevelopSettings m_GameDevelopSettings;
    public static GameDevelopSettings GameDevelopSettings => Instance.m_GameDevelopSettings;

    [Header("# ������ �ɼ�")]
    [Rename("���� ����")] public string GameVerison;
    [Rename("Ÿ��Ʋ �ε� ��ŵ")] public bool TitleLoadingDirectingSkip;
    [Rename("�г��� ���� ��ŵ")] public bool AskForNickNameSkip;
    [Rename("���� �� ���̺� ���� ����")] public bool NoAutoSavePlayerData;
    [Rename("�ΰ��� �׽�Ʈ ȯ��")] public bool IsTestZone;

    private void Awake()
    {
        DontDestroyOnLoad(this);
        Instance = this;
        m_Update = null;
        m_FixedUpdate = null;

        // ��ũ���ͺ� ������Ʈ �ε�
        if (!m_GameDevelopSettings)
            m_GameDevelopSettings = Resources.Load<GameDevelopSettings>("GameDevelopSettings");
        m_GameDevelopSettings.SaveFilePath = $"{Application.persistentDataPath}/PlayerData.json";
        m_PlayerData = PlayerData.GetData(GameDevelopSettings.SaveFilePath);
        GameVerison = GameDevelopSettings.GameVerison;

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

        // Game Setting
        Application.targetFrameRate = 60;

        // Update
        if (m_UISystem != null)
            m_Update += m_UISystem.Tick;
        if (InputSystem != null)
            m_Update += InputSystem.Tick;

        // FixedUpdate
        m_FixedUpdate += m_EnergyRecoverySystem.Tick;
    }

    private void Start()
    {
        m_UISystem.OpenToast<LogoUI>(ToastType.Logo).Initalize = true;
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
}