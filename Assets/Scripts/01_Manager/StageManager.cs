using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using DatabaseSystem;
using System.Linq;

public sealed class StageManager : GameSceneManager
{
    public static StageManager Instance { get; private set; }

    [Header("# ���� ����")]
    public int WorldIdx;
    public int StageIdx;
    public Vector3 PlayerSpawnPosition;
    public List<Area> Areas = new List<Area>();
    public Transform PreloadZone;

    [Header("# �ڵ� ����")]
    [ReadOnly] public List<Character> Monsters = new List<Character>();

    // �ý���
    public PoolSystem PoolSystem;
    public MissionSystem MissionSystem;
    public ComboSystem ComboSystem;

    public List<int> StageDropItemIndices = new List<int>(5);
    public StageResultData StageResult = new StageResultData(new List<StageRewardItemData>());

    Queue<PreloadParam> m_PreloadQueue = new Queue<PreloadParam>();
    bool m_Init;

    // Update Handler
    Action m_Update;
    Action m_FixedUpdate;

    void Awake()
    {
        Instance = this;
        m_Init = false;

        // �ý��� Init
        PoolSystem = new PoolSystem();
        PoolSystem.Init();

        MissionSystem = new MissionSystem();
        MissionSystem.Init();

        ComboSystem = new ComboSystem();
        ComboSystem.Init();

        // �ΰ��ӿ� ���Ǵ� �͵� Init
        foreach (var area in Areas)
            area.Init();

        // ---------------------------------------

        m_Update += ComboSystem.Tick;
    }

    private void Update()
    {
        m_Update?.Invoke();
    }

    #region �ʱ�ȭ
    public void Init(Action onInitalized = null)
    {
        StartCoroutine(InitCoroutine(onInitalized));
    }
    IEnumerator InitCoroutine(Action onInitalized = null)
    {
        // �ó׸ӽ��� active camera�� �������µ� 1frame�� �ɸ�.
        yield return null;

        // BrainCam, FreeLookCam �Ҵ�
        MainCam = m_MainCam;
        GameManager.SceneCode = SceneCode;
        GameManager.InputSystem.CameraRotatable = false;

        // --------------------------------------------------------------------------------------------------------

        // ����ӹ��� �ý��ۿ� ���
        var stageData = TableManager.Instance.StageTable.Find(s => s.WorldIdx == WorldIdx && s.StageIdx == StageIdx);
        List<int> questIndices = new List<int>() { stageData.Quest1Idx, stageData.Quest2Idx, stageData.Quest3Idx };
        MissionSystem.Register(questIndices);

        // --------------------------------------------------------------------------------------------------------

        // ���� ĳ���� ��ȯ
        var player = new GameObject("Player").AddComponent<Player>();
        Player = player;
        player.gameObject.SetActive(true);

        // ���� ���������� �´� ĳ���� ��������
        var record = GameManager.PlayerData.StageRecords.Find(r => r.WorldIdx == WorldIdx && r.StageIdx == StageIdx);

        // ĳ���� �ν��Ͻ�
        string resourcePath = GameManager.GameDevelopSettings.CharacterResourcePath;
        var leader = Character.Get(record.CharacterLeader, player.transform, resourcePath);
        leader.gameObject.SetActive(true);
        leader.transform.position = PlayerSpawnPosition;
        // ������ �ݵ�� �־�� �ϴµ� �������� �ݵ�� ��� ��.
        if (record.CharacterSecond != ObjectCode.NONE)
        {
            var second = Character.Get(record.CharacterSecond, player.transform, resourcePath);
            second.gameObject.SetActive(false);
            second.transform.position = PlayerSpawnPosition;
        }

        if (record.CharacterThird != ObjectCode.NONE)
        {
            var third = Character.Get(record.CharacterThird, player.transform, resourcePath);
            third.gameObject.SetActive(false);
            third.transform.position = PlayerSpawnPosition;
        }

        player.Init();

        // --------------------------------------------------------------------------------------------------------

        // ����ī�޶� ���� ĳ���Ϳ� �ٶ󺸰Բ� �ϱ�
        if (FreeLookCam)
        {
            FreeLookCam.Follow = player.CurrentCharacter.transform;
            FreeLookCam.LookAt = player.CurrentCharacter.transform;
        }

        // --------------------------------------------------------------------------------------------------------

        // �����ϰ� �����ε��� �� ��������� �⺻������ ������ֱ�
        if (PreloadZone == null)
        {
            var _preloadZone = new GameObject();
            _preloadZone.transform.position = new Vector3(2000f, 0, 2000f);
            PreloadZone = _preloadZone.transform;
        }

        // �����ε� ������Ʈ ó��
        yield return StartCoroutine(ProcessingPreloads());

        // --------------------------------------------------------------------------------------------------------

        // Area ������� Ȱ��ȭ�����ֱ�
        Areas.ForEach(a =>
        {
            a.Trigger = true;
            a.Spawner = true;
        });

        // --------------------------------------------------------------------------------------------------------

        // �������� ������ ����Ʈ ����
        var dropData = TableManager.Instance.StageDropItemTable.Find(stage => stage.WorldIdx == WorldIdx && stage.StageIdx == StageIdx);
        if (dropData.DropItem1Index >= 5000)
        {
            if (!StageDropItemIndices.Contains(dropData.DropItem1Index))
                StageDropItemIndices.Add(dropData.DropItem1Index);
        }
        if (dropData.DropItem2Index >= 5000)
        {
            if (!StageDropItemIndices.Contains(dropData.DropItem2Index))
                StageDropItemIndices.Add(dropData.DropItem2Index);
        }
        if (dropData.DropItem3Index >= 5000)
        {
            if (!StageDropItemIndices.Contains(dropData.DropItem3Index))
                StageDropItemIndices.Add(dropData.DropItem3Index);
        }
        if (dropData.DropItem4Index >= 5000)
        {
            if (!StageDropItemIndices.Contains(dropData.DropItem4Index))
                StageDropItemIndices.Add(dropData.DropItem4Index);
        }
        if (dropData.DropItem5Index >= 5000)
        {
            if (!StageDropItemIndices.Contains(dropData.DropItem5Index))
                StageDropItemIndices.Add(dropData.DropItem5Index);
        }

        // --------------------------------------------------------------------------------------------------------

        onInitalized?.Invoke();
        GameManager.InputSystem.CameraRotatable = true;
        m_Init = true;
    }
    #endregion

    /// <summary>
    /// �������� Ŭ����� ȣ���ϼ���
    /// </summary>
    public void StageClear()
    {
        // �̺�Ʈ �˸�
        GameEventSystem.SendEvent(GameEvent.StageClear);

        // -------------------------------------------------------------------------------

        // �������� Ŭ���� �ð�
        StageResult.StageEndTime = DateTime.Now;

        // ���� ó�� �� * 10��
        StageResult.Score += (StageResult.MonsterKillCount * 10);

        // �������� ó�� �� * 100��
        StageResult.Score += (StageResult.BossKillCount * 100);

        // (�������� Ŭ���� �ִ� �ð� - �������� Ŭ���� �ð�) * 100��
        var stageData = TableManager.Instance.StageTable.Find(stage => stage.WorldIdx == WorldIdx && stage.StageIdx == StageIdx);
        if (stageData.ClearTimelimit != 0)
        {
            var duration = StageResult.Duration;
            StageResult.Score += ((stageData.ClearTimelimit - duration.Seconds) * 100);
        }

        StageResult.Combo = ComboSystem.Combo;

        // -------------------------------------------------------------------------------

        // ���� ��ǥ ����� �÷��̾� �����Ϳ� ������Ʈ
        var playerData = GameManager.PlayerData;
        foreach (var record in MissionSystem.QuestRecords.Values)
        {
            var exist = playerData.QuestRecords.Find(r => r.QuestIdx == record.QuestIdx);
            if (exist == null)
                playerData.QuestRecords.Add(record);
            else
            {
                exist.SuccessCount = record.SuccessCount;
                exist.Clear = record.Clear;
            }
        }

        // TODO: �κ��丮�� ����ǰ �־��ֱ�

        // -------------------------------------------------------------------------------

        // �Ͼ�� Fade In, Fade Out�Ҷ��� �������� Ŭ���� UI �����ֱ�
        var flash = GameManager.UISystem.PushToast<FlashTransitionUI>(ToastType.FlashTransition);
        flash.SetData(
            GameManager.UISystem.CloseAllWindow, 
            () =>
            {
                var stageclearUI = GameManager.UISystem.OpenWindow<StageClearUI>(UIType.StageClear);
            },
            4f);
    }

    #region �����ε�
    public void ReservingPreload(PreloadParam param)
    {
        if (m_Init)
        {
            Debug.LogWarning($"������ ���۵� ���Ŀ� {param.PreloadPrefab.name}�� �����ε� �ҷ��� �ϰ��ֽ��ϴ�.");
            return;
        }
        m_PreloadQueue.Enqueue(param);
    }

    /// <summary> �����ε�� ������Ʈ�� ���� ó�� </summary>
    IEnumerator ProcessingPreloads()
    {
        while (m_PreloadQueue.Count != 0)
        {
            var param = m_PreloadQueue.Dequeue();
            GameObject gameObject = Instantiate(param.PreloadPrefab, PreloadZone);

            ParticleSystem ps = gameObject.GetComponent<ParticleSystem>();
            if (ps)
                yield return StartCoroutine(ProcessingPreloadParticle(ps, param.OnProcessCompletedCallback));
            else
                yield return StartCoroutine(ProcessingPreloadGameObject(gameObject, param.OnProcessCompletedCallback));
        }

        yield return null;
    }

    IEnumerator ProcessingPreloadParticle(ParticleSystem ps, Action<GameObject> onProcessCompletedCallback = null)
    {
        yield return null;
        ps.Clear(true);
        ps.gameObject.SetActive(false);

        onProcessCompletedCallback?.Invoke(ps.gameObject);
    }

    IEnumerator ProcessingPreloadGameObject(GameObject go, Action<GameObject> onProcessCompletedCallback = null)
    {
        yield return null;
        go.SetActive(false);

        onProcessCompletedCallback?.Invoke(go);
    }
    #endregion

#if UNITY_EDITOR
    [ContextMenu("# Get Area")]
    void GetArea()
    {
        var areas = FindObjectsOfType<Area>();
        Areas = areas.ToList();
        Areas.Sort();
    }
#endif
}

#if UNITY_EDITOR
[CanEditMultipleObjects]
[CustomEditor(typeof(StageManager))]
public class StageManagerEditor : Editor
{
    //void OnEnable() => SceneView.duringSceneGui += CustomOnSceneGUI;

    private void OnSceneGUI()
    {
        StageManager generator = (StageManager)target;

        generator.PlayerSpawnPosition = Handles.PositionHandle(generator.PlayerSpawnPosition, Quaternion.identity);
        Handles.Label(generator.PlayerSpawnPosition, "Player Spawn Position");
    }

    void CustomOnSceneGUI(SceneView sceneview){}
}
#endif