using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

[Serializable]
public class PlayerData
{
    public Guid Guid;
    [JsonIgnore] public string NickName
    {
        get => m_NickName;
        set
        {
            m_NickName = value;
            OnNickNameUpdate?.Invoke(value);
        }
    }
    [JsonProperty] string m_NickName;
    [JsonIgnore] public Action<string> OnNickNameUpdate;

    [JsonIgnore] public int Level
    {
        get => m_Level;
        set
        {
            m_Level = value;
            OnLevelUpdate?.Invoke(value);
        }
    }
    [JsonProperty] int m_Level;
    [JsonIgnore] public Action<int> OnLevelUpdate;

    [JsonIgnore] public int Experience
    {
        get => m_Experience;
        set
        {
            m_Experience = value;
            OnExperienceUpdate?.Invoke(value);
        }
    }
    [JsonProperty] int m_Experience;
    [JsonIgnore] public Action<int> OnExperienceUpdate;

    [JsonIgnore] public int Energy
    {
        get => m_Energy;
        set
        {
            m_Energy = value;
            OnEnergyUpdate?.Invoke(value);
        }
    }
    [JsonProperty] int m_Energy;
    [JsonIgnore] public Action<int> OnEnergyUpdate;
    public DateTime LastEnergyUpdateTime;

    [JsonIgnore] public int Gold
    {
        get => m_Gold;
        set
        {
            m_Gold = value;
            OnGoldUpdate?.Invoke(value);
        }
    }
    [JsonProperty] int m_Gold;
    [JsonIgnore] public Action<int> OnGoldUpdate;

    [JsonIgnore] static string m_FilePath;

    public bool AskForNickName;

    public List<StageRecordData> StageRecords = new List<StageRecordData>();
    public List<QuestRecordData> QuestRecords = new List<QuestRecordData>();

    public PlayerData()
    {
        Guid = Guid.NewGuid();
        NickName = string.Empty;
        Level = 1;
        Experience = 0;
        Energy = 0;
        Gold = 0;
        AskForNickName = false;
    }

    public static PlayerData GetData(string saveFilePath)
    {
        m_FilePath = saveFilePath;
        string result = FileHelper.GetStringFrom(saveFilePath);

        if (!string.IsNullOrEmpty(result))
            return JsonConvert.DeserializeObject<PlayerData>(result);
        return new PlayerData();
    }

    public void ReceiveDataFrom(QuestSystem questSystem)
    {
        var indices = questSystem.GetRegisterdQuestIndices();
        foreach (var index in indices)
        {
            // �÷��̾� �����Ϳ� �����ϴ� ��� ã�Ƽ� ������ �� ����
            var record = QuestRecords.Find(qr => qr.QuestIdx == index);
            if (record != null)
            {
                record.SuccessCount = questSystem.QuestSuccessCount(index);
                record.Clear = questSystem.QuestClearFlag(index);
            }
            // ������ ���� �߰�
            else
            {
                QuestRecords.Add(new QuestRecordData()
                {
                    QuestIdx = index,
                    SuccessCount = questSystem.QuestSuccessCount(index),
                    Clear = questSystem.QuestClearFlag(index)
                });
            }
        }
    }

    public void Save()
    {
        var saveData = JsonConvert.SerializeObject(this);
        FileHelper.WriteFile(m_FilePath, saveData);
        Debug.Log($"���� ������ ����Ϸ�. ������ {m_FilePath}");
    }

    public void Delete()
    {
        FileHelper.DeleteFile(m_FilePath);
        Debug.Log("���� ������ �����Ϸ�.");
    }
}
