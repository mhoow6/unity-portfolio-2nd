using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdventureUI : UI
{
    public StatusUI StatusDisplay;
    public WorldViewUI WorldView;

    public override UIType Type => UIType.Adventure;

    // 추후 또 다른 월드가 나오면 로직을 수정해야함
    const int WORLD_IDX = 1;

    public override void OnClosed()
    {
        
    }

    public override void OnOpened()
    {
        StatusDisplay.SetData();

        if (!WorldView.Content.IsNullOrDestroyed())
        {
            Destroy(WorldView.Content.gameObject);
            WorldView.Content = null;
        }

        WorldView.SetData(WORLD_IDX);
    }
}
