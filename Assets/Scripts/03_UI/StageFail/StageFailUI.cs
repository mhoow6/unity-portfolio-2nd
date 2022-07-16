using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DatabaseSystem;
using UnityEngine.UI;

public class StageFailUI : UI
{
    public override UIType Type => UIType.StageFail;

    public List<ClearGuideUI> ClearGuides = new List<ClearGuideUI>(3);
    public Text StageName;

    public void OnBackgroundClick()
    {
        if (GameManager.Instance.IsTestZone)
        {
            GameManager.UISystem.CloseWindow();
            GameManager.UISystem.OpenWindow(UIType.InGame);
            return;
        }

        GameManager.Instance.LoadScene(SceneCode.Lobby,
            onSceneLoaded: () =>
            {
                LobbyManager.Instance.Init();
            });
    }

    public override void OnClosed()
    {
        
    }

    public override void OnOpened()
    {
        var sm = StageManager.Instance;
        var stageResult = sm.StageResult;
        if (stageResult.Clear == true)
            return;

        // 스테이지 실패시 나올 가이드 텍스트 지정하기
        int[] useIndices = new int[3] { -1, -1, -1 };
        var guideTextDatas = TableManager.Instance.StageClearGuideTextTable.FindAll(stage => stage.WorldIdx == sm.WorldIdx && stage.StageIdx == sm.StageIdx);
        for (int i = 0; i < 3; i++)
        {
            bool randomize = false;
            int randomIndex = -1;

            // 중복 제외 랜덤 인덱스 뽑기
            while (randomize != true)
            {
                randomIndex = UnityEngine.Random.Range(0, guideTextDatas.Count);
                randomize = true;

                for (int j = 0; j < useIndices.Length; j++)
                {
                    // 이미 사용했던 가이드 텍스트면 거른다.
                    if (useIndices[j] == randomIndex)
                    {
                        randomize = false;
                        break;
                    }
                }
            }
            useIndices[i] = randomIndex;

            // 가이드에 쓰일 텍스트
            ClearGuides[i].SetData($"{guideTextDatas[randomIndex].GuideText}");
        }

        // 스테이지 이름
        var stageData = TableManager.Instance.StageTable.Find(stage => stage.WorldIdx == sm.WorldIdx && stage.StageIdx == sm.StageIdx);
        StageName.text = stageData.StageName;
    }
}
