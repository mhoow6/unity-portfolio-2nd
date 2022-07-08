using DatabaseSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HpRecoveryItem : RecoveryItem
{
    public override void Use()
    {
        base.Use();

        var sm = StageManager.Instance;
        if (sm == null)
            return;

        int playerLevel = sm.Player.CurrentCharacter.Level;
        var itemData = TableManager.Instance.HpRecoveryItemTable.Find(item => item.MinUseLevel <= playerLevel && playerLevel <= item.MaxUseLevel);
        sm.Player.CurrentCharacter.Hp += (itemData.HpRecoveryPoint * m_RecoveryScale);
    }
}
