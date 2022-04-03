using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class LoadingTitleMechanism : Register
{
    public List<LoadingTitleRoad> Roads = new List<LoadingTitleRoad>();
    public LoadingTitleRoad LastRoad
    {
        get
        {
            var sortedList = Roads.OrderBy(r => r.transform.position.z).ToList();
            return sortedList.Last();
        }
    }

    LoadingTitleUI m_LoadingUI;

    const float ROAD_MOVE_SPEED = 2.0f;

    public override void RegisterToGameManager()
    {
        GameManager.Instance.Mechanism_LoadingTitle = this;
    }

    public override void ReleaseToGameManager()
    {
        GameManager.Instance.Mechanism_LoadingTitle = null;
    }

    public void StartDirecting(LoadingTitleUI ui)
    {
        m_LoadingUI = ui;

        StartCoroutine(KeepRoadsMovingToCamera());
    }

    IEnumerator KeepRoadsMovingToCamera()
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
        StartCoroutine(MakeRoadsSmoothlyStop());
    }

    IEnumerator MakeRoadsSmoothlyStop()
    {
        float moveSpeed = ROAD_MOVE_SPEED;
        float timer = 0f;
        float sensitivity = 0f;
        while (moveSpeed > 0.05f)
        {
            timer += Time.deltaTime;
            sensitivity += timer * 0.01f;
            moveSpeed = Mathf.Lerp(moveSpeed, 0, sensitivity);
            //Debug.Log($"MoveSpeed: {moveSpeed}");

            if (Roads.Count > 0)
            {
                foreach (var road in Roads)
                    road.transform.position += -road.transform.forward * Time.deltaTime * moveSpeed;
            }
            yield return null;
        }
    }
}
