using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Player : MonoBehaviour
{
    [ReadOnly] public Playable CurrentCharacter;
    [ReadOnly] public List<Playable> Characters = new List<Playable>();
    public FixedQueue<AniType> AnimationJobs { get; private set; } = new FixedQueue<AniType>(1);

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


    #region �ʱ�ȭ
    bool m_Init = false;

    /// <summary> ���ο� ������ �÷��̾� �ʱ�ȭ�ϱ� </summary>
    public void Init()
    {
        if (m_Init)
            return;
        m_Init = true;
        RegisterToManager();

        // ���� ù��° �ڽ��� �����̴�.
        for (int i = 0; i < transform.childCount; i++)
        {
            var child = transform.GetChild(i);
            var cha = child.GetComponent<Playable>();
            if (cha != null)
            {
                if (i == 0)
                    CurrentCharacter = cha;

                cha.Spawn();

                Characters.Add(cha);
            }
        }

        m_RigidbodyControlCoroutine = ControlRigidbodyCoroutine();
        m_GetInputCoroutine = GetInputCoroutine();
        m_TransformControlCoroutine = ControlTransformCoroutine();

        Moveable = true;
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
    #endregion

    #region ĳ���� �̵�

    #region ����̽� Ȱ��
    public bool Controlable
    {
        set
        {
            if (value)
            {
                StartCoroutine(m_GetInputCoroutine);
                StartCoroutine(m_RigidbodyControlCoroutine);
            }
            else
            {
                StopCoroutine(m_GetInputCoroutine);
                StopCoroutine(m_RigidbodyControlCoroutine);
            }

        }
    }
    [ReadOnly] public bool Moveable;
    public Vector3 MoveVector { get; private set; }
    public Vector3 RotateVector { get; private set; }

    IEnumerator m_RigidbodyControlCoroutine;
    IEnumerator m_TransformControlCoroutine;
    IEnumerator m_GetInputCoroutine;

    const float CHARCTER_ROTATE_SPEED = 20f;
    IEnumerator GetInputCoroutine()
    {
        while (true)
        {
            // ĳ���� �����̱�
            var controllerInput = GameManager.InputSystem.CharacterMoveInput;
            var cam = StageManager.Instance.MainCam;
            if (cam != null)
            {
                // �̵���
                Vector3 cameraForward = new Vector3(cam.transform.forward.x, 0, cam.transform.forward.z).normalized;
                Vector3 cameraRight = new Vector3(cam.transform.right.x, 0, cam.transform.right.z).normalized;
                Vector3 moveVector = cameraForward * controllerInput.y + cameraRight * controllerInput.x;

                if (Moveable)
                    MoveVector = moveVector;
                else
                    MoveVector = Vector3.zero;

                // ȸ����
                float angle = Vector3.SignedAngle(CurrentCharacter.transform.forward, moveVector, Vector3.up);
                Vector3 angularVelocity = new Vector3(0, angle, 0);
                RotateVector = angularVelocity;
            }
            yield return null;
        }
    }

    IEnumerator ControlTransformCoroutine()
    {
        while (true)
        {
            bool skip = false;

            #region ����ó��
            if (MoveVector.magnitude == 0)
                skip = true;
            if (!Moveable)
                skip = true;
            #endregion

            if (!skip)
            {
                // ȸ��
                CurrentCharacter.transform.forward = Vector3.Lerp(CurrentCharacter.transform.forward, MoveVector.normalized, Time.deltaTime * CHARCTER_ROTATE_SPEED);

                // �̵�
                CurrentCharacter.transform.position += MoveVector * Time.deltaTime * CurrentCharacter.MoveSpeed;
            }
                

            yield return null;
        }
    }

    IEnumerator ControlRigidbodyCoroutine()
    {
        while (true)
        {
            CurrentCharacter.Rigidbody.MoveRotation(CurrentCharacter.Rigidbody.rotation * Quaternion.Euler(RotateVector));

            bool movePositionSkip = false;

            #region ����ó��
            if (MoveVector.magnitude == 0)
                movePositionSkip = true;
            if (!Moveable)
                movePositionSkip = true;
            #endregion

            if (!movePositionSkip)
            {
                // �̵������ϸ� ��������
                Vector3 desired = CurrentCharacter.transform.position + (MoveVector * CurrentCharacter.MoveSpeed * Time.fixedDeltaTime);
                CurrentCharacter.Rigidbody.MovePosition(desired);
            }

            yield return new WaitForFixedUpdate();
        }
    }
    #endregion

    #region �������� ĳ���� ������ �̵�
    public bool SmoothlyMoving { get; private set; }

    /// <summary> �������� ĳ���Ͱ� ������ �ð��ȿ� �̵��ϰ� �մϴ�. </summary>
    public void SmoothlyMovingTo(Vector3 destination, float desiredTime)
    {
        if (SmoothlyMoving)
            return;

        StartCoroutine(SmoothlyMovingCoroutine(destination, desiredTime));
    }

    IEnumerator SmoothlyMovingCoroutine(Vector3 destination, float desiredTime)
    {
        float t = 0f;
        SmoothlyMoving = true;

        // ���� ĳ���ͷκ��� ������������ �Ÿ�����
        Vector3 adjust = destination - CurrentCharacter.transform.position;
        while (t < 1)
        {
            t += Time.fixedDeltaTime / desiredTime;

            // Force�� ����ؼ� �������� ���� �����ϱ� ����
            CurrentCharacter.Rigidbody.velocity = Vector3.zero;

            CurrentCharacter.Rigidbody.AddForce(adjust / desiredTime, ForceMode.VelocityChange);

            yield return new WaitForFixedUpdate();
        }
        SmoothlyMoving = false;
    }
    #endregion

    #endregion
}