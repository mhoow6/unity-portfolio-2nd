using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DatabaseSystem;

public class GoldDropItem : DropItem
{
    public override ObjectCode Code => ObjectCode.ITEM_GoldDrop;

    public override void Use()
    {
        var sm = StageManager.Instance;
        if (sm == null)
            return;

        var dropData = TableManager.Instance.StageDropItemTable.Find(stage => stage.WorldIdx == sm.WorldIdx && stage.StageIdx == sm.StageIdx);

        int randomGold = UnityEngine.Random.Range(dropData.MinGoldDropValue, dropData.MaxGoldDropValue);
        sm.StageResult.Gold += randomGold;
    }
}
