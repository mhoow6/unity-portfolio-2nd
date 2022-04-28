using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardController : MonoBehaviour, InputProvider
{
    Vector2 m_Input;
    Vector2 InputProvider.Input => m_Input;

    void Update()
    {
        m_Input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
    }
}
