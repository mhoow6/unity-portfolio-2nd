using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System;

[Serializable]
public class StageRecordData
{
    public int WorldIdx;
    public int StageIdx;
    public bool StageClear;
    public List<StageQuestRecordData> QuestRecords = new List<StageQuestRecordData>();
}

public class StageQuestRecordData
{
    public int Index;
    public bool Clear;
}