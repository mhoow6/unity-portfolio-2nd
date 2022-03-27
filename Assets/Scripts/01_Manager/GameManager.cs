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

    [Header("# 개발자 옵션")]
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
        // UI 상에서 게임 로딩 시작
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

    #region 씬 로딩
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
            // xpos부터 아무 값이 저장이 안 되어 있으면 부모 오브젝트
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

            // 부모의 자식으로 해놓아 에디터에서 관리하기 편하게 함
            UnityEngine.Object.Instantiate(prefab, go.transform, true);
        }

        // 기존 씬에 있던 카메라와 direcitonal light은 삭제
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
            // 자식들 삭제
            for (int i = 0; i < root.transform.childCount; i++)
                UnityEngine.Object.Destroy(root.transform.GetChild(i).gameObject);

            // 루트 삭제
            UnityEngine.Object.Destroy(root);
        }

        // 클리어
        m_roots.Clear();
        MainCam = null;
        DirectLight = null;
    }
    #endregion
}
