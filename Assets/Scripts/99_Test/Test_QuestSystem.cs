using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_QuestSystem : MonoBehaviour
{
    public void SavePlayerData()
    {
        GameManager.Instance.PlayerData.Save();
    }

    public void SavePlayerDataWithQuestRecord()
    {
        GameManager.Instance.QuestSystem.Report(1001, 2);
        GameManager.Instance.QuestSystem.Report(1002, 5);

        GameManager.Instance.PlayerData.Save();
    }

    public void QuestReset()
    {
        GameManager.Instance.QuestSystem.ResetQuest(1001);
        GameManager.Instance.QuestSystem.ResetQuest(1002);

        GameManager.Instance.PlayerData.Save();
    }
}
