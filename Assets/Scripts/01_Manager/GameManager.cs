using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TableSystem;

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

    #region �� �ε�
    List<GameObject> m_roots = new List<GameObject>();
    public void LoadScene(string sceneName)
    {
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
            if (split[0].Equals("Main Camera"))
                prefab = Camera.main.gameObject;
            else
                prefab = Resources.Load<GameObject>($"{go.name}/{split[0]}");

            prefab.transform.position = new Vector3(float.Parse(split[1]), float.Parse(split[2]), float.Parse(split[3]));
            prefab.transform.rotation = Quaternion.Euler(new Vector3(float.Parse(split[4]), float.Parse(split[5]), float.Parse(split[6])));


            // �θ��� �ڽ����� �س��� �����Ϳ��� �����ϱ� ���ϰ� ��
            if (!split[0].Equals("Main Camera"))
                Object.Instantiate(prefab, go.transform, true);
        }
    }

    public void UnloadScene()
    {
        foreach (var root in m_roots)
        {
            // �ڽĵ� ����
            for (int i = 0; i < root.transform.childCount; i++)
            {
                Object.Destroy(root.transform.GetChild(i).gameObject);
            }

            // ��Ʈ ����
            Object.Destroy(root.transform);
        }

        // ����Ʈ Ŭ����
        m_roots.Clear();
    }
    #endregion
}
