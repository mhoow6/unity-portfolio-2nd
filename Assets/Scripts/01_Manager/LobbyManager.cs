using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class LobbyManager : GameSceneManager
{
    public static LobbyManager Instance { get; private set; }

    public MovingRoad MovingRoad;
    public LobbySystem LobbySystem;

    private void Awake()
    {
        Instance = this;
        GameManager.SceneCode = SceneCode.Lobby;
    }

    public void Init()
    {
        // 타이틀 로딩 연출 결정
        if (!GameManager.Initialized)
        {
            var loadingtTitle = GameManager.UISystem.OpenWindow<LoadingTitleUI>(UIType.LoadingTitle);
            loadingtTitle.SetData(
                onGameStartCallback: LobbySystem.Init,
                onQuarterLoadCallback: GameManager.Instance.InitDatabase,
                onHalfLoadCallback: GameManager.Instance.InitPlayerData,
                onThreeQuarterLoadCallback: GameManager.Instance.InitContents);
                
        }
        else
            LobbySystem.InitInstantly();
    }
}
