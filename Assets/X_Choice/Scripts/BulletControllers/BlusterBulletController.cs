using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlusterBulletController : MonoBehaviour
{
    [SerializeField]
    private float m_Speed = 9.0f;
    public PlayerController player;
    [SerializeField]
    private float horizontal = 0;
    private Rigidbody2D m_RB;

    private void Awake()
    {
        m_RB = this.GetComponent<Rigidbody2D>();
    }
    // Start is called before the first frame update
    void Start()
    {
        if (player == null)
        {
            player = GameObject.FindObjectOfType<PlayerController>();
        }

        horizontal = player.gameObject.GetComponentInChildren<SpriteRenderer>().flipX ? 1 : -1;
    }

    // Update is called once per frame
    void Update()
    {

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
            m_RB.bodyType = RigidbodyType2D.Dynamic;
            Debug.Log("Staff bullet met smthng...");
            Destroy(this.gameObject);
        }
    }


}
