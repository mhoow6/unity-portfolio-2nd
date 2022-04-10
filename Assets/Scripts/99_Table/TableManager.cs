﻿using System.Collections.Generic;
using UnityEngine;
using System;
namespace TableSystem
{
	public class TableManager
	{
		public static TableManager Instance { get; private set; } = new TableManager();
		public int LoadedData { get; private set; } = 0;
		public List<AniTypeDialogueTable> AniTypeDialogueTable = new List<AniTypeDialogueTable>();
		public List<CharacterTable> CharacterTable = new List<CharacterTable>();
		public List<PlayerLevelEnergyTable> PlayerLevelEnergyTable = new List<PlayerLevelEnergyTable>();
		public List<PlayerLevelExperienceTable> PlayerLevelExperienceTable = new List<PlayerLevelExperienceTable>();
		public List<QuestTable> QuestTable = new List<QuestTable>();
		public List<RandomNicknameTable> RandomNicknameTable = new List<RandomNicknameTable>();
		public List<SlangTable> SlangTable = new List<SlangTable>();
		public List<StageTable> StageTable = new List<StageTable>();
		public void LoadTable()
		{
			string[] separatingStrings = { "\r\n" };
            var AniTypeDialogueTableTextasset = Resources.Load<TextAsset>("99_Table/Table/AniTypeDialogueTable");
            string[] AniTypeDialogueTableLines = AniTypeDialogueTableTextasset.text.Split(separatingStrings, System.StringSplitOptions.RemoveEmptyEntries);
            for (int i = 4; i < AniTypeDialogueTableLines.Length; i++)
            {
                string[] datas = AniTypeDialogueTableLines[i].Split(',');
                AniTypeDialogueTable info;
                info.ObjectCode = (CharacterCode)Enum.Parse(typeof(CharacterCode),datas[0]);
				info.AniType = (AniType)Enum.Parse(typeof(AniType),datas[1]);
				info.Dialog = datas[2];
				
                AniTypeDialogueTable.Add(info);
                LoadedData++;
            }
        
            var CharacterTableTextasset = Resources.Load<TextAsset>("99_Table/Table/CharacterTable");
            string[] CharacterTableLines = CharacterTableTextasset.text.Split(separatingStrings, System.StringSplitOptions.RemoveEmptyEntries);
            for (int i = 4; i < CharacterTableLines.Length; i++)
            {
                string[] datas = CharacterTableLines[i].Split(',');
                CharacterTable info;
                info.Code = (CharacterCode)Enum.Parse(typeof(CharacterCode),datas[0]);
				info.Type = (CharacterType)Enum.Parse(typeof(CharacterType),datas[1]);
				info.Name = datas[2];
				info.BaseHp = float.Parse(datas[3]);
				info.HpIncreaseRatioByLevelUp = float.Parse(datas[4]);
				info.BaseSp = float.Parse(datas[5]);
				info.SpIncreaseRatioByLevelUp = float.Parse(datas[6]);
				info.BaseDamage = float.Parse(datas[7]);
				info.DamageIncreaseRatioByLevelUp = float.Parse(datas[8]);
				info.BaseDefense = float.Parse(datas[9]);
				info.DefenseIncreaseRatioByLevelUp = float.Parse(datas[10]);
				info.BaseCritical = float.Parse(datas[11]);
				info.CriticalIncreaseRatioByLevelUp = float.Parse(datas[12]);
				
                CharacterTable.Add(info);
                LoadedData++;
            }
        
            var PlayerLevelEnergyTableTextasset = Resources.Load<TextAsset>("99_Table/Table/PlayerLevelEnergyTable");
            string[] PlayerLevelEnergyTableLines = PlayerLevelEnergyTableTextasset.text.Split(separatingStrings, System.StringSplitOptions.RemoveEmptyEntries);
            for (int i = 4; i < PlayerLevelEnergyTableLines.Length; i++)
            {
                string[] datas = PlayerLevelEnergyTableLines[i].Split(',');
                PlayerLevelEnergyTable info;
                info.Level = int.Parse(datas[0]);
				info.MaxEnergy = int.Parse(datas[1]);
				
                PlayerLevelEnergyTable.Add(info);
                LoadedData++;
            }
        
            var PlayerLevelExperienceTableTextasset = Resources.Load<TextAsset>("99_Table/Table/PlayerLevelExperienceTable");
            string[] PlayerLevelExperienceTableLines = PlayerLevelExperienceTableTextasset.text.Split(separatingStrings, System.StringSplitOptions.RemoveEmptyEntries);
            for (int i = 4; i < PlayerLevelExperienceTableLines.Length; i++)
            {
                string[] datas = PlayerLevelExperienceTableLines[i].Split(',');
                PlayerLevelExperienceTable info;
                info.Level = int.Parse(datas[0]);
				info.MaxExperience = int.Parse(datas[1]);
				
                PlayerLevelExperienceTable.Add(info);
                LoadedData++;
            }
        
            var QuestTableTextasset = Resources.Load<TextAsset>("99_Table/Table/QuestTable");
            string[] QuestTableLines = QuestTableTextasset.text.Split(separatingStrings, System.StringSplitOptions.RemoveEmptyEntries);
            for (int i = 4; i < QuestTableLines.Length; i++)
            {
                string[] datas = QuestTableLines[i].Split(',');
                QuestTable info;
                info.Index = int.Parse(datas[0]);
				info.Positive = bool.Parse(datas[1]);
				info.Type = (QuestType)Enum.Parse(typeof(QuestType),datas[2]);
				info.PurposeCount = int.Parse(datas[3]);
				
                QuestTable.Add(info);
                LoadedData++;
            }
        
            var RandomNicknameTableTextasset = Resources.Load<TextAsset>("99_Table/Table/RandomNicknameTable");
            string[] RandomNicknameTableLines = RandomNicknameTableTextasset.text.Split(separatingStrings, System.StringSplitOptions.RemoveEmptyEntries);
            for (int i = 4; i < RandomNicknameTableLines.Length; i++)
            {
                string[] datas = RandomNicknameTableLines[i].Split(',');
                RandomNicknameTable info;
                info.RandomNickname = datas[0];
				
                RandomNicknameTable.Add(info);
                LoadedData++;
            }
        
            var SlangTableTextasset = Resources.Load<TextAsset>("99_Table/Table/SlangTable");
            string[] SlangTableLines = SlangTableTextasset.text.Split(separatingStrings, System.StringSplitOptions.RemoveEmptyEntries);
            for (int i = 4; i < SlangTableLines.Length; i++)
            {
                string[] datas = SlangTableLines[i].Split(',');
                SlangTable info;
                info.SlangWord = datas[0];
				
                SlangTable.Add(info);
                LoadedData++;
            }
        
            var StageTableTextasset = Resources.Load<TextAsset>("99_Table/Table/StageTable");
            string[] StageTableLines = StageTableTextasset.text.Split(separatingStrings, System.StringSplitOptions.RemoveEmptyEntries);
            for (int i = 4; i < StageTableLines.Length; i++)
            {
                string[] datas = StageTableLines[i].Split(',');
                StageTable info;
                info.WorldIdx = int.Parse(datas[0]);
				info.StageIdx = int.Parse(datas[1]);
				info.StageName = datas[2];
				info.StageDescription = datas[3];
				info.Quest1Idx = int.Parse(datas[4]);
				info.Quest2Idx = int.Parse(datas[5]);
				info.Quest3Idx = int.Parse(datas[6]);
				info.EnergyCost = int.Parse(datas[7]);
				info.LockContent = bool.Parse(datas[8]);
				
                StageTable.Add(info);
                LoadedData++;
            }
        }
	}
}
