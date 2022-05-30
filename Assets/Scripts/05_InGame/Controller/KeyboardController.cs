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

        // �⺻����
        if (Input.GetMouseButton(0))
            GameManager.InputSystem.CharacterAttackInput = true;
        else
            GameManager.InputSystem.CharacterAttackInput = false;

        // �뽬
        if (Input.GetKeyDown(KeyCode.LeftShift))
            GameManager.InputSystem.CharacterDashInput = true;
        else
            GameManager.InputSystem.CharacterDashInput = false;

        // ��ų
        if (Input.GetKeyDown(KeyCode.Q))
            GameManager.InputSystem.CharacterUltiInput = true;
        else
            GameManager.InputSystem.CharacterUltiInput = false;
    }
}
