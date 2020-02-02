using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLifeManager : MonoBehaviour, IDamageable
{
    public static PlayerLifeManager Instance;

    public int Life { get; private set; }

    public event System.Action          OnDeath;
    public event System.Action<Vector3> OnDamaged;

    public int MaxLife = 3;
    public float InvibilityTime = 1f;

    private bool        m_canBeDamaged = true;

    public AudioSource PlayerAudioSource;

    private void Awake()
    {
        if (Instance)
            Destroy(gameObject);
        else
            Instance = this;
    }

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
                Death();
                return;
            }

            SoundManager.Instance.PlaySound(SoundTypes.PLAYER_HIT, PlayerAudioSource);
            m_canBeDamaged = false;
            DOVirtual.DelayedCall(InvibilityTime, () => m_canBeDamaged = true);
            OnDamaged?.Invoke(DamageRecoil);
        }
    }

    [Button("Kill Player")]
    void Death()
    {
        OnDeath?.Invoke();
        Master.Instance.GameOver();
        SoundManager.Instance.PlaySound(SoundTypes.PLAYER_DIE, PlayerAudioSource);
    }
}
