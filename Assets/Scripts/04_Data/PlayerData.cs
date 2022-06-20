using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

[Serializable]
public class PlayerData : IEventCallable
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

    public ObjectCode MainMenuCharacter;
    public List<StageRecordData> StageRecords = new List<StageRecordData>();
    public List<QuestRecordData> QuestRecords = new List<QuestRecordData>();
    public List<CharacterRecordData> CharacterDatas = new List<CharacterRecordData>();

    public PlayerData()
    {
        Guid = Guid.NewGuid();
        NickName = string.Empty;
        Level = 1;
        Experience = 0;
        Energy = 0;
        Gold = 0;
        AskForNickName = false;
        MainMenuCharacter = ObjectCode.CHAR_Sparcher;

        // �⺻ ĳ���� ����
        if (CharacterDatas.Find(character => character.Code == ObjectCode.CHAR_Sparcher) == null)
        {
            CharacterDatas.Add(new CharacterRecordData()
            {
                Code = ObjectCode.CHAR_Sparcher,
                Level = 1,
                EquipWeaponCode = ObjectCode.NONE,
            });
        }

        // �׽�Ʈ���� ���� ���� ������
        if (StageRecords.Find(record => record.WorldIdx == 0 && record.StageIdx == 0) == null)
        {
            StageRecords.Add(new StageRecordData()
            {
                WorldIdx = 0,
                StageIdx = 0,
                CharacterLeader = ObjectCode.CHAR_Sparcher,
                CharacterSecond = ObjectCode.NONE,
                CharacterThird = ObjectCode.NONE,
                Clear = false
            });
        }

        // �������� 1-1 �׽�Ʈ �뵵
        if (StageRecords.Find(record => record.WorldIdx == 1 && record.StageIdx == 1) == null)
        {
            StageRecords.Add(new StageRecordData()
            {
                WorldIdx = 1,
                StageIdx = 1,
                CharacterLeader = ObjectCode.CHAR_Sparcher,
                CharacterSecond = ObjectCode.NONE,
                CharacterThird = ObjectCode.NONE,
                Clear = false
            });
        }
    }

    public static PlayerData GetData(string saveFilePath)
    {
        m_FilePath = saveFilePath;
        string result = string.Empty;

        result = FileHelper.GetStringFrom(saveFilePath);

        if (!string.IsNullOrEmpty(result))
            return JsonConvert.DeserializeObject<PlayerData>(result);
        return new PlayerData();
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

    public void DisposeEvents()
    {
        OnEnergyUpdate = null;
        OnExperienceUpdate = null;
        OnGoldUpdate = null;
        OnLevelUpdate = null;
        OnNickNameUpdate = null;
    }
}
