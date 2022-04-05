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
}

[Serializable]
public class QuestRecordData
{
    public int QuestIdx;
    public int SuccessCount;
    public bool Clear;
}