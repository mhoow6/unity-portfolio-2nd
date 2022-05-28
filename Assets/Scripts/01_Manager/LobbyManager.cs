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
        // Ÿ��Ʋ �ε� ���� ����
        if (!GameManager.Initialized)
            GameManager.UISystem.OpenWindow(UIType.LoadingTitle);
        else
            MainLobbySystem.FastInit();
    }
}
