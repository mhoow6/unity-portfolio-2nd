using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyManager : GameSceneManager
{
    public static LobbyManager Instance { get; private set; }

    public LoadingTitleSystem LoadingTitleSystem;
    public MainLobbySystem MainLobbySystem;

    private void Awake()
    {
        Instance = this;
        GameManager.SceneCode = SceneCode.Lobby;
    }

    public void Init()
    {
        // 타이틀 로딩 연출 결정
        if (!GameManager.Initialized)
            GameManager.UISystem.OpenWindow(UIType.LoadingTitle);
        else
            MainLobbySystem.FastInit();
    }
}
