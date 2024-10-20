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

    protected Animator m_Animator;

    NavMeshAgent m_Agent;

    Coroutine m_AttackCoroutine = null;

    IInitialize[] m_Inits;

    AIState m_State = AIState.Follow;

    Vector3 m_VectorToTarget;

    float m_CurrentAttackCoolTime;

    bool m_StateChangable = true;

    bool m_IsAlive = true;

    public Player Player { get; set; }

    public event Action<uint> onEnemyHealthChange = null;

    public event Action onEnemyDie = null;

    readonly int Speed_Hash = Animator.StringToHash("Speed");
    readonly int Attack_Hash = Animator.StringToHash("Attack");
    readonly int Die_Hash = Animator.StringToHash("Die");

    protected virtual void Awake()
    {
        m_Agent = GetComponent<NavMeshAgent>();
        m_Animator = GetComponent<Animator>();
        
        m_Inits = GetComponents<IInitialize>();
    }

    private void Start()
    {
        Health health = GetComponent<Health>();

        if (health != null)
        {
            health.onDie += Die;
        }
    }


    protected override void OnReset()
    {
        //if(m_Player == null)
        //{
        //    m_Player = GameManager.Instance.Player;
        //}

        // m_Agent?.SetDestination(m_Player.transform.position);

        


        m_CurrentAttackCoolTime = attackCoolTime;

        if (m_Inits.Length > 0)
        {
            foreach (IInitialize init in m_Inits)
            {
                init.Initialize();
            }
        }

        m_IsAlive = true;
    }

    protected virtual void Update()
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
        // ���� ��Ÿ���� ����ؼ� �����Ѵ�
        m_CurrentAttackCoolTime -= Time.deltaTime;

        if (m_IsAlive)
        {
            switch (m_State)
            {
                case AIState.Follow:
                    if (!m_Agent.pathPending)
                    {
                        m_Agent.SetDestination(Player.transform.position);
                        m_Animator.SetFloat(Speed_Hash, m_Agent.velocity.sqrMagnitude);
                    }
                    break;
                case AIState.Attack:
                    // ���ڸ��� ����
                    m_Agent.SetDestination(transform.position);

                    // �ִϸ����Ϳ� ���� �ӵ� ����
                    m_Animator.SetFloat(Speed_Hash, m_Agent.velocity.sqrMagnitude);

                    OrientToTarget();
                    Attack();
                    break;
            }
        }
        else
        {
            m_Agent.SetDestination(transform.position);
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

    void Die()
    {
        if (m_IsAlive)
        {
            m_IsAlive = false;
            m_Animator.SetTrigger(Die_Hash);
            DisableTimer(2.0f);
        }
    }

    private bool IsTargetInRange()
    {
        m_VectorToTarget = Player.transform.position - m_Agent.transform.position;
        float distance = Vector3.SqrMagnitude(m_VectorToTarget);

        return distance < attackDistance * attackDistance;
    }

    IEnumerator AttackCoroutine()
    {
        m_StateChangable = false;

        m_Animator.SetTrigger(Attack_Hash);

        // ���� �ִϸ��̼� ��� ���� �ٸ��ൿ�� ������ �ʴ´�.
        yield return new WaitForSeconds(2.0f);

        m_AttackCoroutine = null;
        m_StateChangable = true;
    }
} 
