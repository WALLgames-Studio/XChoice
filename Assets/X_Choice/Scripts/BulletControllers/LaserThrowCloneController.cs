using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserThrowCloneController : MonoBehaviour
{
    public ControlSword m_Sword;
    [SerializeField]
    private float m_Distance;

    [SerializeField]
    private float m_ForwardSpeed = 12.0f;
    [SerializeField]
    private float m_BacwardSpeed = 18.0f;

    private Vector3 m_StartPos;
    private Vector3 m_EndPos;

    [SerializeField]
    private bool m_throwForward = true;




    // Start is called before the first frame update
    void Start()
    {
        m_Distance = m_Sword.ThrowDistance;
        m_StartPos = transform.position;
        m_EndPos = m_StartPos + Vector3.right * m_Distance;
        m_throwForward = true;

        Debug.Log("M_Distance for Throwen Sword: " + m_Distance);
    }

    private void Update()
    {
        if (Mathf.Abs(transform.position.x - m_Sword.transform.position.x) < 0.15f
            && m_throwForward == false)
        {
            m_Sword.DestroyThrowenSword();
        }

        if (transform.position.x == m_EndPos.x && m_throwForward == true)
        {
            m_throwForward = false;
        }

        if (m_throwForward == true)
        {
            transform.position = Vector3.MoveTowards(transform.position, m_EndPos, Time.deltaTime * m_ForwardSpeed);
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, m_Sword.transform.position, Time.deltaTime * m_BacwardSpeed);
        }
    }
}
