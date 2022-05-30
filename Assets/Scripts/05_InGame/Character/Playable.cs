using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DatabaseSystem;

public class Playable : Character
{
    #region �κ� �ִϸ��̼�
    // �κ񿡼� ĳ���� Ŭ���ÿ� �ִϸ��̼��� �������� �ϴµ� �ʿ���
    public List<AniType> LobbyAnimations { get; protected set; } = new List<AniType>();
    protected void SetLobbyAnimations(string runtimeAnimatorControllerPath)
    {
        var table = TableManager.Instance.AniTypeDialogueTable.FindAll(row => row.ObjectCode == Code);
        foreach (var row in table)
            LobbyAnimations.Add((AniType)row.AniType);

        var currentScene = GameManager.SceneCode;
        var config = GameManager.GameDevelopSettings;
        switch (currentScene)
        {
            case SceneCode.Lobby:
                Animator.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>($"{config.AnimationControllerResourcePath}/{runtimeAnimatorControllerPath}");
                break;
            default:
                Animator.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>($"{config.AnimationControllerResourcePath}/Sparcher/InGame_Sparcher");
                break;
        }
    }
    #endregion

    protected override void OnSpawn()
    {
        gameObject.tag = "Player";
        gameObject.layer = GameManager.GameDevelopSettings.BaseObjectLayermask;
    }

    protected override void OnDamaged(Character attacker, int damage, DamageType damageType)
    {
        StageManager.Instance.MissionSystem.ReportAll(QuestType.GET_DAMAGED);
    }

    protected override void OnDead(Character attacker, int damage, DamageType damageType)
    {
        StageManager.Instance.MissionSystem.ReportAll(QuestType.INCAPCITATED);
    }
}
