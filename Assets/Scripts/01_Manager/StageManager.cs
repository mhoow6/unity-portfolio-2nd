using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using DatabaseSystem;

public class StageManager : GameSceneManager
{
    public static StageManager Instance { get; private set; }

    [Header("# 수동 기입")]
    public int WorldIdx;
    public int StageIdx;
    public Vector3 PlayerSpawnPosition;
    public List<Area> Areas = new List<Area>();

    [Header("# 자동 기입")]
    [ReadOnly] public List<Character> Monsters = new List<Character>();

    // 시스템
    public PoolSystem PoolSystem;
    public MissionSystem MissionSystem;

    void Awake()
    {
        Instance = this;

        // 시스템 Init
        PoolSystem = new PoolSystem();
        PoolSystem.Init();

        MissionSystem = new MissionSystem();
        MissionSystem.Init();

        // 인게임에 사용되는 것들 Init
        foreach (var area in Areas)
            area.Init();
    }

    public void Init(Action onInitalized = null)
    {
        StartCoroutine(InitCoroutine(onInitalized));
    }

    /// <summary> 도전 목표 기록을 플레이어 데이터에 업데이트 </summary>
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

    #region Init() 내부 메소드들
    IEnumerator InitCoroutine(Action onInitalized = null)
    {
        // 시네머신이 active camera를 가져오는데 1frame이 걸림.
        yield return null;
        MainCam = m_MainCam;

        RegisterMissionsToSystem();
        SpawnPlayer();
        SetSceneCode();

        // 씬이 로드될때 바로 트리거를 밟을 경우를 대비하여 비활성화 시킨 트리거가 있으니 다 true로 바꾸자
        Areas.ForEach((a) => { a.TriggerActive = true; });

        onInitalized?.Invoke();
    }

    /// <summary> 유저가 고른 캐릭터대로 소환 </summary>
    void SpawnPlayer()
    {
        var player = new GameObject("Player").AddComponent<Player>();
        Player = player;
        player.gameObject.SetActive(true);

        // 현재 스테이지에 맞는 캐릭터 가져오기
        var record = GameManager.PlayerData.StageRecords.Find(r => r.WorldIdx == WorldIdx && r.StageIdx == StageIdx);

        // 캐릭터 인스턴싱
        string resourcePath = GameManager.GameDevelopSettings.CharacterResourcePath;
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
        if (FreeLookCam)
        {
            FreeLookCam.Follow = player.CurrentCharacter.transform;
            FreeLookCam.LookAt = player.CurrentCharacter.transform;
        }
    }

    /// <summary> 긴급 목표를 시스템에 등록 </summary>
    void RegisterMissionsToSystem()
    {
        var stageData = TableManager.Instance.StageTable.Find(s => s.WorldIdx == WorldIdx && s.StageIdx == StageIdx);
        List<int> questIndices = new List<int>() { stageData.Quest1Idx, stageData.Quest2Idx, stageData.Quest3Idx };
        MissionSystem.Register(questIndices);
    }

    /// <summary> 월드인덱스와 스테이지인덱스에 알맞는 enum 정해주기 </summary>
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