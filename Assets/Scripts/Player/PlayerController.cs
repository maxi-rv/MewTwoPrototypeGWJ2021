using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // GAMEOBJECT COMPONENTS
    private Rigidbody2D rigidBody2D;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    [SerializeField] private Collider2D pushBox;
    [SerializeField] private Collider2D hurtBox;
    [SerializeField] private CheckHit checkHit;
    [SerializeField] private CheckGround checkGround;
    [SerializeField] private CheckWall checkWall;
    [SerializeField] private CheckTree checkTree;
    [SerializeField] private Material matDefault;
    [SerializeField] private Material matWhite;
    [SerializeField] private GameObject shurikenPrefab;
    
    // VARIABLES
    public float maxHP = 5f;
    public float currentHP;
    [SerializeField] private bool secondJumpAvailable;
    [SerializeField] private bool wallJumpAvailable;
    [SerializeField] private bool treeJumpAvailable;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpForce;
    [SerializeField] private float wallJumpForce;
    [SerializeField] private float shurikenSpeed;
    [SerializeField] private bool onTheGround;
    [SerializeField] private bool againstWall;
    [SerializeField] private bool overATree;
    [SerializeField] private bool climbingTree;
    [SerializeField] private int leavePlatformFlagger;

    // INPUT VARIABLES
    private float horizontalAxis;
    private float verticalAxis;
    private bool jumpButton;
    private bool hangButton;
    private bool attackButton;

    // Awake is called when the script instance is being loaded.
    void Awake()
    {
        // Gets COMPONENTS from the Children
        animator = gameObject.GetComponent<Animator>();
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        rigidBody2D = gameObject.GetComponent<Rigidbody2D>();

        //Getting the dafault material for later use.
        matDefault = spriteRenderer.material;

        // Initializes Variables
        currentHP = maxHP;
        secondJumpAvailable = false;
        wallJumpAvailable = false;
        treeJumpAvailable = false;
        againstWall = false;
        overATree = false;
        climbingTree = false;
        leavePlatformFlagger = 0;
    }

    // FixedUpdate is called multiple times per frame.
    void FixedUpdate()
    {
        CheckHit();
        CheckGround();
        Movement();
    }

    // Update is called once per frame.
    void Update()
    {
        // Gets Axis Input
        horizontalAxis = Input.GetAxisRaw("Horizontal");
        verticalAxis = Input.GetAxisRaw("Vertical");
        jumpButton = Input.GetButtonDown("Jump");
        hangButton = Input.GetButtonDown("Jump");
        attackButton = Input.GetButtonDown("Attack");

        //Checks contact with enviroment elements 
        againstWall = checkWall.againstWallLeft || checkWall.againstWallRight;
        overATree = checkTree.overATree;
        //Checking the Ground is a separate check on "CheckGround()"

        if(attackButton)
        {
            animator.SetTrigger("RangedAttack");
        }

        if(hangButton && overATree && !onTheGround)
        {
            rigidBody2D.bodyType = RigidbodyType2D.Kinematic;
        }

        //Checking for the JumpButton is here instead of UpdateFixed, because it needs to check immediately
        if(jumpButton)
        {
            if (!onTheGround && checkWall.againstWallRight)
            {
                WallJump(-1);
                wallJumpAvailable = false;
            }
            else if (!onTheGround && checkWall.againstWallLeft)
            {
                WallJump(1);
                wallJumpAvailable = false;
            }
            else if (!onTheGround && secondJumpAvailable)
            {
                Jump();
                secondJumpAvailable = false;
            }
            else if(onTheGround && verticalAxis<0f && checkGround.otherTag=="Platform")
            {
                disablePushBox();
                leavePlatformFlagger = 1;
            }
            else if (onTheGround)
            {
                JumpStart();
            }
        }
    }

    // Checks Input, calls for Movement, and sets facing side.
    private void Movement()
    {
        // If Left or Right is pressed.
        if (horizontalAxis > 0.5f || horizontalAxis < -0.5f)
        {
            MovementOnAxis(horizontalAxis);

            animator.SetBool("Moving", true);

            if(rigidBody2D.velocity.y <= 1f)
                FlipSprite(horizontalAxis);
        }

        // If NO Left or Right is pressed.
        if(horizontalAxis < 0.1f && horizontalAxis > -0.1f)
        {
            if(onTheGround)
                MovementOnAxis(0f);

            animator.SetBool("Moving", false);
        }
    }

    // Changes the velocity of the Rigidbody according to the values passed (-1f, 0f, or 1f).
    // Because it can recieve 0f as value, this function can stop movement.
    // moveHorizontal : X Value.
    private void MovementOnAxis(float moveHorizontal)
    {
        float movementDirection = moveHorizontal * moveSpeed;
        
        //if(againstWall)
        //{
        //    rigidBody2D.velocity = new Vector2(movementDirection, rigidBody2D.velocity.y/1.25f);
        //}
        //else if

        if(wallJumpAvailable)
        {
            rigidBody2D.velocity = new Vector2(movementDirection, rigidBody2D.velocity.y);
        }
        else if(!wallJumpAvailable && rigidBody2D.velocity.normalized.y<=0.25f)
        {
            rigidBody2D.velocity = new Vector2(movementDirection, rigidBody2D.velocity.y);
        }   
    }

    // Flips the sprite on the X Axis according to the Axis input.
    // Needs to be called by another function, which must check the facing sides first.
    private void FlipSprite(float HorizontalAxis)
    {
        if(HorizontalAxis > 0.1f)
        {
            spriteRenderer.flipX = false;
        }

        if(HorizontalAxis < -0.1f)
        {
            spriteRenderer.flipX = true;
        }
    }

    // Checks on other scripts that are constantly checking for collision. 
    private void CheckGround()
    {
        onTheGround = checkGround.onTheGround;

        if(!onTheGround)
        {
            if (rigidBody2D.velocity.normalized.y<0f)
            {
                animator.SetBool("Falling", true);
                animator.SetBool("Jumping", false);
            }
        }

        if (onTheGround)
        {
            if(checkGround.otherTag=="Platform")
            {
                
            }
            else if(checkGround.otherTag == "Ground")
            {
                animator.SetBool("Jumping", false);
                animator.SetBool("Falling", false);
                secondJumpAvailable = true;
                wallJumpAvailable = true;
            }
        }
        
    }

    // Adds a vertical force to the player.
    private void JumpStart()
    {
        rigidBody2D.velocity = new Vector2(rigidBody2D.velocity.x, 0f);
        animator.SetBool("JumpingStart", true);
    }

    public void Jump()
    {
        rigidBody2D.velocity = new Vector2(rigidBody2D.velocity.x, 0f);
        rigidBody2D.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
        checkGround.onTheGround = false;
        animator.SetBool("JumpingStart", false);
        animator.SetBool("Jumping", true);
        animator.SetBool("Falling", false);
    }

    // Adds an inclined force to the player.
    private void WallJump(float sign)
    {
        rigidBody2D.velocity = new Vector2(0f, 0f);
        rigidBody2D.AddForce(new Vector2(sign*wallJumpForce/1.5f, wallJumpForce/1f), ForceMode2D.Impulse);
        checkGround.onTheGround = false;
        animator.SetBool("Jumping", true);
        animator.SetBool("Falling", false);
        FlipSprite(sign);
    }

    

    // Checks if the HurtBox has collided with a HitBox from another Object.
    private void CheckHit()
    {
        if(checkHit.isHurt)
        {
            GetHurt();
        }
    }

    // Sets required variables to execute the hurting state.
    private void GetHurt()
    {
        checkHit.isHurt = false;
        hurtBox.enabled = false;
        currentHP--;

        // Make a knockback or some hurting effect!!!
        FlashWhite();
        Invoke("FlashBack", 0.1f);
        Invoke("FlashWhite", 0.2f);
        Invoke("FlashBack", 0.3f);
        Invoke("FlashWhite", 0.4f);
        Invoke("FlashBack", 0.5f);
        Invoke("FlashWhite", 0.6f);
        Invoke("FlashBack", 0.7f);
        Invoke("StopHurt", 0.8f);
    }

    private void FlashWhite()
    {
        spriteRenderer.material = matWhite;
    }

    private void FlashBack()
    {
        spriteRenderer.material = matDefault;
    }

    // Sets required variables to stop the hurting state.
    public void StopHurt()
    {
        hurtBox.enabled = true;
    }
    
    private void disablePushBox()
    {
        pushBox.enabled = false;
    }

    private void enablePushBox()
    {
        pushBox.enabled = true;
    }

    // Sets required variables to stop the attacking state.
    public void StopAttack()
    {
        attackButton = false;
    }

    //...
    public void ShootShuriken()
    {
        if(spriteRenderer.flipX == true)
        {
            Quaternion shurikenRotation = new Quaternion(0f, 0f, 0f, 0f);
            shurikenRotation.eulerAngles = new Vector3(0f, 0f, 90f);
            Vector2 plusVector = new Vector2(-0.35f, 0.14f);

            GameObject shuriken = Instantiate(shurikenPrefab, rigidBody2D.position+plusVector, shurikenRotation);
            Rigidbody2D shurikenRB = shuriken.GetComponent<Rigidbody2D>();
            
            Vector2 arrowDirection = new Vector2(-1f, 0f);
            shurikenRB.AddForce(arrowDirection*shurikenSpeed, ForceMode2D.Impulse);
        }

        if(spriteRenderer.flipX == false)
        {
            Quaternion shurikenRotation = new Quaternion(0f, 0f, 0f, 0f);
            shurikenRotation.eulerAngles = new Vector3(0f, 0f, 270);
            Vector2 plusVector = new Vector2(0.35f, 0.14f);

            GameObject arrow = Instantiate(shurikenPrefab, rigidBody2D.position+plusVector, shurikenRotation);
            Rigidbody2D arrowRB = arrow.GetComponent<Rigidbody2D>();
            
            Vector2 arrowDirection = new Vector2(1f, 0f);
            arrowRB.AddForce(arrowDirection*shurikenSpeed, ForceMode2D.Impulse);
        }
        
    }
}
