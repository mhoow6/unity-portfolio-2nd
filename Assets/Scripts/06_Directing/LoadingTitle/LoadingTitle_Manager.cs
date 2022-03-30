using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class LoadingTitle_Manager : MonoSingleton<LoadingTitle_Manager>
{
    public List<LoadingTitle_Road> Roads = new List<LoadingTitle_Road>();
    const float RoadMoveSpeed = 2.0f;
    LoadingUI m_LoadingUI;

    public LoadingTitle_Road LastRoad
    {
        get
        {
            var sortedList = Roads.OrderBy(r => r.transform.position.z).ToList();
            return sortedList.Last();
        }
    }

    public void StartDirecting(LoadingUI ui)
    {
        m_LoadingUI = ui;

        StartCoroutine(KeepMovingToCamera());
    }

    IEnumerator KeepMovingToCamera()
    {
        while (!m_LoadingUI.IsLoadingComplete)
        {
            if (Roads.Count > 0)
            {
                foreach (var road in Roads)
                    road.transform.position += -road.transform.forward * Time.deltaTime * RoadMoveSpeed;
            }

            yield return null;
        }
        StartCoroutine(SmoothlyStop());
    }

    IEnumerator SmoothlyStop()
    {
        while (true)
        {
            yield return null;
        }
    }
}
