using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Health))]
public class EnemyBase : RecycleObject
{
    public enum AIState
    {
        Follow,
        Attack
    }

    public float attackCoolTime = 2.0f;
    public float attackDistance = 1.5f;

    NavMeshAgent m_Agent;

    Player m_Player;

    Animator m_Animator;

    Coroutine m_AttackCoroutine = null;

    AIState m_State = AIState.Follow;

    Vector3 m_VectorToTarget;

    float m_CurrentAttackCoolTime;

    bool m_StateChangable = true;

    public event Action<uint> onEnemyHealthChange = null;

    public event Action onEnemyDie = null;

    readonly int Speed_Hash = Animator.StringToHash("Speed");
    readonly int Attack_Hash = Animator.StringToHash("Attack");

    private void Awake()
    {
        m_Agent = GetComponent<NavMeshAgent>();
        m_Animator = GetComponent<Animator>();

        m_CurrentAttackCoolTime = attackCoolTime;
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
        if (m_StateChangable) 
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
                    if (!IsTargetInRange())
                    {
                        m_State = AIState.Follow;
                    }
                    break;
            }
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
                    m_Animator.SetFloat(Speed_Hash, m_Agent.velocity.sqrMagnitude);
                }
                break;
            case AIState.Attack:
                m_Agent.SetDestination(transform.position);
                OrientToTarget();
                Attack();
                break;
        }
    }

    private void OrientToTarget()
    {
        transform.forward = Vector3.ProjectOnPlane(m_VectorToTarget, Vector3.up);
    }

    private void Attack()
    {
        if (m_CurrentAttackCoolTime < 0.0f && m_AttackCoroutine == null)
        {
            Util.ResetAndRunCoroutine(this, ref m_AttackCoroutine, AttackCoroutine);
            m_CurrentAttackCoolTime = attackCoolTime;
        }
    }

    private bool IsTargetInRange()
    {
        m_VectorToTarget = m_Player.transform.position - m_Agent.transform.position;
        float distance = Vector3.SqrMagnitude(m_VectorToTarget);

        return distance < attackDistance * attackDistance;
    }

    IEnumerator AttackCoroutine()
    {
        m_StateChangable = false;

        m_Animator.SetTrigger(Attack_Hash);

        // 공격 애니메이션 재생 도중 다른행동을 취하지 않는다.
        yield return new WaitForSeconds(2.0f);

        m_AttackCoroutine = null;
        m_StateChangable = true;
    }
} 
