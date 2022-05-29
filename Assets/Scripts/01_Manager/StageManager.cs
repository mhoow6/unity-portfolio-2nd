using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using DatabaseSystem;

public class StageManager : GameSceneManager
{
    public static StageManager Instance { get; private set; }

    [Header("# ���� ����")]
    public int WorldIdx;
    public int StageIdx;
    public Vector3 PlayerSpawnPosition;
    public List<Area> Areas = new List<Area>();

    [Header("# �ڵ� ����")]
    [ReadOnly] public List<Character> Monsters = new List<Character>();

    // �ý���
    public PoolSystem PoolSystem;
    public MissionSystem MissionSystem;

    void Awake()
    {
        Instance = this;

        // �ý��� Init
        PoolSystem = new PoolSystem();
        PoolSystem.Init();

        MissionSystem = new MissionSystem();
        MissionSystem.Init();

        // �ΰ��ӿ� ���Ǵ� �͵� Init
        foreach (var area in Areas)
            area.Init();
    }

    public void Init(Action onInitalized = null)
    {
        StartCoroutine(InitCoroutine(onInitalized));
    }

    /// <summary> ���� ��ǥ ����� �÷��̾� �����Ϳ� ������Ʈ </summary>
	public void UpdatePlayerMissionRecords()
    {
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
    }

    #region Init() ���� �޼ҵ��
    IEnumerator InitCoroutine(Action onInitalized = null)
    {
        // �ó׸ӽ��� active camera�� �������µ� 1frame�� �ɸ�.
        yield return null;
        MainCam = m_MainCam;

        RegisterMissionsToSystem();
        SpawnPlayer();
        SetSceneCode();

        // ���� �ε�ɶ� �ٷ� Ʈ���Ÿ� ���� ��츦 ����Ͽ� ��Ȱ��ȭ ��Ų Ʈ���Ű� ������ �� true�� �ٲ���
        Areas.ForEach((a) => { a.TriggerActive = true; });

        onInitalized?.Invoke();
    }

    /// <summary> ������ �� ĳ���ʹ�� ��ȯ </summary>
    void SpawnPlayer()
    {
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
        // ����ī�޶� ���� ĳ���Ϳ� �ٶ󺸰Բ� �ϱ�
        if (FreeLookCam)
        {
            FreeLookCam.Follow = player.CurrentCharacter.transform;
            FreeLookCam.LookAt = player.CurrentCharacter.transform;
        }
    }

    /// <summary> ��� ��ǥ�� �ý��ۿ� ��� </summary>
    void RegisterMissionsToSystem()
    {
        var stageData = TableManager.Instance.StageTable.Find(s => s.WorldIdx == WorldIdx && s.StageIdx == StageIdx);
        List<int> questIndices = new List<int>() { stageData.Quest1Idx, stageData.Quest2Idx, stageData.Quest3Idx };
        MissionSystem.Register(questIndices);
    }

    /// <summary> �����ε����� ���������ε����� �˸´� enum �����ֱ� </summary>
    void SetSceneCode()
    {
        if (WorldIdx == 1 && StageIdx == 1)
            GameManager.SceneCode = SceneCode.Stage0101;
        else if (WorldIdx == 0 && StageIdx == 0)
            GameManager.SceneCode = SceneCode.Stage0000;
        else
            GameManager.SceneCode = SceneCode.None;
    }
    #endregion
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