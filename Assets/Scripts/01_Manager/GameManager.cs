using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DatabaseSystem;
using System;
using Cinemachine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    // Global Data
    public Configuration Config;
    public PlayerData PlayerData { get; private set; }
    [ReadOnly] public Player Player;

    Camera m_MainCam;
    public Camera MainCam
    {
        get
        {
            return m_MainCam;
        }
        set
        {
            m_MainCam = value;
            var brain = value.GetComponent<CinemachineBrain>();
            if (brain)
            {
                m_BrainCam = brain;
                var freelook = brain.ActiveVirtualCamera.VirtualCameraGameObject.GetComponent<CinemachineFreeLook>();
                if (freelook)
                    m_FreeLookCam = freelook;
            }
        }
    }
    CinemachineBrain m_BrainCam;
    public CinemachineBrain BrainCam => m_BrainCam;

    CinemachineFreeLook m_FreeLookCam;
    public CinemachineFreeLook FreeLookCam => m_FreeLookCam;

    [ReadOnly] public Light DirectionalLight;
    [ReadOnly] public SceneType SceneType;
    public bool AutoTargeting;

    // Game System
    public UISystem UISystem;
    EnergyRecoverySystem EnergyRecoverySystem;
    public QuestSystem QuestSystem { get; private set; }
    public InputSystem InputSystem;

    // Update Handler
    Action m_Update;
    Action m_FixedUpdate;

    [Header("# 개발자 옵션")]
    [Rename("게임 버젼")] public string GameVerison;
    [Rename("타이틀 로딩 스킵")] public bool TitleLoadingDirectingSkip;
    [Rename("닉네임 묻기 스킵")] public bool AskForNickNameSkip;
    [Rename("종료 시 세이브 저장 끄기")] public bool NoAutoSavePlayerData;
    [Rename("테스트 환경")] public bool IsTestZone;

    private void Awake()
    {
        DontDestroyOnLoad(this);
        Instance = this;
        m_Update = null;
        m_FixedUpdate = null;

        // Config
        if (!Config)
            Config = Resources.Load<Configuration>("Configuration");
        Config.SaveFilePath = $"{Application.persistentDataPath}/PlayerData.json";

        PlayerData = PlayerData.GetData(Config.SaveFilePath);
        GameVerison = Config.GameVerison;

        // System Init
        TableManager.Instance.LoadTable();
        JsonManager.Instance.LoadJson();

        if (UISystem != null)
            UISystem.Init();

        EnergyRecoverySystem = new EnergyRecoverySystem();
        EnergyRecoverySystem.Init();

        QuestSystem = new QuestSystem();
        QuestSystem.Init();

        if (InputSystem != null)
            InputSystem.Init();

        // Game Setting
        Application.targetFrameRate = 60;

        // Update
        if (UISystem != null)
            m_Update += UISystem.Tick;
        if (InputSystem != null)
            m_Update += InputSystem.Tick;

        // FixedUpdate
        m_FixedUpdate += EnergyRecoverySystem.Tick;

#if UNITY_EDITOR
        // 씬 타입 결정
        if (IsTestZone)
            SceneType = SceneType.Test;
        else
            SceneType = SceneType.MainMenu;
#endif
    }

    void Start()
    {
        // 씬에 메인카메라, 방향광원을 가지고 있으면 찾아서 게임매니저에 등록
        Migration migration = FindObjectOfType<Migration>();
        if (migration)
            migration.Do();

#if UNITY_EDITOR
        // 테스트 상황에선 여기서 플레이어 캐릭터를 소환시켜야 정상작동함
        if (IsTestZone && StageManager.Instance)
            StageManager.Instance.SpawnPlayer();
#endif

        // UI 상에서 게임 로딩 시작
        if (UISystem != null && !TitleLoadingDirectingSkip)
            UISystem.OpenWindow<LoadingTitleUI>(UIType.Loading);
    }

    private void FixedUpdate()
    {
        m_FixedUpdate?.Invoke();
    }

    private void Update()
    {
        m_Update?.Invoke();

        // 임시
        if (Input.GetKeyDown(KeyCode.T))
            UISystem.OpenWindow(UIType.InGame);
    }

    void OnApplicationQuit()
    {
        if (!NoAutoSavePlayerData)
            PlayerData.Save();
    }

    #region 플레이어 데이터
    /// <summary>
    /// 플레이어의 캐릭터 데이터를 업데이트 합니다.
    /// </summary>
    public void UpdatePlayerData()
    {
        foreach (var cha in Player.Characters)
        {
            var exist = PlayerData.CharacterDatas.Find(c => c.Code == cha.Code);
            if (exist == null)
                PlayerData.CharacterDatas.Add(cha.Data);
            else
                exist = cha.Data;
        }
    }
    #endregion

    [ContextMenu("# Get Attached System")]
    void GetAttachedSystem()
    {
        // UI System
        var uisys = GetComponent<UISystem>();
        if (uisys != null)
        {
            UISystem = uisys;
        }

        // InputSystem
        var inputsys = GetComponent<InputSystem>();
        if (inputsys != null)
        {
            InputSystem = inputsys;
        }
    }
}

public enum SceneType
{
    MainMenu,
    InGame,
    Test = -1
}
