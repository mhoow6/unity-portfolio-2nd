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

    PlayerData m_PlayerData;

    public void SetData(int worldIdx, int stageIdx)
    {
        m_PlayerData = GameManager.Instance.PlayerData;
        var stageQuestData = TableManager.Instance.StageQuestTable.Find(q => q.WorldIdx == worldIdx && q.StageIdx == stageIdx);
        var stageRecord = GameManager.Instance.PlayerData.StageRecords.Find(sr => sr.WorldIdx == worldIdx && sr.StageIdx == stageIdx);

        for (int i = 0; i < Medals.Length; i++)
        {
            var medal = Medals[i];
            medal.gameObject.SetActive(false);
        }

        if (m_PlayerData != null)
        {
            // ¸Þ´Þ °¹¼ö¿Í Äù½ºÆ®ÀÇ °¹¼ö´Â °°´Ù.
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
                    if (record.Clear && stageRecord.StageClear)
                        Medals[i].gameObject.SetActive(true);
            }
        }
    }
}
