using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBoss : MonoBehaviour,
    IDamageable
{
    public const int MAX_COUNT_OF_THROWINGS = 4;
    public const string WHIP_LR_MATERIAL = "X_Choice\\Materials\\Shaders\\Material_WhipLineRendererShader";
    [SerializeField]
    private int m_ThrowCounts = 0;

    [SerializeField]
    private SpriteRenderer[] m_WhipParts = new SpriteRenderer[4];


    [SerializeField]
    private float m_LerpDelta = 0.1f;
    [SerializeField]
    private bool m_CanMove = true;
    [SerializeField]
    private string m_PLayerTag = "Player";

    public enum WeaponType { Blaster, LaserSword, Staff, Whip }

    [SerializeField]
    private bool[] m_GottenSkills = new bool[4];
    [SerializeField]
    private int m_WeaponID = -1;
    [SerializeField]
    private GameObject m_PlayerGO;
    [SerializeField]
    private float m_DistanceToPlayerX;
    private float m_DistanceToPlayerY;
    [SerializeField]
    private float m_Aplpha = 0;
    [SerializeField]
    private float[] m_ThrowingAngles = new float[2];
    [SerializeField]
    private bool m_PlayerFlipX = false;
    [SerializeField]
    LineRenderer m_WhipLine;


    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        ProccesDamageFromPlayer();
    }

    private void ThrowByWhip()
    { 
        if (m_ThrowCounts == MAX_COUNT_OF_THROWINGS)
        {
            // Disabling Middle and End parts around Enemy (tight)
            foreach (var _part in m_WhipParts)
            {
                _part.enabled = false;
            }

            this.GetComponentInChildren<SpriteRenderer>().color = Color.white;
            // material - Material_WhipLineRendererShader
            m_WhipLine.positionCount = 0;
            m_WhipLine.enabled = false;

            m_ThrowCounts = 0;
            m_GottenSkills[3] = false;
            m_WhipLine = null;
            m_PlayerGO.GetComponent<WeaponManager>().EnableWeapon();
            return;
        }


        // Draw Whip Line from Player to Enemy

        m_WhipLine = gameObject.GetComponent<LineRenderer>();
        m_WhipLine.SetPosition(0, m_PlayerGO.transform.position);
        m_WhipLine.SetPosition(1, transform.position);
        //


        if (m_ThrowingAngles[0] == 0)
        {

            if (m_Aplpha > m_ThrowingAngles[0] + 0.2f || m_Aplpha < m_ThrowingAngles[1] - 0.2f)
            {
                this.GetComponentInChildren<SpriteRenderer>().color = Color.white;
            }

            if (m_Aplpha > m_ThrowingAngles[1])
            {
                this.GetComponentInChildren<SpriteRenderer>().color = Color.red;
                m_ThrowCounts += 1;
                m_LerpDelta = -Mathf.Abs(m_LerpDelta);
            }

            if (m_Aplpha < m_ThrowingAngles[0])
            {
                this.GetComponentInChildren<SpriteRenderer>().color = Color.red;
                m_ThrowCounts += 1;
                m_LerpDelta = Mathf.Abs(m_LerpDelta);
            }
        }
        else
        {

            if (m_Aplpha < m_ThrowingAngles[0] - 0.2f || m_Aplpha > m_ThrowingAngles[1] + 0.2f)
            {
                this.GetComponentInChildren<SpriteRenderer>().color = Color.white;
            }


            if (m_Aplpha < m_ThrowingAngles[1]) // 0
            {
                this.GetComponentInChildren<SpriteRenderer>().color = Color.red;
                m_LerpDelta = Mathf.Abs(m_LerpDelta);
                m_ThrowCounts += 1;
            }

            if (m_Aplpha > m_ThrowingAngles[0]) // PI
            {
                this.GetComponentInChildren<SpriteRenderer>().color = Color.red;

                m_LerpDelta = -Mathf.Abs(m_LerpDelta);
                m_ThrowCounts += 1;
            }
        }

        m_Aplpha += m_LerpDelta;

        float _xDelta = Mathf.Cos(m_Aplpha);
        float _yDelta = Mathf.Sin(m_Aplpha);
        // Debug.Log("(COS, SIN) = (" + _xDelta + ", " + _yDelta);
//        Debug.Log("Alpha = " + m_Aplpha);

        

        Vector3 _newPos = new Vector3(
            m_PlayerGO.transform.position.x + m_DistanceToPlayerX * _xDelta,
            m_PlayerGO.transform.position.y + m_DistanceToPlayerY * _yDelta,
            0);

       // Debug.Log(_newPos);

        transform.position = _newPos;

    }

    private void ProccesDamageFromPlayer()
    {
        switch (m_WeaponID)
        {
            case (int)WeaponType.Blaster:
                    break;
            case (int)WeaponType.LaserSword:
                break;
            case (int)WeaponType.Staff:
                break;
            case (int)WeaponType.Whip:
                for (int i = 0; i < m_GottenSkills.Length; i++)
                {
                    if (m_GottenSkills[i] == true)
                    {
                        switch (i)
                        {
                            case 0:


                                break;
                            case 1:


                                break;
                            case 2:


                                break;
                            case 3:


                                ThrowByWhip();

                                break;

                        }
                    }
                }

                break;
        }
    }


    public void Damage(int _skillId, int _weaponType)
    {
        m_PlayerGO = GameObject.FindGameObjectsWithTag(m_PLayerTag)[0];
        m_GottenSkills[_skillId] = true;
        m_WeaponID = _weaponType;


        switch (m_WeaponID)
        {
            case (int)WeaponType.Blaster:
                break;
            case (int)WeaponType.LaserSword:
                break;
            case (int)WeaponType.Staff:
                break;
            case (int)WeaponType.Whip:
                m_PlayerGO.GetComponent<WeaponManager>().DisableWeapon();
                Debug.Log("Arrrr, Player Whip Ult");

                // Presset LineRenderer conditions
                m_WhipLine = gameObject.GetComponent<LineRenderer>();
                m_WhipLine.enabled = true;
                m_WhipLine.widthMultiplier = 0.3f;
                m_WhipLine.positionCount = 2;
                //


                for (int i = 0; i < m_GottenSkills.Length; i++)
                {
                    if (m_GottenSkills[i] == true)
                    {
                        switch (i)
                        {
                            case 0:


                                break;
                            case 1:


                                break;
                            case 2:


                                break;
                            case 3:

                                Debug.Log("Get Damage from 3rd Whip skill");

                                // Enabling Middle and End parts around Enemy (tight)
                                foreach (var _part in m_WhipParts)
                                {
                                    _part.enabled = true;
                                }

                                

                                // Calculate angles and player sides for later realizing
                                m_DistanceToPlayerX = Mathf.
                                                        Abs(transform.position.x - m_PlayerGO.transform.position.x);
                                m_DistanceToPlayerY = m_DistanceToPlayerX;

                                m_PlayerFlipX = m_PlayerGO.GetComponentInChildren<SpriteRenderer>().
                                    flipX;

                                if (m_PlayerFlipX == true)
                                {
                                    m_ThrowingAngles[0] = 0;
                                    m_ThrowingAngles[1] = Mathf.PI;
                                    m_LerpDelta = Mathf.Abs(m_LerpDelta);
                                }
                                else
                                {
                                    m_ThrowingAngles[0] = Mathf.PI;
                                    m_ThrowingAngles[1] = 0;
                                    m_LerpDelta = -Mathf.Abs(m_LerpDelta);
                                }

                                m_Aplpha = m_ThrowingAngles[0];

                                // Disable boss to move itself
                                m_CanMove = false;
                                break;

                        }
                    }
                }

                break;

        }
    }

}
