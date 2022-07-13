using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageDropItem : DropItem
{
    public override ObjectCode Code => ObjectCode.LOOT_Item;

    public override void Use()
    {
        var sm = StageManager.Instance;
        if (sm == null)
            return;

        if (sm.StageDropItemIndices.Count == 0)
            return;

        int randomIdx = UnityEngine.Random.Range(0, sm.StageDropItemIndices.Count);
        int dropItemIndex = sm.StageDropItemIndices[randomIdx];

        var exist = sm.StageResult.Rewards.Find(reward => reward.Index == dropItemIndex);
        if (exist != null)
        {
            exist.Quantity++;
        }
        else
        {
            sm.StageResult.Rewards.Add(new StageRewardItemData()
            {
                Index = dropItemIndex,
                Quantity = 1
            });
        }
    }
}
