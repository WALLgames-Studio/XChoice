using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponParent : MonoBehaviour, ILevitatable
{
    [SerializeField]
    protected PlayerController m_PlayerController;
    [SerializeField]
    protected SpriteRenderer m_Player_Sr;
    [SerializeField]
    protected Vector2 m_WeaponPosition;
    [SerializeField]
    protected bool isAdopted = false;
    [SerializeField]
    protected SpriteRenderer spriteRenderer;
    [SerializeField]
    protected GameObject player;
    [SerializeField]
    protected WeaponManager m_WeaponManager;
    [SerializeField]
    protected float _horizontal = 0;
    [SerializeField]
    protected float step = 0.1f;
    [SerializeField]
    protected float range = 0;
    [SerializeField]
    protected float levitatingRange = 0.5f;

    [SerializeField]
    protected float[] m_SkillsReloads = new float[4];

    // Attacks Resetters
    [SerializeField]
    protected bool m_ResetFirstAtack;
    [SerializeField]
    protected bool m_ResetSecondAtack;
    [SerializeField]
    protected bool m_ResetThirdAtack;
    [SerializeField]
    protected bool m_ResetFourthAtack;

    protected Coroutine m_FAResetter;
    protected Coroutine m_SAResetter;
    protected Coroutine m_TAResetter;
    protected Coroutine m_FthAResetter;

    public Vector2 WeaponPosition
    {
        get { return m_WeaponPosition; }
        set
        {
            m_WeaponPosition = new Vector2(value.x, value.y);
        }
    }

    public bool IsAdopted
    {
        get
        {
            return isAdopted;
        }
        set
        {
            isAdopted = value;
        }
    }

    private void Awake()
    {
        for(int i=0; i<m_SkillsReloads.Length; i++)
        {
            m_SkillsReloads[i] = 0.5f;
        }
    }


    protected void Start()
    {
        
        spriteRenderer = this.gameObject.GetComponentInChildren<SpriteRenderer>();

    }

    // Update is called once per frame
    protected void Update()
    {
        ReloadAtacks();
        ControlAtack();
        GetInHands();
        Levitate();

    }



    protected virtual void GetInHands()
    {
        if (isAdopted == true)
        {
            player = this.transform.parent.gameObject;
            m_PlayerController = player.GetComponent<PlayerController>();

            if (m_Player_Sr == null)
            {
                m_Player_Sr = player.GetComponentInChildren<SpriteRenderer>();
            }

            if (m_Player_Sr.flipX)
            {
                transform.localRotation = Quaternion.Euler(0, 180, 0);
            }

            if (!m_Player_Sr.flipX)
            {
                transform.localPosition = new Vector3(-0.2f, -0.1f, 0);
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

    protected virtual void ControlAtack()
    {
        if (isAdopted)
        {
            if (Input.GetKey(KeyCode.J))
            {
                if (m_ResetFirstAtack == false)
                {
                    FirstAtack();
                    m_ResetFirstAtack = true;
                }
            }

            if (Input.GetKey(KeyCode.K))
            {
                if (m_ResetSecondAtack == false)
                {
                    SecondAtack();
                    m_ResetSecondAtack = true;
                }
            }

            if (Input.GetKey(KeyCode.L))
            {
                if (m_ResetThirdAtack == false)
                {
                    ThirdAtack();
                    m_ResetThirdAtack = true;
                }
            }

            if (Input.GetKey(KeyCode.RightShift))
            {
                if (m_ResetFourthAtack == false)
                {
                    FourthAtack();
                    m_ResetFourthAtack = true;
                }
            }
        }
    }

    protected virtual IEnumerator ResetFAtack()
    {
        yield return new WaitForSeconds(m_SkillsReloads[0]);
        m_ResetFirstAtack = false;
        m_FAResetter = null;
    }

    protected virtual IEnumerator ResetSAtack()
    {
        yield return new WaitForSeconds(m_SkillsReloads[1]);
        m_ResetSecondAtack = false;
        m_SAResetter = null;
    }

    protected virtual IEnumerator ResetTAtack()
    {
        yield return new WaitForSeconds(m_SkillsReloads[2]);
        m_ResetThirdAtack = false;
        m_TAResetter = null;
    }

    protected virtual IEnumerator ResetFthAtack()
    {
        yield return new WaitForSeconds(m_SkillsReloads[3]);
        m_ResetFourthAtack = false;
        m_FthAResetter = null;
    }


    protected virtual void ReloadAtacks()
    {
        if (m_ResetFirstAtack == true)
        {
            if (m_FAResetter == null)
            {
                m_FAResetter = StartCoroutine(nameof(ResetFAtack));
            }
        }
        if (m_ResetSecondAtack == true)
        {
            if (m_SAResetter == null)
            {
                m_SAResetter = StartCoroutine(nameof(ResetSAtack));
            }
        }
        if (m_ResetThirdAtack == true)
        {
            if (m_TAResetter == null)
            {
                m_TAResetter = StartCoroutine(nameof(ResetTAtack));
            }
        }
        if (m_ResetFourthAtack == true)
        {
            if (m_FthAResetter == null)
            {
                m_FthAResetter = StartCoroutine(nameof(ResetFthAtack));
            }
        }
    }

    protected virtual void FourthAtack()
    {
        throw new NotImplementedException();
    }

    protected virtual void ThirdAtack()
    {
        throw new NotImplementedException();
    }

    protected virtual void SecondAtack()
    {
        throw new NotImplementedException();
    }

    protected virtual void FirstAtack()
    {
        throw new NotImplementedException();
    }

    protected virtual IEnumerator ResetAttack(int i)
    {
        yield return new WaitForSeconds(0.25f);
        
    }
}
