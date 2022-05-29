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
    public FixedQueue<AniType> AnimationJobs { get; private set; } = new FixedQueue<AniType>(1);
    public Vector3 MoveVector { get; private set; }
    public Vector3 RotateVector { get; private set; }
    public bool Moveable;

    IEnumerator m_ControlCoroutine;
    IEnumerator m_GetInputCoroutine;

    const float CHARCTER_ROTATE_SPEED = 20f;

    public void Init()
    {
        RegisterToGameSceneManager();

        // ���� ù��°�� Ȱ��ȭ�� ���� ���� ĳ������
        for (int i = 0; i < transform.childCount; i++)
        {
            var child = transform.GetChild(i);
            var cha = child.GetComponent<Character>();
            if (cha != null)
            {
                cha.tag = "Player";

                if (child.gameObject.activeSelf)
                    CurrentCharacter = cha;

                Characters.Add(cha);
            }
        }

        m_ControlCoroutine = ControlCoroutine();
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
        ReleaseFromGameManager();
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
                Vector3 angularVelocity = new Vector3(0, angle * Time.fixedDeltaTime * CHARCTER_ROTATE_SPEED, 0);
                RotateVector = angularVelocity;

                // �̵���
                MoveVector = moveVector;
            }
            yield return null;
        }
    }

    IEnumerator ControlCoroutine()
    {
        while (true)
        {
            CurrentCharacter.Rigidbody.MoveRotation(CurrentCharacter.Rigidbody.rotation * Quaternion.Euler(RotateVector));
            if (MoveVector.magnitude != 0 && Moveable)
            {
                // �̵������ϸ� ��������
                Vector3 desired = CurrentCharacter.transform.position + (MoveVector * CurrentCharacter.Speed * Time.deltaTime);
                CurrentCharacter.Rigidbody.MovePosition(desired);

                // �ִϸ��̼�
                AnimationJobs.Enqueue(AniType.RUN_0);
            }
            else
                AnimationJobs.Enqueue(AniType.IDLE_0);

            yield return new WaitForFixedUpdate();
        }
    }

    void RegisterToGameSceneManager()
    {
        switch (GameManager.SceneCode)
        {
            case SceneCode.Logo:
                break;
            case SceneCode.Lobby:
                LobbyManager.Instance.Player = this;
                break;
            case SceneCode.Stage0101:
                StageManager.Instance.Player = this;
                break;
            case SceneCode.None:
                break;
            default:
                break;
        }
    }

    void ReleaseFromGameManager()
    {
        switch (GameManager.SceneCode)
        {
            case SceneCode.Logo:
                break;
            case SceneCode.Lobby:
                LobbyManager.Instance.Player = null;
                break;
            case SceneCode.Stage0101:
                StageManager.Instance.Player = null;
                break;
            case SceneCode.None:
                break;
            default:
                break;
        }
    }
}
