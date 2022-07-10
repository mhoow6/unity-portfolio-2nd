using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlarmTrigger : AreaTrigger
{
    public AlarmEvent AlarmEvent;

    protected override void OnAwake()
    {
        m_AutoDisable = true;
        m_AutoWall = true;
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
                // 알람 효과 발생
                var alarmUI = GameManager.UISystem.PushToast<BossWarningUI>(ToastType.BossWarning);
                alarmUI.SetData(() =>
                {
                    var area = sm.Areas.Find(a => a.Index == m_AreaIdx);
                    if (area != null)
                        area.React(AlarmEvent);
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

public interface IAlarmReactable
{
    public void React(AlarmEvent alarmEvent);
}