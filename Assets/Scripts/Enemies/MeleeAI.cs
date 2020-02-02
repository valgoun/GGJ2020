using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MeleeAI : BaseAI
{
    public float AttackAnimLength = 0.3f;
    public float AttackDelay = 0.2f;
    public float AttackRecoilForce = 2f;

    [SerializeField]
    private BoxCollider m_hitTrigger = null;
    private bool m_isAttacking = false;

    new void Start()
    {
        base.Start();
        m_animator.SetBool("Sword Out", true);
    }

    new void Update()
    {
        if(!m_isAttacking)
            base.Update();
        UpdateAnim();
    }
    protected override void TriggerAttack()
    {
        if(!m_isAttacking)
        {
            m_isAttacking = true;
            m_animator.SetTrigger("Attack Sword");

            DOVirtual.DelayedCall(AttackAnimLength, () => m_isAttacking = false);
            DOVirtual.DelayedCall(AttackDelay, PerformAttack);
        }
    }

    private void PerformAttack()
    {
        Collider[] results = Physics.OverlapBox(m_hitTrigger.transform.position + m_hitTrigger.center, m_hitTrigger.size, m_hitTrigger.transform.rotation);
        for (int i = 0; i < results.Length; i++)
        {
            if(results[i].gameObject.CompareTag("Player"))
            {
                results[i].GetComponent<IDamageable>().Damage(transform.forward * AttackRecoilForce);
                return;
            }
        }
    }

}
