using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private LayerMask groundLayer;
    [SerializeField]
    private LayerMask wallMask;
    public Rigidbody2D m_RigidBody;
    [SerializeField]
    private SpriteRenderer m_SpriteRenderer;
    [SerializeField]
    private Animator m_Animator;
    [SerializeField]
    private float m_Speed               = 5.0f;
    [SerializeField]
    private float m_JumpForce           = 250.0f;
    private float m_HorizontalInput     = 0.0f;
    private float m_VerticalInput       = 0.0f;
    [SerializeField]
    private bool m_IsGrounded = false;
    [SerializeField]
    private float m_CheckGroundRayCastLength = 0.8f;
    [SerializeField]
    private bool m_ResetCheckGround = false;
    [SerializeField]
    private float m_ResetJumpSeconds = 0.75f;
    [SerializeField]
    private float m_ShiftDeltaMeters = 5.0f;
    [SerializeField]
    private float m_SpeedRunTimer = 0.0f;
    [SerializeField]
    private float m_SpeedRunAddXVelocity = 5.0f;
    [SerializeField]
    private bool m_ShiftIsTrigerred;
    [SerializeField]
    private float m_SpeedRunManaTime = 3.0f;
    [SerializeField]
    private bool m_SpeedRunIsEnabled = true;
    RaycastHit2D raycastHit2D;
    [SerializeField]
    private bool m_ResetTimer = false;
    [SerializeField]
    private bool m_SpeedRunTimerWasEnough = false;
    [SerializeField]
    private float m_ShiftPressedTimer = 0;

    private float m_RawHorizontal;

    private Vector3 deltaVector;

    

    [SerializeField]
    private bool m_LeftSide;
    [SerializeField]
    private bool m_RightSide;

    Coroutine resetTimerCoroutine;

    public float HorizontalInput
    {
        get { return m_HorizontalInput; }
    }

    public float RawHorizontalInput
    {
        get { return m_RawHorizontal; }
    }

    public bool LeftSideWall
    {
        get
        {
            return m_LeftSide;
        }
    }

    public bool RightSideWall
    {
        get
        {
            return m_RightSide;
        }
    }

    public bool IsGgrounded
    {
        get
        {
            return m_IsGrounded;
        }
    }

    public float JumpForce
    {
        get
        {
            return m_JumpForce;
        }
    }

   

    void Start()
    {
        m_RigidBody = this.GetComponent<Rigidbody2D>();
        m_SpriteRenderer = this.GetComponentInChildren<SpriteRenderer>();
        m_Animator = this.GetComponentInChildren<Animator>();

        m_RigidBody.gravityScale = 3.0f;
    }

    void Update()
    {
        GetControllerInput();
        MovePlayer();
        AnimatePlayer();
        StopNearObstacles();
        Levitate();

        m_IsGrounded = CheckGround();


        Debug.DrawRay(transform.position, -transform.up * m_CheckGroundRayCastLength);
    }

    private void FixedUpdate()
    {
        Jump();
    }

    private IEnumerator ResetRunModeTimer()
    {
        yield return new WaitForSeconds(m_SpeedRunManaTime * 1.5f);
        m_ResetTimer = false;
        resetTimerCoroutine = null;
    }

    private Vector3 ShiftForward(Vector3 _deltaInput)
    {
        return new Vector3(_deltaInput.x + (m_HorizontalInput * m_ShiftDeltaMeters), 0, 0); ;
    }

    private void AnimatePlayer()
    {
        m_Animator.SetFloat("horizontalModule", Math.Abs(m_HorizontalInput));

        if (m_ShiftIsTrigerred)
        {
            m_Animator.SetBool("shiftMode", true);

            // count TIMER when we press SHIFT
            m_ShiftPressedTimer += Time.deltaTime;
        }
        else
        {
            m_ShiftPressedTimer = 0;
        }

        if (m_ShiftPressedTimer >=0.35f || m_ShiftIsTrigerred == false)
        {
            // cancel SHIFT mode
            m_Animator.SetBool("shiftMode", false);
        }

        // continue with SPEED RUN mode
        m_Animator.SetBool("speedRunMode", m_SpeedRunIsEnabled);
    }

    private void MovePlayer()
    {
        deltaVector = new Vector3(m_HorizontalInput * m_Speed * Time.deltaTime, 0, 0);
        // Debug.Log(deltaVector);

        SpeedRunTimerControl();

        transform.position += deltaVector;


    }

    private void SpeedRunTimerControl()
    {
        bool decreaseSpeedRunTimer = false;

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            m_ShiftIsTrigerred = true;
            deltaVector = ShiftForward(deltaVector);
        }

        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            m_ShiftIsTrigerred = false;
            m_ShiftPressedTimer = 0;
            decreaseSpeedRunTimer = true;
            m_SpeedRunIsEnabled = false;
        }

        if (m_ShiftPressedTimer >= 0.35f)
        {
            // SPEED RUN mode

            if ((m_SpeedRunTimer < m_SpeedRunManaTime)
                && m_ResetTimer == false
                && decreaseSpeedRunTimer == false)
            {
                m_SpeedRunTimer += Time.deltaTime;
                m_SpeedRunIsEnabled = true;

                // Change speed multiplier for additional speed
                float additionalSpeed = m_HorizontalInput < 0 ? -m_SpeedRunAddXVelocity : m_SpeedRunAddXVelocity;
                deltaVector = new Vector3((m_HorizontalInput * m_Speed + additionalSpeed) * Time.deltaTime, 0, 0);
            }
            else
            {
                decreaseSpeedRunTimer = true;
                m_ResetTimer = true;
                m_SpeedRunIsEnabled = false;
            }

            Debug.Log("X velocity: " + m_RigidBody.velocity.x);
        }

        // Decrease used mana time / Recover Mana
        if (decreaseSpeedRunTimer == true)
        {
            m_SpeedRunTimer -= Time.deltaTime * 3;
        }

        if (m_SpeedRunTimer < 0)
        {
            m_SpeedRunTimer = 0;
        }

        if (m_ResetTimer == true && resetTimerCoroutine == null)
        {
            resetTimerCoroutine = StartCoroutine(nameof(ResetRunModeTimer));
        }
    }

    private void GetControllerInput()
    {
        m_RawHorizontal = Input.GetAxisRaw("Horizontal");
        m_HorizontalInput = Input.GetAxis("Horizontal");
        m_VerticalInput = Input.GetAxis("Vertical");

        if (m_LeftSide == true && m_HorizontalInput < 0)
        {
            m_HorizontalInput = 0;
        }
        else if (m_RightSide == true && m_HorizontalInput > 0)
        {
            m_HorizontalInput = 0;
        }

        FlipPlayerSprite();
    }

    private void FlipPlayerSprite()
    {
        if (m_HorizontalInput < 0)
        {
            m_SpriteRenderer.flipX = false;
        }

        if (m_HorizontalInput > 0)
        {
            m_SpriteRenderer.flipX = true;
        }
    }

    private void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && CheckGround())
        {
            m_RigidBody.AddForce(Vector2.up * m_JumpForce);
            m_ResetCheckGround = true;
            StartCoroutine(ResetGroundCheck());
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Space) && m_LeftSide)
            {
                m_RigidBody.AddForce((Vector2.up + Vector2.right) * m_JumpForce, ForceMode2D.Force);
                m_LeftSide = false;
            }
            else if (Input.GetKeyDown(KeyCode.Space) && m_RightSide)
            {
                m_RigidBody.AddForce((Vector2.up + Vector2.left) * m_JumpForce, ForceMode2D.Force);
                m_RightSide = false;
            }

            if (m_RigidBody.velocity.x < 0)
            {
                m_SpriteRenderer.flipX = false;
            }

            if (m_RigidBody.velocity.x > 0)
            {
                m_SpriteRenderer.flipX = true;
            }
        }
    }

    public bool CheckGround()
    {
        
        raycastHit2D = Physics2D.Raycast(transform.position,
            -transform.up,
            m_CheckGroundRayCastLength,
            groundLayer);
        
        if (raycastHit2D.collider != null && m_ResetCheckGround == false)
        {
            return true;
        }

        return false;
    }

    private IEnumerator ResetGroundCheck()
    {
        yield return new WaitForSeconds(m_ResetJumpSeconds);
        m_ResetCheckGround = false;
    }

    private void StopNearObstacles()
    {
        float sideDistance = 0.5f;
        Vector3 startPoint = new Vector3();

        startPoint = new Vector3(transform.position.x, transform.position.y, transform.position.z);

        RaycastHit2D leftCast = Physics2D.Raycast(startPoint, Vector2.left, sideDistance, 1 << 6);

        Debug.DrawRay(startPoint, Vector3.left * sideDistance, Color.black);


        startPoint = new Vector3(transform.position.x, transform.position.y, transform.position.z);

        RaycastHit2D rightCast = Physics2D.Raycast(startPoint, Vector2.right, sideDistance, 1 << 6);


        Debug.DrawRay(startPoint, Vector3.right * sideDistance, Color.red);

        if (leftCast)
        {
            Debug.Log(leftCast.collider.gameObject.name);
            m_LeftSide = true;   
        }
        else
        {
            m_LeftSide = false;
        }

        if (rightCast)
        {
            Debug.Log(rightCast.collider.gameObject.name);
            m_RightSide = true;
        }
        else
        {
            m_RightSide = false;
        }
    }

    private void Levitate()
    {

        if (Input.GetKey(KeyCode.Space)
            && m_IsGrounded == false)
        {
            if (m_RigidBody.velocity.y <= 0)
            {
                m_RigidBody.gravityScale = 0.5f;
                m_RigidBody.velocity = new Vector2(m_RigidBody.velocity.x, 0);
            }
        }
    }

}
