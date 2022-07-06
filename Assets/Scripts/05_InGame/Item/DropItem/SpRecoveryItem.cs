using DatabaseSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpRecoveryItem : DropItem
{
    public override ObjectCode Code => ObjectCode.ITEM_SPRecoveryDrop;

    public override void Use()
    {
        base.Use();

        var sm = StageManager.Instance;
        if (sm == null)
            return;

        int playerLevel = sm.Player.CurrentCharacter.Level;
        var itemData = TableManager.Instance.SpRecoveryItemTable.Find(item => item.MinUseLevel <= playerLevel && playerLevel <= item.MaxUseLevel);
        sm.Player.CurrentCharacter.Sp += itemData.SpRecoveryPoint;
    }
}
