using System.Collections;
using System.Collections.Generic;
using DatabaseSystem;

public class QuestSystem : GameSystem
{
    // QuestIdx: PurposeCount
    protected Dictionary<int, int> m_QuestIdxPurposeCountDic = new Dictionary<int, int>();

    // QuestIdx: RecordData
    public Dictionary<int, QuestRecordData> QuestRecords = new Dictionary<int, QuestRecordData>();

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
				record.Clear = row.Positive ? true : false;
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