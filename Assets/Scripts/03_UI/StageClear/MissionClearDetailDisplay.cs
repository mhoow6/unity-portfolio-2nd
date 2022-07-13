using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MissionClearDetailDisplay : Display
{
    public Text Mission;
    public GameObject ClearBadge;
    public GameObject FailBadge;
    public GameObject ClearRewards;

    public void SetData(bool clear, string missionText)
    {
        Mission.text = missionText;

        ClearBadge.gameObject.SetActive(clear);
        ClearRewards.gameObject.SetActive(clear);

        FailBadge.gameObject.SetActive(!clear);
    }
}
