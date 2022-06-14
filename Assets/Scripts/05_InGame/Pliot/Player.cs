using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Player : MonoBehaviour
{
    [ReadOnly] public Character CurrentCharacter;
    [ReadOnly] public List<Character> Characters = new List<Character>();
    public FixedQueue<AniType> AnimationJobs { get; private set; } = new FixedQueue<AniType>(1);

    private void Update()
    {
        // 여기에서 프레임별로 애니메이션을 하나씩 받아오도록 처리
        if (AnimationJobs.Count > 0)
            CurrentCharacter.Animator.SetInteger(CurrentCharacter.ANITYPE_HASHCODE, (int)AnimationJobs.Dequeue());
    }

    void OnDestroy()
    {
        ReleaseFromManager();
    }


    #region 초기화
    bool m_Init = false;

    /// <summary> 새로운 씬에서 플레이어 초기화하기 </summary>
    public void Init()
    {
        if (m_Init)
            return;
        m_Init = true;
        RegisterToManager();

        // 제일 첫번째 자식이 리더이다.
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

        m_RigidbodyControlCoroutine = ControlRigidbodyCoroutine();
        m_GetInputCoroutine = GetInputCoroutine();
        m_TransformControlCoroutine = ControlTransformCoroutine();
        m_AgentControlCoroutine = 

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

    #region 컨트롤러 활용하여 캐릭터 이동
    public bool Controlable
    {
        set
        {
            if (value)
            {
                StartCoroutine(m_GetInputCoroutine);
                StartCoroutine(m_TransformControlCoroutine);
            }
            else
            {
                StopCoroutine(m_GetInputCoroutine);
                StopCoroutine(m_TransformControlCoroutine);
            }

        }
    }
    [ReadOnly] public bool Moveable;
    public Vector3 MoveVector { get; private set; }
    public Vector3 RotateVector { get; private set; }

    IEnumerator m_RigidbodyControlCoroutine;
    IEnumerator m_TransformControlCoroutine;
    IEnumerator m_AgentControlCoroutine;
    IEnumerator m_GetInputCoroutine;

    const float CHARCTER_ROTATE_SPEED = 20f;
    IEnumerator GetInputCoroutine()
    {
        while (true)
        {
            // 캐릭터 움직이기
            var controllerInput = GameManager.InputSystem.CharacterMoveInput;
            var cam = StageManager.Instance.MainCam;
            if (cam != null)
            {
                // 이동값
                Vector3 cameraForward = new Vector3(cam.transform.forward.x, 0, cam.transform.forward.z).normalized;
                Vector3 cameraRight = new Vector3(cam.transform.right.x, 0, cam.transform.right.z).normalized;
                Vector3 moveVector = cameraForward * controllerInput.y + cameraRight * controllerInput.x;
                MoveVector = moveVector;

                // 회전값
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

            #region 예외처리
            if (MoveVector.magnitude == 0)
                skip = true;
            if (!Moveable)
                skip = true;
            #endregion

            if (!skip)
            {
                // 회전
                CurrentCharacter.transform.forward = Vector3.Lerp(CurrentCharacter.transform.forward, MoveVector.normalized, Time.deltaTime * CHARCTER_ROTATE_SPEED);

                // 이동
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

            bool skip = false;

            #region 예외처리
            if (MoveVector.magnitude == 0)
                skip = true;
            if (!Moveable)
                skip = true;
            #endregion

            if (!skip)
            {
                // 이동가능하면 움직이자
                Vector3 desired = CurrentCharacter.transform.position + (MoveVector * CurrentCharacter.MoveSpeed * Time.fixedDeltaTime);
                CurrentCharacter.Rigidbody.MovePosition(desired);
            }

            yield return new WaitForFixedUpdate();
        }
    }

    IEnumerator ControlAgentCoroutine()
    {
        while (true)
        {
            bool skip = false;

            #region 예외처리
            if (MoveVector.magnitude == 0)
                skip = true;
            if (!Moveable)
                skip = true;
            #endregion

            if (!skip)
            {

            }

            yield return null;
        }
    }
    #endregion

    #region 목적지로 캐릭터 무조건 이동
    public bool SmoothlyMoving { get; private set; }

    /// <summary> 목적지로 캐릭터가 정해진 시간안에 이동하게 합니다. </summary>
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

        while (t < 1)
        {
            t += Time.deltaTime / desiredTime;

            Vector3 desired = Vector3.Lerp(CurrentCharacter.transform.position, destination, t);
            CurrentCharacter.Agent.destination = desired;

            yield return null;
        }
        SmoothlyMoving = false;
    }
    #endregion
}
