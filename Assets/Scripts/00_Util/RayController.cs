using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.AI;


public class RayController : MonoBehaviour
{
    delegate void Handler();

    public bool Active;
    public bool Stop;
    public float moveSpeed = 2.0f;
    public float rotateSpeed = 8.0f;
    public Vector3 movePos;

    Vector3 prevPos;
    Handler update;
    public NavMeshAgent Agent { get => agent; }
    [SerializeField]
    NavMeshAgent agent;
    NavMeshPath path;

    public Vector3 delta;

    private void Awake()
    {
        Stop = true;
        Active = true;
        movePos = transform.position;
        prevPos = transform.position;

        update = Point;
        update += DeltaCalculate;
        if (agent != null)
        {
            agent.updateRotation = false;
            path = new NavMeshPath();
            update += AgentMove;
            update += AgentRotate;
        }
        else
        {
            update += Rotate;
            update += Move;
        }

    }

    private void Update()
    {
        if (update != null)
            update();
    }

    public float CalculateDistance(Vector3 target)
    {
        float distance = 0f;

        if (agent != null)
        {
            path.ClearCorners();
            agent.CalculatePath(target, path);

            if (path.corners.Length == 1)
            {
                distance += Vector3.Distance(transform.position, path.corners[0]);
                return distance;
            }

            for (int i = 1; i < path.corners.Length; i++)
                distance += Vector3.Distance(path.corners[i - 1], path.corners[i]);
        }
        else
            distance = Vector3.Distance(transform.position, target);

        return distance;
    }

    public void ClearPath()
    {
        agent.ResetPath();
        movePos = transform.position;
    }

    void Point()
    {
        if (Active)
        {
            if (Input.GetMouseButtonDown(1))
            {
                if (EventSystem.current.IsPointerOverGameObject() == false)
                {
                    Stop = false;

                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    RaycastHit hitinfo;

                    if (Physics.Raycast(ray, out hitinfo, Mathf.Infinity))
                        movePos = hitinfo.point;
                }
            }
        }
    }

    void Move()
    {
        if (Stop == false)
            transform.position = Vector3.MoveTowards(transform.position, movePos, Time.deltaTime * moveSpeed);
    }

    void AgentMove()
    {
        if (Stop == false)
            agent.destination = movePos;
    }

    void Rotate()
    {
        if (Stop == false)
        {
            Vector3 adjust = movePos;
            adjust.y = transform.position.y;

            Vector3 relativePos = adjust - transform.position;
            if (relativePos != Vector3.zero)
            {
                Quaternion toRotation = Quaternion.LookRotation(relativePos);
                transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, Time.deltaTime * rotateSpeed);
            }
        }
    }

    void AgentRotate()
    {
        if (Stop == false)
        {
            Vector3 direction = agent.desiredVelocity;

            if (direction != Vector3.zero)
            {
                Quaternion toRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, Time.deltaTime * rotateSpeed);
            }
        }
    }

    void DeltaCalculate()
    {
        delta = prevPos - transform.position;
        if (delta.magnitude != 0)
            prevPos = transform.position;
    }
}