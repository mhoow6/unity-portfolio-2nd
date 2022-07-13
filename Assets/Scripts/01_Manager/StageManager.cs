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

    [Header("# 수동 기입")]
    public int WorldIdx;
    public int StageIdx;
    public Vector3 PlayerSpawnPosition;
    public List<Area> Areas = new List<Area>();
    public Transform PreloadZone;

    [Header("# 자동 기입")]
    [ReadOnly] public List<Character> Monsters = new List<Character>();

    // 시스템
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

        // 시스템 Init
        PoolSystem = new PoolSystem();
        PoolSystem.Init();

        MissionSystem = new MissionSystem();
        MissionSystem.Init();

        ComboSystem = new ComboSystem();
        ComboSystem.Init();

        // 인게임에 사용되는 것들 Init
        foreach (var area in Areas)
            area.Init();

        // ---------------------------------------

        m_Update += ComboSystem.Tick;
    }

    private void Update()
    {
        m_Update?.Invoke();
    }

    #region 초기화
    public void Init(Action onInitalized = null)
    {
        StartCoroutine(InitCoroutine(onInitalized));
    }
    IEnumerator InitCoroutine(Action onInitalized = null)
    {
        // 시네머신이 active camera를 가져오는데 1frame이 걸림.
        yield return null;

        // BrainCam, FreeLookCam 할당
        MainCam = m_MainCam;
        GameManager.SceneCode = SceneCode;
        GameManager.InputSystem.CameraRotatable = false;

        // --------------------------------------------------------------------------------------------------------

        // 긴급임무를 시스템에 등록
        var stageData = TableManager.Instance.StageTable.Find(s => s.WorldIdx == WorldIdx && s.StageIdx == StageIdx);
        List<int> questIndices = new List<int>() { stageData.Quest1Idx, stageData.Quest2Idx, stageData.Quest3Idx };
        MissionSystem.Register(questIndices);

        // --------------------------------------------------------------------------------------------------------

        // 유저 캐릭터 소환
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

        // --------------------------------------------------------------------------------------------------------

        // 메인카메라가 현재 캐릭터에 바라보게끔 하기
        if (FreeLookCam)
        {
            FreeLookCam.Follow = player.CurrentCharacter.transform;
            FreeLookCam.LookAt = player.CurrentCharacter.transform;
        }

        // --------------------------------------------------------------------------------------------------------

        // 깜빡하고 프리로드존 안 만들었으면 기본값으로 만들어주기
        if (PreloadZone == null)
        {
            var _preloadZone = new GameObject();
            _preloadZone.transform.position = new Vector3(2000f, 0, 2000f);
            PreloadZone = _preloadZone.transform;
        }

        // 프리로드 오브젝트 처리
        yield return StartCoroutine(ProcessingPreloads());

        // --------------------------------------------------------------------------------------------------------

        // Area 구성요소 활성화시켜주기
        Areas.ForEach(a =>
        {
            a.Trigger = true;
            a.Spawner = true;
        });

        // --------------------------------------------------------------------------------------------------------

        // 스테이지 아이템 리스트 세팅
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
    /// 스테이지 클리어시 호출하세요
    /// </summary>
    public void StageClear()
    {
        // 이벤트 알림
        GameEventSystem.SendEvent(GameEvent.StageClear);

        // -------------------------------------------------------------------------------

        // 스테이지 클리어 시간
        StageResult.StageEndTime = DateTime.Now;

        // 몬스터 처리 수 * 10점
        StageResult.Score += (StageResult.MonsterKillCount * 10);

        // 보스몬스터 처리 수 * 100점
        StageResult.Score += (StageResult.BossKillCount * 100);

        // (스테이지 클리어 최대 시간 - 스테이지 클리어 시간) * 100점
        var stageData = TableManager.Instance.StageTable.Find(stage => stage.WorldIdx == WorldIdx && stage.StageIdx == StageIdx);
        if (stageData.ClearTimelimit != 0)
        {
            var duration = StageResult.Duration;
            StageResult.Score += ((stageData.ClearTimelimit - duration.Seconds) * 100);
        }

        StageResult.Combo = ComboSystem.Combo;

        // -------------------------------------------------------------------------------

        // 도전 목표 기록을 플레이어 데이터에 업데이트
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

        // TODO: 인벤토리에 전리품 넣어주기

        // -------------------------------------------------------------------------------

        // 하얗게 Fade In, Fade Out할때는 스테이지 클리어 UI 보여주기
        var flash = GameManager.UISystem.PushToast<FlashTransitionUI>(ToastType.FlashTransition);
        flash.SetData(
            GameManager.UISystem.CloseAllWindow, 
            () =>
            {
                var stageclearUI = GameManager.UISystem.OpenWindow<StageClearUI>(UIType.StageClear);
            },
            4f);
    }

    #region 프리로드
    public void ReservingPreload(PreloadParam param)
    {
        if (m_Init)
        {
            Debug.LogWarning($"게임이 시작된 이후에 {param.PreloadPrefab.name}을 프리로드 할려고 하고있습니다.");
            return;
        }
        m_PreloadQueue.Enqueue(param);
    }

    /// <summary> 프리로드된 오브젝트에 대한 처리 </summary>
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