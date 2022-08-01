using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlarmTrigger : AreaTrigger
{
    public AlarmEvent AlarmEvent;

    protected override void OnAwake()
    {
        m_AutoDisable = true;
    }

    protected override void OnAreaEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        var sm = StageManager.Instance;

        switch (AlarmEvent)
        {
            case AlarmEvent.NONE:
                break;
            case AlarmEvent.SpawnBoss:
                // �˶� ȿ�� �߻�
                var alarmUI = GameManager.UISystem.PushToast<BossWarningUI>(ToastType.BossWarning);
                alarmUI.SetData(() =>
                {
                    var area = sm.Areas.Find(a => a.Index == m_AreaIdx);
                    if (area != null)
                    {
                        area.Wall = true;
                        area.React(AlarmEvent);
                    }
                        
                });
                break;
            default:
                break;
        }
    }
}

public enum AlarmEvent
{
    NONE = 0,
    SpawnBoss,
}