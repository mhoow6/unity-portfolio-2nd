using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TableSystem;
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
    public UISystem System_UI;
    public EnergyRecoverySystem System_EnergyRecovery { get; private set; }

    // Mechanism
    [ReadOnly] public LoadingTitleMechanism Mechanism_LoadingTitle;
    [ReadOnly] public MainMenuMechanism Mechanism_MainMenu;

    // Update Handler
    Action m_Update;
    Action m_FixedUpdate;

    [Header("# ������ �ɼ�")]
    public bool TitleLoadingSkip;

    private void Awake()
    {
        DontDestroyOnLoad(this);
        Instance = this;
        m_Update = null;

        // Data
        Config = Resources.Load<Configuration>("Configuration");
        Config.SaveFilePath = $"{Application.persistentDataPath}/PlayerData.json";

        PlayerData = PlayerData.GetData(Config.SaveFilePath);
        // ---

        // Manager Init
        TableManager.Instance.LoadTable();
        if (System_UI != null)
            System_UI.Init();
        System_EnergyRecovery = new EnergyRecoverySystem();
        System_EnergyRecovery.Init();
        // ---

        // Game Setting
        Application.targetFrameRate = 60;
        // ---

        // Update
        if (System_UI != null)
            m_Update += System_UI.Tick;
        // ---

        // FixedUpdate
        m_FixedUpdate += System_EnergyRecovery.Tick;
        // ---
    }

    void Start()
    {
        // UI �󿡼� ���� �ε� ����
        if (System_UI != null)
        {
            var ui = System_UI.OpenWindow<LoadingTitleUI>(UIType.Loading);
            ui.LoadingTitle(TitleLoadingSkip);
        }
    }

    private void FixedUpdate()
    {
        m_FixedUpdate?.Invoke();
    }

    private void Update()
    {
        m_Update?.Invoke();
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
}
