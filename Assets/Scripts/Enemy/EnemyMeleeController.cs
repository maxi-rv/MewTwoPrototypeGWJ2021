using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Koffie.SimpleTasks; //GRACIAS MARTIN!

public class EnemyMeleeController : MonoBehaviour
{
    // GAMEOBJECT COMPONENTS
    private Rigidbody2D rigidBody2D;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private RaycastHit2D searchPlayer;
    [SerializeField] private Collider2D bodyHitBox;
    [SerializeField] private Collider2D hitBox;
    [SerializeField] private Collider2D hurtBox;
    [SerializeField] private Collider2D pushBox;
    [SerializeField] private DamageInfo damageInfo;
    [SerializeField] private CheckHit checkHit;
    [SerializeField] private CheckGround checkGround;
    [SerializeField] private Material matDefault;
    [SerializeField] private Material matWhite;
    [SerializeField] private PoofBehaviour poofPrefab;
    [SerializeField] private LogBehaviour logPrefab;
    [SerializeField] private GameObject shurikenCollectablePrefab;
    [SerializeField] private GameObject foodCollectablePrefab;
    [SerializeField] private LayerMask playerLayer;
    private STask stopAttackTask;

    // VARIABLES
    [SerializeField] private float maxHP;
    private float currentHP;
    [SerializeField] private float meleeDamage;
    [SerializeField] private float detectionRange;
    [SerializeField] private float attackRange;
    [SerializeField] private float attackForce;
    [SerializeField] private float attackForceVertical;
    [SerializeField] private float AttackCooldown;
    [SerializeField] private float hurtForce;
    private bool onTheGround;
    private bool beginAttack;
    private bool attacking;
    private bool onCooldown;
    [SerializeField] private bool CanAttack;

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
        beginAttack = false;
        attacking = false;
        onCooldown = false;

        damageInfo.damage = meleeDamage;
    }

    void FixedUpdate()
    {
        CheckHit();
        CheckGround();
        CheckForPlayer();
    }

    private void CheckForPlayer()
    {
        //Fixing the x axis of the object, because the sprite is not centered.
        float xAligner = 0f;
        if(spriteRenderer.flipX==false)
                xAligner = -0.15f;
            else
                xAligner = 0.15f;

        //Sets origin point for raycasting
        Vector3 origin = new Vector3(transform.position.x+xAligner, transform.position.y, transform.position.z);

        Collider2D col = Physics2D.OverlapCircle(origin, this.detectionRange, playerLayer);

        if (col != null && !onCooldown && CanAttack)
        {
            bool isMelee = Vector2.Distance(col.transform.position, origin) < attackRange;
            float direction = -Mathf.Sign(origin.x - col.transform.position.x);

            if(isMelee)
            {
                if(!attacking)
                {
                    animator.SetTrigger("Attacking");
                    attacking = true;
                    rigidBody2D.velocity = Vector2.zero;
                }
            }
            else if(onTheGround)
            {
                if(!beginAttack)
                {
                    rigidBody2D.velocity = Vector2.zero;
                    rigidBody2D.AddForce(new Vector2(attackForce*direction, attackForceVertical), ForceMode2D.Impulse);
                    FlipSprite(direction);
                    animator.SetBool("BeginAttack", true);
                    beginAttack = true;
                    attacking = false;
                }
            }
        }
        else
        {
            if(beginAttack && onTheGround)
            {
                animator.SetBool("BeginAttack", false);
                beginAttack = false;
                attacking = false;
                rigidBody2D.velocity = Vector2.zero;
            }
        }
    }

    // Flips the sprite on the X Axis according to the Axis input.
    // Needs to be called by another function, which must check the facing sides first.
    private void FlipSprite(float HorizontalAxis)
    {
        if(HorizontalAxis > 0.1f) //Change Condition!!!
        {
            spriteRenderer.flipX = false;
            bodyHitBox.offset = new Vector2(-0.14f, -0.038f);
            hitBox.offset = new Vector2(0.19f, 0f);
            hurtBox.offset = new Vector2(-0.146f, -0.039f);
            pushBox.offset = new Vector2(-0.145f, -0.026f);
            checkGround.gameObject.GetComponent<Collider2D>().offset = new Vector2(-0.174f, -0.225f);
        }
        else if(HorizontalAxis < -0.1f) //Change Condition!!!
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
        currentHP = currentHP - checkHit.receivedDamage;
        checkHit.receivedDamage = 0f;

        if(currentHP>0)
        {
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

            int rng = UnityEngine.Random.Range(1,100);

            if(rng<=25)
                Instantiate(shurikenCollectablePrefab, position, rotation);
            else if(25<rng && rng<=75)
                Instantiate(foodCollectablePrefab, position, rotation);

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

    public void StopAttack()
    {
        stopAttackTask?.Kill();
        stopAttackTask = STasks.Do(() => onCooldown = false, after: AttackCooldown);

        onCooldown = true;
        beginAttack = false;
        attacking = false;
        rigidBody2D.velocity = new Vector2(0f, 0f);
        animator.SetBool("BeginAttack", false);
    }
}
