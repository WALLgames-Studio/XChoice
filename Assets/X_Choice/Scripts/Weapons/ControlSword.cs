using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlSword : WeaponParent
{
    [SerializeField]
    Animator m_Animator;

    [SerializeField]
    float m_AtackYDelta = 1.5f;
    [SerializeField]
    float m_AtackXDelta = 1.5f;
    [SerializeField]
    float m_ThrowDistance = 10.0f;
    [SerializeField]
    Vector3 m_StartPos;
    [SerializeField]
    Vector3 m_FinishPos;
    [SerializeField]
    bool m_ThrowSword = false;
    [SerializeField]
    private GameObject m_throwSwordPrefab;
    [SerializeField]
    GameObject throwenSword;
    [SerializeField]
    Collider2D m_Collider; 

    private bool m_CanThrowSword = false;
    Coroutine m_ThirdAtackResetter = null;


    public float ThrowDistance
    {
        get
        {
            if (m_Player_Sr.flipX == true)
            {
                return m_ThrowDistance;
            }

            return -m_ThrowDistance;

        }
    }

    private void Awake()
    {
        // SKills Reloads Setting
        m_SkillsReloads[0] = 0.1f;
        m_SkillsReloads[1] = 1.5f;
        m_SkillsReloads[2] = 2.5f;
        m_SkillsReloads[3] = 2.0f; // 10.0f;
    }


    void Start()
    {
        if (m_Collider == null)
        {
            m_Collider = this.GetComponent<Collider2D>();
        }

        if (m_Animator == null)
        {
            m_Animator = this.GetComponentInChildren<Animator>();
        }

        base.Start();
    }
    // Update is called once per frame
    void Update()
    {
        base.Update();
    }


    internal void DestroyThrowenSword()
    {
        Destroy(throwenSword);
    }



    protected override void GetInHands()
    {

        if (isAdopted == true)
        {
            // Make Collider triggered
            m_Collider.isTrigger = true;

            if (player == null)
            {
                player = this.transform.parent.gameObject;
                m_WeaponManager = player.GetComponent<WeaponManager>();
            }

            if (m_Player_Sr == null)
            {
                m_Player_Sr = player.GetComponentInChildren<SpriteRenderer>();
            }

            transform.localPosition = Vector3.zero;

            player = this.transform.parent.gameObject;

            if (m_Player_Sr.flipX)
            {
                transform.localPosition = new Vector3(0.35f, 0.05f, 0.0f);
                transform.localRotation = Quaternion.Euler(0, 180, 0);
            }

            if (!m_Player_Sr.flipX)
            {
                transform.localPosition = new Vector3(-0.45f, 0.05f, 0.0f);
                transform.localRotation = Quaternion.Euler(0, 0, 0);
            }

        }
    }

    protected override void ControlAtack()
    {
       base.ControlAtack();
    }

    // Lunge forward (0)
    protected override void FirstAtack()
    {
        SetBoolToAnimator(0);
    }

    // Forward jump with top to bot hit (1)
    protected override void SecondAtack()
    {
        SetBoolToAnimator(1);
    }

    // Trow the sword (2)
    protected override void ThirdAtack()
    {
        if (m_WeaponManager == null)
        {
            m_WeaponManager = player.GetComponent<WeaponManager>();
        }
        m_WeaponManager.SwordJump(m_AtackXDelta, m_AtackYDelta);

        SetBoolToAnimator(2);
    }

    // Some Ult (3)
    protected override void FourthAtack()
    {
        Vector3 _swordPos = transform.position - Vector3.up * 0.5f;
        throwenSword = Instantiate(m_throwSwordPrefab, _swordPos, Quaternion.identity);
        throwenSword.GetComponent<LaserThrowCloneController>().m_Sword = this;
        m_ResetFourthAtack = true;

        SetBoolToAnimator(3);
    }

    private void SetBoolToAnimator(int i)
    {
        switch (i)
        {
            case 0:
                m_Animator.SetBool("FirstAtack", true); // Try as right
                m_Animator.SetBool("SecondAtack", false);
                m_Animator.SetBool("ThirdAtack", false);
                m_Animator.SetBool("FourthAtack", false);
                break;
            case 1:
                m_Animator.SetBool("FirstAtack", false);
                m_Animator.SetBool("SecondAtack", true);
                m_Animator.SetBool("ThirdAtack", false);
                m_Animator.SetBool("FourthAtack", false);
                break;
            case 2:
                m_Animator.SetBool("FirstAtack", false);
                m_Animator.SetBool("SecondAtack", false);
                m_Animator.SetBool("ThirdAtack", true);
                m_Animator.SetBool("FourthAtack", false);
                break;
            case 3:
                m_Animator.SetBool("FirstAtack", false);
                m_Animator.SetBool("SecondAtack", false);
                m_Animator.SetBool("ThirdAtack", false);
                m_Animator.SetBool("FourthAtack", true);
                break;
        }
        
        StartCoroutine(ResetAttack(i));
    }

    protected IEnumerator ResetAttack(int i)
    {
        yield return new WaitForSeconds(0.1f);

        switch (i)
        {
            case 0:
                m_Animator.SetBool("FirstAtack", false);
                break;
            case 1:
                m_Animator.SetBool("SecondAtack", false);
                break;
            case 2:
                m_Animator.SetBool("ThirdAtack", false);
                break;
            case 3:
                m_Animator.SetBool("FourthAtack", false);
                break;
        }
    }

    

    protected override IEnumerator ResetFthAtack()
    {
        yield return new WaitForSeconds(m_SkillsReloads[3]);
        m_ResetFourthAtack = false;
        m_FthAResetter = null;

        m_CanThrowSword = true;
        
    }
}
