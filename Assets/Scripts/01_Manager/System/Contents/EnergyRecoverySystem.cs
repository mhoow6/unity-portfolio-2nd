using System;
using System.Collections;
using System.Collections.Generic;
using DatabaseSystem;
using UnityEngine;

public class EnergyRecoverySystem : IGameSystem
{
    PlayerData m_PlayerData;
    const int RECOVERY_MINUTE = 1;

    public void Init()
    {
        m_PlayerData = GameManager.PlayerData;
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
