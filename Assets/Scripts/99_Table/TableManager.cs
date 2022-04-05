using System.Collections.Generic;
using UnityEngine;
using System;
namespace TableSystem
{
	public class TableManager
	{
		public static TableManager Instance { get; private set; } = new TableManager();
		public int LoadedData { get; private set; } = 0;
		public List<PlayerLevelEnergyTable> PlayerLevelEnergyTable = new List<PlayerLevelEnergyTable>();
		public List<PlayerLevelExperienceTable> PlayerLevelExperienceTable = new List<PlayerLevelExperienceTable>();
		public List<QuestTable> QuestTable = new List<QuestTable>();
		public List<SparcherAniTypeDialogueTable> SparcherAniTypeDialogueTable = new List<SparcherAniTypeDialogueTable>();
		public List<StageQuestTable> StageQuestTable = new List<StageQuestTable>();
		public void LoadTable()
		{
			string[] separatingStrings = { "\r\n" };
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
				info.Type = (QuestType)Enum.Parse(typeof(QuestType),datas[1]);
				info.PurposeCount = int.Parse(datas[2]);
				
                QuestTable.Add(info);
                LoadedData++;
            }
        
            var SparcherAniTypeDialogueTableTextasset = Resources.Load<TextAsset>("99_Table/Table/SparcherAniTypeDialogueTable");
            string[] SparcherAniTypeDialogueTableLines = SparcherAniTypeDialogueTableTextasset.text.Split(separatingStrings, System.StringSplitOptions.RemoveEmptyEntries);
            for (int i = 4; i < SparcherAniTypeDialogueTableLines.Length; i++)
            {
                string[] datas = SparcherAniTypeDialogueTableLines[i].Split(',');
                SparcherAniTypeDialogueTable info;
                info.AniType = (AniType)Enum.Parse(typeof(AniType),datas[0]);
				info.Dialog = datas[1];
				
                SparcherAniTypeDialogueTable.Add(info);
                LoadedData++;
            }
        
            var StageQuestTableTextasset = Resources.Load<TextAsset>("99_Table/Table/StageQuestTable");
            string[] StageQuestTableLines = StageQuestTableTextasset.text.Split(separatingStrings, System.StringSplitOptions.RemoveEmptyEntries);
            for (int i = 4; i < StageQuestTableLines.Length; i++)
            {
                string[] datas = StageQuestTableLines[i].Split(',');
                StageQuestTable info;
                info.WorldIdx = int.Parse(datas[0]);
				info.StageIdx = int.Parse(datas[1]);
				info.Quest1Idx = int.Parse(datas[2]);
				info.Quest2Idx = int.Parse(datas[3]);
				info.Quest3Idx = int.Parse(datas[4]);
				
                StageQuestTable.Add(info);
                LoadedData++;
            }
        }
	}
}
