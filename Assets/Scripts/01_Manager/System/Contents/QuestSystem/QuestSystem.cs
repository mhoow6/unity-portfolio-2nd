using System.Collections;
using System.Collections.Generic;
using DatabaseSystem;

public class QuestSystem : IGameSystem
{
    // QuestIdx: RecordData
    public Dictionary<int, QuestRecordData> QuestRecords = new Dictionary<int, QuestRecordData>();

	// QuestIdx
	List<int> m_ReportBlockQuests = new List<int>();

	// QuestIdx: PurposeCount
	protected Dictionary<int, int> m_QuestIdxPurposeCountDic = new Dictionary<int, int>();

	/// <summary> 퀘스트 등록 용도 </summary>
	public void Register(List<int> questIndices)
	{
		m_ReportBlockQuests.Clear();

		foreach (int index in questIndices)
		{
			var row = TableManager.Instance.QuestTable.Find(q => q.Index == index);
			if (row.Index != 0)
            {
				bool initFlag = row.Positive ? false : true;
				int purposeCount = row.PurposeCount;

				// 플레이어가 이미 깬 경우 스킵
				var playerRecord = GameManager.PlayerData.QuestRecords.Find(quest => quest.QuestIdx == index);
				if (playerRecord != null)
                {
					if (playerRecord.Clear)
						continue;
                }
					

				// 시스템에 이미 등록되있을 경우 스킵
				if (QuestRecords.TryGetValue(index, out var record))
					continue;
				else
				{
					var newRecord = new QuestRecordData()
					{
						QuestIdx = index,
						SuccessCount = 0,
						Clear = initFlag,
						Type = row.Type
					};
					QuestRecords.Add(index, newRecord);

					m_QuestIdxPurposeCountDic.Add(index, purposeCount);
				}
			}
		}
	}

	/// <summary> 퀘스트 등록 용도 </summary>
	public void Register(List<Questable> quests)
    {
		m_ReportBlockQuests.Clear();

		foreach (var quest in quests)
		{
			int purposeCount = quest.PurposeCount;

			// 플레이어가 이미 깬 경우 스킵
			var playerRecord = GameManager.PlayerData.QuestRecords.Find(q => q.QuestIdx == quest.Index);
			if (playerRecord != null)
            {
				if (playerRecord.Clear)
					continue;
			}

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
		if (m_ReportBlockQuests.Contains(questIdx))
			return;

        if (QuestRecords.TryGetValue(questIdx, out var record))
		{
			int purposeCount = m_QuestIdxPurposeCountDic[questIdx];
			record.SuccessCount += addCount;
			if (record.SuccessCount >= purposeCount)
			{
				record.SuccessCount = purposeCount;
				record.Clear = !record.Clear;
				m_ReportBlockQuests.Add(questIdx);
			}
		}
    }

	/// <summary> 해당 퀘스트 타입의 목표를 달성했다는 보고 용도 </summary>
	public void ReportAll(QuestType type, int target = -1, int addCount = 1)
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