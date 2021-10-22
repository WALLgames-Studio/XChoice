using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    [SerializeField]
    private Rigidbody2D m_RigidBody;
    [SerializeField]
    private GameObject m_Weapon;
    private bool m_ResetSwordJump;
    [SerializeField]
    public bool m_SwordJump = false;
    private Vector2 m_SwordJumpDeltas;
    private Vector2 m_PosBeforeJump;
    private float m_HorizontalInput;
    private SpriteRenderer m_SpriteRenderer;
    private float m_JumpForce;
    private Coroutine m_TimeSlowDowner;
    private Coroutine m_ResetterSwordJump;
    private PlayerController m_PlayerController;
    [SerializeField]
    private GameObject m_WhipHilt;

    private void Start()
    {
        if (m_RigidBody == null)
        {
            m_RigidBody = GetComponent<Rigidbody2D>();
        }
        if (m_PlayerController == null)
        {
            m_PlayerController = GetComponent<PlayerController>();
        }

        m_JumpForce = m_PlayerController.JumpForce;
        m_SpriteRenderer = this.GetComponentInChildren<SpriteRenderer>();
        

    }

    private void Update()
    {        
        m_HorizontalInput = m_SpriteRenderer.flipX ? 1 : -1;
        ControlSwordJumpAtack();
        ThrowWeaponOnGround();
    }

    private void AdoptWeapon(Collision2D collision)
    {
        //        Debug.Log(collision.gameObject.name);
        ControlBlaster bc = collision.gameObject.GetComponent<ControlBlaster>();
        if (bc != null)
        {
            m_Weapon = collision.gameObject;
            m_Weapon.transform.parent = this.transform;
            bc.IsAdopted = true;

            if (m_Weapon)
            {
                m_Weapon.transform.position = Vector3.zero;
            }

            return;
        }

        ControlSword sword = collision.gameObject.GetComponent<ControlSword>();
        if (sword != null)
        {
            m_Weapon = collision.gameObject;
            m_Weapon.transform.parent = this.transform;
            sword.IsAdopted = true;

            if (m_Weapon)
            {
                m_Weapon.transform.position = Vector3.zero;
            }

            return;
        }

        ControlWhip whip = collision.gameObject.GetComponent<ControlWhip>();
        if (whip != null)
        {
            m_Weapon = collision.gameObject;
            m_Weapon.transform.parent = this.transform;
            whip.IsAdopted = true;

            if (m_Weapon)
            {
                m_Weapon.transform.position = Vector3.zero;
            }

            return;
        }

        ControlStaff staff = collision.gameObject.GetComponent<ControlStaff>();
        if (staff != null)
        {
            m_Weapon = collision.gameObject;
            m_Weapon.transform.parent = this.transform;
            staff.IsAdopted = true;

            if (m_Weapon)
            {
                m_Weapon.transform.position = Vector3.zero;
            }
        }

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (m_Weapon == null)
        {
            AdoptWeapon(collision);
        }
    }

    public void SwordJump(float x, float y)
    {
        m_SwordJumpDeltas = new Vector2(x * m_HorizontalInput, y);
        m_PosBeforeJump = transform.position;
        m_SwordJump = true;        
    }

    private void ControlSwordJumpAtack()
    {
        if (m_ResetSwordJump == true)
        {
            if (m_ResetterSwordJump == null)
            {
                m_ResetterSwordJump = StartCoroutine(nameof(ResetSwordJump));
            }


        }

        float _horizontal = m_SpriteRenderer.flipX ? 1 : -1;

        if (m_SwordJump == true
            && m_ResetSwordJump == false)
        {
            m_ResetSwordJump = true;

            // Adding forces
            m_RigidBody.AddForce(
                (Vector2.up * 2) * m_JumpForce,
                ForceMode2D.Force);

            m_RigidBody.AddForce(
                Vector2.right * _horizontal * 2 * m_JumpForce,
                ForceMode2D.Force);
            
        }

        if (Mathf.Abs(transform.position.y - m_PosBeforeJump.y) >= m_SwordJumpDeltas.y
            && m_SwordJump == true)
        {
            m_SwordJump = false;

            m_RigidBody.velocity = Vector2.zero;
            m_PosBeforeJump = transform.position;

            if (m_TimeSlowDowner == null)
            {
                m_TimeSlowDowner = StartCoroutine(SlowTimeForAWhile(_horizontal));
            }
        }

    }

    private IEnumerator ResetSwordJump()
    {
        yield return new WaitForSeconds(2.5f);
        m_ResetSwordJump = false;
        m_ResetterSwordJump = null;
    }

    private IEnumerator SlowTimeForAWhile(float horizontal)
    {
        
        Time.timeScale = 0.5f;

        // Flying in the slow mo
        m_RigidBody.velocity = Vector2.zero;
        m_RigidBody.bodyType = RigidbodyType2D.Kinematic;

        yield return new WaitForSeconds(0.25f);

        m_RigidBody.bodyType = RigidbodyType2D.Dynamic;
        Time.timeScale = 1;

        // Adding forces after slow mo
        if (!Vector3.Equals(m_PosBeforeJump, Vector3.zero))
        {
            m_RigidBody.AddForce(
                (Vector2.right * horizontal) * m_JumpForce,
                ForceMode2D.Force);
            m_RigidBody.AddForce(
                Vector2.down * m_JumpForce,
                ForceMode2D.Force);

            if (Mathf.Abs(transform.position.x - m_PosBeforeJump.x) > m_SwordJumpDeltas.x)
            {
                m_RigidBody.velocity *= Vector2.up;
                m_PosBeforeJump = Vector3.zero;
            }
        }

        m_TimeSlowDowner = null;        
    }



    private void ThrowWeaponOnGround()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            if (m_Weapon != null)
            {
                var _hotizontal = m_PlayerController.
                    GetComponentInChildren<SpriteRenderer>().
                    flipX ? 1 : -1;

                m_Weapon.transform.parent = null;
                m_Weapon.GetComponent<WeaponParent>().IsAdopted = false;
                Vector3 newPos = m_Weapon.transform.localPosition
                    + Vector3.right
                        * _hotizontal
                        * 2.5f;
                m_Weapon.transform.localPosition = newPos;
                StartCoroutine(nameof(SetTriggerFalseOnWeapon));
            }
        }
    }

    private IEnumerator SetTriggerFalseOnWeapon()
    {
        yield return new WaitForSeconds(1.5f);
        m_Weapon.GetComponent<Collider2D>().isTrigger = false;
        m_Weapon = null;
    }


    internal void EnableWeapon()
    {
        var _weaponSprites = m_Weapon.GetComponentsInChildren<SpriteRenderer>();
        foreach (var _sprite in _weaponSprites)
        {
            _sprite.enabled = true;
        }
    }

    internal void DisableWeapon()
    {
        var _weaponSprites = m_Weapon.GetComponentsInChildren<SpriteRenderer>();
        foreach (var _sprite in _weaponSprites)
        {
            _sprite.enabled = false;
        }
    }
}
