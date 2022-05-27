using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DatabaseSystem;

public class Playable : Character
{
    #region ���θ޴� �ִϸ��̼�
    // ���θ޴����� ĳ���� Ŭ���ÿ� �ִϸ��̼��� �������� �ϴµ� �ʿ���
    public List<AniType> AnimationsWhenUserClick { get; protected set; } = new List<AniType>();
    bool m_IsAnimationAndDialogSet;

    void SetMainMenuAnimations()
    {
        if (m_IsAnimationAndDialogSet)
            return;

        var table = TableManager.Instance.AniTypeDialogueTable.FindAll(row => row.ObjectCode == Code);
        foreach (var row in table)
            AnimationsWhenUserClick.Add((AniType)row.AniType);

        m_IsAnimationAndDialogSet = true;
    }
    #endregion

    protected override void OnSpawn()
    {
        SetMainMenuAnimations();
    }

    protected override void OnDamaged(Character attacker, float updateHp)
    {
        StageManager.MissionSystem.ReportAll(QuestType.GET_DAMAGED);
    }

    protected override void OnDead()
    {
        StageManager.MissionSystem.ReportAll(QuestType.INCAPCITATED);
    }
}
