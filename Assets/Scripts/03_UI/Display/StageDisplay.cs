using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TableSystem;

public class StageDisplay : Display
{
    public Image StageImage;
    public Image[] Medals;
    public Text StageText;

    StageRecordData m_StageRecordData;

    public void SetData(int worldIdx, int stageIdx)
    {
        m_StageRecordData = GameManager.Instance.PlayerData.StageRecords.Find(record => record.WorldIdx == worldIdx && record.StageIdx == stageIdx);

        for (int i = 0; i < Medals.Length; i++)
        {
            var medal = Medals[i];
            medal.gameObject.SetActive(false);
        }

        if (m_StageRecordData != null)
        {
            // ¸Þ´Þ
            if (m_StageRecordData.StageClear)
            {
                // ¸Þ´Þ °¹¼ö¿Í Äù½ºÆ®ÀÇ °¹¼ö´Â °°´Ù.
                for (int i = 0; i < Medals.Length; i++)
                {
                    var record = m_StageRecordData.QuestRecords.Find(r => r.Index == i + 1);
                    if (record.Clear)
                        Medals[i].gameObject.SetActive(true);
                    else
                        Medals[i].gameObject.SetActive(false);
                }
            }
        }
    }
}
