using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelControlller : MonoBehaviour
{
    private int levelPoints = 0;

    public int LevelPoints
    {
        get
        {
            return levelPoints;
        }

        set
        {
            levelPoints += value;
            if (levelPoints > 100000)
            {
                levelPoints = 100000;
            }
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


}
