using System.Collections;
using System.Collections.Generic;
using DatabaseSystem;

public class QuestSystem : IGameSystem
{
    // QuestIdx: RecordData
    public Dictionary<int, QuestRecordData> QuestRecords = new Dictionary<int, QuestRecordData>();

    // QuestIdx: PurposeCount
    protected Dictionary<int, int> m_QuestIdxPurposeCountDic = new Dictionary<int, int>();

	/// <summary> 퀘스트 등록 용도 </summary>
	public void Register(List<int> questIndices)
	{
		foreach (int index in questIndices)
		{
			var row = TableManager.Instance.QuestTable.Find(q => q.Index == index);
			if (row.Index != 0)
            {
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
	}

	/// <summary> 퀘스트 등록 용도 </summary>
	public void Register(List<Questable> quests)
    {
		foreach (var quest in quests)
		{
			int purposeCount = quest.PurposeCount;

			if (QuestRecords.ContainsKey(quest.Index))
				continue;
			else
			{
				var newRecord = new QuestRecordData()
				{
					QuestIdx = quest.Index,
					Type = (QuestType)quest.QuestType,
					SuccessCount = 0,
					Clear = false
				};

				QuestRecords.Add(quest.Index, newRecord);
			}
			m_QuestIdxPurposeCountDic.Add(quest.Index, purposeCount);
		}
	}

	/// <summary> 해당 퀘스트의 목표를 달성했다는 보고 용도 </summary>
	public void Report(int questIdx, int addCount = 1)
    {
        if (QuestRecords.TryGetValue(questIdx, out var record))
		{
			int purposeCount = m_QuestIdxPurposeCountDic[questIdx];
			record.SuccessCount += addCount;
			if (record.SuccessCount >= purposeCount)
			{
				record.SuccessCount = purposeCount;
				record.Clear = !record.Clear;
			}
		}
    }

	/// <summary> 해당 퀘스트 타입의 목표를 달성했다는 보고 용도 </summary>
	public void ReportAll(QuestType type, int addCount = 1)
    {
        foreach (var kvp in QuestRecords)
        {
			if (kvp.Value.Type == type)
				Report(kvp.Key, addCount);
		}
    }

	public void ReportAll(QuestType type, int target, int addCount = 1)
	{
		foreach (var kvp in QuestRecords)
		{
			if (kvp.Value.Type == type)
			{
				var row = TableManager.Instance.QuestTable.Find(q => q.Index == kvp.Key);
				if (row.Target == target)
					Report(kvp.Key, addCount);
			}

		}
	}

	public void Init()
    {

    }

    public void Tick()
    {
        
    }
}