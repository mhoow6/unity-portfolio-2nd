using System.Collections.Generic;
using UnityEngine;
namespace TableSystem
{
	public class TableManager
	{
		public static TableManager Instance { get; private set; } = new TableManager();
		public int LoadedData { get; private set; } = 0;
		public List<PlayerLevelEnergyTable> PlayerLevelEnergyTables = new List<PlayerLevelEnergyTable>();
		public List<PlayerLevelExperienceTable> PlayerLevelExperienceTables = new List<PlayerLevelExperienceTable>();
		public void LoadTable()
		{
			string[] separatingStrings = { "\r\n" };
            var PlayerLevelEnergyTableTextasset = Resources.Load<TextAsset>("99_Table/PlayerLevelEnergyTable");
            string[] PlayerLevelEnergyTableLines = PlayerLevelEnergyTableTextasset.text.Split(separatingStrings, System.StringSplitOptions.RemoveEmptyEntries);
            for (int i = 4; i < PlayerLevelEnergyTableLines.Length; i++)
            {
                string[] datas = PlayerLevelEnergyTableLines[i].Split(',');
                PlayerLevelEnergyTable info;
                info.Level = int.Parse(datas[0]);
				info.MaxEnergy = int.Parse(datas[1]);
				
                PlayerLevelEnergyTables.Add(info);
                LoadedData++;
            }
        
            var PlayerLevelExperienceTableTextasset = Resources.Load<TextAsset>("99_Table/PlayerLevelExperienceTable");
            string[] PlayerLevelExperienceTableLines = PlayerLevelExperienceTableTextasset.text.Split(separatingStrings, System.StringSplitOptions.RemoveEmptyEntries);
            for (int i = 4; i < PlayerLevelExperienceTableLines.Length; i++)
            {
                string[] datas = PlayerLevelExperienceTableLines[i].Split(',');
                PlayerLevelExperienceTable info;
                info.Level = int.Parse(datas[0]);
				info.MaxExperience = int.Parse(datas[1]);
				
                PlayerLevelExperienceTables.Add(info);
                LoadedData++;
            }
        }
	}
}
