using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlBlaster : WeaponParent
{
    private Vector3 m_BulletPosition;
    [SerializeField]
    private GameObject bulletPrefab;
    [SerializeField]
    private GameObject m_bombPrefab;
    [SerializeField]
    private float delayAmongBullets = 0.5f;
    [SerializeField]
    private bool resetBullet = false;
    private Coroutine bulletResetter;
    private PlayerController playerController;

    [SerializeField]
    private bool m_resetBomb = false;
    private Coroutine m_BombResetter;
    [SerializeField]
    private float m_DelayAmongBombs = 2.5f;

    [SerializeField]
    private bool m_resetJump = false;
    private Coroutine m_jumpResetter;
    [SerializeField]
    private float m_DelayAmongJumps = 4.5f;
    [SerializeField]
    BoxCollider2D m_Collider;


    [SerializeField]
    LineRenderer blasterLine;
    [SerializeField]
    GameObject laserObject;
    [SerializeField]
    private float blusterJumpHeight = 10;

    private bool flippBlaster = false;


    private void Start()
    {
        spriteRenderer = this.gameObject.GetComponentInChildren<SpriteRenderer>();
        m_Collider = this.GetComponent<BoxCollider2D>();

        // SKills Reloads Setting
        m_SkillsReloads[0] = 0.1f;
        m_SkillsReloads[1] = 5.5f;
        m_SkillsReloads[2] = 3.5f;
        m_SkillsReloads[3] = 2.0f; // 10.0f;
    }

    // Update is called once per frame
    void Update()
    {
        GetInHands();
        Levitate();
        ControlAtack();

    }

    private IEnumerator ResetBullet()
    {
        if (resetBullet == true)
        {
            yield return new WaitForSeconds(delayAmongBullets);
            resetBullet = false;
            bulletResetter = null;
        }
    }

    private IEnumerator ResetBombs()
    {
        if (m_resetBomb == true)
        {
            yield return new WaitForSeconds(m_DelayAmongBombs);
            m_resetBomb = false;
            m_BombResetter = null;
        }
    }

    private IEnumerator ResetBlusterJump()
    {
        if (m_resetJump == true)
        {
            Rigidbody2D player_Rb = player.GetComponent<Rigidbody2D>();
            SpriteRenderer player_Sr = player.GetComponentInChildren<SpriteRenderer>();

            yield return new WaitForSeconds(0.1f);

            float _horizontal = 0;

            if (player_Sr.flipX == true)
            {
                _horizontal = 1;
            }

            if (player_Sr.flipX == false)
            {
                _horizontal = -1;
            }
            player_Rb.AddForce(new Vector3(_horizontal, 1, 0) * blusterJumpHeight, ForceMode2D.Force);

            yield return new WaitForSeconds(m_DelayAmongJumps);
            m_resetJump = false;
            m_jumpResetter = null;
        }
    }

    // Pull simple bullets
    private void SimpleFire()
    {
        if (isAdopted == true)
        {
            var bullet = Instantiate(bulletPrefab, m_BulletPosition, bulletPrefab.transform.rotation);
            Destroy(bullet, 0.75f);
        }
    }

    private void BombFire()
    {
        Vector3 startPos = transform.position + new Vector3(0.1f, 0.1f, 0);
        var bomb = Instantiate(m_bombPrefab, startPos, m_bombPrefab.transform.rotation);
        Destroy(bomb, 2.0f);
    }

    // Jump ob bluster fire bomb
    private void BlusterJump()
    {
        Rigidbody2D player_Rb = player.GetComponent<Rigidbody2D>();
        SpriteRenderer player_Sr = player.GetComponentInChildren<SpriteRenderer>();


        player_Rb.AddForce(Vector3.up * blusterJumpHeight / 2, ForceMode2D.Force);

    }

    private void GetInHands()
    {
        if (isAdopted == true)
        {

            m_Collider.isTrigger = true;

            player = this.transform.parent.gameObject;
            SpriteRenderer player_Sr = player.GetComponentInChildren<SpriteRenderer>();

            if (player_Sr.flipX)
            {
                m_BulletPosition = transform.position - Vector3.up * 0.2f + Vector3.right * 0.75f;

                transform.localPosition = new Vector3(0.2f, -0.1f);
            }

            if (!player_Sr.flipX)
            {
                m_BulletPosition = transform.position - Vector3.up * 0.2f - Vector3.right * 0.75f;

                transform.localPosition = new Vector3(-0.2f, -0.1f);
            }

        }
    }

    public void Levitate()
    {
        if (isAdopted == false)
        {
            float modifier = Time.deltaTime * 2.5f;
            if (Mathf.Abs(range) <= levitatingRange)
            {

                range += step * modifier;
                Vector3 posDelta = new Vector3(0, range * modifier, 0);
                transform.position += posDelta;
//                Debug.Log(range);
            }
            else
            {
                step = -step;
                range = 0;
            }


        }
    }

    private void RenderBlasterLine()
    {

        if (laserObject == null)
        {
            laserObject = new GameObject();
            laserObject.transform.parent = this.transform;
        }

        laserObject.transform.position = this.transform.position;
        playerController = player.GetComponent<PlayerController>();
        var _flipX = playerController.GetComponentInChildren<SpriteRenderer>().flipX;
        if (_flipX == false)
        {
            _horizontal = -1;
        }

        if (_flipX == true)
        {
            _horizontal = 1;
        }

        Vector3 startPoint = laserObject.transform.position - Vector3.up * 0.2f;
        Vector3 endPoint = new Vector3(Mathf.Abs(startPoint.x) * 100 * _horizontal, startPoint.y, startPoint.z);

        var blasterPoints = new Vector3[2];
        blasterPoints[0] = startPoint;
        blasterPoints[1] = endPoint;

        if (blasterLine == null)
            blasterLine = laserObject.AddComponent<LineRenderer>();

        blasterLine.SetPositions(blasterPoints);

        blasterLine.transform.parent = this.transform;
        blasterLine.startWidth = 0.3f;
        blasterLine.endWidth = 0.35f;
        blasterLine.startColor = Color.yellow;
        blasterLine.endColor = Color.yellow;
    }

    protected override void FirstAtack()
    {
        if (resetBullet == false)
        {
            SimpleFire();
            resetBullet = true;
            bulletResetter = StartCoroutine(nameof(ResetBullet));
        }
    }
    protected override void SecondAtack()
    {
        if (m_resetBomb == false)
        {
            BombFire();
            m_resetBomb = true;

            m_BombResetter = StartCoroutine(nameof(ResetBombs));
        }
    }

    protected override void ThirdAtack()
    {
        if (m_resetJump == false)
        {
            BlusterJump();
            m_resetJump = true;

            m_jumpResetter = StartCoroutine(nameof(ResetBlusterJump));
        }
    }

    protected override void FourthAtack()
    {
        RenderBlasterLine();
    }

    protected override void ControlAtack()
    {
        if (isAdopted)
        {
            // Default fire with bullets
            if (Input.GetKey(KeyCode.J))
            {
                FirstAtack();
            }
            // Bomb
            if (Input.GetKey(KeyCode.K))
            {
                SecondAtack();
            }

            // Jump on Bluster
            if (Input.GetKey(KeyCode.L))
            {
                ThirdAtack();
            }

            // Big Bluster Laser
            this.GetComponent<BoxCollider2D>().isTrigger = true;
            if (Input.GetKey(KeyCode.RightShift))
            {
                FourthAtack();
            }

            if (Input.GetKeyUp(KeyCode.RightShift))
            {
                Debug.Log("StopFire!");
//                blasterLine = null;
                Destroy(laserObject);
            }
        }
    }

}
