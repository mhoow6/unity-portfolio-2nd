using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TableSystem;

public class Character : MonoBehaviour
{
    public readonly int ANITYPE_HASHCODE = Animator.StringToHash("AniType");
    public Animator Animator { get; private set; }

    public virtual CharacterCode Code { get; }
    public CharacterType Type { get; private set; }
    public string Name { get; private set; }

    public CharacterData Data;

    protected void Start()
    {
        // ������Ʈ ���̱�
        if (!Animator)
            Animator = GetComponent<Animator>();
        // ---

        SetMainMenuAnimations();
        SetPropertiesFromTable();      

        OnSpawn();
    }

    protected void OnDestroy()
    {
        OnDead();
    }

    protected virtual void OnSpawn() { }

    protected virtual void OnDead() { }

    #region ���θ޴�
    // ���θ޴����� ĳ���� Ŭ���ÿ� �ִϸ��̼��� �������� �ϴµ� �ʿ���
    public List<AniType> AnimationsWhenUserClick { get; protected set; } = new List<AniType>();
    bool m_IsAnimationAndDialogSet;

    void SetMainMenuAnimations()
    {
        if (m_IsAnimationAndDialogSet)
            return;

        var table = TableManager.Instance.AniTypeDialogueTable.FindAll(row => row.ObjectCode == Code);
        foreach (var row in table)
            AnimationsWhenUserClick.Add(row.AniType);

        m_IsAnimationAndDialogSet = true;
    }

    void SetPropertiesFromTable()
    {
        var table = TableManager.Instance.CharacterTable.Find(c => c.Code == Code);
        var record = GameManager.Instance.PlayerData.CharacterDatas.Find(c => c.Code == Code);

        Name = table.Name;
        Type = table.Type;

        if (record != null)
            Data = record;
        else
            Data = GameManager.Instance.CharacterSystem.GetData(Code);
    }
    #endregion
}
