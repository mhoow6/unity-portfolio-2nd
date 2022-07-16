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

        // �������� ���н� ���� ���̵� �ؽ�Ʈ �����ϱ�
        int[] useIndices = new int[3] { -1, -1, -1 };
        var guideTextDatas = TableManager.Instance.StageClearGuideTextTable.FindAll(stage => stage.WorldIdx == sm.WorldIdx && stage.StageIdx == sm.StageIdx);
        for (int i = 0; i < 3; i++)
        {
            bool randomize = false;
            int randomIndex = -1;

            // �ߺ� ���� ���� �ε��� �̱�
            while (randomize != true)
            {
                randomIndex = UnityEngine.Random.Range(0, guideTextDatas.Count);
                randomize = true;

                for (int j = 0; j < useIndices.Length; j++)
                {
                    // �̹� ����ߴ� ���̵� �ؽ�Ʈ�� �Ÿ���.
                    if (useIndices[j] == randomIndex)
                    {
                        randomize = false;
                        break;
                    }
                }
            }
            useIndices[i] = randomIndex;

            // ���̵忡 ���� �ؽ�Ʈ
            ClearGuides[i].SetData($"{guideTextDatas[randomIndex].GuideText}");
        }

        // �������� �̸�
        var stageData = TableManager.Instance.StageTable.Find(stage => stage.WorldIdx == sm.WorldIdx && stage.StageIdx == sm.StageIdx);
        StageName.text = stageData.StageName;
    }
}
