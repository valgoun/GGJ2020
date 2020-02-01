using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedAI : BaseAI
{
    public float        AttackAnimLength    = 0.3f;
    public float        AttackDelay         = 0.2f;
    public float        AttackRecoilForce   = 2f;
    public float        BulletSpeed         = 25f;
    public GameObject   BulletPrefab        = null;

    [SerializeField]
    private Transform m_shootOrigin = null;

    private bool m_isAttacking = false;

    // Update is called once per frame
    new void Update()
    {
        if (!m_isAttacking)
            base.Update();
    }

    protected override void TriggerAttack()
    {
        if (!m_isAttacking)
        {
            m_isAttacking = true;

            DOVirtual.DelayedCall(AttackAnimLength, () => m_isAttacking = false);
            DOVirtual.DelayedCall(AttackDelay, PerformAttack);
        }
    }

    private void PerformAttack()
    {
        var bullet = GameObject.Instantiate(BulletPrefab, m_shootOrigin.position, m_shootOrigin.rotation);
        bullet.GetComponent<Rigidbody>().velocity = transform.forward * BulletSpeed;
        Destroy(bullet, 5f);
    }
}
