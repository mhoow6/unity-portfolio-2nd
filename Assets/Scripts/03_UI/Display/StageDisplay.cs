using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TableSystem;
using System.Linq;

public class StageDisplay : Display
{
    public Image StageImage;
    public Image[] Medals;
    public Text StageText;
    public GameObject Veil;
    public Text VeilText;

    PlayerData m_PlayerData;
    int m_WorldIdx;
    int m_StageIdx;

    public void OnVeilBtnClick()
    {
        var warning = GameManager.Instance.UISystem.OpenWindow<WarningUI>(UIType.Warning);
        warning.SetData($"조건:{m_WorldIdx}-{m_StageIdx - 1} 클리어");
    }

    public void OnStageEnterBtnClick()
    {
        Debug.Log("스테이지 진입");
    }

    public void SetData(int worldIdx, int stageIdx)
    {
        m_PlayerData = GameManager.Instance.PlayerData;
        m_WorldIdx = worldIdx;
        m_StageIdx = stageIdx;

        var stageQuestData = TableManager.Instance.StageQuestTable.Find(q => q.WorldIdx == worldIdx && q.StageIdx == stageIdx);
        var stageRecord = m_PlayerData.StageRecords.Find(sr => sr.WorldIdx == worldIdx && sr.StageIdx == stageIdx);

        StageText.text = $"{worldIdx}-{stageIdx}";
        VeilText.text = $"{worldIdx}-{stageIdx}";
        Veil.gameObject.SetActive(false);
        VeilText.gameObject.SetActive(false);

        for (int i = 0; i < Medals.Length; i++)
        {
            var medal = Medals[i];
            medal.gameObject.SetActive(false);
        }

        if (m_PlayerData != null)
        {
            // 메달 갯수와 퀘스트의 갯수는 같다.
            for (int i = 0; i < Medals.Length; i++)
            {
                QuestRecordData record = null;

                switch (i)
                {
                    case 0:
                        int quest1Idx = stageQuestData.Quest1Idx;
                        record = m_PlayerData.QuestRecords.Find(q => q.QuestIdx == quest1Idx);
                        break;
                    case 1:
                        int quest2Idx = stageQuestData.Quest2Idx;
                        record = m_PlayerData.QuestRecords.Find(q => q.QuestIdx == quest2Idx);
                        break;
                    case 2:
                        int quest3Idx = stageQuestData.Quest3Idx;
                        record = m_PlayerData.QuestRecords.Find(q => q.QuestIdx == quest3Idx);
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
            var prevStageRecord = m_PlayerData.StageRecords.Find(sr => sr.WorldIdx == worldIdx && sr.StageIdx == prevStageIdx);
            if (prevStageRecord == null)
            {
                StageImage.gameObject.SetActive(false);
                Veil.gameObject.SetActive(true);
                VeilText.gameObject.SetActive(true);
            }
        }
    }
}
