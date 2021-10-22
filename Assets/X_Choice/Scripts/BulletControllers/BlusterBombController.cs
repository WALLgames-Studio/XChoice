using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlusterBombController : MonoBehaviour
{
    public PlayerController playerController;
    public Rigidbody2D m_Rb;
    private float m_horizontal;
    [SerializeField]
    private float m_Force = 150.0f;

    // Start is called before the first frame update
    void Start()
    {
        m_Rb = GetComponent<Rigidbody2D>();

        if (playerController == null)
        {
            playerController = GameObject.FindObjectOfType<PlayerController>();
        }

        var _flipX = playerController.GetComponentInChildren<SpriteRenderer>().flipX;

        m_horizontal = _flipX ? 1 : -1;

        ThroughBomb();
    }

    

    public void ThroughBomb()
    {
        Vector3 vector = new Vector3(m_horizontal * 3.5f, 1.5f, 0) * m_Force + playerController.transform.position;
        m_Rb.AddForce(vector, ForceMode2D.Force);
    }
}
