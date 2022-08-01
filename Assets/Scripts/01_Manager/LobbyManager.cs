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
        // Lobby씬에서 Lobby씬을 다시 로드하는 경우 
        // LobbySystem과 MovingRoad가 이전 Lobby씬에 존재했던 것을 사용하여
        // 파괴된 것을 참조하게 되는 현상이 발생한다.
        LobbySystem = GameObject.FindObjectOfType(typeof(LobbySystem)) as LobbySystem;
        MovingRoad = GameObject.FindObjectOfType(typeof(MovingRoad)) as MovingRoad;

        // 타이틀 로딩 연출 결정
        if (!GameManager.Initialized)
        {
            // 테스트로 실행할 경우 클라우드 로그인 시도 X
            if (GameManager.Instance.DontTryCloudLogin)
            {
                var loadingtTitle = GameManager.UISystem.OpenWindow<LoadingTitleUI>(UIType.LoadingTitle);
                loadingtTitle.SetData(
                    onGameStartCallback: LobbySystem.Init,
                    onQuarterLoadCallback: GameManager.Instance.InitDatabase,
                    onHalfLoadCallback: () =>
                    {
                        var warning = GameManager.UISystem.OpenWindow<WarningUI>(UIType.Warning, false);
                        warning.SetData("테스트 목적으로 게임을 시작합니다. (로컬 데이터로 게임을 진행)");

                        GameManager.Instance.GetPlayerDataFromLocal();
                        GameManager.Instance.InitContents();
                    });

            }
            else
            {
                bool tryLogin = false;

                var loadingtTitle = GameManager.UISystem.OpenWindow<LoadingTitleUI>(UIType.LoadingTitle);
                loadingtTitle.SetData(
                    onGameStartCallback: LobbySystem.Init,
                    onQuarterLoadCallback: GameManager.Instance.InitDatabase,
                    onHalfLoadCallback: () =>
                    {
                        GPGSBinder.Inst.Login((success, user) =>
                        {
                        // 로그인 성공시
                        if (success)
                            {
                                var warning = GameManager.UISystem.OpenWindow<WarningUI>(UIType.Warning, false);
                                warning.SetData("로그인 성공");

                                GPGSBinder.Inst.LoadCloud("PlayerData", (success, cloudData) =>
                                {
                                    // 클라우드 로드 성공시
                                    if (success)
                                    {
                                        if (!string.IsNullOrEmpty(cloudData))
                                        {
                                            Debug.LogWarning($"클라우드 데이터 로드 성공: {cloudData}");
                                            GameManager.Instance.GetPlayerDataFromCloud(cloudData);
                                        }
                                        else
                                        {
                                            Debug.LogWarning($"클라우드에 데이터가 없음.");
                                            GameManager.Instance.GetPlayerDataFromLocal();
                                        }

                                    }
                                    else
                                    {
                                        Debug.LogWarning($"클라우드 데이터 로드 실패");
                                        GameManager.Instance.GetPlayerDataFromLocal();
                                    }
                                    tryLogin = true;
                                    GameManager.Instance.InitContents();
                                });


                            }
                            else
                            {
                                var warning = GameManager.UISystem.OpenWindow<WarningUI>(UIType.Warning, false);
                                warning.SetData("구글 플레이 서비스 로그인에 실패했습니다. 로컬 데이터로 게임을 진행합니다.");

                                GameManager.Instance.GetPlayerDataFromLocal();
                                GameManager.Instance.InitContents();

                                tryLogin = true;

                            }
                        });

                    },
                    waitingForPredicate: () =>
                    {
                        return tryLogin;
                    });

            }
        }
        else
            LobbySystem.InitInstantly();
    }
}
