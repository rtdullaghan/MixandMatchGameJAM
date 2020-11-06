using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class Player : MonoBehaviour
{
    Rigidbody2D myRigidBody;
    Animator myAnimator;
    CapsuleCollider2D myBodyColider;
    BoxCollider2D myFeet;
    public BoxCollider2D myArms = new BoxCollider2D();
    float GravityScaleAtStart;

    [SerializeField]
    float runningSpeed = 3f;
    [SerializeField]
    float jumpSpeed = 28f;
    [SerializeField]
    float climbSpeed = 2f;
    // Start is called before the first frame update
    void Start()
    {
        myRigidBody = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        myBodyColider = GetComponent<CapsuleCollider2D>();
        myFeet = GetComponent<BoxCollider2D>();
       // myArms = transform.GetChild(0).gameObject.GetComponent<BoxCollider2D>();
        GravityScaleAtStart = myRigidBody.gravityScale;
    }

    // Update is called once per frame
    void Update()
    {
        Run();
        WallJump();
        Climb();
        Jump();
        FlipSprite();
    }

    void Run()
    {
        float controlThrow;
        Vector2 playerVelocity;
       // if (myArms.IsTouchingLayers(LayerMask.GetMask("Ground"))) return ;
        controlThrow = CrossPlatformInputManager.GetAxis("Horizontal"); // Value is between -1 and +1
        playerVelocity = new Vector2(controlThrow * runningSpeed, myRigidBody.velocity.y);
        myRigidBody.velocity = playerVelocity;

        myAnimator.SetBool("Running", true);
    }
    void WallJump()
    {
        if (!myArms.IsTouchingLayers(LayerMask.GetMask("Ground"))) return;

        float controlThrow = -CrossPlatformInputManager.GetAxis("Horizontal");
        
        if (CrossPlatformInputManager.GetButtonDown("Jump"))
        {
            Vector2 jumpVelocity = new Vector2((jumpSpeed) * controlThrow, jumpSpeed);
            myRigidBody.velocity = jumpVelocity;
        }
    }

    void FlipSprite()
    {
        bool playerHasHorizontalSpeed = Mathf.Abs(myRigidBody.velocity.x) > Mathf.Epsilon;
        if (playerHasHorizontalSpeed)
        {
            transform.localScale = new Vector2(Mathf.Sign(myRigidBody.velocity.x), 1f);
        }
        else { myAnimator.SetBool("Running", false); }
    }

    void Jump()
    {
        if (!myFeet.IsTouchingLayers(LayerMask.GetMask("Ground")) && !myFeet.IsTouchingLayers(LayerMask.GetMask("Climbing"))) return;
        if (myArms.IsTouchingLayers(LayerMask.GetMask("Ground"))) return;
       
        if (CrossPlatformInputManager.GetButtonDown("Jump"))
        {
            Vector2 jumpVelocity = new Vector2(0f, jumpSpeed);
            myRigidBody.velocity = jumpVelocity;
        }
    }

    void Climb()
    {
        if (!myFeet.IsTouchingLayers(LayerMask.GetMask("Climbing"))) {
            myAnimator.SetBool("Climbing", false);
            myRigidBody.gravityScale = GravityScaleAtStart;
            return;
        }

        float controlThrow = CrossPlatformInputManager.GetAxis("Vertical");
        Vector2 climbVelocity = new Vector2(myRigidBody.velocity.x, controlThrow * climbSpeed);
        myRigidBody.velocity = climbVelocity;
        myRigidBody.gravityScale = 0f;

        bool playerHasVerticalSpeed = Mathf.Abs(myRigidBody.velocity.y) > Mathf.Epsilon;
        myAnimator.SetBool("Climbing", playerHasVerticalSpeed);
    }
}
