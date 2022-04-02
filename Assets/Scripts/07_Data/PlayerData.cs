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

    public PlayerData()
    {
        Guid = Guid.NewGuid();
        NickName = string.Empty;
        Level = 1;
        Experience = 0;
        Energy = 0;
    }

    public static PlayerData GetData(string saveFilePath)
    {
        m_FilePath = saveFilePath;
        string result = FileHelper.GetStringFrom(saveFilePath);

        if (!string.IsNullOrEmpty(result))
            return JsonConvert.DeserializeObject<PlayerData>(result);
        return new PlayerData();
    }

    public void Save()
    {
        var saveData = JsonConvert.SerializeObject(this);
        FileHelper.WriteFile(m_FilePath, saveData);
        Debug.Log($"게임 데이터 저장완료. 저장경로 {m_FilePath}");
    }

    public void Delete()
    {
        FileHelper.DeleteFile(m_FilePath);
        Debug.Log("게임 데이터 삭제완료.");
    }
}
