using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaffBulletController : MonoBehaviour
{
    [SerializeField]
    private float m_Speed = 5.0f;
    public PlayerController player;
    [SerializeField]
    private float horizontal = 0;
    [SerializeField]
    private Rigidbody2D m_RB;
    [SerializeField]
    private ParticleSystem m_ParticleSystem;
    [SerializeField]
    private Transform m_Light;
    private float m_LightRotateZ = 0;

    // Start is called before the first frame update
    void Start()
    {
        if (m_ParticleSystem == null)
        {
            m_ParticleSystem = this.GetComponentInChildren<ParticleSystem>();
        }

        m_RB = this.GetComponent<Rigidbody2D>();
        if (player == null)
        {
            player = GameObject.FindObjectOfType<PlayerController>();
        }

        horizontal = player.gameObject.GetComponentInChildren<SpriteRenderer>().flipX ? 1 : -1;
        m_ParticleSystem.transform.rotation =
            horizontal < 0
                ? Quaternion.Euler(0, 90, 0)
                    : Quaternion.Euler(0, -90, 0);

        
    }

    // Update is called once per frame
    void Update()
    {
        m_Light.transform.rotation = Quaternion.Euler(0, 0, m_LightRotateZ);
        m_LightRotateZ += Time.deltaTime * 360;

        if (m_LightRotateZ > 360)
        {
            m_LightRotateZ = 0;
        }

        Vector3 moveDelta = new Vector3(
            m_Speed * horizontal * Time.deltaTime,
            0,
            0);

        // Debug.Log(moveDelta);

        transform.position += moveDelta;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision != null)
        {
            Debug.Log("Staff bullet met smthng...");

            m_RB.bodyType = RigidbodyType2D.Dynamic;

            Destroy(this.gameObject, 0.5f);
        }
    }
}
