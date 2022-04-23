using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class Player : MonoBehaviour
{
    [ReadOnly] public Character CurrentCharacter;
    [ReadOnly] public List<Character> Characters = new List<Character>();

    void Start()
    {
        GameManager.Instance.Player = this;

        // ���� ù��°�� Ȱ��ȭ�� ���� ���� ĳ������
        for (int i = 0; i < transform.childCount; i++)
        {
            var child = transform.GetChild(i);
            var cha = child.GetComponent<Character>();

            if (child.gameObject.activeSelf)
                CurrentCharacter = cha;
            else
                child.gameObject.SetActive(false);

            Characters.Add(cha);
        }
    }

    private void Update()
    {
        // ĳ���� �����̱�
        var ControllerInput = GameManager.Instance.InputSystem.ControllerInput;
        var cam = GameManager.Instance.MainCam;
        if (cam != null)
        {
            Vector3 cameraForward = new Vector3(cam.transform.forward.x, 0, cam.transform.forward.z).normalized;
            Vector3 cameraRight = new Vector3(cam.transform.right.x, 0, cam.transform.right.z).normalized;
            Vector3 moveVector = cameraForward * ControllerInput.y + cameraRight * ControllerInput.x;

            if (moveVector.magnitude != 0)
            {
                // ȸ��
                CurrentCharacter.transform.forward = moveVector.normalized;

                // �ִϸ��̼�
                CurrentCharacter.Animator.SetInteger(CurrentCharacter.ANITYPE_HASHCODE, (int)AniType.RUN_0);
            }
            else
            {
                // �ִϸ��̼�
                CurrentCharacter.Animator.SetInteger(CurrentCharacter.ANITYPE_HASHCODE, (int)AniType.IDLE_0);
            }
                

            // �̵�
            CurrentCharacter.transform.position += moveVector * Time.deltaTime * CurrentCharacter.Data.Speed;
        }
    }

    void OnDestroy()
    {
        GameManager.Instance.Player = null;
    }
}
