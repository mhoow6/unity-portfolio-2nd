using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UIType
{
    MainLobby,
    Loading,
    Adventure,
    NickNameInput,
    Equipment, // �̱���
    Shop, // �̱���
    Quest, // �̱���
    Warning,
    Setting, // �̱���
    Confirm,
    ReadyForBattle,
    InGame,
    BattleResult, // �̱���
    WeaponDetail, // �̱���
    ItemDetail, // �̱���
    CharacterDetail, // �̱���
    Sortie,
    SceneTransition,
}

public abstract class UI : MonoBehaviour
{
    public abstract UIType Type { get; }
    public abstract void OnOpened();
    public abstract void OnClosed();
}
