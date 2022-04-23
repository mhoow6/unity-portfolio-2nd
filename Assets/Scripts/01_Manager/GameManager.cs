using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DatabaseSystem;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    // Global Data
    public Configuration Config { get; private set; }
    public PlayerData PlayerData { get; private set; }
    [ReadOnly] public Player Player;
    public Camera MainCam;
    public Light DirectLight;

    // Game System
    public UISystem UISystem;
    EnergyRecoverySystem EnergyRecoverySystem;
    public QuestSystem QuestSystem { get; private set; }
    public InputSystem InputSystem;
    public SceneSystem SceneSystem { get; private set; }

    // Update Handler
    Action m_Update;
    Action m_FixedUpdate;

    [Header("# ������ �ɼ�")]
    [ReadOnly] public string GameVerison;
    public bool TitleLoadingDirectingSkip;
    public bool AskForNickNameSkip;
    public bool NoAutoSavePlayerData;
    public bool IsTestZone;

    private void Awake()
    {
        DontDestroyOnLoad(this);
        Instance = this;
        m_Update = null;
        m_FixedUpdate = null;

        // Config
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

        SceneSystem = new SceneSystem();
        SceneSystem.Init();

        // Game Setting
        Application.targetFrameRate = 60;

        // Update
        if (UISystem != null)
            m_Update += UISystem.Tick;
        if (InputSystem != null)
            m_Update += InputSystem.Tick;

        // FixedUpdate
        m_FixedUpdate += EnergyRecoverySystem.Tick;
        
    }

    void Start()
    {
        // UI �󿡼� ���� �ε� ����
        if (UISystem != null && !TitleLoadingDirectingSkip)
        {
            var ui = UISystem.OpenWindow<LoadingTitleUI>(UIType.Loading);
            ui.LoadingTitle();
        }
    }

    private void FixedUpdate()
    {
        m_FixedUpdate?.Invoke();
    }

    private void Update()
    {
        m_Update?.Invoke();

        // �ӽ�
        if (Input.GetKeyDown(KeyCode.LeftAlt))
            UISystem.OpenWindow(UIType.InGame);
    }

    void OnApplicationQuit()
    {
        if (!NoAutoSavePlayerData)
            PlayerData.Save();
    }

    #region �� ���� �ε�
    List<GameObject> m_roots = new List<GameObject>();
    bool m_isSceneLoaded => m_roots.Count > 0;
     void LoadScene(string sceneName)
    {
        if (m_isSceneLoaded)
            UnloadScene();

        var prevCam = MainCam;
        var prevLight = DirectLight;

        // name, xpos, ypos, zpos, xrot, yrot, zrot
        var texts = FileHelper.GetStringsFromByCSVFormat($"99_Table/{sceneName}");
        if (texts == null)
            return;

        GameObject go = null;
        for (int i = 1; i < texts.Count; i++)
        {
            var split = texts[i].Split(',');
            // xpos���� �ƹ� ���� ������ �� �Ǿ� ������ �θ� ������Ʈ
            if (split.Length == 1)
            {
                go = new GameObject(split[0]);
                m_roots.Add(go);
                continue;
            }

            GameObject prefab = null;
            prefab = Resources.Load<GameObject>($"{go.name}/{split[0]}");

            prefab.transform.position = new Vector3(float.Parse(split[1]), float.Parse(split[2]), float.Parse(split[3]));
            prefab.transform.rotation = Quaternion.Euler(new Vector3(float.Parse(split[4]), float.Parse(split[5]), float.Parse(split[6])));
            prefab.transform.localScale = new Vector3(float.Parse(split[7]), float.Parse(split[8]), float.Parse(split[9]));

            // �θ��� �ڽ����� �س��� �����Ϳ��� �����ϱ� ���ϰ� ��
            UnityEngine.Object.Instantiate(prefab, go.transform, true);
        }

        // ���� ���� �ִ� ī�޶�� direcitonal light�� ����
        if (prevCam && prevLight)
        {
            Destroy(prevCam.gameObject);
            Destroy(prevLight.gameObject);
        }
    }

     void UnloadScene()
    {
        foreach (var root in m_roots)
        {
            // �ڽĵ� ����
            for (int i = 0; i < root.transform.childCount; i++)
                UnityEngine.Object.Destroy(root.transform.GetChild(i).gameObject);

            // ��Ʈ ����
            UnityEngine.Object.Destroy(root);
        }

        // Ŭ����
        m_roots.Clear();
        MainCam = null;
        DirectLight = null;
    }
    #endregion

    [ContextMenu("# Get Attached System")]
    void GetAttachedSystem()
    {
        // UI System
        var obj = GameObject.FindObjectOfType<UISystem>();
        UISystem = obj;

        // InputSystem
        var obj2 = GameObject.FindObjectOfType<InputSystem>();
        InputSystem = obj2;
    }
}
