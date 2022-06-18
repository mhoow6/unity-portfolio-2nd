using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardController : MonoBehaviour, InputProvider
{
    Vector2 m_Input;

    public string DeviceName => "Keyboard";

    Vector2 InputProvider.Input => m_Input;

    void Update()
    {
        m_Input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        // 기본공격
        if (Input.GetMouseButton(0))
            GameManager.InputSystem.PressAButton = true;
        else
            GameManager.InputSystem.PressAButton = false;

        // 대쉬
        if (Input.GetKeyDown(KeyCode.LeftShift))
            GameManager.InputSystem.PressXButton = true;
        else
            GameManager.InputSystem.PressXButton = false;

        // 스킬
        if (Input.GetKeyDown(KeyCode.Q))
            GameManager.InputSystem.PressBButton = true;
        else
            GameManager.InputSystem.PressBButton = false;
    }
}
