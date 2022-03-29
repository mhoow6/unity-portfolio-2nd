using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class LoadingTitle_Manager : MonoSingleton<LoadingTitle_Manager>
{
    public List<LoadingTitle_Road> Roads = new List<LoadingTitle_Road>();
    public float RoadMoveSpeed = 2.0f;

    public LoadingTitle_Road FarRoad
    {
        get
        {
            var sortedList = Roads.OrderBy(r => r.transform.position.z).ToList();
            return sortedList.Last();
        }
    }

    private void Update()
    {
        if (Roads.Count > 0)
        {
            foreach (var road in Roads)
                road.transform.position += -road.transform.forward * Time.deltaTime * RoadMoveSpeed;
        }
    }
}
