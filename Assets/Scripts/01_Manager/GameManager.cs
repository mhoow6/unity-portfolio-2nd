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


    public UISystem UISystem;

    private void Awake()
    {
        DontDestroyOnLoad(this);
        Instance = this;
        Config = Resources.Load<Configuration>("Configuration");

        // Init
        TableManager.Instance.LoadTable();
        UISystem.Init();
        // ---
    }

    void Start()
    {
        // UI 상에서도 게임 가짜 로딩 시작
        var ui = UISystem.OpenWindow<LoadingUI>(UIType.Loading);
        ui.LoadingTitle();
    }

    #region 씬
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
            // xpos부터 아무 값이 저장이 안 되어 있으면 씬을 저장할 때 부모 오브젝트로 저장한 것이다.
            if (split.Length == 1)
            {
                go = new GameObject(split[0]);
                m_roots.Add(go);
                continue;
            }

            var prefab = Resources.Load<GameObject>($"{go.name}/{split[0]}");

            prefab.transform.position = new Vector3(float.Parse(split[1]), float.Parse(split[2]), float.Parse(split[3]));
            prefab.transform.rotation = Quaternion.Euler(new Vector3(float.Parse(split[4]), float.Parse(split[5]), float.Parse(split[6])));

            Object.Instantiate(prefab, go.transform, true);
        }
    }

    public void UnloadScene()
    {
        foreach (var root in m_roots)
        {
            // 자식들 삭제
            for (int i = 0; i < root.transform.childCount; i++)
            {
                Object.Destroy(root.transform.GetChild(i).gameObject);
            }

            // 루트 삭제
            Object.Destroy(root.transform);
        }

        // 리스트 클리어
        m_roots.Clear();
    }
    #endregion
}
