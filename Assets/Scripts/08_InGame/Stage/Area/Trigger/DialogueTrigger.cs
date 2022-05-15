using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DatabaseSystem;

public class DialogueTrigger : AreaTrigger
{
    [SerializeField] WorldSpaceDialogue m_WorldSpaceDialogue;

    protected override void OnAreaEnter(Collider other)
    {
        m_AutoDisable = true;
        //int worldIdx = StageManager.Instance.WorldIdx;
        //int stageIdx = StageManager.Instance.StageIdx;

        // TEST
        int worldIdx = 1;
        int stageIdx = 1;
        m_AreaIdx = 1;

        var datas = TableManager.Instance.StageDialogueTable.FindAll(d => d.WorldIdx == worldIdx && d.StageIdx == stageIdx && d.AreaIdx == m_AreaIdx);

        if (m_WorldSpaceDialogue)
            m_WorldSpaceDialogue.SetData(datas);
    }
}
