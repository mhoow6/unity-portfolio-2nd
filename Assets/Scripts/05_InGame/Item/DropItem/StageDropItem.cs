using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageDropItem : DropItem
{
    public override ObjectCode Code => ObjectCode.NONE;

    public override void Use()
    {
        var sm = StageManager.Instance;
        if (sm == null)
            return;

        if (sm.StageDropItems.Count == 0)
            return;

        int randomIdx = UnityEngine.Random.Range(0, sm.StageDropItems.Count - 1);
        ObjectCode dropItemCode = sm.StageDropItems[randomIdx];

        var exist = sm.StageResult.Rewards.Find(reward => reward.Code == dropItemCode);
        if (exist != null)
        {
            exist.Quantity++;
        }
        else
        {
            sm.StageResult.Rewards.Add(new StageRewardItemData()
            {
                Code = dropItemCode,
                Quantity = 1
            });
        }
    }
}
