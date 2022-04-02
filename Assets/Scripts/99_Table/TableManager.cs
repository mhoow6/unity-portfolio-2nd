using System.Collections.Generic;
using UnityEngine;
namespace TableSystem
{
	public class TableManager
	{
		public static TableManager Instance { get; private set; } = new TableManager();
		public int LoadedData { get; private set; } = 0;
		public List<PlayerLevelExperienceTable> PlayerLevelExperienceTables = new List<PlayerLevelExperienceTable>();
		public void LoadTable()
		{
			string[] separatingStrings = { "\r\n" };
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
