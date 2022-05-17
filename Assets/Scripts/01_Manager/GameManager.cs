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
    public Player Player;

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
            // ����ī�޶� �ٲܶ� �ó׸ӽ� Brain�� �ִ� ��� ����
            var brain = value.GetComponent<CinemachineBrain>();
            if (brain)
            {
                m_BrainCam = brain;
                // ����ī�޶� �ٲܶ� FreeLook�� �ִ� ��� ����
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

    [HideInInspector] public Light DirectionalLight;
    [ReadOnly] public SceneType SceneType;
    public bool AutoTargeting;

    // Game System
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

        // Config
        if (!Config)
            Config = Resources.Load<Configuration>("Configuration");
        Config.SaveFilePath = $"{Application.persistentDataPath}/PlayerData.json";

        PlayerData = PlayerData.GetData(Config.SaveFilePath);
        GameVerison = Config.GameVerison;

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

    void Start()
    {
        // ���� ����ī�޶�, ���Ɽ���� ������ ������ ã�Ƽ� ���ӸŴ����� ���
        Migration migration = FindObjectOfType<Migration>();
        if (migration)
            migration.Do();

#if UNITY_EDITOR
        // �׽�Ʈ ��Ȳ���� ���⼭ �÷��̾� ĳ���͸� ��ȯ���Ѿ� �����۵���
        if (IsTestZone)
            StageManager.Instance.SpawnPlayer();
#endif

        // UI �󿡼� ���� �ε� ����
        if (!TitleLoadingDirectingSkip)
            m_UISystem.OpenWindow<LoadingTitleUI>(UIType.Loading);
    }

    private void FixedUpdate()
    {
        m_FixedUpdate?.Invoke();
    }

    private void Update()
    {
        m_Update?.Invoke();

        // UNDONE: �ӽ�
        if (IsTestZone && Input.GetKeyDown(KeyCode.T))
            m_UISystem.OpenWindow(UIType.InGame);
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
