using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // GAMEOBJECT COMPONENTS
    private Rigidbody2D rigidBody2D;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    [SerializeField] private Collider2D hitBox;
    [SerializeField] private Collider2D hurtBox;
    [SerializeField] private Collider2D pushBox;
    [SerializeField] private Collider2D checkTreeBox;
    [SerializeField] private CheckHit checkHit;
    [SerializeField] private CheckGround checkGround;
    [SerializeField] private CheckWall checkWall;
    [SerializeField] private CheckCollectables checkCollectables;
    [SerializeField] private CheckTree checkTree;
    [SerializeField] private Material matDefault;
    [SerializeField] private Material matWhite;
    [SerializeField] private ShurikenBehaviour shurikenPrefab;
    
    // VARIABLES
    public float maxHP;
    public float currentHP;
    public int shurikenCant;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpForce;
    [SerializeField] private float wallJumpForce;
    [SerializeField] private float shurikenSpeed;
    [SerializeField] private float treeJumpForce;
    [SerializeField] private float hurtForce;
    private int platformInstanceID;
    private bool secondJumpAvailable;
    private bool wallJumpAvailable;
    private bool climbTreeAvailable;
    private bool onTheGround;
    private bool againstWall;
    private bool overATree;
    private bool climbingTree;
    private bool attacking;
    private bool isHurt;

    // INPUT VARIABLES
    private float horizontalAxis;
    private float verticalAxis;
    private bool jumpButton;
    private bool hangButton;
    private bool rangedAttackButton;
    private bool meleeAttackButton;

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
        climbTreeAvailable = false;
        againstWall = false;
        overATree = false;
        climbingTree = false;
        attacking = false;
        isHurt = false;
    }

    // FixedUpdate is called multiple times per frame.
    void FixedUpdate()
    {
        CheckHit();
        CheckGround();
        CheckCollectables();

        if(!isHurt && (!attacking) && !climbingTree)
            CheckMovement();
    }

    // Update is called once per frame.
    void Update()
    {
        // Gets Axis Input
        horizontalAxis = Input.GetAxisRaw("Horizontal");
        verticalAxis = Input.GetAxisRaw("Vertical");
        jumpButton = Input.GetButtonDown("Jump");
        hangButton = Input.GetButtonDown("Jump");
        rangedAttackButton = Input.GetButtonDown("AttackRange");
        meleeAttackButton = Input.GetButtonDown("AttackMelee");
        hangButton = Input.GetButtonDown("Hang");

        //Checks contact with enviroment elements 
        againstWall = checkWall.againstWallLeft || checkWall.againstWallRight;
        overATree = checkTree.overATree;
        //Checking the Ground is a separate check on "CheckGround()"

        if(!isHurt)
            CheckActions();
    }

    private void CheckActions()
    {
        if(!attacking && (rangedAttackButton || meleeAttackButton))
        {
            if(climbingTree)
            {
                rigidBody2D.gravityScale = 3f;
                climbingTree = false;
                animator.SetBool("Hanging", false);
            }

            if(onTheGround)
                this.MovementOnAxis(0f);
            
            if(rangedAttackButton && shurikenCant>0)
            {
                shurikenCant--;
                attacking = true;
                animator.SetTrigger("RangedAttacking");
            }
            else if(meleeAttackButton && onTheGround)
            {
                attacking = true;
                animator.SetTrigger("MeleeAttacking");
            }
        }

        if(hangButton && overATree && !onTheGround && climbTreeAvailable)
        {
            if(climbingTree)
            {
                rigidBody2D.gravityScale = 3f;
                climbingTree = false;
                animator.SetBool("Hanging", false);
            }
            else
            {
                rigidBody2D.velocity = new Vector2(0f, 0f);
                rigidBody2D.gravityScale = 0f;
                climbingTree = true;
                //climbTreeAvailable = false;
                animator.SetBool("Hanging", true);
            }
        }

        //Checking for the JumpButton is here instead of UpdateFixed, because it needs to check immediately
        if(jumpButton)
        {
            rigidBody2D.gravityScale = 3f;

            if(climbingTree)
            {
                climbingTree = false;
                TreeJump();
            }  
            else if (onTheGround)
            {
                if(verticalAxis<0f && checkGround.otherTag=="Platform")
                {
                    platformInstanceID = checkGround.otherInstanceID;
                    disablePushBox();
                }
                else
                    JumpStart();
            }
            else if (!onTheGround && checkWall.againstWallRight)
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
        }
    }

    // Checks Input, calls for Movement, and sets facing side.
    private void CheckMovement()
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
            checkTreeBox.offset = new Vector2(0.05f, 0.1f);
            hitBox.offset = new Vector2(0.27f, 0.05f);
        }
        else if(HorizontalAxis < -0.1f)
        {
            spriteRenderer.flipX = true;
            checkTreeBox.offset = new Vector2(-0.05f, 0.1f);
            hitBox.offset = new Vector2(-0.27f, 0.05f);
        }
    }

    // Checks on other scripts that are constantly checking for collision. 
    private void CheckGround()
    {
        onTheGround = checkGround.onTheGround;

        if(!onTheGround)
        {
            //AND FALLING!
            if (rigidBody2D.velocity.normalized.y < 0f)
            {
                animator.SetBool("Falling", true);
                animator.SetBool("Jumping", false);
                rigidBody2D.gravityScale = 1.5f;
            }
        }
        else
        {
            animator.SetBool("Jumping", false);
            animator.SetBool("Falling", false);
            
            if(checkGround.otherTag=="Platform" && checkGround.otherInstanceID!=platformInstanceID)
                enablePushBox();
            else if(checkGround.otherTag=="Ground")
                enablePushBox();

            secondJumpAvailable = true;
            wallJumpAvailable = true;
            climbTreeAvailable = true;
            rigidBody2D.gravityScale = 3f;
        }
    }

    private void CheckCollectables()
    {
        if(checkCollectables.foundSomething)
        {
            if(checkCollectables.otherTag == "Food")
            {
                if(currentHP<maxHP)
                    currentHP++;
            }
            else if(checkCollectables.otherTag == "Ammo")
            {
                shurikenCant++;
            }

            checkCollectables.foundSomething = false;
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
        onTheGround = false;
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
        onTheGround = false;
        animator.SetBool("Jumping", true);
        animator.SetBool("Falling", false);
        FlipSprite(sign);
    }

    public void TreeJump()
    {
        rigidBody2D.velocity = new Vector2(rigidBody2D.velocity.x, 0f);
        rigidBody2D.AddForce(new Vector2(0f, treeJumpForce), ForceMode2D.Impulse);
        checkGround.onTheGround = false;
        onTheGround = false;
        animator.SetBool("Jumping", true);
        animator.SetBool("Falling", false);
        animator.SetBool("Hanging", false);
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
        disableHurtBox();
        currentHP--;
        isHurt = true;

        if(currentHP>0)
        {
            isHurt = true;
            animator.SetTrigger("Damaged");
            FlashWhite();
            Invoke("FlashBack", 0.1f);
            Invoke("FlashWhite", 0.2f);
            Invoke("FlashBack", 0.3f);
            Invoke("FlashWhite", 0.4f);
            Invoke("FlashBack", 0.5f);
            Invoke("FlashWhite", 0.6f);
            Invoke("FlashBack", 0.7f);
            Invoke("FlashWhite", 0.8f);
            Invoke("FlashBack", 0.9f);
            Invoke("enableHurtBox", 1f);
        }
        else
        {
            animator.SetTrigger("Damaged");
            animator.SetBool("Dead", true);
        }

        rigidBody2D.velocity = new Vector2(0f, 0f);
        if(spriteRenderer.flipX == true) //Mirando a la Izquierda
        {
            rigidBody2D.AddForce(new Vector2(hurtForce, hurtForce), ForceMode2D.Impulse);
        }
        else //Mirando a la Derecha
        {
            rigidBody2D.AddForce(new Vector2(-hurtForce, hurtForce), ForceMode2D.Impulse);
        }
        checkGround.onTheGround = false;
        onTheGround = false;
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
        rigidBody2D.velocity = new Vector2(0f, 0f);
        isHurt = false;
    }

    public void StopDead()
    {
        rigidBody2D.velocity = new Vector2(0f, 0f);
    }
    
    private void disableHurtBox()
    {
        hurtBox.enabled = false;
    }

    private void enableHurtBox()
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
        rangedAttackButton = false;
        meleeAttackButton = false;
        attacking = false;
    }

    // Called from the Animation.
    // Instantiates a PlayerShuriken and shoots it on the direction the player is facing.
    public void ShootShuriken()
    {
        if(spriteRenderer.flipX == true) //DISPARA A LA IZQUIERDA
        {
            Quaternion shurikenRotation = new Quaternion(0f, 0f, 0f, 0f);
            shurikenRotation.eulerAngles = new Vector3(0f, 0f, 90f);
            Vector2 plusVector = new Vector2(-0.3f, 0.14f);

            ShurikenBehaviour shuriken = Instantiate(shurikenPrefab, rigidBody2D.position+plusVector, shurikenRotation);

            shuriken.velocity = new Vector2(-1f, 0f);
            shuriken.shurikenSpeed = shurikenSpeed;
        }
        else //DISPARA A LA DERECHA
        {
            Quaternion shurikenRotation = new Quaternion(0f, 0f, 0f, 0f);
            shurikenRotation.eulerAngles = new Vector3(0f, 0f, 270f);
            Vector2 plusVector = new Vector2(0.3f, 0.14f);

            ShurikenBehaviour shuriken = Instantiate(shurikenPrefab, rigidBody2D.position+plusVector, shurikenRotation);

            shuriken.velocity = new Vector2(1f, 0f);
            shuriken.shurikenSpeed = shurikenSpeed;
        }
        
    }
}
