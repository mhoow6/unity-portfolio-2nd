using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class StageRecordCreateWizard : ScriptableWizard
{
    static PlayerData m_PlayerData;
    public StageRecordData m_Data = new StageRecordData();

    [MenuItem("Custom Tools/Editor/Stage Record Create")]
    public static void Open()
    {
        DisplayWizard<StageRecordCreateWizard>("스테이지 기록 추가");
    }

    public static void SetData(PlayerData playerData) => m_PlayerData = playerData;

    private void OnWizardCreate()
    {
        if (m_PlayerData.StageRecords.Find(stage => stage.WorldIdx == m_Data.WorldIdx && stage.StageIdx == m_Data.StageIdx) == null)
        {
            m_PlayerData.StageRecords.Add(m_Data);
            SavefileEditor.Instance.UpdatePlayerData();
        }
    }
}
