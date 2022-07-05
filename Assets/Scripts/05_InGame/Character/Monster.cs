using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using DatabaseSystem;

public abstract class Monster : Character
{
    #region AI
    public NavMeshAgent Agent { get; private set; }
    public Behaviorable BehaviorData { get; private set; }

    public static Behaviorable GetBehaviorData(ObjectCode objectCode)
    {
        Behaviorable result = null;

        switch (objectCode)
        {
            case ObjectCode.CHAR_MonsterPirate:
                result = JsonManager.Instance.JsonDatas[4000] as Behaviorable;
                break;
            default:
                break;
        }

        return result;
    }

    public void GoingTo(Vector3 posiiton)
    {
        Agent.SetDestination(posiiton);
        OnGoingTo(posiiton);
    }
    #endregion

    #region LookAt
    public void LookAtLerp(Quaternion desired, float rotateTime = 3f, Action lookAtDoneCallback = null)
    {
        StartCoroutine(LookAtLerpCoroutine(desired, rotateTime, lookAtDoneCallback));
    }

    public void LookAtWith(Transform target, Action doSomething)
    {
        transform.LookAt(target);
        doSomething?.Invoke();
    }
    #endregion

    public FixedQueue<AniType> AnimationJobs { get; private set; } = new FixedQueue<AniType>(1);

    // -----------------------------------------------------------------------

    #region AI
    protected virtual void OnGoingTo(Vector3 position) { }
    #endregion

    protected override void OnSpawn()
    {
        var manager = StageManager.Instance;
        if (manager != null)
            manager.Monsters.Add(this);

        gameObject.tag = "Monster";

        Agent = GetComponent<NavMeshAgent>();
        Agent.updateRotation = true;

        // BehaviorData 가져오기
        BehaviorData = GetBehaviorData(Code);
        Agent.speed = MoveSpeed;
        OnMoveSpeedUpdate += (speed) =>
        {
            Agent.speed = speed;
        };
    }

    protected override void OnLive()
    {
        if (AnimationJobs.Count > 0)
            Animator.SetInteger(ANITYPE_HASHCODE, (int)AnimationJobs.Dequeue());
    }

    protected override void OnDead(Character attacker, int damage)
    {
        var manager = StageManager.Instance;
        if (manager != null)
        {
            // 스테이지 매니저의 몬스터 리스트에서 삭제
            manager.Monsters.Remove(this);

            // 자신이 태어난 스포너가 있으면 알려주기
            AreaSpawner spawner = null;
            foreach (var area in manager.Areas)
            {
                spawner = area.FindSpawner(this);
                if (spawner != null)
                    break;
            }
            if (spawner != null)
                spawner.NotifyDead(this);
        }

        // 죽는 애니메이션
        AnimationJobs.Enqueue(AniType.DEAD_0);

        // 시간이 지나서 자동으로 사라지도록 함
        StartCoroutine(OnDeadCoroutine());

        // 플레이어가 적을 죽였으니 적 처치 횟수 증가
        StageManager.Instance.MissionSystem.ReportAll(QuestType.KILL_ENEMY);

        if (m_TargetLockOnImage)
            GameManager.UISystem.Pool.Release(m_TargetLockOnImage);
    }

    // -----------------------------------------------------------------------

    #region LookAt
    IEnumerator LookAtLerpCoroutine(Quaternion desired, float rotateTime, Action lookAtDoneCallback = null)
    {
        float timer = 0f;
        while (timer < rotateTime)
        {
            timer += Time.deltaTime;
            transform.rotation = Quaternion.Lerp(transform.rotation, desired, timer / rotateTime);

            yield return null;
        }
        transform.rotation = desired;

        lookAtDoneCallback?.Invoke();
    }
    #endregion

    #region 몬스터 사망시
    float m_DeathTimer;
    const float INVISIBLE_TIME = 3f;

    IEnumerator OnDeadCoroutine()
    {
        while (true)
        {
            m_DeathTimer += Time.deltaTime;
            if (m_DeathTimer > INVISIBLE_TIME)
            {
                Destroy(gameObject);
                m_DeathTimer = 0f;
                yield break;
            }
            yield return null;
        }
    }
    #endregion
}
