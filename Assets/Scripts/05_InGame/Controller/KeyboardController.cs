using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardController : MonoBehaviour, InputProvider
{
    Vector2 m_Input;
    Vector2 InputProvider.Input => m_Input;

    SkillButtonDisplay m_ConnectedAttackButton;
    SkillButtonDisplay m_ConnectedDashButton;
    SkillButtonDisplay m_ConnectedSkillButton;

    public void ConnectButtons(SkillButtonDisplay attack, SkillButtonDisplay dash, SkillButtonDisplay skill)
    {
        m_ConnectedAttackButton = attack;
        m_ConnectedDashButton = dash;
        m_ConnectedSkillButton = skill;
    }

    public void Dispose()
    {
        m_ConnectedAttackButton = m_ConnectedDashButton = m_ConnectedSkillButton = null;
    }

    void Update()
    {
        m_Input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        // 기본공격
        if (Input.GetMouseButton(0))
            m_ConnectedAttackButton.Press();

        // 대쉬
        if (Input.GetKeyDown(KeyCode.LeftShift))
            m_ConnectedDashButton.Press();

        // 스킬
        if (Input.GetKeyDown(KeyCode.Q))
            m_ConnectedSkillButton.Press();
    }
}
