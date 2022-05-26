using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using DatabaseSystem;

public class StageManager : MonoSingleton<StageManager>
{
    [Header("# ���� ����")]
    public int WorldIdx;
    public int StageIdx;
    public SceneType SceneType;
    public Vector3 PlayerSpawnPosition;
    public Camera PlayerCamera;
    public List<Area> Areas = new List<Area>();

    [Header("# �ڵ� ����")]
    [ReadOnly] public List<Character> Monsters = new List<Character>();

    // �ý���
    public static PoolSystem PoolSystem => Instance.m_PoolSystem;
    public PoolSystem m_PoolSystem;

    public static MissionSystem MissionSystem => Instance.m_MissionSystem;
    MissionSystem m_MissionSystem;

    protected override void Awake()
    {
        base.Awake();

        // �ý��� Init
        m_PoolSystem = new PoolSystem();
        m_PoolSystem.Init();

        m_MissionSystem = new MissionSystem();
        m_MissionSystem.Init();

        // �ΰ��ӿ� ���Ǵ� �͵� Init
        foreach (var area in Areas)
            area.Init();
    }

    public void StartStage()
    {
        HandOverPropertiesToGameManager();
        RegisterMissions();
        SpawnPlayer();

        // ���� �ε�ɶ� �ٷ� Ʈ���Ÿ� ���� ��츦 ����Ͽ� ��Ȱ��ȭ ��Ų Ʈ���Ű� ������ �� true�� �ٲ���
        Areas.ForEach((a) => { a.TriggerActive = true; });
    }

    #region StartStage()�� �޼ҵ��
    /// <summary> ������ �� ĳ���ʹ�� ��ȯ </summary>
    void SpawnPlayer()
    {
        var player = new GameObject("Player").AddComponent<Player>();
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
        var freelookCam = GameManager.FreeLookCam;
        if (freelookCam)
        {
            freelookCam.Follow = player.CurrentCharacter.transform;
            freelookCam.LookAt = player.CurrentCharacter.transform;
        }
    }

    /// <summary> ��� ��ǥ�� �ý��ۿ� ��� </summary>
    void RegisterMissions()
    {
        var stageData = TableManager.Instance.StageTable.Find(s => s.WorldIdx == WorldIdx && s.StageIdx == StageIdx);
        List<int> questIndices = new List<int>() { stageData.Quest1Idx, stageData.Quest2Idx, stageData.Quest3Idx };
        MissionSystem.Register(questIndices);
    }

    /// <summary> ���ӸŴ����� ���� �ѱ�� </summary>
    void HandOverPropertiesToGameManager()
    {
        GameManager.SceneType = SceneType;
        GameManager.MainCam = PlayerCamera;
    }
    #endregion

    /// <summary> ���� ��ǥ ����� �÷��̾� �����Ϳ� ������Ʈ </summary>
	public void UpdatePlayerMissionRecords()
    {
        var playerData = GameManager.PlayerData;
        foreach (var record in m_MissionSystem.QuestRecords.Values)
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