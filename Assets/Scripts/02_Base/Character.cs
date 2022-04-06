using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public readonly int ANITYPE_HASHCODE = Animator.StringToHash("AniType");
    public Animator Animator { get; protected set; }

    
    protected virtual void OnSpawn()
    {
        Animator = GetComponent<Animator>();
        GetAnimationsWhenUserClick();
    }

    protected virtual void OnDead() { }

    #region ���θ޴� ��Ŀ����
    // ���θ޴����� ĳ���� Ŭ���ÿ� �ִϸ��̼��� �������� �ϴµ� �ʿ���
    public List<AniType> AnimationsWhenUserClick { get; private set; } = new List<AniType>();
    //protected Dictionary<AniType, string> m_AnimationDialogueMap = new Dictionary<AniType, string>();
    bool m_IsAnimationAndDialogSet;

    public void GetAnimationsWhenUserClick()
    {
        if (m_IsAnimationAndDialogSet)
            return;

        // TODO: �����丵
        if (this is Sparcher)
        {
            var table = TableSystem.TableManager.Instance.SparcherAniTypeDialogueTable;
            foreach (var row in table)
            {
                AnimationsWhenUserClick.Add(row.AniType);
                //m_AnimationDialogueMap.Add(row.AniType, row.Dialog);
            }
        }
        m_IsAnimationAndDialogSet = true;
    }

    //public void SpeakDialogueAtMainMenu(AniType type)
    //{
    //    var ui = GameManager.Instance.UISystem.CurrentWindow as MainMenuUI;
    //    if (ui != null && m_AnimationDialogueMap.TryGetValue(type, out string dialogue))
    //    {
    //        ui.CharacterDialog.gameObject.SetActive(true);
    //        ui.CharacterDialog.text = $"{dialogue}";
    //    }
    //}

    #endregion
}
