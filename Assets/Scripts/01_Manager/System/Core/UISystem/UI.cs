using DatabaseSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UIType
{
    MainLobby,
    LoadingTitle,
    Adventure,
    NickNameInput,
    Warning,
    Confirm,
    ReadyForBattle,
    InGame,
    Sortie,
    Logo,
    StageClear,
    BattleResult
}

public abstract class UI : MonoBehaviour
{
    public abstract UIType Type { get; }
    public abstract void OnOpened();
    public abstract void OnClosed();

    /// <summary> Äù½ºÆ® ÀÎµ¦½º¿¡ µû¸¥ Äù½ºÆ® ¼³¸í </summary>
    protected string QuestDescription(int questIdx)
    {
        var questData = TableManager.Instance.QuestTable.Find(quest => quest.Index == questIdx);
        if (questData.Index == 0)
            return string.Empty;

        var quest1DescriptionData = TableManager.Instance.QuestDescriptionTable.Find(quest => quest.Type == questData.Type);
        if (questData.Target > 0)
        {
            if (Enum.IsDefined(typeof(ObjectCode), questData.Target))
                return string.Format(quest1DescriptionData.Description, questData.PurposeCount, Character.GetName((ObjectCode)questData.Target));
            else
                return string.Format(quest1DescriptionData.Description, questData.PurposeCount, questData.Target);
        }
        else
            return string.Format(quest1DescriptionData.Description, questData.PurposeCount);
    }
}
