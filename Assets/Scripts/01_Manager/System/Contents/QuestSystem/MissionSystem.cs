using DatabaseSystem;
using System;
using System.Collections.Generic;

public class MissionSystem : QuestSystem
{
	/// <summary> �������� ���ӿ��� �� ���� ��ǥ �ʱ�ȭ �뵵 </summary>
	[Obsolete]
	public void ResetMissions(int questIdx)
	{
		var questTable = TableManager.Instance.QuestTable;
		var row = questTable.Find(q => q.Index == questIdx);

		if (QuestRecords.TryGetValue(questIdx, out var record))
		{
			record.SuccessCount = 0;
			record.Clear = row.Positive ? false : true;
		}
	}
}
