using System;
using System.Collections;
using System.Collections.Generic;
using TableSystem;
using UnityEngine;

public class EnergyRecoverySystem : GameSystem
{
    PlayerData m_PlayerData;
    int m_RecoveryMinute;

    public void Init()
    {
        m_PlayerData = GameManager.Instance.PlayerData;
        m_RecoveryMinute = GameManager.Instance.Config.EnergyRecoveryMinute;
    }

    public void Tick()
    {
        int maxEnergy = TableManager.Instance.PlayerLevelEnergyTable.Find(info => info.Level == m_PlayerData.Level).MaxEnergy;
        if (m_PlayerData.Energy >= maxEnergy)
            return;

        TimeSpan interval = DateTime.Now - m_PlayerData.LastEnergyUpdateTime;
        if (interval.Minutes >= m_RecoveryMinute)
        {
            m_PlayerData.Energy++;
            m_PlayerData.LastEnergyUpdateTime = DateTime.Now;
        }
    }
}
