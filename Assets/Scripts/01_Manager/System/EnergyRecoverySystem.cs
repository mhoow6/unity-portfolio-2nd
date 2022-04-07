using System;
using System.Collections;
using System.Collections.Generic;
using TableSystem;
using UnityEngine;

public class EnergyRecoverySystem : GameSystem
{
    PlayerData m_PlayerData;
    const int RECOVERY_MINUTE = 10;

    public void Init()
    {
        m_PlayerData = GameManager.Instance.PlayerData;
    }

    public void Tick()
    {
        int maxEnergy = TableManager.Instance.PlayerLevelEnergyTable.Find(info => info.Level == m_PlayerData.Level).MaxEnergy;
        if (m_PlayerData.Energy >= maxEnergy)
            return;

        TimeSpan interval = DateTime.Now - m_PlayerData.LastEnergyUpdateTime;
        if (interval.Minutes >= RECOVERY_MINUTE)
        {
            m_PlayerData.Energy++;
            m_PlayerData.LastEnergyUpdateTime = DateTime.Now;
        }
    }
}
