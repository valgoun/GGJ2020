using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform    CameraTarget = null;
    public float        SmoothingFactor = 0.1f;

    private Vector3 m_OffSet = Vector3.zero;
    private Vector3 m_Velotity = Vector3.zero;

    public void Start()
    {
        m_OffSet = CameraTarget.position - transform.position;        
    }
    public void FixedUpdate()
    {
        transform.position = Vector3.SmoothDamp(transform.position, CameraTarget.position, ref m_Velotity, SmoothingFactor);
    }

}
