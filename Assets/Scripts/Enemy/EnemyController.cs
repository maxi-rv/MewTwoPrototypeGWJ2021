using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    // GAMEOBJECT COMPONENTS
    private Rigidbody2D rigidBody2D;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    [SerializeField] private Collider2D bodyHitBox;
    [SerializeField] private Collider2D hitBox;
    [SerializeField] private Collider2D hurtBox;
    [SerializeField] private Collider2D pushBox;
    [SerializeField] private CheckHit checkHit;
    [SerializeField] private CheckGround checkGround;
    [SerializeField] private Material matDefault;
    [SerializeField] private Material matWhite;
    [SerializeField] private ShurikenBehaviour shurikenPrefab;
    [SerializeField] private PoofBehaviour poofPrefab;
    [SerializeField] private LogBehaviour logPrefab;

    // VARIABLES
    [SerializeField] private float maxHP;
    [SerializeField] private float currentHP;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float hurtForce;
    private bool onTheGround;

    // Start is called before the first frame update
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
    }

    void FixedUpdate()
    {
        CheckHit();
        CheckGround();

        
    }

    // Flips the sprite on the X Axis according to the Axis input.
    // Needs to be called by another function, which must check the facing sides first.
    private void FlipSprite(float HorizontalAxis)
    {
        if(HorizontalAxis > 0.1f)
        {
            spriteRenderer.flipX = false;
            bodyHitBox.offset = new Vector2(-0.14f, -0.038f);
            hitBox.offset = new Vector2(0.19f, 0f);
            hurtBox.offset = new Vector2(-0.146f, -0.039f);
            pushBox.offset = new Vector2(-0.145f, -0.026f);
            checkGround.gameObject.GetComponent<Collider2D>().offset = new Vector2(-0.174f, -0.225f);
        }
        else if(HorizontalAxis < -0.1f)
        {
            spriteRenderer.flipX = true;
            bodyHitBox.offset = new Vector2(0.14f, -0.038f);
            hitBox.offset = new Vector2(-0.19f, 0f);
            hurtBox.offset = new Vector2(0.146f, -0.039f);
            pushBox.offset = new Vector2(0.145f, -0.026f);
            checkGround.gameObject.GetComponent<Collider2D>().offset = new Vector2(0.174f, -0.225f);
        }
    }

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
                rigidBody2D.gravityScale = 2f;
            }
        }
        else
        {
            animator.SetBool("Jumping", false);
            animator.SetBool("Falling", false);
            rigidBody2D.gravityScale = 3f;
        }
    }

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
        currentHP--;

        if(currentHP>0)
        {
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
        }
        else
        {
            Quaternion rotation = new Quaternion(0f, 0f, 0f, 0f);
            Vector3 position = gameObject.transform.position;

            if(spriteRenderer.flipX==false)
                position = position + new Vector3(-0.15f,0f,0f);
            else
                position = position + new Vector3(0.15f,0f,0f);

            Instantiate(poofPrefab, position, rotation);
            Instantiate(logPrefab, position, logPrefab.gameObject.transform.rotation);
            GameObject.Destroy(gameObject);
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
    }
}
