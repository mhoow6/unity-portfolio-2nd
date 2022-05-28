using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DatabaseSystem;

public class Playable : Character
{
    #region 메인메뉴 애니메이션
    // 메인메뉴에서 캐릭터 클릭시에 애니메이션이 나오도록 하는데 필요함
    public List<AniType> AnimationsWhenUserClick { get; protected set; } = new List<AniType>();
    bool m_IsAnimationAndDialogSet;
    protected void SetMainMenuAnimations(string runtimeAnimatorControllerPath)
    {
        if (m_IsAnimationAndDialogSet)
            return;

        var table = TableManager.Instance.AniTypeDialogueTable.FindAll(row => row.ObjectCode == Code);
        foreach (var row in table)
            AnimationsWhenUserClick.Add((AniType)row.AniType);

        var currentScene = GameManager.SceneCode;
        var config = GameManager.GameDevelopSettings;
        switch (currentScene)
        {
            case SceneCode.Lobby:
                Animator.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>($"{config.AnimationControllerResourcePath}/{runtimeAnimatorControllerPath}");
                break;
        }

        m_IsAnimationAndDialogSet = true;
    }
    #endregion

    protected override void OnDamaged(Character attacker, float updateHp)
    {
        StageManager.Instance.MissionSystem.ReportAll(QuestType.GET_DAMAGED);
    }

    protected override void OnDead()
    {
        StageManager.Instance.MissionSystem.ReportAll(QuestType.INCAPCITATED);
    }
}
