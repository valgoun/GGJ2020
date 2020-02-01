using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour, ITreatedInput
{
    public float        Speed           = 5f;
    public float        FireSpeed       = 2.5f;
    public float        RecoilPower     = 15f;
    public Transform    ShootOrigin     = null;
    public GameObject   BulletPrefabs   = null;
    public float        BulletSpeed     = 15f;

    private Rigidbody   m_body     = null;
    private Vector2     m_inputs   = Vector2.zero;
    private float       m_timer    = 0f;

    private PlayerLifeManager m_playerLife;

     
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
        if (m_timer <= -0.2f)
            transform.forward = Vector3.forward;

        m_timer -= Time.deltaTime;
    }

    public void FixedUpdate()
    {
        m_body.MovePosition(m_body.position + new Vector3(m_inputs.x, 0, m_inputs.y) * Speed * Time.fixedDeltaTime);
    }

    public bool OnMelee()
    {
        return false;
    }

    public void OnMove(Vector2 direction)
    {
        m_inputs = direction;
    }

    public bool OnShoot(Vector2 direction)
    {
        if (m_timer <= 0)
        {
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
