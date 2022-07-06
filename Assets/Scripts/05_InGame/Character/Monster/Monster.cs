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

        // BehaviorData ��������
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
            // �������� �Ŵ����� ���� ����Ʈ���� ����
            manager.Monsters.Remove(this);

            // �ڽ��� �¾ �����ʰ� ������ �˷��ֱ�
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

        // �״� �ִϸ��̼�
        AnimationJobs.Enqueue(AniType.DEAD_0);

        // �ð��� ������ �ڵ����� ��������� ��
        StartCoroutine(OnDeadCoroutine());

        // �÷��̾ ���� �׿����� �� óġ Ƚ�� ����
        StageManager.Instance.MissionSystem.ReportAll(QuestType.KILL_ENEMY);

        if (m_TargetLockOnImage)
            GameManager.UISystem.Pool.Release(m_TargetLockOnImage);

        // 50%Ȯ���� SP,HP ȸ�������� ���
        int randomPoint = UnityEngine.Random.Range(0, 100);
        if (randomPoint >= 50 || GameManager.CheatSettings.DropRecoveryItemClearly)
        {
            string interactablePath = GameManager.GameDevelopSettings.InteractableResourcePath;

            var spDropItem = manager.PoolSystem.Load<SpRecoveryItem>($"{interactablePath}/DropItem_SpRecovery");
            var hpDropItem = manager.PoolSystem.Load<HpRecoveryItem>($"{interactablePath}/DropItem_HpRecovery");

            Vector3 dropItemStartPosition = transform.position;
            dropItemStartPosition = new Vector3(dropItemStartPosition.x, dropItemStartPosition.y + 0.2f, dropItemStartPosition.z);

            // 2m �ݰ���� ������ ���� ����
            Vector3 spDropItemEndPosition = Vector3.zero;
            float spRadius = UnityEngine.Random.Range(-2, 2);
            spDropItemEndPosition = new Vector3(dropItemStartPosition.x + spRadius, dropItemStartPosition.y + 0.2f, dropItemStartPosition.z + spRadius);

            Vector3 hpDropItemEndPosition = Vector3.zero;
            float hpRadius = UnityEngine.Random.Range(-2, 2);
            hpDropItemEndPosition = new Vector3(dropItemStartPosition.x + hpRadius, dropItemStartPosition.y + 0.2f, dropItemStartPosition.z + hpRadius);

            MoveInParabolaParam spDropItemParam = new MoveInParabolaParam()
            {
                StartPosition = dropItemStartPosition,
                EndPosition = spDropItemEndPosition,
                Height = 0.3f,
                SimulateTime = 1f
            };

            MoveInParabolaParam hpDropItemParam = new MoveInParabolaParam()
            {
                StartPosition = dropItemStartPosition,
                EndPosition = hpDropItemEndPosition,
                Height = 0.3f,
                SimulateTime = 1f
            };

            spDropItem.MoveInParabola(spDropItemParam);
            hpDropItem.MoveInParabola(hpDropItemParam);
        }
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

    #region ���� �����
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
