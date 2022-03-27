using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameSystem : MonoBehaviour
{
    public abstract void Init();
    public abstract void Tick();
}
