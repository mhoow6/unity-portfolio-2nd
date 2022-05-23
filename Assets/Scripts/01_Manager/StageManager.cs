using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using DatabaseSystem;

public class StageManager : MonoSingleton<StageManager>
{
    [Header("# 수동 기입")]
    public int WorldIdx;
    public int StageIdx;
    public Vector3 PlayerSpawnPosition;
    public List<Area> Areas = new List<Area>();

    [Header("# 자동 기입")]
    [ReadOnly] public List<Character> Monsters = new List<Character>();

    // 시스템
    public static PoolSystem PoolSystem => Instance.m_PoolSystem;
    public PoolSystem m_PoolSystem;

    public static MissionSystem MissionSystem => Instance.m_MissionSystem;
    MissionSystem m_MissionSystem;

    protected override void Awake()
    {
        base.Awake();

        // 시스템 Init
        m_PoolSystem = new PoolSystem();
        m_PoolSystem.Init();

        m_MissionSystem = new MissionSystem();
        m_MissionSystem.Init();

        // 인게임에 사용되는 것들 Init
        foreach (var area in Areas)
            area.Init();
    }

    private void Start()
    {
        if (!GameManager.Instance.IsTestZone)
        {
            SpawnPlayer();

            // 목표 등록
            var stageData = TableManager.Instance.StageTable.Find(s => s.WorldIdx == WorldIdx && s.StageIdx == StageIdx);
            List<int> questIndices = new List<int>() { stageData.Quest1Idx, stageData.Quest2Idx, stageData.Quest3Idx};
            MissionSystem.Register(questIndices);
        }
            
    }

    /// <summary> 유저가 고른 캐릭터대로 소환 </summary> ///
    public void SpawnPlayer()
    {
        var player = new GameObject("Player").AddComponent<Player>();
        player.gameObject.SetActive(true);
        // 현재 스테이지에 맞는 캐릭터 가져오기
         var record = GameManager.PlayerData.StageRecords.Find(r => r.WorldIdx == WorldIdx && r.StageIdx == StageIdx);
        // UNDONE: Test
        //var record = new StageRecordData()
        //{
        //    CharacterLeader = ObjectCode.CHAR_Sparcher,
        //    CharacterSecond = ObjectCode.CHAR_Sparcher,
        //    CharacterThird = ObjectCode.CHAR_Sparcher
        //};

        // 캐릭터 인스턴싱
        string resourcePath = GameManager.Config.CharacterResourcePath;
        var leader = Character.Get(record.CharacterLeader, player.transform, resourcePath);
        leader.gameObject.SetActive(true);
        leader.transform.position = PlayerSpawnPosition;
        // 리더는 반드시 있어야 하는데 나머지는 반드시 없어도 됨.
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

        // 메인카메라가 현재 캐릭터에 바라보게끔 하기
        var freelookCam = GameManager.Instance.FreeLookCam;
        if (freelookCam)
        {
            freelookCam.Follow = player.CurrentCharacter.transform;
            freelookCam.LookAt = player.CurrentCharacter.transform;
        }
    }

    /// <summary> 도전 목표 기록을 플레이어 데이터에 업데이트 </summary>
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

    [ContextMenu("# Get Attached System")]
    void GetAttachedSystem()
    {
        
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