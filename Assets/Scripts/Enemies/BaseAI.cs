using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class BaseAI : MonoBehaviour, IDamageable
{
    public Transform Target;
    public float StoppingDistance;
    public float DeathDelay = 0.2f;

    protected NavMeshAgent  m_agent;
    protected Rigidbody     m_body;
    protected bool          m_isAlive = true;

    // Start is called before the first frame update
    protected void Start()
    {
        m_agent = GetComponent<NavMeshAgent>();
        m_body  = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    protected void Update()
    {
        if (!m_isAlive)
            return;

        m_agent.SetDestination(Target.position);
        if (Vector3.SqrMagnitude(transform.position - Target.position) <= StoppingDistance * StoppingDistance)
        {
            TriggerAttack();
            m_agent.isStopped = true;
        }
        else if(m_agent.isStopped)
        {
            m_agent.isStopped = false;
        }

        transform.LookAt(new Vector3(Target.position.x, transform.position.y, Target.position.z), Vector3.up);
    }

    protected abstract void TriggerAttack();
    public virtual void Damage(Vector3 DamageRecoil)
    {
        m_isAlive = false;
        m_agent.isStopped = false;
        m_body.AddForce(DamageRecoil, ForceMode.VelocityChange);
        Destroy(gameObject, DeathDelay);
    }
}
