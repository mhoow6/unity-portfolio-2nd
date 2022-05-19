using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DatabaseSystem;
using System.Linq;

public class StageDisplay : Display
{
    public Image StageImage;
    public Image[] Medals;
    public Text StageText;
    public GameObject Veil;
    public Text VeilText;
    public GameObject ContentLock;

    int m_WorldIdx;
    int m_StageIdx;
    bool m_IsStageLocked => ContentLock.gameObject.activeSelf;

    public void OnVeilBtnClick()
    {
        var warning = GameManager.UISystem.OpenWindow<WarningUI>(UIType.Warning);
        string message = string.Empty;

        if (m_IsStageLocked)
            message = $"Coming Soon..";
        else
            message = $"조건:{m_WorldIdx}-{m_StageIdx - 1} 클리어";

        warning.SetData(message);
    }

    public void OnStageEnterBtnClick()
    {
        var battle = GameManager.UISystem.OpenWindow<ReadyForBattleUI>(UIType.ReadyForBattle);
        battle.SetData(m_WorldIdx, m_StageIdx);
    }

    public void SetData(int worldIdx, int stageIdx)
    {
        var playerData = GameManager.PlayerData;
        m_WorldIdx = worldIdx;
        m_StageIdx = stageIdx;

        var stageData = TableManager.Instance.StageTable.Find(q => q.WorldIdx == worldIdx && q.StageIdx == stageIdx);
        var stageRecord = playerData.StageRecords.Find(sr => sr.WorldIdx == worldIdx && sr.StageIdx == stageIdx);

        StageText.text = $"{worldIdx}-{stageIdx}";
        VeilText.text = $"{worldIdx}-{stageIdx}";
        Veil.gameObject.SetActive(false);
        VeilText.gameObject.SetActive(false);
        ContentLock.SetActive(false);

        for (int i = 0; i < Medals.Length; i++)
        {
            var medal = Medals[i];
            medal.gameObject.SetActive(false);
        }

        if (playerData != null)
        {
            // 메달 갯수와 퀘스트의 갯수는 같다.
            for (int i = 0; i < Medals.Length; i++)
            {
                QuestRecordData record = null;

                switch (i)
                {
                    case 0:
                        int quest1Idx = stageData.Quest1Idx;
                        record = playerData.QuestRecords.Find(q => q.QuestIdx == quest1Idx);
                        break;
                    case 1:
                        int quest2Idx = stageData.Quest2Idx;
                        record = playerData.QuestRecords.Find(q => q.QuestIdx == quest2Idx);
                        break;
                    case 2:
                        int quest3Idx = stageData.Quest3Idx;
                        record = playerData.QuestRecords.Find(q => q.QuestIdx == quest3Idx);
                        break;
                }

                if (record != null  && stageRecord != null)
                    if (record.Clear && stageRecord.Clear)
                        Medals[i].gameObject.SetActive(true);
            }
        }

        // 1-1을 제외한 스테이지는 이전 스테이지 클리어해야 개방이 된다.
        if (stageIdx > 1)
        {
            int prevStageIdx = stageIdx - 1;
            var prevStageRecord = playerData.StageRecords.Find(sr => sr.WorldIdx == worldIdx && sr.StageIdx == prevStageIdx);
            if (prevStageRecord == null)
                StageClearAction(false);
            else
            {
                if (prevStageRecord.Clear)
                    StageClearAction(true);
                else
                    StageClearAction(false);
            }
        }

        // 컨텐츠 개방여부
        if (stageData.LockContent)
        {
            StageClearAction(false);
            ContentLock.SetActive(true);
        }
    }

    void StageClearAction(bool clear)
    {
        StageImage.gameObject.SetActive(clear);
        Veil.gameObject.SetActive(!clear);
        VeilText.gameObject.SetActive(!clear);
    }
}
