using System.Collections.Generic;
using UnityEngine;
namespace TableSystem
{
	public class TableManager
	{
		public static TableManager Instance { get; private set; } = new TableManager();
		public int LoadedData { get; private set; } = 0;
		public List<Test2Info> Test2Infos = new List<Test2Info>();
		public List<TestInfo> TestInfos = new List<TestInfo>();
		public void LoadTable()
		{
			string[] separatingStrings = { "\r\n" };
            var Test2InfoTextasset = Resources.Load<TextAsset>("99_Table/Test2Info");
            string[] Test2InfoLines = Test2InfoTextasset.text.Split(separatingStrings, System.StringSplitOptions.RemoveEmptyEntries);
            for (int i = 4; i < Test2InfoLines.Length; i++)
            {
                string[] datas = Test2InfoLines[i].Split(',');
                Test2Info info;
                info.index = int.Parse(datas[0]);
				info.name = datas[1];
				info.speed = float.Parse(datas[2]);
				
                Test2Infos.Add(info);
                LoadedData++;
            }
        
            var TestInfoTextasset = Resources.Load<TextAsset>("99_Table/TestInfo");
            string[] TestInfoLines = TestInfoTextasset.text.Split(separatingStrings, System.StringSplitOptions.RemoveEmptyEntries);
            for (int i = 4; i < TestInfoLines.Length; i++)
            {
                string[] datas = TestInfoLines[i].Split(',');
                TestInfo info;
                info.index = int.Parse(datas[0]);
				info.name = datas[1];
				info.speed = float.Parse(datas[2]);
				
                TestInfos.Add(info);
                LoadedData++;
            }
        }
	}
}
