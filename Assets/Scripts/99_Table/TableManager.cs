﻿using System.Collections.Generic;
using UnityEngine;
using System;
namespace DatabaseSystem
{
	public partial class TableManager
	{
		public static TableManager Instance { get; private set; } = new TableManager();
		public int LoadedData { get; private set; } = 0;
		public List<AniTypeDialogueTable> AniTypeDialogueTable = new List<AniTypeDialogueTable>();
		public List<CharacterLevelExperienceTable> CharacterLevelExperienceTable = new List<CharacterLevelExperienceTable>();
		public List<CharacterTable> CharacterTable = new List<CharacterTable>();
		public List<HpRecoveryItemTable> HpRecoveryItemTable = new List<HpRecoveryItemTable>();
		public List<ItemExplainTable> ItemExplainTable = new List<ItemExplainTable>();
		public List<ItemTable> ItemTable = new List<ItemTable>();
		public List<PlayerLevelEnergyTable> PlayerLevelEnergyTable = new List<PlayerLevelEnergyTable>();
		public List<PlayerLevelExperienceTable> PlayerLevelExperienceTable = new List<PlayerLevelExperienceTable>();
		public List<QuestDescriptionTable> QuestDescriptionTable = new List<QuestDescriptionTable>();
		public List<QuestTable> QuestTable = new List<QuestTable>();
		public List<RandomNicknameTable> RandomNicknameTable = new List<RandomNicknameTable>();
		public List<SlangTable> SlangTable = new List<SlangTable>();
		public List<SpRecoveryItemTable> SpRecoveryItemTable = new List<SpRecoveryItemTable>();
		public List<StageClearGuideTextTable> StageClearGuideTextTable = new List<StageClearGuideTextTable>();
		public List<StageDialogueTable> StageDialogueTable = new List<StageDialogueTable>();
		public List<StageDropItemTable> StageDropItemTable = new List<StageDropItemTable>();
		public List<StageTable> StageTable = new List<StageTable>();
		public List<WeaponTable> WeaponTable = new List<WeaponTable>();
		public void LoadTable()
		{
			string[] separatingStrings = { "\r\n" };
            var AniTypeDialogueTableTextasset = Resources.Load<TextAsset>("99_Database/Table/AniTypeDialogueTable");
            string[] AniTypeDialogueTableLines = AniTypeDialogueTableTextasset.text.Split(separatingStrings, System.StringSplitOptions.RemoveEmptyEntries);
            for (int i = 4; i < AniTypeDialogueTableLines.Length; i++)
            {
                string[] datas = AniTypeDialogueTableLines[i].Split(',');
                AniTypeDialogueTable info;
                info.ObjectCode = (ObjectCode)Enum.Parse(typeof(ObjectCode),datas[0]);
				info.AniType = int.Parse(datas[1]);
				info.Dialog = datas[2];
				
                AniTypeDialogueTable.Add(info);
                LoadedData++;
            }
        
            var CharacterLevelExperienceTableTextasset = Resources.Load<TextAsset>("99_Database/Table/CharacterLevelExperienceTable");
            string[] CharacterLevelExperienceTableLines = CharacterLevelExperienceTableTextasset.text.Split(separatingStrings, System.StringSplitOptions.RemoveEmptyEntries);
            for (int i = 4; i < CharacterLevelExperienceTableLines.Length; i++)
            {
                string[] datas = CharacterLevelExperienceTableLines[i].Split(',');
                CharacterLevelExperienceTable info;
                info.Level = int.Parse(datas[0]);
				info.MaxExperience = int.Parse(datas[1]);
				
                CharacterLevelExperienceTable.Add(info);
                LoadedData++;
            }
        
            var CharacterTableTextasset = Resources.Load<TextAsset>("99_Database/Table/CharacterTable");
            string[] CharacterTableLines = CharacterTableTextasset.text.Split(separatingStrings, System.StringSplitOptions.RemoveEmptyEntries);
            for (int i = 4; i < CharacterTableLines.Length; i++)
            {
                string[] datas = CharacterTableLines[i].Split(',');
                CharacterTable info;
                info.Code = (ObjectCode)Enum.Parse(typeof(ObjectCode),datas[0]);
				info.Type = (CharacterType)Enum.Parse(typeof(CharacterType),datas[1]);
				info.Name = datas[2];
				info.BaseHp = int.Parse(datas[3]);
				info.HpIncreaseRatioByLevelUp = float.Parse(datas[4]);
				info.BaseSp = int.Parse(datas[5]);
				info.SpIncreaseRatioByLevelUp = float.Parse(datas[6]);
				info.BaseDamage = int.Parse(datas[7]);
				info.DamageIncreaseRatioByLevelUp = float.Parse(datas[8]);
				info.BaseDefense = int.Parse(datas[9]);
				info.DefenseIncreaseRatioByLevelUp = float.Parse(datas[10]);
				info.BaseCritical = int.Parse(datas[11]);
				info.CriticalIncreaseRatioByLevelUp = float.Parse(datas[12]);
				info.BaseSpeed = float.Parse(datas[13]);
				info.PortraitName = datas[14];
				info.LobbyAnimatorPath = datas[15];
				info.MaxGroggyExhaustion = int.Parse(datas[16]);
				info.GroggyRecoverySpeed = int.Parse(datas[17]);
				info.GroggyNaturalRecoverySpeed = int.Parse(datas[18]);
				
                CharacterTable.Add(info);
                LoadedData++;
            }
        
            var HpRecoveryItemTableTextasset = Resources.Load<TextAsset>("99_Database/Table/HpRecoveryItemTable");
            string[] HpRecoveryItemTableLines = HpRecoveryItemTableTextasset.text.Split(separatingStrings, System.StringSplitOptions.RemoveEmptyEntries);
            for (int i = 4; i < HpRecoveryItemTableLines.Length; i++)
            {
                string[] datas = HpRecoveryItemTableLines[i].Split(',');
                HpRecoveryItemTable info;
                info.ItemIdx = int.Parse(datas[0]);
				info.MinUseLevel = int.Parse(datas[1]);
				info.MaxUseLevel = int.Parse(datas[2]);
				info.HpRecoveryPoint = int.Parse(datas[3]);
				
                HpRecoveryItemTable.Add(info);
                LoadedData++;
            }
        
            var ItemExplainTableTextasset = Resources.Load<TextAsset>("99_Database/Table/ItemExplainTable");
            string[] ItemExplainTableLines = ItemExplainTableTextasset.text.Split(separatingStrings, System.StringSplitOptions.RemoveEmptyEntries);
            for (int i = 4; i < ItemExplainTableLines.Length; i++)
            {
                string[] datas = ItemExplainTableLines[i].Split(',');
                ItemExplainTable info;
                info.Index = int.Parse(datas[0]);
				info.MainExplain = datas[1];
				info.SubExplain = datas[2];
				
                ItemExplainTable.Add(info);
                LoadedData++;
            }
        
            var ItemTableTextasset = Resources.Load<TextAsset>("99_Database/Table/ItemTable");
            string[] ItemTableLines = ItemTableTextasset.text.Split(separatingStrings, System.StringSplitOptions.RemoveEmptyEntries);
            for (int i = 4; i < ItemTableLines.Length; i++)
            {
                string[] datas = ItemTableLines[i].Split(',');
                ItemTable info;
                info.Index = int.Parse(datas[0]);
				info.Type = (ItemType)Enum.Parse(typeof(ItemType),datas[1]);
				info.GroupType = (ItemGroupType)Enum.Parse(typeof(ItemGroupType),datas[2]);
				info.Name = datas[3];
				info.StarCount = int.Parse(datas[4]);
				info.IconName = datas[5];
				info.Point = int.Parse(datas[6]);
				info.MaxAmount = int.Parse(datas[7]);
				
                ItemTable.Add(info);
                LoadedData++;
            }
        
            var PlayerLevelEnergyTableTextasset = Resources.Load<TextAsset>("99_Database/Table/PlayerLevelEnergyTable");
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
        
            var PlayerLevelExperienceTableTextasset = Resources.Load<TextAsset>("99_Database/Table/PlayerLevelExperienceTable");
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
        
            var QuestDescriptionTableTextasset = Resources.Load<TextAsset>("99_Database/Table/QuestDescriptionTable");
            string[] QuestDescriptionTableLines = QuestDescriptionTableTextasset.text.Split(separatingStrings, System.StringSplitOptions.RemoveEmptyEntries);
            for (int i = 4; i < QuestDescriptionTableLines.Length; i++)
            {
                string[] datas = QuestDescriptionTableLines[i].Split(',');
                QuestDescriptionTable info;
                info.Type = (QuestType)Enum.Parse(typeof(QuestType),datas[0]);
				info.Description = datas[1];
				
                QuestDescriptionTable.Add(info);
                LoadedData++;
            }
        
            var QuestTableTextasset = Resources.Load<TextAsset>("99_Database/Table/QuestTable");
            string[] QuestTableLines = QuestTableTextasset.text.Split(separatingStrings, System.StringSplitOptions.RemoveEmptyEntries);
            for (int i = 4; i < QuestTableLines.Length; i++)
            {
                string[] datas = QuestTableLines[i].Split(',');
                QuestTable info;
                info.Index = int.Parse(datas[0]);
				info.Positive = bool.Parse(datas[1]);
				info.Type = (QuestType)Enum.Parse(typeof(QuestType),datas[2]);
				info.Target = int.Parse(datas[3]);
				info.PurposeCount = int.Parse(datas[4]);
				
                QuestTable.Add(info);
                LoadedData++;
            }
        
            var RandomNicknameTableTextasset = Resources.Load<TextAsset>("99_Database/Table/RandomNicknameTable");
            string[] RandomNicknameTableLines = RandomNicknameTableTextasset.text.Split(separatingStrings, System.StringSplitOptions.RemoveEmptyEntries);
            for (int i = 4; i < RandomNicknameTableLines.Length; i++)
            {
                string[] datas = RandomNicknameTableLines[i].Split(',');
                RandomNicknameTable info;
                info.RandomNickname = datas[0];
				
                RandomNicknameTable.Add(info);
                LoadedData++;
            }
        
            var SlangTableTextasset = Resources.Load<TextAsset>("99_Database/Table/SlangTable");
            string[] SlangTableLines = SlangTableTextasset.text.Split(separatingStrings, System.StringSplitOptions.RemoveEmptyEntries);
            for (int i = 4; i < SlangTableLines.Length; i++)
            {
                string[] datas = SlangTableLines[i].Split(',');
                SlangTable info;
                info.SlangWord = datas[0];
				
                SlangTable.Add(info);
                LoadedData++;
            }
        
            var SpRecoveryItemTableTextasset = Resources.Load<TextAsset>("99_Database/Table/SpRecoveryItemTable");
            string[] SpRecoveryItemTableLines = SpRecoveryItemTableTextasset.text.Split(separatingStrings, System.StringSplitOptions.RemoveEmptyEntries);
            for (int i = 4; i < SpRecoveryItemTableLines.Length; i++)
            {
                string[] datas = SpRecoveryItemTableLines[i].Split(',');
                SpRecoveryItemTable info;
                info.ItemIdx = int.Parse(datas[0]);
				info.MinUseLevel = int.Parse(datas[1]);
				info.MaxUseLevel = int.Parse(datas[2]);
				info.SpRecoveryPoint = int.Parse(datas[3]);
				
                SpRecoveryItemTable.Add(info);
                LoadedData++;
            }
        
            var StageClearGuideTextTableTextasset = Resources.Load<TextAsset>("99_Database/Table/StageClearGuideTextTable");
            string[] StageClearGuideTextTableLines = StageClearGuideTextTableTextasset.text.Split(separatingStrings, System.StringSplitOptions.RemoveEmptyEntries);
            for (int i = 4; i < StageClearGuideTextTableLines.Length; i++)
            {
                string[] datas = StageClearGuideTextTableLines[i].Split(',');
                StageClearGuideTextTable info;
                info.Index = int.Parse(datas[0]);
				info.WorldIdx = int.Parse(datas[1]);
				info.StageIdx = int.Parse(datas[2]);
				info.GuideText = datas[3];
				
                StageClearGuideTextTable.Add(info);
                LoadedData++;
            }
        
            var StageDialogueTableTextasset = Resources.Load<TextAsset>("99_Database/Table/StageDialogueTable");
            string[] StageDialogueTableLines = StageDialogueTableTextasset.text.Split(separatingStrings, System.StringSplitOptions.RemoveEmptyEntries);
            for (int i = 4; i < StageDialogueTableLines.Length; i++)
            {
                string[] datas = StageDialogueTableLines[i].Split(',');
                StageDialogueTable info;
                info.Index = int.Parse(datas[0]);
				info.WorldIdx = int.Parse(datas[1]);
				info.StageIdx = int.Parse(datas[2]);
				info.AreaIdx = int.Parse(datas[3]);
				info.IsLeft = bool.Parse(datas[4]);
				info.NpcTalk = bool.Parse(datas[5]);
				info.NpcName = datas[6];
				info.NpcImage = datas[7];
				info.Dialogue = datas[8];
				
                StageDialogueTable.Add(info);
                LoadedData++;
            }
        
            var StageDropItemTableTextasset = Resources.Load<TextAsset>("99_Database/Table/StageDropItemTable");
            string[] StageDropItemTableLines = StageDropItemTableTextasset.text.Split(separatingStrings, System.StringSplitOptions.RemoveEmptyEntries);
            for (int i = 4; i < StageDropItemTableLines.Length; i++)
            {
                string[] datas = StageDropItemTableLines[i].Split(',');
                StageDropItemTable info;
                info.WorldIdx = int.Parse(datas[0]);
				info.StageIdx = int.Parse(datas[1]);
				info.MinGoldDropValue = int.Parse(datas[2]);
				info.MaxGoldDropValue = int.Parse(datas[3]);
				info.DropItem1Index = int.Parse(datas[4]);
				info.DropItem2Index = int.Parse(datas[5]);
				info.DropItem3Index = int.Parse(datas[6]);
				info.DropItem4Index = int.Parse(datas[7]);
				info.DropItem5Index = int.Parse(datas[8]);
				
                StageDropItemTable.Add(info);
                LoadedData++;
            }
        
            var StageTableTextasset = Resources.Load<TextAsset>("99_Database/Table/StageTable");
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
				info.StageImage = datas[9];
				info.SceneCode = (SceneCode)Enum.Parse(typeof(SceneCode),datas[10]);
				info.WorldPrefabName = datas[11];
				info.ClearTimelimit = int.Parse(datas[12]);
				
                StageTable.Add(info);
                LoadedData++;
            }
        
            var WeaponTableTextasset = Resources.Load<TextAsset>("99_Database/Table/WeaponTable");
            string[] WeaponTableLines = WeaponTableTextasset.text.Split(separatingStrings, System.StringSplitOptions.RemoveEmptyEntries);
            for (int i = 4; i < WeaponTableLines.Length; i++)
            {
                string[] datas = WeaponTableLines[i].Split(',');
                WeaponTable info;
                info.Index = int.Parse(datas[0]);
				info.Type = (WeaponType)Enum.Parse(typeof(WeaponType),datas[1]);
				info.Name = datas[2];
				info.StarCount = int.Parse(datas[3]);
				info.IconName = datas[4];
				info.BaseDamage = int.Parse(datas[5]);
				info.DamageIncreaseRatioByLevelUp = float.Parse(datas[6]);
				info.BaseCritical = int.Parse(datas[7]);
				info.CriticalIncreaseRatioByLevelUp = float.Parse(datas[8]);
				
                WeaponTable.Add(info);
                LoadedData++;
            }
        }

            public void Clear()
            {
                LoadedData = 0;
                              
                AniTypeDialogueTable.Clear();
              
                CharacterLevelExperienceTable.Clear();
              
                CharacterTable.Clear();
              
                HpRecoveryItemTable.Clear();
              
                ItemExplainTable.Clear();
              
                ItemTable.Clear();
              
                PlayerLevelEnergyTable.Clear();
              
                PlayerLevelExperienceTable.Clear();
              
                QuestDescriptionTable.Clear();
              
                QuestTable.Clear();
              
                RandomNicknameTable.Clear();
              
                SlangTable.Clear();
              
                SpRecoveryItemTable.Clear();
              
                StageClearGuideTextTable.Clear();
              
                StageDialogueTable.Clear();
              
                StageDropItemTable.Clear();
              
                StageTable.Clear();
              
                WeaponTable.Clear();

            }
	}
}
