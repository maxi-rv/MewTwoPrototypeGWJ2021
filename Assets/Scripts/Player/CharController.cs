using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharController : MonoBehaviour
{
    // GAMEOBJECT COMPONENTS
    private Rigidbody2D rigidBody2D;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private RaycastHit2D searchPlayer;
    [SerializeField] private Collider2D hitBox;
    [SerializeField] private Collider2D hurtBox;
    [SerializeField] private Collider2D pushBox;
    [SerializeField] private CheckHit checkHit;
    [SerializeField] private CheckGround checkGround;
    [SerializeField] private Material matDefault;
    [SerializeField] private Material matWhite;

    // VARIABLES
    [SerializeField] private float maxHP;
    private float currentHP;
    [SerializeField] private float hurtForce;
    private bool onTheGround;
    private bool isHurt;

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
        isHurt = false;
    }

    void FixedUpdate()
    {
        CheckHit();
        CheckGround();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Flips the sprite on the X Axis according to the Axis input.
    // Needs to be called by another function, which must check the facing sides first.
    private void FlipSprite(float HorizontalAxis)
    {
        if(HorizontalAxis > 0.1f)
        {
            //
        }
        else if(HorizontalAxis < -0.1f)
        {
            //
        }
    }

    private void CheckGround()
    {
        onTheGround = checkGround.onTheGround;
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
        isHurt = checkHit.isHurt;
        checkHit.isHurt = false;
        currentHP = currentHP - checkHit.receivedDamage;
        checkHit.receivedDamage = 0f;

        if(currentHP>0)
        {
            //Flashing hurt effect
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
            Invoke("StopHurt", 0.9f);
        }
        else
        {
            //Death
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
}
