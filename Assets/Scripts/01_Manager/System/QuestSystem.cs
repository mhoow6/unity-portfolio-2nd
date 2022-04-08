using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TableSystem;

public class QuestSystem : GameSystem
{
    // QuestIdx: Clear
    Dictionary<int, bool> m_QuestIdxClearDic = new Dictionary<int, bool>();

    // QuestIdx: SuccssCount
    Dictionary<int, int> m_QuestIdxSuccessCountDic = new Dictionary<int, int>();

    // QuestIdx: PurposeCount
    Dictionary<int, int> m_QuestIdxPurposeCountDic = new Dictionary<int, int>();

    PlayerData m_playerData;
    List<QuestTable> m_QuestTable = new List<QuestTable>();

    public List<int> GetRegisterdQuestIndices()
    {
        List<int> result = new List<int>();
        foreach (var questIdx in m_QuestIdxPurposeCountDic.Keys)
            result.Add(questIdx);
        return result;
    }

    public List<int> GetSuccessQuestIndices()
    {
        List<int> result = new List<int>();
        foreach (var kvp in m_QuestIdxClearDic)
        {
            int index = kvp.Key;
            bool success = kvp.Value;

            if (success)
                result.Add(index);
        }
        return result;
    }

    public bool QuestClearFlag(int questIdx)
    {
        if (m_QuestIdxClearDic.TryGetValue(questIdx, out bool clear))
            return clear;
        return false;
    }

    public int QuestSuccessCount(int questIdx)
    {
        if (m_QuestIdxSuccessCountDic.TryGetValue(questIdx, out int count))
            return count;
        return 0;
    }

    public void ResetQuest(int questIdx)
    {
        var quests = TableManager.Instance.QuestTable;

        if (m_QuestIdxSuccessCountDic.TryGetValue(questIdx, out _))
        {
            m_QuestIdxSuccessCountDic[questIdx] = 0;
            m_QuestIdxClearDic[questIdx] = quests.Find(q => q.Index == questIdx).Positive ? false : true;
        }

        // 플레이어 데이터에 갱신
        m_playerData.ReceiveDataFrom(this);
    }

    public void Report(int questIdx, int addCount = 1)
    {
        if (m_QuestIdxSuccessCountDic.TryGetValue(questIdx, out int successCount))
        {
            successCount += addCount;
            int purposeCount = m_QuestIdxPurposeCountDic[questIdx];

            // POSITIVE_ -> 만족 NEGATIVE_ -> 실패
            if (successCount > purposeCount)
            {
                // 성공횟수는 목표횟수를 넘을 수 없다.
                successCount = purposeCount;
                m_QuestIdxClearDic[questIdx] = !m_QuestIdxClearDic[questIdx];
            }

            m_QuestIdxSuccessCountDic[questIdx] = successCount;
        }

        // 플레이어 데이터에 갱신
        m_playerData.ReceiveDataFrom(this);
    }

    public void ReportAll(QuestType type, int addCount = 1)
    {
        foreach (var q in m_QuestTable)
        {
            if (q.Type == type)
                Report(q.Index, addCount);
        }
    }

    public void Init()
    {
        m_QuestTable = TableManager.Instance.QuestTable;

        m_playerData = GameManager.Instance.PlayerData;
        List<QuestRecordData> playerQuestRecordDatas = m_playerData.QuestRecords;

        foreach (var q in m_QuestTable)
        {
            bool playerQuestClear = q.Positive;

            int playerQuestSuccessCount = 0;
            var playerQuestRecord = playerQuestRecordDatas.Find(pqr => pqr.QuestIdx == q.Index);
            if (playerQuestRecord != null)
            {
                playerQuestSuccessCount = playerQuestRecord.SuccessCount;
                playerQuestClear = playerQuestRecord.Clear;
            }

            m_QuestIdxClearDic.Add(q.Index, playerQuestClear);
            m_QuestIdxSuccessCountDic.Add(q.Index, playerQuestSuccessCount);
            m_QuestIdxPurposeCountDic.Add(q.Index, q.PurposeCount);
        }
            
    }

    public void Tick()
    {
        
    }
}
