using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using DatabaseSystem;

[Serializable]
public class PlayerData : ISubscribable
{
    public Guid Guid;

    // --------------------------------------------------------------------

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

    // --------------------------------------------------------------------

    public bool AskForNickName;
    public bool NewbieGift;

    // --------------------------------------------------------------------

    public ObjectCode MainMenuCharacter;
    public List<StageRecordData> StageRecords = new List<StageRecordData>();
    public List<QuestRecordData> QuestRecords = new List<QuestRecordData>();
    public List<CharacterRecordData> CharacterDatas = new List<CharacterRecordData>();
    public Inventory Inventory = new Inventory();

    // --------------------------------------------------------------------

    [JsonProperty] DateTime m_LastGameConnectTime;

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
        LastEnergyUpdateTime = DateTime.MinValue;
    }

    public static PlayerData GetData(string saveFilePath)
    {
        string result = string.Empty;

        result = FileHelper.GetStringFrom(saveFilePath);

        if (!string.IsNullOrEmpty(result))
            return JsonConvert.DeserializeObject<PlayerData>(result);
        return new PlayerData();
    }

    public static PlayerData Deserialize(string data)
    {
        return JsonConvert.DeserializeObject<PlayerData>(data);
    }

    public static string Serialize(PlayerData playerData)
    {
        return JsonConvert.SerializeObject(playerData);
    }

    public void Save(string saveFilePath)
    {
        m_LastGameConnectTime = DateTime.Now;

        var saveData = JsonConvert.SerializeObject(this);
        FileHelper.WriteFile(saveFilePath, saveData);

        Debug.LogWarning($"게임 데이터 저장완료. 저장경로 {saveFilePath}");
    }

    public void Delete(string saveFilePath)
    {
        FileHelper.DeleteFile(saveFilePath);
        Debug.LogWarning("게임 데이터 삭제완료.");
    }

    public void DisposeEvents()
    {
        OnEnergyUpdate = null;
        OnExperienceUpdate = null;
        OnGoldUpdate = null;
        OnLevelUpdate = null;
        OnNickNameUpdate = null;
    }

    public void LevelUp(int gainExperience)
    {
        // 스테이지 결과에 따른 플레이어 레벨업
        int maxExperience = TableManager.Instance.PlayerLevelExperienceTable.Find(row => row.Level == Level).MaxExperience; // 300
        int previousPlayerExperience = Experience;
        while (maxExperience < gainExperience)
        {
            // 레벨업이 되는 상황이므로 레벨업
            Level++;
            Experience = 0;

            gainExperience -= (maxExperience - previousPlayerExperience);

            previousPlayerExperience = Experience;
            maxExperience = TableManager.Instance.PlayerLevelExperienceTable.Find(row => row.Level == Level).MaxExperience;
        }
        Experience += Mathf.Abs(gainExperience);
        Debug.LogWarning($"[PlayerData.LevelUp]: 플레이어는 레벨:{Level}과 경험치: {Experience}가 되었습니다.");
    }
}
