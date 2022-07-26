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
        // Lobby������ Lobby���� �ٽ� �ε��ϴ� ��� 
        // LobbySystem�� MovingRoad�� ���� Lobby���� �����ߴ� ���� ����Ͽ�
        // �ı��� ���� �����ϰ� �Ǵ� ������ �߻��Ѵ�.
        LobbySystem = GameObject.FindObjectOfType(typeof(LobbySystem)) as LobbySystem;
        MovingRoad = GameObject.FindObjectOfType(typeof(MovingRoad)) as MovingRoad;

        // Ÿ��Ʋ �ε� ���� ����
        if (!GameManager.Initialized)
        {
            var loadingtTitle = GameManager.UISystem.OpenWindow<LoadingTitleUI>(UIType.LoadingTitle);
            loadingtTitle.SetData(
                onGameStartCallback: LobbySystem.Init,
                onQuarterLoadCallback: GameManager.Instance.InitDatabase,
                onHalfLoadCallback: () =>
                {
                    GPGSBinder.Inst.Login((success, user) =>
                    {
                        // �α��� ������
                        if (success)
                        {
                            var warning = GameManager.UISystem.OpenWindow<WarningUI>(UIType.Warning, false);
                            warning.SetData("�α��� ����");

                            GPGSBinder.Inst.LoadCloud("PlayerData", (success, cloudData) =>
                            {
                                // Ŭ���� �ε� ������
                                if (success)
                                {
                                    Debug.LogWarning($"Ŭ���� ������ �ε� ����: {cloudData}");
                                    if (!string.IsNullOrEmpty(cloudData))
                                        GameManager.Instance.GetPlayerDataFromCloud(cloudData);
                                    else
                                        GameManager.Instance.GetPlayerDataFromLocal();
                                }
                                else
                                {
                                    Debug.LogWarning($"Ŭ���� ������ �ε� ����");
                                    GameManager.Instance.GetPlayerDataFromLocal();
                                }

                                GameManager.Instance.InitContents();
                            });
                        }
                        else
                        {
                            var warning = GameManager.UISystem.OpenWindow<WarningUI>(UIType.Warning, false);
                            warning.SetData("���� �÷��� ���� �α��ο� �����߽��ϴ�. ���� �����ͷ� ������ �����մϴ�.");

                            GameManager.Instance.GetPlayerDataFromLocal();
                            GameManager.Instance.InitContents();
                        }
                    });

                });
                
        }
        else
            LobbySystem.InitInstantly();
    }
}
