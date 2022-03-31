using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

[Serializable]
public class PlayerData
{
    public Guid Guid;
    public string NickName;
    public int Level;
    public int Experience;

    [JsonIgnore] static string m_FilePath;

    public PlayerData()
    {
        Guid = Guid.NewGuid();
        NickName = string.Empty;
        Level = 1;
        Experience = 0;
    }

    public static PlayerData GetData(string saveFilePath)
    {
        m_FilePath = saveFilePath;
        string result = FileHelper.GetStringFromFileStream(saveFilePath);

        if (!string.IsNullOrEmpty(result))
            return JsonConvert.DeserializeObject<PlayerData>(result);
        return new PlayerData();
    }

    public void Save()
    {
        var saveData = JsonConvert.SerializeObject(this);
        FileHelper.WriteFile(m_FilePath, saveData);
    }

    public void Delete()
    {
        FileHelper.DeleteFile(m_FilePath);
    }
}
