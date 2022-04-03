using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AniType
{
    IDLE,
    RUN,
    DEAD_1,
    DEAD_2,
    DEAD_3,
    ATTACK,
    JUMP,
}

public class Character : MonoBehaviour
{
    public readonly int ANITYPE_HASHCODE = Animator.StringToHash("AniType");
    public Animator Animator { get; protected set; }

    
    protected virtual void OnSpawn()
    {
        Animator = GetComponent<Animator>();
        GetDialogueUsedInMainMenu();
    }

    protected virtual void OnDead() { }

    #region 메인메뉴 매커니즘
    // 메인메뉴에서 캐릭터 클릭시에 애니메이션이 나오도록 하는데 필요함
    public List<AniType> AnimationsWhenUserClick { get; private set; } = new List<AniType>();
    protected Dictionary<AniType, string> m_AnimationDialogueMap = new Dictionary<AniType, string>();
    public bool GetDialogue;

    public void GetDialogueUsedInMainMenu()
    {
        if (GetDialogue)
            return;

        // TODO: 리팩토링
        if (this is Sparcher)
        {
            var table = TableSystem.TableManager.Instance.SparcherAniTypeDialogueTable;
            foreach (var row in table)
            {
                AnimationsWhenUserClick.Add(row.AniType);
                m_AnimationDialogueMap.Add(row.AniType, row.Dialog);
            }
        }
        GetDialogue = true;
    }

    public void SpeakDialogueAtMainMenu(AniType type)
    {
        var ui = GameManager.Instance.System_UI.CurrentWindow as MainMenuUI;
        if (ui != null && m_AnimationDialogueMap.TryGetValue(type, out string dialogue))
            ui.CharacterDialog.text = $"{dialogue}";
    }
    
    #endregion
}
