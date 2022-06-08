using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [ReadOnly] public Character CurrentCharacter;
    [ReadOnly] public List<Character> Characters = new List<Character>();
    public bool Controlable
    {
        set
        {
            if (value)
            {
                StartCoroutine(m_GetInputCoroutine);
                StartCoroutine(m_ControlCoroutine);
            }
            else
            {
                StopCoroutine(m_GetInputCoroutine);
                StopCoroutine(m_ControlCoroutine);
            }
                
        }
    }
    public bool Moveable;

    public FixedQueue<AniType> AnimationJobs { get; private set; } = new FixedQueue<AniType>(1);
    public Vector3 MoveVector { get; private set; }
    public Vector3 RotateVector { get; private set; }

    IEnumerator m_ControlCoroutine;
    IEnumerator m_GetInputCoroutine;

    const float CHARCTER_ROTATE_SPEED = 20f;

    public void Init()
    {
        RegisterToManager();

        // ���� ù��° �ڽ��� �����̴�.
        for (int i = 0; i < transform.childCount; i++)
        {
            var child = transform.GetChild(i);
            var cha = child.GetComponent<Character>();
            if (cha != null)
            {
                if (i == 0)
                    CurrentCharacter = cha;

                cha.Spawn();

                Characters.Add(cha);
            }
        }

        m_ControlCoroutine = ControlCharacterCoroutine();
        m_GetInputCoroutine = GetInputCoroutine();

        Moveable = true;
    }

    private void Update()
    {
        // ���⿡�� �����Ӻ��� �ִϸ��̼��� �ϳ��� �޾ƿ����� ó��
        if (AnimationJobs.Count > 0)
            CurrentCharacter.Animator.SetInteger(CurrentCharacter.ANITYPE_HASHCODE, (int)AnimationJobs.Dequeue());
    }

    void OnDestroy()
    {
        ReleaseFromManager();
    }

    IEnumerator GetInputCoroutine()
    {
        while (true)
        {
            // ĳ���� �����̱�
            var controllerInput = GameManager.InputSystem.CharacterMoveInput;
            var cam = StageManager.Instance.MainCam;
            if (cam != null)
            {
                Vector3 cameraForward = new Vector3(cam.transform.forward.x, 0, cam.transform.forward.z).normalized;
                Vector3 cameraRight = new Vector3(cam.transform.right.x, 0, cam.transform.right.z).normalized;
                Vector3 moveVector = cameraForward * controllerInput.y + cameraRight * controllerInput.x;

                // ȸ����
                float angle = Vector3.SignedAngle(CurrentCharacter.transform.forward, moveVector, Vector3.up);
                Vector3 angularVelocity = new Vector3(0, angle, 0);
                RotateVector = angularVelocity;

                // �̵���
                MoveVector = moveVector;
            }
            yield return null;
        }
    }

    IEnumerator ControlCharacterCoroutine()
    {
        while (true)
        {
            CurrentCharacter.Rigidbody.MoveRotation(CurrentCharacter.Rigidbody.rotation * Quaternion.Euler(RotateVector));
            if (MoveVector.magnitude != 0 && Moveable)
            {
                // �̵������ϸ� ��������
                Vector3 desired = CurrentCharacter.transform.position + (MoveVector * CurrentCharacter.MoveSpeed * Time.deltaTime);
                CurrentCharacter.Rigidbody.MovePosition(desired);
            }

            yield return new WaitForFixedUpdate();
        }
    }

    void RegisterToManager()
    {
        switch (GameManager.SceneCode)
        {
            case SceneCode.Lobby:
                LobbyManager.Instance.Player = this;
                break;
            default:
                StageManager.Instance.Player = this;
                break;
        }
    }

    void ReleaseFromManager()
    {
        switch (GameManager.SceneCode)
        {
            case SceneCode.Lobby:
                LobbyManager.Instance.Player = null;
                break;
            default:
                StageManager.Instance.Player = null;
                break;
        }
    }
}
