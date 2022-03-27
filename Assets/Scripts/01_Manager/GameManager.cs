using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TableSystem;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public Configuration Config { get; private set; }
    [ReadOnly]
    public Player Player;
    public Camera MainCam;
    public Light DirectLight;

    // System
    public UISystem UISystem;
    // --

    // Update Handler
    Action m_fixedUpdate;
    // ---

    [Header("# ������ �ɼ�")]
    public bool TitleLoadingSkip;

    private void Awake()
    {
        DontDestroyOnLoad(this);
        Instance = this;
        Config = Resources.Load<Configuration>("Configuration");

        // Init
        TableManager.Instance.LoadTable();
        if (UISystem != null)
            UISystem.Init();
        // ---

        // Setting
        Application.targetFrameRate = 60;
        // ---

        // FixedUpdate
        if (UISystem != null)
            m_fixedUpdate += UISystem.Tick;
        // ---
    }

    void Start()
    {
        // UI �󿡼� ���� �ε� ����
        if (UISystem != null)
        {
            var ui = UISystem.OpenWindow<LoadingUI>(UIType.Loading);
            ui.LoadingTitle(TitleLoadingSkip);
        }
    }

    void FixedUpdate()
    {
        m_fixedUpdate?.Invoke();
    }

    #region �� �ε�
    List<GameObject> m_roots = new List<GameObject>();
    bool m_IsSceneLoaded => m_roots.Count > 0;
    public void LoadScene(string sceneName)
    {
        if (m_IsSceneLoaded)
            UnloadScene();

        var prevCam = MainCam;
        var prevLight = DirectLight;

        // name, xpos, ypos, zpos, xrot, yrot, zrot
        var texts = FileHelper.GetLinesFromTableTextAsset($"99_Table/{sceneName}");
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

    public void UnloadScene()
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
