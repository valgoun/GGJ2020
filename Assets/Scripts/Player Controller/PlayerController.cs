using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float        Speed           = 5f;
    public float        FireSpeed       = 2.5f;
    public float        RecoilPower     = 15f;
    public Transform    ShootOrigin     = null;
    public GameObject   BulletPrefabs   = null;
    public float        BulletSpeed     = 15f;

    private Keyboard    m_keyboard = null;
    private Rigidbody   m_body     = null;
    private Vector2     m_inputs   = Vector2.zero;
    private float       m_timer    = 0f;

     
    // Start is called before the first frame update
    public void Start()
    {
        m_keyboard = Keyboard.current;
        m_body = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    public void Update()
    {
        ProcessMoveInputs();


        if (m_keyboard.upArrowKey.isPressed)
            ProcessFireInput(Vector3.forward);
        else if (m_keyboard.downArrowKey.isPressed)
            ProcessFireInput(Vector3.back);
        else if (m_keyboard.rightArrowKey.isPressed)
            ProcessFireInput(Vector3.right);
        else if (m_keyboard.leftArrowKey.isPressed)
            ProcessFireInput(Vector3.left);
        else if (m_timer <= -0.2f)
        {
            //m_timer = 1f / FireSpeed;
            transform.forward = Vector3.forward;
        }

        m_timer -= Time.deltaTime;
    }

    public void FixedUpdate()
    {
        m_body.MovePosition(m_body.position + new Vector3(m_inputs.x, 0, m_inputs.y) * Speed * Time.fixedDeltaTime);
    }

    private void ProcessFireInput(Vector3 direction)
    {
        if(m_timer <= 0)
        {
            m_timer = 1f / FireSpeed; 
            transform.forward = direction;
            m_body.AddForce(-direction * RecoilPower * m_timer, ForceMode.VelocityChange);
            var bullet = GameObject.Instantiate(BulletPrefabs, ShootOrigin.position, ShootOrigin.rotation);
            bullet.GetComponent<Rigidbody>().velocity = direction * BulletSpeed;
            Destroy(bullet, 5f);
        }
    }

    private void ProcessMoveInputs()
    {
        m_inputs = Vector2.zero;

        bool dPressed = m_keyboard.dKey.isPressed;
        bool qPressed = m_keyboard.aKey.isPressed;
        bool zPressed = m_keyboard.wKey.isPressed;
        bool sPressed = m_keyboard.sKey.isPressed;

        m_inputs.x += dPressed ? 1 : 0;
        m_inputs.x -= qPressed ? 1 : 0;
        m_inputs.y += zPressed ? 1 : 0;
        m_inputs.y -= sPressed ? 1 : 0;

        m_inputs.Normalize();
    }
}
