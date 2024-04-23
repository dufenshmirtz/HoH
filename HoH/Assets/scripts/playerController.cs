using TMPro;
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    public float moveSpeed = 5f; // Initialize moveSpeed
    public float jumpForce = 10f;
    public int maxHealth = 100;
    public int currHealth;
    int isonpad = 0;
    public Animator animator;
    private Rigidbody2D rb;
    private bool isGrounded;
    private bool isCrouching;
    public Transform attackPoint;
    public float attackRange = 0.5f;
    public LayerMask player2Layer;
    private bool isBlocking=false;
    public helthbarscript healthbar;
    public int heavyDMG = 20;
    public int lightDMG = 5;
    AudioManager audiomngr;
    public TextMeshProUGUI P1Name,P2Name;
    public GameObject playAgainButton;
    public GameObject mainMenuButton;



    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        currHealth = maxHealth;
        healthbar.SetMaxHealth(maxHealth);
        P1Name.text = PlayerPrefs.GetString("Player1Choice");

        P2Name.gameObject.SetActive(false);
        playAgainButton.SetActive(false);
        mainMenuButton.SetActive(false);
    }

    void Update()
    {
        

        // Running animations...
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
        {
            if (isGrounded)
            {
                animator.SetBool("IsRunning", true);
            }

            float moveDirection = Input.GetKey(KeyCode.A) ? -1f : 1f; // -1 for A, 1 for D
            rb.velocity = new Vector2(moveDirection * moveSpeed, rb.velocity.y);
            transform.localScale = new Vector3(Mathf.Sign(moveDirection), 1, 1); // Flip sprite according to movement direction
            
        }
        else
        {
            animator.SetBool("IsRunning", false);
            rb.velocity = new Vector2(0, rb.velocity.y);
            
        }

        // Jumping
        if (Input.GetKeyDown(KeyCode.W) && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            animator.SetTrigger("Jump");
            audiomngr.PlaySFX(audiomngr.jump, audiomngr.jumpVolume);
        }

        // Heavy Punching
        if (Input.GetKeyDown(KeyCode.Space))
        {
            animator.SetTrigger("Punch");
            audiomngr.PlaySFX(audiomngr.heavyswoosh, audiomngr.heavySwooshVolume);

        }

        // Punching
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            animator.SetTrigger("Punch2");

        }
        //Blocking
        if (Input.GetKeyDown(KeyCode.C))
        {
            animator.SetTrigger("critsi");
            animator.SetBool("Crouch", true);
            isBlocking = true;
        }
        else if (Input.GetKeyUp(KeyCode.C))
        {
            
            animator.SetBool("Crouch", false);
            isBlocking = false;
        }
        //SHOOTIN
        if (Input.GetKeyDown(KeyCode.Q))
        {
            animator.SetTrigger("Shoot");

        }

        //downkey
        if (Input.GetKeyDown(KeyCode.S))
        {
            Collider2D[] colliders = GetComponents<Collider2D>();

            colliders[3].enabled = false;

        }



        // Animation control for jumping, falling, and landing
        animator.SetBool("IsGrounded", isGrounded);
        animator.SetFloat("VerticalSpeed", rb.velocity.y);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Ground"))
        {
            isGrounded = true;
            animator.SetBool("Jump", false);
        }

        if (other.CompareTag("Platform"))
        {
            isGrounded = true;
            animator.SetBool("Jump", false);
            isonpad++;
            Collider2D[] colliders = GetComponents<Collider2D>();

            colliders[3].enabled = true;
        }

    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Ground"))
        {
            isGrounded = false;
        }

        if (other.CompareTag("Platform"))
        {
            
            isonpad--;

            if (isonpad == 0)  
            {
                isGrounded = false;
                Collider2D[] colliders = GetComponents<Collider2D>();

                colliders[3].enabled = false;
            }
            
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }

    public void DealDmg()
    {
        Collider2D hitEnemy = Physics2D.OverlapCircle(attackPoint.position, attackRange, player2Layer);

        if (hitEnemy != null)
        {
            Debug.Log("-40 biatch");
            hitEnemy.GetComponent<Player2Controller>().TakeDamage(heavyDMG);
            audiomngr.PlaySFX(audiomngr.heavyattack, audiomngr.heavyAttackVolume);
        }
        else
        {
            audiomngr.PlaySFX(audiomngr.swoosh, audiomngr.swooshVolume);
        }
        
    }

    public void DealLightDmg()
    {
        Collider2D hitEnemy = Physics2D.OverlapCircle(attackPoint.position, attackRange, player2Layer);

        if (hitEnemy != null)
        {
            Debug.Log("-5 biatch");
            hitEnemy.GetComponent<Player2Controller>().TakeDamage(lightDMG);
            audiomngr.PlaySFX(audiomngr.lightattack, audiomngr.lightAttackVolume);
        }
        else
        {
            audiomngr.PlaySFX(audiomngr.swoosh,audiomngr.swooshVolume);
        }

    }

    public void TakeDamage(int dmg)
    {

        if (isBlocking) 
        {
            if(dmg==heavyDMG) //if its heavy attack take half the damage
            {
                currHealth -= dmg / 2;
                Debug.Log("P1 took less dmg!");
                healthbar.SetHealth(currHealth);
            }
            //if its light attack take no dmg
            
        }
        else
        {
            currHealth -= dmg;


            animator.SetTrigger("tookDmg");

            Debug.Log("P1 took all dmg!");

            healthbar.SetHealth(currHealth);
        }
        

        if (currHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        animator.SetBool("isDead", true);
        Collider2D[] colliders = GetComponents<Collider2D>();
        foreach (Collider2D collider in colliders)
        {
            collider.enabled = false;
        }

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Static;
        }

        audiomngr.StopMusic();
        audiomngr.PlaySFX(audiomngr.death, audiomngr.deathVolume);

        P2Name.text = PlayerPrefs.GetString("Player2Choice")+" prevails!";

        P2Name.gameObject.SetActive(true);

        // Enable play again and main menu buttons
        playAgainButton.SetActive(true);
        mainMenuButton.SetActive(true);

        Debug.Log("nigga 1 ded");
    }

    void PermaDeath()
    {
        animator.SetBool("permanentDeath", true);
        this.enabled = false;
    }

    void HeavyPunchStart()
    {
        animator.SetBool("isHeavypunching", true);
    }

    void HeavyPunchEnd()
    {
        animator.SetBool("isHeavypunching", false);
    }

    private void Awake()
    {
        audiomngr= GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }

}
