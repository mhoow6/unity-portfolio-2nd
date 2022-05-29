using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class StageRecordWizard : ScriptableWizard
{
    static PlayerData m_PlayerData;
    public StageRecordData m_Data = new StageRecordData();

    [MenuItem("Custom Tools/Editor/Stage Record Wizard")]
    public static void Open()
    {
        DisplayWizard<StageRecordWizard>("�������� ��� �߰�");
    }

    public static void SetData(PlayerData playerData) => m_PlayerData = playerData;

    private void OnWizardCreate()
    {
        m_PlayerData.StageRecords.Add(m_Data);
        Debug.Log($"{m_Data.WorldIdx}-{m_Data.StageIdx}�� ����߰��� �õ��մϴ�.");
    }
}
