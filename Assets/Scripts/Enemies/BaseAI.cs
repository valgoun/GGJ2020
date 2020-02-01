using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class BaseAI : MonoBehaviour
{
    public Transform Target;
    public float StoppingDistance;

    protected NavMeshAgent m_agent;

    // Start is called before the first frame update
    protected void Start()
    {
        m_agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    protected void Update()
    {
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
}
