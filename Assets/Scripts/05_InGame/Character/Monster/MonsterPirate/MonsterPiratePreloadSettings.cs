using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MonsterPirate_PreloadSettings", menuName = "Preload/MonsterPirate Preload Settings", order = 1)]
public class MonsterPiratePreloadSettings : PreloadSettings
{
    public GameObject Prefab_BulletFired;
    public GameObject Prefab_Bullet;

    public override void Instantitate()
    {
        var sm = StageManager.Instance;
        if (sm == null)
            return;

        if (sm.PreloadZone)
        {
            var bulletfired = sm.PoolSystem.Load<MonsterPirateBulletFiredEffect>(Prefab_BulletFired);
            bulletfired.gameObject.SetActive(false);
            sm.PoolSystem.Release(bulletfired);

            var bullet = sm.PoolSystem.Load<MonsterPirateBullet>(Prefab_Bullet);
            bullet.gameObject.SetActive(false);
            sm.PoolSystem.Release(bullet);
        }
    }
}
