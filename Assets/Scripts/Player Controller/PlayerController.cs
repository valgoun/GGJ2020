using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float Speed = 5f;

    private Keyboard    m_keyboard = null;
    private Rigidbody   m_body     = null;
    private Vector2     m_inputs   = Vector2.zero;
     
    // Start is called before the first frame update
    public void Start()
    {
        m_keyboard = Keyboard.current;
        m_body = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    public void Update()
    {
        ProcessInputs();
        if(m_inputs.sqrMagnitude > 0.0f)
            transform.forward = new Vector3(m_inputs.x, 0, m_inputs.y);
    }

    public void FixedUpdate()
    {
        m_body.MovePosition(m_body.position + new Vector3(m_inputs.x, 0, m_inputs.y) * Speed * Time.fixedDeltaTime);
    }

    private void ProcessInputs()
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
