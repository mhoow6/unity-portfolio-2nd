using System;
using System.Collections;
using System.Collections.Generic;
using DatabaseSystem;
using UnityEngine;

public class EnergyRecoverySystem : IGameSystem
{
    const int RECOVERY_MINUTE = 1;

    public void Init()
    {
        
    }

    public void Tick()
    {
        int maxEnergy = TableManager.Instance.PlayerLevelEnergyTable[GameManager.PlayerData.Level - 1].MaxEnergy;
        if (GameManager.PlayerData.Energy >= maxEnergy)
            return;

        TimeSpan interval = DateTime.Now - GameManager.PlayerData.LastEnergyUpdateTime;
        if (interval.Minutes >= RECOVERY_MINUTE)
            AddEnergy();
    }

    public void AddEnergy(int addEnergy = 1)
    {
        GameManager.PlayerData.Energy += addEnergy;
        GameManager.PlayerData.LastEnergyUpdateTime = DateTime.Now;
    }
}
