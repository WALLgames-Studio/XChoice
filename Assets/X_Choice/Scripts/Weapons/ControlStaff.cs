using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlStaff : WeaponParent
{
    PolygonCollider2D m_Collider;
    [SerializeField]
    private Animator m_Animator;
    [SerializeField]
    private float m_Horizontal;
    [SerializeField]
    private GameObject m_StaffBullet;
    [SerializeField]
    private Transform m_BulletPlace;

    private Coroutine m_FireCoroutine;

    [SerializeField]
    private GameObject m_ThirdsSkilParent;
    [SerializeField]
    private GameObject m_UltSkillParent;

    [SerializeField]
    private SpriteRenderer[] m_ThirdsSkillLightBoxes;

    [SerializeField]
    private float m_UltDistance = 10.0f;
    [SerializeField]
    private SpriteRenderer[] m_UltSkillSprites;
    private Vector3 m_InitiateUltDownSpritePosition;

    private Coroutine m_ThirdSkillCoroutine;
    private Coroutine m_FourthSkillCoroutine;

    private RaycastHit2D raycastHit2D;


    private void Awake()
    {
        m_Collider = this.GetComponent<PolygonCollider2D>();

        

    }

    // Start is called before the first frame update
    void Start()
    {
        base.Start();
        m_Animator = this.GetComponentInChildren<Animator>();

        // SKills Reloads Setting
        m_SkillsReloads[0] = 1.1f;
        m_SkillsReloads[1] = 5.5f;
        m_SkillsReloads[2] = 3.5f;
        m_SkillsReloads[3] = 6.0f; // 10.0f;
    }

    // Update is called once per frame
    void Update()
    {
        base.Update();

    }

    protected override void GetInHands()
    {
        if (isAdopted == true)
        { 
            m_Collider.isTrigger = true;

            player = this.transform.parent.gameObject;
            m_PlayerController = player.GetComponent<PlayerController>();

            if (m_Player_Sr == null)
            {
                m_Player_Sr = player.GetComponentInChildren<SpriteRenderer>();
            }

            if (m_Player_Sr.flipX)
            {
                transform.localRotation = Quaternion.Euler(0, 180, 0);
                transform.localPosition = new Vector3(0.4f, -0.35f, 0);
            }

            if (!m_Player_Sr.flipX)
            {
                transform.localRotation = Quaternion.Euler(0, 0, 0);
                transform.localPosition = new Vector3(-0.4f, -0.35f, 0);
            }

            AnimateStaff();

        }
    }

    protected override void FirstAtack()
    {
        if (m_FireCoroutine == null)
        {
           StartCoroutine(nameof(InstantiateStaffBullet));
        }
        SetBoolToAnimator(0);
    }

    protected override void SecondAtack()
    {
        SetBoolToAnimator(1);
    }

    protected override void ThirdAtack()
    {
        SetBoolToAnimator(2);

        if (m_ThirdSkillCoroutine == null)
        {
            m_ThirdSkillCoroutine = StartCoroutine(nameof(ThirdSkillLights));
        }
    }

    private IEnumerator ThirdSkillLights()
    {
        m_ThirdsSkilParent.SetActive(true);

        foreach (var _sprite in m_ThirdsSkillLightBoxes)
        {
            yield return new WaitForSeconds(0.1f);
            _sprite.enabled = true;
        }

        yield return new WaitForSeconds(.25f);

        foreach (var _sprite in m_ThirdsSkillLightBoxes)
        {
            yield return new WaitForSeconds(0.1f);
            _sprite.enabled = false;
        }

        m_ThirdsSkilParent.SetActive(false);

        m_ThirdSkillCoroutine = null;
    }

    protected override void FourthAtack()
    {
        SetBoolToAnimator(3);

        if (m_FourthSkillCoroutine == null)
        {
            m_FourthSkillCoroutine = StartCoroutine(nameof(UltAppeare));
        }

    }

    private IEnumerator UltAppeare()
    {
        GameObject _rootParent = null;

        m_UltSkillSprites = m_UltSkillParent.GetComponentsInChildren<SpriteRenderer>();

        _horizontal = m_Player_Sr.flipX ? 1 : -1;

        Vector3 _playerPos = player.transform.position;
        Vector3 _maxPos = _playerPos + Vector3.right * m_UltDistance * _horizontal;

        Debug.DrawRay(_playerPos, _maxPos, Color.green);

        // Hit enemy
        raycastHit2D = Physics2D.Raycast(
            player.transform.position + Vector3.right * _horizontal,
            Vector2.right * _horizontal,
            m_UltDistance,
            1 << 7);

        if (raycastHit2D.collider != null)
        {
            GameObject _enemyGO = raycastHit2D.collider.gameObject;
            var _enemyController = _enemyGO.GetComponent<EnemyBoss>();
            Debug.Log("Got enemy collider with RayCast");
            if (_enemyController != null)
            {
                Debug.Log("Staff Ult!");
                _enemyController.Damage(3, 2);
            }

            RaycastHit2D _groundHit = Physics2D.Raycast(
                _enemyGO.transform.position,
                Vector2.down * 3,
                m_UltDistance,
                1 << 3);

            Debug.DrawRay(_enemyGO.transform.position, Vector3.down * 3, Color.yellow);

            if (_groundHit.collider != null)
            {
                m_InitiateUltDownSpritePosition = new Vector3(
                    _enemyGO.transform.position.x,
                    _groundHit.collider.gameObject.transform.position.y + 0.5f,
                    _enemyGO.transform.position.z);

                yield return new WaitForSeconds(0.1f);

                yield return new WaitForSeconds(0.5f);
                _rootParent = Instantiate(m_UltSkillParent, m_InitiateUltDownSpritePosition, Quaternion.identity);
            }
        }

        Destroy(_rootParent, 5.5f);
        m_FourthSkillCoroutine = null;

    }

    private IEnumerator InstantiateStaffBullet()
    {
        yield return new WaitForSeconds(0.35f);
        var _bullet = Instantiate(m_StaffBullet, m_BulletPlace.position, Quaternion.identity);
    }

    private void SetBoolToAnimator(int i)
    {
        switch (i)
        {
            case 0:
                m_Animator.SetBool("FirstAtack", true); 
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

    protected override IEnumerator ResetAttack(int i)
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

    protected override void ControlAtack()
    {
        base.ControlAtack();
    }

    private void AnimateStaff()
    {
        m_Horizontal = m_PlayerController.HorizontalInput;
        m_Animator.SetFloat("Horizontal", Mathf.Abs(m_Horizontal));
    }
}
