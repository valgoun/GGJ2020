using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;
using System;

public abstract class BaseAI : MonoBehaviour, IDamageable, IDestroyable
{
    public Transform Target;
    public float StoppingDistance;
    public float DeathDelay = 0.2f;
    public float RotationSpeed = 120f;

    protected NavMeshAgent  m_agent;
    protected Rigidbody     m_body;
    protected Animator      m_animator;
    protected bool          m_isAlive = true;

    public event Action OnDestroyEvent;

    // Start is called before the first frame update
    protected void Start()
    {
        m_agent     = GetComponent<NavMeshAgent>();
        m_body      = GetComponent<Rigidbody>();
        m_animator  = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    protected void Update()
    {
        if (!m_isAlive)
            return;

        m_agent.SetDestination(Target.position);
        if (Vector3.SqrMagnitude(Target.position - transform.position) <= StoppingDistance * StoppingDistance)
        {
            if(Vector3.Dot((Target.position - transform.position).normalized, transform.forward) > 0.95f)
            {
                TriggerAttack();
                m_agent.isStopped = true;
            }
        }
        else if(m_agent.isStopped)
        {
            m_agent.isStopped = false;
        }

        Quaternion desiredRotation = Quaternion.LookRotation(new Vector3(Target.position.x, transform.position.y, Target.position.z) - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, Time.deltaTime * Mathf.Deg2Rad * RotationSpeed);
    }

    protected abstract void TriggerAttack();

    protected void UpdateAnim()
    {
        if(m_agent.speed > 0f)
            m_animator.SetFloat("Speed", m_agent.velocity.magnitude / m_agent.speed);
    }

    public virtual void Damage(Vector3 DamageRecoil)
    {
        m_isAlive = false;
        m_agent.isStopped = false;
        m_body.AddForce(DamageRecoil, ForceMode.VelocityChange);
        Destroy(gameObject, DeathDelay);
    }

    private void OnDestroy()
    {
        OnDestroyEvent?.Invoke();
    }
}
