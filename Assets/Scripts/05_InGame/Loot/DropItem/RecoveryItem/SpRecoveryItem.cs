using DatabaseSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpRecoveryItem : RecoveryItem
{
    public override ObjectCode Code => ObjectCode.LOOT_SPRecovery;

    public override void Pickup()
    {
        base.Pickup();

        var sm = StageManager.Instance;
        if (sm == null)
            return;

        int playerLevel = sm.Player.CurrentCharacter.Level;
        var itemData = TableManager.Instance.SpRecoveryItemTable.Find(item => item.MinUseLevel <= playerLevel && playerLevel <= item.MaxUseLevel);
        sm.Player.CurrentCharacter.Sp += (itemData.SpRecoveryPoint * m_RecoveryScale);
    }
}
