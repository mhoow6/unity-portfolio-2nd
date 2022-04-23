using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DatabaseSystem;

public class QuestSystem : GameSystem
{
    // QuestIdx: PurposeCount
    Dictionary<int, int> m_QuestIdxPurposeCountDic = new Dictionary<int, int>();
	
	// QuestIdx: RecordData
	Dictionary<int, QuestRecordData> QuestRecords = new Dictionary<int, QuestRecordData>();

	public List<int> QuestIndices
    {
		get
        {
			List<int> results = new List<int>();
            foreach (var key in QuestRecords.Keys)
				results.Add(key);
			return results;
		}
    }

    PlayerData m_playerData;
    List<QuestTable> m_QuestTable = new List<QuestTable>();

	/// <summary> 스테이지 시작시 도전 목표 등록 용도 </summary>
	public void RegisterStageQuests(List<int> questIndices)
	{
		foreach (int index in questIndices)
		{
			var row = m_QuestTable.Find(q => q.Index == index);
			
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

	/// <summary> 스테이지 게임오버 시 퀘스트 카운트 초기화 용도 </summary>
	public void ResetQuest(int questIdx)
    {
        var quests = TableManager.Instance.QuestTable;
		var row = m_QuestTable.Find(q => q.Index == questIdx);

		if (QuestRecords.TryGetValue(questIdx, out var record))
        {
			record.SuccessCount = 0;
			record.Clear = row.Positive ? false : true;
		}
    }

	/// <summary> 게임중 퀘스트 시스템에게 해당 퀘스트의 목표를 달성했다는 보고 용도 </summary>
	public void Report(int questIdx, int addCount = 1)
    {
        if (QuestRecords.TryGetValue(questIdx, out var record))
		{
			var row = m_QuestTable.Find(q => q.Index == questIdx);
			
			record.SuccessCount += addCount;
			if (record.SuccessCount >= row.PurposeCount)
			{
				record.SuccessCount = row.PurposeCount;
				record.Clear = row.Positive ? true : false;
			}
		}
    }

	/// <summary> 게임중 퀘스트 시스템에게 해당 퀘스트 타입의 목표를 달성했다는 보고 용도 </summary>
	public void ReportAll(QuestType type, int addCount = 1)
    {
        foreach (var q in m_QuestTable)
        {
            if (q.Type == type)
                Report(q.Index, addCount);
        }
    }

	/// <summary> 스테이지 클리어시 퀘스트 기록을 데이터에 저장하는 용도 </summary>
	public void UpdatePlayerQuestRecords()
    {
        foreach (var record in QuestRecords.Values)
        {
			var exist = m_playerData.QuestRecords.Find(r => r.QuestIdx == record.QuestIdx);
			if (exist == null)
				m_playerData.QuestRecords.Add(record);
			else
            {
				exist.SuccessCount = record.SuccessCount;
				exist.Clear = record.Clear;
			}
		}
	}

    public void Init()
    {
        m_QuestTable = TableManager.Instance.QuestTable;
        m_playerData = GameManager.Instance.PlayerData;

		
    }

    public void Tick()
    {
        
    }
}