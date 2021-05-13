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
    [SerializeField] private Material matDefault;
    [SerializeField] private Material matWhite;
    
    // VARIABLES
    private float maxHP = 5f;
    private float currentHP;
    [SerializeField] private bool secondJumpAvailable;
    [SerializeField] private bool wallJumpAvailable;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float regularJumpForce;
    [SerializeField] private float wallJumpForce;
    [SerializeField] private bool onTheGround;
    private bool againstWall;

    // INPUT VARIABLES
    private float HorizontalAxis;
    private float VerticalAxis;
    private bool JumpButton;

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
        againstWall = false;
    }

    // FixedUpdate is called multiple times per frame.
    void FixedUpdate()
    {
        //CheckHit();
        CheckContact();
        Movement();
    }

    // Update is called once per frame.
    // Is executed after FixedUpdate.
    void Update()
    {
        // Gets Axis Input
        HorizontalAxis = Input.GetAxisRaw("Horizontal");
        VerticalAxis = Input.GetAxisRaw("Vertical");
        JumpButton = Input.GetButtonDown("Jump");
        againstWall = checkWall.getAgainstWallLeft() || checkWall.getAgainstWallRight();

        //CHhecking for the JumpButton is here instead of UpdateFixed, because it needs to check immediately
        if(JumpButton)
        {
            
            if (!onTheGround && checkWall.getAgainstWallRight())
            {
                WallJump(-1);
                wallJumpAvailable = false;
            }
            else if (!onTheGround && checkWall.getAgainstWallLeft())
            {
                WallJump(1);
                wallJumpAvailable = false;
            }
            else if (!onTheGround && secondJumpAvailable)
            {
                Jump();
                secondJumpAvailable = false;
            }
            else if (onTheGround)
            {
                Jump();
            }
        }
    }

    // Checks Input, calls for Movement, and sets facing side.
    private void Movement()
    {
        // If Left or Right is pressed.
        if (HorizontalAxis > 0.5f || HorizontalAxis < -0.5f)
        {
            MovementOnAxis(HorizontalAxis);

            animator.SetBool("Moving", true);

            if(rigidBody2D.velocity.y <= 1f)
                FlipSprite(HorizontalAxis);
        }

        // If NO Left or Right is pressed.
        if(HorizontalAxis < 0.1f && HorizontalAxis > -0.1f)
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
        
        if(againstWall)
        {
            rigidBody2D.velocity = new Vector2(movementDirection, rigidBody2D.velocity.y/1.25f);
        }
        else if(wallJumpAvailable)
        {
            rigidBody2D.velocity = new Vector2(movementDirection, rigidBody2D.velocity.y);
        }
        else if(!wallJumpAvailable && rigidBody2D.velocity.y<=0f)
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
    private void CheckContact()
    {
        onTheGround = checkGround.getIfOnTheGround();

        if (!onTheGround)
        {
            animator.SetBool("Jumping", true);
        }

        if (onTheGround)
        {
            animator.SetBool("Jumping", false);
            secondJumpAvailable = true;
            wallJumpAvailable = true;
        }
    }

    // Adds a vertical force to the player.
    private void Jump()
    {
        rigidBody2D.velocity = new Vector2(rigidBody2D.velocity.x, 0f);
        rigidBody2D.AddForce(new Vector2(0f, regularJumpForce), ForceMode2D.Impulse);
        checkGround.setIfOnTheGround(false);
        animator.SetBool("Jumping", true);
    }

    // Adds an inclined force to the player.
    private void WallJump(float sign)
    {
        rigidBody2D.velocity = new Vector2(0f, 0f);
        rigidBody2D.AddForce(new Vector2(sign*wallJumpForce/1.5f, wallJumpForce/1.25f), ForceMode2D.Impulse);
        checkGround.setIfOnTheGround(false);
        animator.SetBool("Jumping", true);
        FlipSprite(sign);
    }


}
