using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DatabaseSystem;

public class DialogueTrigger : AreaTrigger
{
    protected override void OnAreaEnter(Collider other)
    {
        //int worldIdx = StageManager.Instance.WorldIdx;
        //int stageIdx = StageManager.Instance.StageIdx;

        //var datas = TableManager.Instance.StageDialogueTable.FindAll(d => d.WorldIdx == worldIdx && d.StageIdx == stageIdx && d.AreaIdx == AreaIdx);

        //var ui = GameManager.Instance.UISystem.OpenWindow<InGameDialogueUI>(UIType.InGameDialogue);
        //ui.SetData(datas);
    }
}
