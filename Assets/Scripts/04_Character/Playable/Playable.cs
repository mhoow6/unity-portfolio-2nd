using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Playable : Character
{
    protected override void OnDamaged(Character attacker, float updateHp)
    {
        StageManager.MissionSystem.ReportAll(QuestType.GET_DAMAGED);
    }

    protected override void OnDead()
    {
        StageManager.MissionSystem.ReportAll(QuestType.INCAPCITATED);
    }
}
