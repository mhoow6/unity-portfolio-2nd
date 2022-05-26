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
                StartCoroutine(m_ControlCoroutine);
            else
                StopCoroutine(m_ControlCoroutine);
        }
    }
    public FixedQueue<AniType> AnimationJobs { get; private set; } = new FixedQueue<AniType>(1);
    public Vector3 MoveVector { get; private set; }
    public bool Moveable;

    IEnumerator m_ControlCoroutine;

    const float CHARCTER_ROTATE_SPEED = 20f;

    public void Init()
    {
        GameManager.Instance.Player = this;

        // 제일 첫번째로 활성화된 것이 현재 캐릭터임
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
        Moveable = true;
    }

    private void Update()
    {
        // 여기에서 프레임별로 애니메이션을 하나씩 받아오도록 처리
        if (AnimationJobs.Count > 0)
            CurrentCharacter.Animator.SetInteger(CurrentCharacter.ANITYPE_HASHCODE, (int)AnimationJobs.Dequeue());
    }

    void OnDestroy()
    {
        GameManager.Instance.Player = null;
    }

    IEnumerator ControlCoroutine()
    {
        while (true)
        {
            // 캐릭터 움직이기
            var ControllerInput = GameManager.InputSystem.CharacterMoveInput;

            var cam = GameManager.MainCam;
            if (cam != null)
            {
                Vector3 cameraForward = new Vector3(cam.transform.forward.x, 0, cam.transform.forward.z).normalized;
                Vector3 cameraRight = new Vector3(cam.transform.right.x, 0, cam.transform.right.z).normalized;
                Vector3 moveVector = cameraForward * ControllerInput.y + cameraRight * ControllerInput.x;

                if (moveVector.magnitude != 0)
                {
                    // 회전
                    float angle = Vector3.SignedAngle(CurrentCharacter.transform.forward, moveVector, Vector3.up);
                    Vector3 angularVelocity = new Vector3(0, angle * Time.fixedDeltaTime * CHARCTER_ROTATE_SPEED, 0);
                    CurrentCharacter.Rigidbody.MoveRotation(CurrentCharacter.Rigidbody.rotation * Quaternion.Euler(angularVelocity));

                    // 애니메이션
                    AnimationJobs.Enqueue(AniType.RUN_0);
                }
                else
                {
                    // 애니메이션
                    AnimationJobs.Enqueue(AniType.IDLE_0);
                }

                // 이동
                if (Moveable)
                {
                    Vector3 desired = CurrentCharacter.transform.position + (moveVector * CurrentCharacter.Speed * Time.deltaTime);
                    CurrentCharacter.Rigidbody.MovePosition(desired);
                }

                MoveVector = moveVector;
            }

            yield return new WaitForFixedUpdate();
        }
    }
}
