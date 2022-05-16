using DatabaseSystem;
using System.Collections.Generic;

public class AchievementSystem : QuestSystem
{
	/// <summary> �������� ���۽� ���� ��ǥ ��� �뵵 </summary>
	public void RegisterStageQuests(List<int> questIndices)
	{
		foreach (int index in questIndices)
		{
			var row = TableManager.Instance.QuestTable.Find(q => q.Index == index);

			bool initFlag = row.Positive ? false : true;
			int purposeCount = row.PurposeCount;

			if (QuestRecords.TryGetValue(index, out var record))
				continue;
			else
			{
				var newRecord = new QuestRecordData()
				{
					QuestIdx = index,
					SuccessCount = 0,
					Clear = initFlag,
				};
				QuestRecords.Add(index, newRecord);
			}
			m_QuestIdxPurposeCountDic.Add(index, purposeCount);
		}
	}

	/// <summary> �������� ���ӿ��� �� ����Ʈ ī��Ʈ �ʱ�ȭ �뵵 </summary>
	public void ResetStageQuest(int questIdx)
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
