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

    float m_CurrentAttackCoolTime;
    float m_OriginalSpeed;

    bool m_CanMove;

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
        m_Agent.SetDestination(m_Player.transform.position);
    }

    private void Update()
    {
        UpdateAIState();
        EnemyStateAction();
    }

    private void EnemyStateAction()
    {
        // 공격 쿨타임은 계속해서 감소한다
        m_CurrentAttackCoolTime -= Time.deltaTime;

        switch (m_State)
        {
            case AIState.Follow:
                m_Agent.SetDestination(m_Player.transform.position - m_Player.transform.forward * 0.15f);
                break;
            case AIState.Attack:
                if (m_CurrentAttackCoolTime < 0.0f)
                {
                    m_Animator.SetTrigger(Attack_Hash);
                    m_CurrentAttackCoolTime = attackCoolTime;
                }
                break;
        }

        m_Animator.SetFloat(Speed_Hash, m_Agent.velocity.sqrMagnitude);
    }

    private void UpdateAIState()
    {
        switch (m_State)
        {
            case AIState.Follow:
                if (!m_Agent.pathPending && IsTargetInRange())
                {
                    m_State = AIState.Attack;
                }
                break;
            case AIState.Attack:
                if(!m_Agent.pathPending && !IsTargetInRange())
                {
                    m_State = AIState.Follow;
                }
                break;
        }
    }

    private bool IsTargetInRange()
    {
        float distance = Vector3.SqrMagnitude(m_Player.transform.position - m_Agent.transform.position);
        return distance < stoppingDistance;
    }
} 
