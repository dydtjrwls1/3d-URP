using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public class EnemyBase : RecycleObject
{
    public enum AIState
    {
        Follow,
        Attack
    }

    public float attackCoolTime = 2.0f;
    public float stoppingDistance = 1.5f;

    NavMeshAgent m_Agent;

    Player m_Player;

    Animator m_Animator;

    AIState m_State = AIState.Follow;

    Vector3 m_VectorToTarget;

    float m_CurrentAttackCoolTime;
    float m_OriginalSpeed;

    bool m_StateChangable = true;

    readonly int Speed_Hash = Animator.StringToHash("Speed");
    readonly int Attack_Hash = Animator.StringToHash("Attack");

    private void Awake()
    {
        m_Agent = GetComponent<NavMeshAgent>();
        m_Animator = GetComponent<Animator>();

        m_CurrentAttackCoolTime = attackCoolTime;
        m_OriginalSpeed = m_Agent.speed;
    }

    private void Start()
    {
        m_Player = GameManager.Instance.Player;
        m_Agent.SetDestination(m_Player.transform.position);
    }

    protected override void OnReset()
    {
        // m_Agent.SetDestination(m_Player.transform.position);
    }

    private void Update()
    {
        UpdateAIState();
        EnemyStateAction();
    }

    private void UpdateAIState()
    {
        switch (m_State)
        {
            case AIState.Follow:
                if (IsTargetInRange())
                {
                    m_State = AIState.Attack;
                }
                break;
            case AIState.Attack:
                if (!IsTargetInRange() && m_StateChangable)
                {
                    m_State = AIState.Follow;
                }
                break;
        }
    }

    private void EnemyStateAction()
    {
        // 공격 쿨타임은 계속해서 감소한다
        m_CurrentAttackCoolTime -= Time.deltaTime;

        switch (m_State)
        {
            case AIState.Follow:
                if (!m_Agent.pathPending)
                {
                    m_Agent.SetDestination(m_Player.transform.position);
                }
                break;
            case AIState.Attack:
                OrientToTarget();
                Attack();
                break;
        }

        m_Animator.SetFloat(Speed_Hash, m_Agent.velocity.sqrMagnitude);
    }

    private void OrientToTarget()
    {
        transform.forward = Vector3.ProjectOnPlane(m_VectorToTarget, Vector3.up);
    }

    private void Attack()
    {
        if (m_CurrentAttackCoolTime < 0.0f)
        {
            
            m_CurrentAttackCoolTime = attackCoolTime;
        }
    }

    private bool IsTargetInRange()
    {
        m_VectorToTarget = m_Player.transform.position - m_Agent.transform.position;
        float distance = Vector3.SqrMagnitude(m_VectorToTarget);

        return distance < stoppingDistance;
    }

    IEnumerator AttackCoroutine()
    {
        m_StateChangable = false;

        m_Animator.SetTrigger(Attack_Hash);

        // 공격 애니메이션 재생 도중 다른행동을 취하지 않는다.
        yield return new WaitForSeconds(m_Animator.GetCurrentAnimatorClipInfo(0).Length);

        m_StateChangable = true;
    }
} 
