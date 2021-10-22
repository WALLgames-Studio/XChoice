using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallJumpingController : MonoBehaviour
{
    private GameObject m_WallObject;
    public int jumpWallLayer = 6;

    [SerializeField]
    private bool isOnJumpWall = false;

    private PlayerController playerController;

    // Start is called before the first frame update
    void Start()
    {
        playerController = this.GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        SlideDown();
        StopVelocityAtGround();
    }

    private void SlideDown()
    {
        if (playerController.IsGgrounded == false &&
            playerController.m_RigidBody.velocity.y < 0
            && (playerController.LeftSideWall || playerController.RightSideWall))
        {
            playerController.m_RigidBody.gravityScale = 0.25f;
        }
        else
        {
            playerController.m_RigidBody.gravityScale = 3;
        }
    }
    
    private void StopVelocityAtGround()
    {
        if (playerController.IsGgrounded && playerController.m_RigidBody.velocity.x != 0)
        {
            playerController.m_RigidBody.velocity = new Vector2(0, playerController.m_RigidBody.velocity.y);
        }
    }
}
