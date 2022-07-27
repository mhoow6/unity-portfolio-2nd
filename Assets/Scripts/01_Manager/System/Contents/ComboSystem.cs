using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComboSystem : IGameSystem
{
    public int MaxCombo { get; private set; }
    public Action OnComboDelete;

    int m_Combo;
    float m_ComboMaintainTimer;
    bool m_ComboMaintainTimerClear;
    bool m_ComboStart;

    const float COMBO_MAINTAIN_TIME = 5f;

    public void Init()
    {
        
    }

    public void Tick()
    {
        if (m_ComboMaintainTimerClear)
        {
            m_ComboMaintainTimer = 0f;
            m_ComboMaintainTimerClear = false;
            m_ComboStart = true;
        }

        if (m_ComboStart)
        {
            m_ComboMaintainTimer += Time.deltaTime;
            if (m_ComboMaintainTimer > COMBO_MAINTAIN_TIME)
            {
                MaxCombo = Mathf.Max(m_Combo, MaxCombo);
                m_ComboMaintainTimer = 0f;
                m_Combo = 0;
                m_ComboStart = false;

                OnComboDelete?.Invoke();
            }
        }

    }

    public void Report(int comboCount)
    {
        m_Combo += comboCount;

        m_ComboMaintainTimerClear = true;
    }
}
