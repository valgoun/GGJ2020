﻿using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLifeManager : MonoBehaviour, IDamageable
{
    
    public int Life { get; private set; }

    public event System.Action          OnDeath;
    public event System.Action<Vector3> OnDamaged;

    public int MaxLife = 3;
    public float InvibilityTime = 1f;

    private bool        m_canBeDamaged = true;

    // Start is called before the first frame update
    void Start()
    {
        Life = MaxLife;
    }

    public void Damage(Vector3 DamageRecoil)
    {
        if(m_canBeDamaged)
        {
            Life--;

            if(Life <= 0)
            {
                OnDeath?.Invoke();
                return;
            }

            m_canBeDamaged = false;
            DOVirtual.DelayedCall(InvibilityTime, () => m_canBeDamaged = true);
            OnDamaged?.Invoke(DamageRecoil);
        }
    }
}
