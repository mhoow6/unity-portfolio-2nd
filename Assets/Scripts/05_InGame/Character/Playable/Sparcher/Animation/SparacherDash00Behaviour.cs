using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SparacherDash00Behaviour : SparcherBehaviour
{
    protected override AniType m_AniType => AniType.DASH_0;

    protected override void OnAnimationEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnAnimationEnter(animator, stateInfo, layerIndex);

        // UNDONE: 캐릭터 앞으로 튀어나게끔 구현하기
        var current = m_Player.CurrentCharacter;
        current.Rigidbody.MovePosition(current.transform.forward * 10f);
    }
}
