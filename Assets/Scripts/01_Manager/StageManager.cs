using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using DatabaseSystem;
using Cinemachine;

public class StageManager : MonoSingleton<StageManager>
{
    // �����Ϳ��� ���� ���� �־���� �ϴ� �͵�
    public int WorldIdx;
    public int StageIdx;
    public Vector3 PlayerSpawnPosition;

    public List<Area> Areas = new List<Area>();
    [ReadOnly] public List<Character> Monsters = new List<Character>();

    public PoolSystem Pool { get; private set; }

    private void Start()
    {
        // �ý��� Init
        Pool = new PoolSystem();
        Pool.Init();

        // �ΰ��ӿ� ���Ǵ� �͵� Init
        foreach (var area in Areas)
            area.Init();

        if (!GameManager.Instance.IsTestZone)
            SpawnPlayer();
    }

    /// <summary> ������ �� ĳ���ʹ�� ��ȯ </summary> ///
    public void SpawnPlayer()
    {
        var player = new GameObject("Player").AddComponent<Player>();
        player.gameObject.SetActive(true);
        // ���� ���������� �´� ĳ���� ��������
        // var record = GameManager.Instance.PlayerData.StageRecords.Find(r => r.WorldIdx == WorldIdx && r.StageIdx == StageIdx);
        // Test
        var record = new StageRecordData()
        {
            CharacterLeader = ObjectCode.CHAR_Sparcher,
            CharacterSecond = ObjectCode.CHAR_Sparcher,
            CharacterThird = ObjectCode.CHAR_Sparcher
        };

        string resourcePath = GameManager.Instance.Config.CharacterResourcePath;
        var leader = Character.Get(record.CharacterLeader, player.transform, resourcePath);
        leader.gameObject.SetActive(true);
        leader.transform.position = PlayerSpawnPosition;

        var second = Character.Get(record.CharacterSecond, player.transform, resourcePath);
        second.gameObject.SetActive(false);
        second.transform.position = PlayerSpawnPosition;

        var third = Character.Get(record.CharacterThird, player.transform, resourcePath);
        third.gameObject.SetActive(false);
        third.transform.position = PlayerSpawnPosition;

        player.Init();

        // ����ī�޶� ���� ĳ���Ϳ� �ٶ󺸰Բ� �ϱ�
        GameManager.Instance.FreeLookCam.Follow = player.CurrentCharacter.transform;
        GameManager.Instance.FreeLookCam.LookAt = player.CurrentCharacter.transform;
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
    void OnEnable()
    {
        //SceneView.duringSceneGui += CustomOnSceneGUI;
    }

    private void OnSceneGUI()
    {
        StageManager generator = (StageManager)target;

        // Set the colour of the next handle to be drawn:
        generator.PlayerSpawnPosition = Handles.PositionHandle(generator.PlayerSpawnPosition, Quaternion.identity);
        Handles.Label(generator.PlayerSpawnPosition, "Player Spawn Position");
    }

    void CustomOnSceneGUI(SceneView sceneview)
    {
        
    }
}
#endif