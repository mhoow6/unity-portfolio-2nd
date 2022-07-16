using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayablePreloadSettings : PreloadSettings
{
    public abstract void Instantitate(Playable parent);
}
