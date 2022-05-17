using System.Collections;
using System.Collections.Generic;
using DatabaseSystem;

public class QuestSystem : GameSystem
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

			if (QuestRecords.TryGetValue(quest.Index, out var record))
				continue;
			else
			{
				var newRecord = new QuestRecordData()
				{
					QuestIdx = quest.Index,
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
			var row = TableManager.Instance.QuestTable.Find(q => q.Index == questIdx);
			
			record.SuccessCount += addCount;
			if (record.SuccessCount >= row.PurposeCount)
			{
				record.SuccessCount = row.PurposeCount;
				record.Clear = row.Positive;
			}
		}
    }

	/// <summary> 해당 퀘스트 타입의 목표를 달성했다는 보고 용도 </summary>
	public void ReportAll(QuestType type, int addCount = 1)
    {
        foreach (var q in TableManager.Instance.QuestTable)
        {
            if (q.Type == type)
                Report(q.Index, addCount);
        }
    }

    public void Init()
    {

    }

    public void Tick()
    {
        
    }
}