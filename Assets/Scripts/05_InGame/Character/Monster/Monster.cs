using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using DatabaseSystem;
using System.Linq;

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

    public void SetWalkBehavior(Vector3 posiiton)
    {
        Agent.SetDestination(posiiton);
        OnSetWalkBehavior(posiiton);
    }

    public void SetAttackBehavior()
    {
        OnSetAttackBehavior();
    }

    public void SetIdleBehavior()
    {
        OnSetIdleBehavior();
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
    protected virtual void OnSetWalkBehavior(Vector3 position) { }
    protected virtual void OnSetAttackBehavior() { }
    protected virtual void OnSetIdleBehavior() { }
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

        // 50%확률로 SP,HP 회복아이템 드랍
        int randomPoint = UnityEngine.Random.Range(0, 100);
        string interactablePath = GameManager.GameDevelopSettings.InteractableResourcePath;
        if (randomPoint >= 50 || GameManager.CheatSettings.DropItem100Percent)
        {
            var spDropItem = manager.PoolSystem.Load<DropItem>($"{interactablePath}/DropItem_SpRecovery");
            var hpDropItem = manager.PoolSystem.Load<DropItem>($"{interactablePath}/DropItem_HpRecovery");

            ThrowDropItem(spDropItem, 0.2f);
            ThrowDropItem(hpDropItem, 0.2f);
        }

        // 100%확률로 골드 드랍
        var goldDropItem = manager.PoolSystem.Load<DropItem>($"{interactablePath}/DropItem_Gold");
        ThrowDropItem(goldDropItem, 0.1f);

        // 100% 확률로 스테이지 아이템 리스트 중 하나를 드랍
        var stageDropItem = manager.PoolSystem.Load<DropItem>($"{interactablePath}/DropItem_Box");
        ThrowDropItem(stageDropItem);
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

    void ThrowDropItem(DropItem dropItem, float adjustHeight = 0f)
    {
        Vector3 dropStartPosition = transform.position;
        dropStartPosition = new Vector3(dropStartPosition.x, dropStartPosition.y + adjustHeight, dropStartPosition.z);

        bool colliderCheck = false;
        Vector3 dropEndPosition = dropStartPosition;
        float itemDropDistance = UnityEngine.Random.Range(-2, 2);
        dropEndPosition = new Vector3(dropStartPosition.x + itemDropDistance, dropStartPosition.y, dropStartPosition.z + itemDropDistance);

        // 경로의 점들을 체크해서, 뭔가에 부딪히게 되면 해당 좌표에서 밑으로 ray를 쏴서 그 위치에 닿게 해야한다.
        int layermask = GameManager.GameDevelopSettings.TerrainLayermask;
        float timer = 0f;
        int coordinateInPathCount = 0;

        BoxCollider dropItemCollider = dropItem.GetComponent<BoxCollider>();
        float colliderMaxSize = dropItemCollider.size.x;
        if (colliderMaxSize < dropItemCollider.size.y)
            colliderMaxSize = dropItemCollider.size.y;

        if (colliderMaxSize < dropItemCollider.size.z)
            colliderMaxSize = dropItemCollider.size.z;


        while (colliderCheck == false)
        {
            timer += Time.deltaTime;
            coordinateInPathCount++;

            if (coordinateInPathCount < 10)
                continue;

            Vector3 position = MathParabola.Parabola(dropStartPosition, dropEndPosition, 0.3f, timer / 1f);

            // 콜라이더의 최대 사이즈 반경으로 감지
            var hitColliders = UnityEngine.Physics.OverlapSphere(position, colliderMaxSize, 1 << layermask);

            if (hitColliders.Length > 0)
            {
                colliderCheck = true;
                if (position.RaycastDown(out Vector3 groundPoint))
                    dropEndPosition = groundPoint;

                break;
            }

            if (timer > 1f)
                colliderCheck = true;
        }

        MoveInParabolaParam itemParam = new MoveInParabolaParam()
        {
            StartPosition = dropStartPosition,
            EndPosition = dropEndPosition,
            Height = 0.3f,
            SimulateTime = 1f
        };
        dropItem.MoveInParabola(itemParam);
    }
    #endregion
}
