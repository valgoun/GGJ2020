using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;

public class PlayerController : MonoBehaviour, ITreatedInput
{
    public static PlayerController Instance;

    public event System.Action OnMeleeAttack;

    public float        Speed               = 5f;
    public float        FireSpeed           = 2.5f;
    public float        RecoilPower         = 15f;
    public Transform    ShootOrigin         = null;
    public GameObject   BulletPrefabs       = null;
    public float        BulletSpeed         = 15f;
    public float        MeleeAttackRadius   = 5f;
    public float        MeleeAttackForce    = 15f;
    public float        MeleeAttackCooldown = 1f;

    private Rigidbody   m_body              = null;
    private Vector2     m_inputs            = Vector2.zero;
    private float       m_timer             = 0f;
    private bool        m_canMeleeAttack    = true;

    private PlayerLifeManager m_playerLife;

    private void Awake()
    {
        if (Instance)
            Destroy(gameObject);
        else
            Instance = this;
    }

    // Start is called before the first frame update
    public void Start()
    {
        InputManager.Instance.RegisteredPlayer = this;

        m_body = GetComponent<Rigidbody>();
        m_playerLife = GetComponent<PlayerLifeManager>();
        m_playerLife.OnDamaged += DamageReaction;
    }

    // Update is called once per frame
    public void Update()
    {
        m_timer -= Time.deltaTime;
    }

    public void FixedUpdate()
    {
        m_body.MovePosition(m_body.position + new Vector3(m_inputs.x, 0, m_inputs.y) * Speed * Time.fixedDeltaTime);
    }

    public bool OnMelee()
    {
        if(m_canMeleeAttack)
        {
            SoundManager.Instance.PlaySound(SoundTypes.PLAYER_ATTACK_MELEE, m_playerLife.PlayerAudioSource);

            Collider[] results = Physics.OverlapSphere(transform.position, MeleeAttackRadius);
            for (int i = 0; i < results.Length; i++)
            {
                if (results[i].GetComponent<IDamageable>() != null && results[i].gameObject != gameObject)
                {
                    results[i].GetComponent<IDamageable>().Damage((results[i].transform.position - transform.position).normalized * MeleeAttackForce);
                }
            }

            m_canMeleeAttack = false;
            DOVirtual.DelayedCall(MeleeAttackCooldown, () => m_canMeleeAttack = true);
            OnMeleeAttack?.Invoke();
            return true;
        }
        return false;
    }

    public void OnMove(Vector2 direction)
    {
        m_inputs = direction;
        if (m_timer <= -0.2f)
        {
            if (direction == Vector2.zero)
                transform.forward = Vector3.back;
            else
                transform.forward = new Vector3(m_inputs.x, 0, m_inputs.y);
        }
    }

    public bool OnShoot(Vector2 direction)
    {
        if (m_timer <= 0)
        {
            SoundManager.Instance.PlaySound(SoundTypes.PLAYER_ATTACK_GUN, m_playerLife.PlayerAudioSource);

            Vector3 dir = new Vector3(direction.x, 0, direction.y);

            m_timer = 1f / FireSpeed;
            transform.forward = dir;
            m_body.AddForce(-dir * RecoilPower * m_timer, ForceMode.VelocityChange);
            var bullet = GameObject.Instantiate(BulletPrefabs, ShootOrigin.position, ShootOrigin.rotation);
            bullet.GetComponent<Rigidbody>().velocity = dir * BulletSpeed;
            Destroy(bullet, 5f);

            return true;
        }

        return false;
    }

    private void DamageReaction(Vector3 recoilDirection)
    {
        m_body.AddForce(recoilDirection, ForceMode.VelocityChange);
    }
}
