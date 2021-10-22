using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlWhip : WeaponParent
{
    [SerializeField]
    private GameObject m_WhipHilt;

    RaycastHit2D raycastHit2D;
    [SerializeField]
    float m_AtackYDelta = 1.7f;
    [SerializeField]
    float m_AtackXDelta = 1.25f;
    [SerializeField]
    Animator m_Animator;
    [SerializeField]
    Transform[] m_WhipPoints;
    private float[] m_PointsVerticalDeltas;
    private LineRenderer m_WhipLine;
    Coroutine m_ReverseIdleDeltas;
    [SerializeField]
    private bool m_FirstAtack = false;
    [SerializeField]
    private bool m_SecondAtack = false;
    [SerializeField]
    private bool m_ThirdAtack = false;
    [SerializeField]
    private bool m_Idle = true;
    [SerializeField]
    private int m_WhipLength = 0;
    [SerializeField]
    private BoxCollider2D m_Collider;
    private float m_HorizontalPlayerInput = 0;
    [SerializeField]
    private float m_ThirdSkillDistance = 6.5f;

    [SerializeField]
    private GameObject[] m_WhipColliders;

    [SerializeField]
    private LayerMask enemyLayer;

    

    private void Awake()
    {
        if (m_Animator == null)
        {
            m_Animator = this.GetComponentInChildren<Animator>();
        }
        m_WhipLine = this.GetComponentInChildren<LineRenderer>();

        // SKills Reloads Setting
        m_SkillsReloads[0] = 0.1f;
        m_SkillsReloads[1] = 1.5f;
        m_SkillsReloads[2] = 3.5f;
        m_SkillsReloads[3] = 2.0f; // 10.0f;
    }


    // Start is called before the first frame update
    void Start()
    {
        base.Start();
        m_Collider = this.GetComponent<BoxCollider2D>();
        m_Animator.SetBool("PlayerOnGround", true);
    }

    // Update is called once per frame
    void Update()
    {

        base.Update();
        AnimateWhip();

        if (raycastHit2D)
        {
            ControlWhipHiltWhileUlt();
        }
    }

    private void ControlWhipHiltWhileUlt()
    {
        if (raycastHit2D.collider.GetComponent<IDamageable>() != null)
        {
            if (this.GetComponentsInChildren<SpriteRenderer>()[0].enabled == false)
            {
                m_WhipHilt.GetComponentInChildren<SpriteRenderer>().enabled = true;
                Vector3 _direction = raycastHit2D.collider.gameObject.transform.position - m_WhipHilt.transform.position;
                var _lookRotation = Quaternion.LookRotation(_direction);

                _lookRotation.y = 0;
                _lookRotation.x = 0;

                float _zRot = _lookRotation.eulerAngles.z;

                if (_zRot > 180)
                {
                    float _dif = Mathf.Abs(_zRot - 180);
                    _zRot = _dif;
                }

                m_WhipHilt.transform.rotation = Quaternion.Euler(0, 0, _zRot);
            }
            else
            {
                m_WhipHilt.GetComponentInChildren<SpriteRenderer>().enabled = false;
            }
        }
    }




    private void AnimateWhip()
    {
        if (isAdopted)
        {
            var _playerController = player.GetComponent<PlayerController>();
            if (_playerController != null)
            {
                m_HorizontalPlayerInput = _playerController.HorizontalInput;
                m_Animator.SetFloat("Horizontal", Mathf.Abs(m_HorizontalPlayerInput));
                bool _grounded = _playerController.CheckGround()
                    || _playerController.LeftSideWall
                    || _playerController.RightSideWall;
                m_Animator.SetBool("PlayerOnGround", _grounded);
            }
        }
    }


    protected override void GetInHands()
    {
        if (isAdopted == true)
        {
            m_Collider.isTrigger = true;
            player = this.transform.parent.gameObject;
            if (m_Player_Sr == null)
            {
                m_Player_Sr = player.GetComponentInChildren<SpriteRenderer>();
            }

            if (m_Player_Sr.flipX)
            {
                var _whipParts = this.GetComponentsInChildren<SpriteRenderer>();
                for (int i = 0; i < _whipParts.Length; i++)
                {
                    _whipParts[i].sortingOrder = 9;
                }

                transform.localRotation = Quaternion.Euler(0, 180, 10);
                transform.localPosition = new Vector3(-0.6f, -1.1f, 0);
            }

            if (!m_Player_Sr.flipX)
            {
                var _whipParts = this.GetComponentsInChildren<SpriteRenderer>();
                for(int i=0; i<_whipParts.Length; i++)
                {
                    _whipParts[i].sortingOrder = 11;
                }

                transform.localRotation = Quaternion.Euler(0, 0, 10);
                transform.localPosition = new Vector3(0.9f, -1.1f, 0);
            }

        }
    }

    

    protected override void FirstAtack()
    {
        SetBoolToAnimator(0);

    }

    protected override void SecondAtack()
    {
        if (m_WeaponManager == null)
        {
             if (player == null)
            {
                player = this.transform.parent.gameObject;
            }

            m_WeaponManager = player.GetComponent<WeaponManager>();
        }
        m_WeaponManager.SwordJump(m_AtackXDelta, m_AtackYDelta);
        SetBoolToAnimator(1);
    }

    protected override void ThirdAtack()
    {
        SetBoolToAnimator(2);
    }

    protected override void FourthAtack()
    {
        Debug.Log("Try Ult atack");
        float _horizontal = m_Player_Sr.flipX ? 1 : -1;
        // Hit raycast
        raycastHit2D = Physics2D.Raycast(
            player.transform.position + Vector3.right * _horizontal,
            Vector2.right * _horizontal,
            m_ThirdSkillDistance,
            1 << 7);
      
        // Draw ray cast for debug
        Debug.DrawRay(
            player.transform.position + Vector3.right * _horizontal,
            Vector2.right
                * _horizontal
                * m_ThirdSkillDistance,
            Color.black);

        if (raycastHit2D.collider != null)
        {
            GameObject _enemyGO = raycastHit2D.collider.gameObject;
            var _enemyController = _enemyGO.GetComponent<EnemyBoss>();
            Debug.Log("Got enemy collider with RayCast");
            if (_enemyController != null)
            {
                Debug.Log("Whip Ult!");
                _enemyController.Damage(3, 3);
            }
        }
    }

    

    protected override void ControlAtack()
    {
        base.ControlAtack();
    }

    private void SetBoolToAnimator(int i)
    {
        bool _right = m_Player_Sr.flipX;

        if (_right)
        {
            switch (i)
            {
                case 0:
                    m_Animator.SetBool("FirstAtackL", true); // try for Right the same as Left atack
                    break;
                case 1:
                    m_Animator.SetBool("SecondAtackL", true); // try for Right the same as Left atack
                    break;
                case 2:
                    m_Animator.SetBool("ThirdAtackL", true); // try for Right the same as Left atack
                    break;
            }
        }
        else
        {
            switch (i)
            {
                case 0:
                    m_Animator.SetBool("FirstAtackL", true);
                    break;
                case 1:
                    m_Animator.SetBool("SecondAtackL", true);
                    break;
                case 2:
                    m_Animator.SetBool("ThirdAtackL", true);
                    break;
            }
        }

        StartCoroutine(ResetAttack(i));
    }

    protected override IEnumerator ResetAttack(int i)
    {
        yield return new WaitForSeconds(0.1f); 

        switch (i)
        {
            case 0:
                m_Animator.SetBool("FirstAtackL", false);
                m_Animator.SetBool("FirstAtackR", false);
                break;
            case 1:
                m_Animator.SetBool("SecondAtackL", false);
                m_Animator.SetBool("SecondAtackR", false);
                break;
            case 2:
                m_Animator.SetBool("ThirdAtackL", false);
                m_Animator.SetBool("ThirdAtackR", false);
                break;
        }
    }

    //private void Idle()
    //{
    //    for(int i=2; i<m_PointsVerticalDeltas.Length; i++)
    //    {
    //        Vector3 _nextPos = m_WhipLine.GetPosition(i) + Vector3.up * m_PointsVerticalDeltas[i] * Time.deltaTime;
    //        m_WhipLine.SetPosition(i, _nextPos);
    //    }

    //    if (Mathf.Abs(m_WhipLine.GetPosition(m_WhipLength-2).y - m_WhipLine.GetPosition(0).y) > 0.15f)
    //    {
    //        if (m_ReverseIdleDeltas == null)
    //        {
    //            m_ReverseIdleDeltas = StartCoroutine(nameof(RevertWhipPointsYDeirections));
    //        }
    //    }

    //}

    //private IEnumerator RevertWhipPointsYDeirections()
    //{
    //    for (int i=1; i<m_PointsVerticalDeltas.Length; i++)
    //    {
    //        yield return new WaitForSeconds(0.05f);
    //        m_PointsVerticalDeltas[i] *= -1;
    //    }

    //    yield return new WaitForSeconds(0.25f);
    //    m_ReverseIdleDeltas = null;
    //}
}
