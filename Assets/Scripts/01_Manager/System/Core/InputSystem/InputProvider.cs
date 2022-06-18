using UnityEngine;

public interface InputProvider
{
    public string DeviceName { get; }
    public Vector2 Input { get; }
}
