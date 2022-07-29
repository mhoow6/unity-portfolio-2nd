using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DatabaseSystem;
using System;
using Cinemachine;
using UnityEngine.SceneManagement;

public sealed class GameManager : MonoBehaviour
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

    public static EnergyRecoverySystem EnergyRecoverySystem => Instance.m_EnergyRecoverySystem;
    EnergyRecoverySystem m_EnergyRecoverySystem;

    public static AchievementSystem AchievementSystem => Instance.m_AchievementSystem;
    public AchievementSystem m_AchievementSystem { get; private set; }

    public static InputSystem InputSystem => Instance.m_InputSystem;
    [SerializeField] InputSystem m_InputSystem;

    DataInitializeSystem m_DataInitializeSystem;

    // Update Handler
    Action m_CoreSystemUpdate;
    Action m_ContentSystemUpdate;

    [Header("# ���� ����")]
    [SerializeField] GameDevelopSettings m_GameDevelopSettings;
    public static GameDevelopSettings GameDevelopSettings => Instance.m_GameDevelopSettings;

    public static GameGlobalData GlobalData => Instance.m_GameGlobalData;
    [SerializeField] GameGlobalData m_GameGlobalData;

    [SerializeField] GameSettings m_GameSettings;
    public static GameSettings GameSettings => Instance.m_GameSettings;

    [SerializeField] CheatSettings m_CheatSettings;
    public static CheatSettings CheatSettings => Instance.m_CheatSettings;

    [Header("# ������ �ɼ�")]
    [Rename("���� ����")] public string GameVerison;
    [Rename("�г��� ���� ��ŵ")] public bool AskForNickNameSkip;
    [Rename("���� �� ���̺� ���� ����")] public bool NoAutoSavePlayerData;
    [Rename("�ΰ��� �׽�Ʈ ȯ��")] public bool IsTestZone;
    [Rename("����Ͽ��� �α� �����ֱ�")] public bool DebugLog;
    [Rename("ġƮ Ȱ��ȭ")] public bool EnableCheat;

    private void Awake()
    {
        DontDestroyOnLoad(this);
        Instance = this;
        m_CoreSystemUpdate = null;
        m_ContentSystemUpdate = null;

        SceneManager.sceneUnloaded += (Scene arg0) =>
        {
            GameEventSystem.Clear();
        };

        // ---------------------------------------------------

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
        if (EnableCheat == false)
            m_CheatSettings = new CheatSettings();
#endif

        // ---------------------------------------------------

        // Scriptable Object
        if (!m_GameDevelopSettings)
            m_GameDevelopSettings = Resources.Load<GameDevelopSettings>("GameDevelopSettings");
        m_GameDevelopSettings.SaveFilePath = $"{Application.persistentDataPath}/PlayerData.json";
        GameVerison = GameDevelopSettings.GameVerison;

        // ---------------------------------------------------

        if (m_UISystem != null)
            m_UISystem.Init();

        if (InputSystem != null)
            InputSystem.Init();

        // ---------------------------------------------------

        // Update
        if (m_UISystem != null)
            m_CoreSystemUpdate += m_UISystem.Tick;
        if (InputSystem != null)
            m_CoreSystemUpdate += InputSystem.Tick;

        // ---------------------------------------------------

    }

    private void Start()
    {
        m_UISystem.PushToast<LogoUI>(ToastType.Logo);
    }

    private void FixedUpdate()
    {
        m_ContentSystemUpdate?.Invoke();
    }

    private void Update()
    {
        m_CoreSystemUpdate?.Invoke();
    }

    void OnApplicationQuit()
    {
        if (!NoAutoSavePlayerData)
            SavePlayerData();
    }

    #region �� �ε�
    public void LoadScene(SceneCode scene, Action onPrevSceneLoad = null, Action onSceneLoading = null, Action onSceneLoaded = null)
    {
        if (scene == SceneCode.Logo)
        {
            Debug.LogError("������ �����ϰ� ���� �ΰ�� �ٽ� ���ư� �� �����ϴ�.");
            return;
        }    

        StartCoroutine(LoadSceneCoroutine(scene, onPrevSceneLoad, onSceneLoading, onSceneLoaded));
    }

    IEnumerator LoadSceneCoroutine(SceneCode scene, Action onPrevSceneLoad = null, Action onSceneLoading = null, Action onSceneLoaded = null)
    {
        AsyncOperation async = SceneManager.LoadSceneAsync((int)scene, LoadSceneMode.Single);

        onPrevSceneLoad?.Invoke();
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

    /// <summary> ������ �ʱ�ȭ�մϴ�. </summary>
    public void RefreshGame()
    {
        Initialized = false;

        m_PlayerData = null;

        // �����ͺ��̽� Ŭ����
        TableManager.Instance.Clear();
        JsonManager.Instance.ClearJson();

        // ������ ���� �ý��� �ߴ�
        m_ContentSystemUpdate = null;

        LoadScene(SceneCode.Lobby, onSceneLoaded: LobbyManager.Instance.Init);
    }

    public void InitContents()
    {
        m_EnergyRecoverySystem = new EnergyRecoverySystem();
        m_EnergyRecoverySystem.Init();

        m_DataInitializeSystem = new DataInitializeSystem();
        m_DataInitializeSystem.Init();

        m_AchievementSystem = new AchievementSystem();
        m_AchievementSystem.Init();

        // ---------------------------------------------------

        m_ContentSystemUpdate += m_EnergyRecoverySystem.Tick;

        // PlayerData Initialize
        m_DataInitializeSystem.Initalize(m_PlayerData);
    }

    public void InitDatabase()
    {
        TableManager.Instance.LoadTable();
        JsonManager.Instance.LoadJson();
    }

    public void GetPlayerDataFromLocal()
    {
        m_PlayerData = null;
        m_PlayerData = PlayerData.GetData(GameDevelopSettings.SaveFilePath);
    }

    public void GetPlayerDataFromCloud(string data)
    {
        m_PlayerData = null;
        m_PlayerData = PlayerData.Deserialize(data);
    }

    public void SavePlayerData()
    {
        m_PlayerData.Save();
        if (GPGSBinder.Inst.DoLogin)
        {
            GPGSBinder.Inst.SaveCloud("PlayerData", PlayerData.Serialize(m_PlayerData), (success) =>
            {
                if (success)
                    Debug.LogWarning("Ŭ���忡 ���� ������ ���� ����");
                else
                    Debug.LogWarning("Ŭ���忡 ���� ������ ���� ����");
            });
        }
    }

    public void DeletePlayerData()
    {
        m_PlayerData.Delete();
        if (GPGSBinder.Inst.DoLogin)
        {
            GPGSBinder.Inst.DeleteCloud("PlayerData", (success) =>
            {
                if (success)
                    Debug.LogWarning("Ŭ���忡 ���� ������ ���� ����");
                else
                    Debug.LogWarning("Ŭ���忡 ���� ������ ���� ����");
            });
        }
    }


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