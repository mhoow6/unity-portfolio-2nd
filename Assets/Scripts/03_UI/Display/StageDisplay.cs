using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageDisplay : Display
{
    public Image StageImage;
    public Image[] Medals;
    public Text StageText;

    StageRecordData m_StageRecordData;

    public void SetData(int worldIdx, int stageIdx)
    {
        m_StageRecordData = GameManager.Instance.PlayerData.StageRecords.Find(r => r.WorldIdx == worldIdx && r.StageIdx == stageIdx);
        if (m_StageRecordData != null)
        {
            // ¸Þ´Þ
            if (m_StageRecordData.Clear)
            {

            }
            else
            {
                for (int i = 0; i < Medals.Length; i++)
                {
                    var medal = Medals[i];
                    medal.gameObject.SetActive(false);
                }
            }
        }
    }
}
