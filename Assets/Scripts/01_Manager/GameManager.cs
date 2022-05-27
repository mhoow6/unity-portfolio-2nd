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

    public Player Player;

    #region 카메라
    public static Camera MainCam
    {
        get
        {
            return Instance.m_MainCam;
        }
        set
        {
            Instance.m_MainCam = value;
            // 메인카메라를 바꿀때 시네머신 Brain이 있는 경우 적용
            var brain = value.GetComponent<CinemachineBrain>();
            if (brain)
            {
                Instance.m_BrainCam = brain;
                // 메인카메라를 바꿀때 FreeLook이 있는 경우 적용
                
                if (brain.ActiveVirtualCamera != null)
                {
                    var freelook = brain.ActiveVirtualCamera.VirtualCameraGameObject.GetComponent<CinemachineFreeLook>();
                    if (freelook)
                        Instance.m_FreeLookCam = freelook;
                }
            }
        }
    }
    [SerializeField] Camera m_MainCam;
    
    CinemachineBrain m_BrainCam;
    public static CinemachineBrain BrainCam => Instance.m_BrainCam;

    CinemachineFreeLook m_FreeLookCam;
    public static CinemachineFreeLook FreeLookCam => Instance.m_FreeLookCam;
    #endregion

    public static SceneType SceneType
    {
        get
        {
            return Instance.m_SceneType;
        }
        set
        {
            Instance.m_SceneType = value;
        }
    }
    [ReadOnly, SerializeField] SceneType m_SceneType;

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
    [Header("# 스크립터블 오브젝트")]
    [SerializeField] GameDevelopSettings m_GameDevelopSettings;
    public static GameDevelopSettings GameDevelopSettings => Instance.m_GameDevelopSettings;
    [SerializeField] BuildIndexSettings m_BuildIndexSettings;

    [Header("# 개발자 옵션")]
    [Rename("게임 버젼")] public string GameVerison;
    [Rename("타이틀 로딩 스킵")] public bool TitleLoadingDirectingSkip;
    [Rename("닉네임 묻기 스킵")] public bool AskForNickNameSkip;
    [Rename("종료 시 세이브 저장 끄기")] public bool NoAutoSavePlayerData;
    [Rename("인게임 테스트 환경")] public bool IsTestZone;

    private void Awake()
    {
        DontDestroyOnLoad(this);
        Instance = this;
        m_Update = null;
        m_FixedUpdate = null;

        // 스크립터블 오브젝트 로드
        if (!m_GameDevelopSettings)
            m_GameDevelopSettings = Resources.Load<GameDevelopSettings>("GameDevelopSettings");
        m_GameDevelopSettings.SaveFilePath = $"{Application.persistentDataPath}/PlayerData.json";
        m_PlayerData = PlayerData.GetData(GameDevelopSettings.SaveFilePath);
        GameVerison = GameDevelopSettings.GameVerison;

        if (!m_BuildIndexSettings)
            m_BuildIndexSettings = Resources.Load<BuildIndexSettings>("BuildIndexSettings");

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

    #region 플레이어 데이터
    /// <summary> 업적 시스템에 기록된 업적 기록을 플레이어 데이터에 업데이트 </summary>
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

    /// <summary> characterCode에 해당하는 캐릭터를 얻습니다. </summary>
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

    /// <summary> characterCode에 해당하는 캐릭터를 없앱니다. </summary>
    public void RemovePlayerCharacter(ObjectCode characterCode)
    {
        var exist = PlayerData.CharacterDatas.Find(c => c.Code == characterCode);
        if (exist != null)
            PlayerData.CharacterDatas.Remove(exist);
    }
    #endregion

    #region 씬 로드
    public void LoadStage(int worldIdx, int stageIdx)
    {
        StartCoroutine(LoadStageCoroutine(worldIdx, stageIdx, () => { StageManager.Instance.Init(); }));
    }

    public void LoadLobby()
    {
        StartCoroutine(LoadStageCoroutine(0, 0, () => { StageManager.Instance.Init(); }));
    }

    IEnumerator LoadStageCoroutine(int worldIdx, int stageIdx, Action onLoadStageCallback = null)
    {
        // UI를 열어 씬이 로드되는 과정 가리기
        var transition = m_UISystem.OpenWindow<SceneTransitionUI>(UIType.SceneTransition);

        var pair = m_BuildIndexSettings.Pair.Find(map => map.Set.WorldIdx == worldIdx && map.Set.StageIdx == stageIdx);
        AsyncOperation async = SceneManager.LoadSceneAsync(pair.BuildIndex, LoadSceneMode.Single);
        while (!async.isDone)
        {
            Debug.Log($"async progress: {async.progress}%\nasync.allowSceneActivation = {async.allowSceneActivation}");

            // 씬이 로드되면
            if (async.progress >= 0.9f)
            {
                async.allowSceneActivation = true;
            }
            yield return null;
        }
        onLoadStageCallback?.Invoke();
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

public enum SceneType
{
    MainMenu,
    InGame,
    Test = -1
}
