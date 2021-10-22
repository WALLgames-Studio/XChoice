using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaffRootController : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer[] m_Sprites;

    // Start is called before the first frame update
    void Start()
    {
        m_Sprites = this.GetComponentsInChildren<SpriteRenderer>();
        StartCoroutine(nameof(Disappeare));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator Disappeare()
    {
        yield return new WaitForSeconds(3.0f);
        foreach (var _sprite in m_Sprites)
        {
            _sprite.enabled = false;
        }
    }
}
