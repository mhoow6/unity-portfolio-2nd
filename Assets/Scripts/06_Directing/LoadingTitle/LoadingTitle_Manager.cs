using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class LoadingTitle_Manager : MonoSingleton<LoadingTitle_Manager>
{
    public List<LoadingTitle_Road> Roads = new List<LoadingTitle_Road>();
    public LoadingTitle_Road LastRoad
    {
        get
        {
            var sortedList = Roads.OrderBy(r => r.transform.position.z).ToList();
            return sortedList.Last();
        }
    }

    LoadingUI m_LoadingUI;

    const float ROAD_MOVE_SPEED = 2.0f;

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
                    road.transform.position += -road.transform.forward * Time.deltaTime * ROAD_MOVE_SPEED;
            }

            yield return null;
        }
        StartCoroutine(SmoothlyStop());
    }

    IEnumerator SmoothlyStop()
    {
        float moveSpeed = ROAD_MOVE_SPEED;
        float timer = 0f;
        while (moveSpeed > 0.05f)
        {
            timer += Time.deltaTime;
            moveSpeed = Mathf.Lerp(moveSpeed, 0, timer);
            Debug.Log($"MoveSpeed: {moveSpeed}");

            if (Roads.Count > 0)
            {
                foreach (var road in Roads)
                    road.transform.position += -road.transform.forward * Time.deltaTime * moveSpeed;
            }
            yield return null;
        }
    }
}
