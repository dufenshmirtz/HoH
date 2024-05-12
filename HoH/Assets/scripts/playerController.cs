using System.Collections;
using System.Xml.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerAnimationController : MonoBehaviour
{
    public float moveSpeed = 4f; // Initialize moveSpeed
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
    bool FullGerosActive = false;
    float OGMoveSpeed;
    int grounds = 0;
    private bool onCooldown = false;
    public Slider cooldownSlider;
    float cdTimer=0f;
    public Image cdbarimage;
    public Sprite activeSprite,ogSprite;
    private float dashingPower = 40f;
    private float dashingTime = 0.1f;
    bool dashin = false;
    public Player2Controller enemy;
    bool dashhit = false;
    bool counterOn = false;
    bool countered = false;




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

        OGMoveSpeed = moveSpeed;

        cooldownSlider.maxValue = 1f;
    }

    void Update()
    {
        

        // Running animations...
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
        {
            if ((dashin))
            {
                return;
            }
            

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

        //downkey
        if (Input.GetKeyDown(KeyCode.S))
        {
            Collider2D[] colliders = GetComponents<Collider2D>();

            colliders[3].enabled = false;
        }

        //Koubi Nikis
        if (P1Name.text == "Angel" && Input.GetKeyDown(KeyCode.Q) && !onCooldown)
        {
            cdbarimage.sprite = activeSprite;

            //Activate abilty function
            StartCoroutine(AngelStateCoroutine(8f));
            

            UpdateCooldownSlider(60);
        }

        //Full Geros
        if (P1Name.text == "Larry" && Input.GetKeyDown(KeyCode.Q) && !onCooldown)
        {
            
            FullGerosActive = true;
            moveSpeed += 2f;

            animator.SetTrigger("FullGeros");

            cdbarimage.sprite = activeSprite;

            Invulnerable();

            StartCoroutine(FullGerosDeactivateAfterDelay(10));

            UpdateCooldownSlider(60);
        }
        //Dufen-Dash
        if (P1Name.text == "Fotis" && Input.GetKeyDown(KeyCode.Q) && !onCooldown)
        {
            cdbarimage.sprite = activeSprite;
            StartCoroutine(DashAnimation());
            UpdateCooldownSlider(2); // Start cooldown for T key
        }

        //counter
        if (P1Name.text == "Kolovos" && Input.GetKeyDown(KeyCode.Q) && !onCooldown)
        {

            audiomngr.PlaySFX(audiomngr.counterScream, audiomngr.counterVol);
            animator.SetTrigger("counter");
            counterOn = true;


            // Start the cooldown timer
            cdTimer = 20f;
            onCooldown = true;

            StartCoroutine(AbilityCooldown(20f));

        }

        if (countered)
        {
            Debug.Log("mouni");
            audiomngr.PlaySFX(audiomngr.counterClong, audiomngr.counterClongVol);
            DealCounterDmg();
            countered = false;
            UpdateCooldownSlider(2);
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
            grounds++;
            Debug.Log("grounds++ p1");
        }

        if (other.CompareTag("Platform"))
        {
            isGrounded = true;
            animator.SetBool("Jump", false);
            isonpad++;
            Collider2D[] colliders = GetComponents<Collider2D>();

            colliders[3].enabled = true;
        }

        if(dashin && other.CompareTag("Player") && !dashhit)
        {
            DealDashDmg();
            dashhit = true;
            return;
        }

        if (other.CompareTag("Player"))
        {
            isGrounded = true;
            animator.SetBool("Jump", false);
            grounds++;
            Debug.Log("grounds++ p1");
        }

    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Ground"))
        {
            grounds--;
            Debug.Log("grounds-- p1");
            if (grounds == 0)
            {
                isGrounded = false;
                Debug.Log("grounds=0 p1");
            }
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

        if (dashin)
        {
            
            return;
        }

        if (other.CompareTag("Player"))
        {
            grounds--;
            Debug.Log("grounds--- p1");
            if (grounds == 0)
            {
                isGrounded = false;
                Debug.Log("grounds=0 p1");
            }
        }

    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }

    public void DealDashDmg()
    {

        
        enemy.TakeDamage(15);
        audiomngr.PlaySFX(audiomngr.dashHit, audiomngr.heavyAttackVolume);
               
        
    }

    public void DealCounterDmg()
    {


        enemy.TakeDamage(20);
        


    }

    public void DealDmg()
    {
        Collider2D hitEnemy = Physics2D.OverlapCircle(attackPoint.position, attackRange, player2Layer);

        if (hitEnemy != null)
        {
            
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
        if(FullGerosActive)
        {
            return;
        }

        if (counterOn)
        {
            countered = true;
            return;
        }

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


    IEnumerator AngelStateCoroutine(float duration)
    {
        // Disable colliders and set the Rigidbody to Static
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

        // Set the player as dead
        animator.SetBool("isDead", true);


        // Loop for the specified duration
        //float elapsedTime = 0f;
        for (int i = 0; i < duration; i++)
        {

            UnityEngine.Debug.Log("here");
            // Regenerate health
            currHealth += 5;
            healthbar.SetHealth(currHealth);


            yield return new WaitForSeconds(1f);
        }



        animator.SetBool("isDead", false);

        animator.SetBool("permanentDeath", false);

        this.enabled = true;

        animator.SetTrigger("Angel");

        // Enable colliders and restore the Rigidbody
        //Rejuvenation();

    }

    void Invulnerable()
    {
        Collider2D[] colliders = GetComponents<Collider2D>();

        foreach (Collider2D collider in colliders)
        {
            collider.enabled = false;
        }

        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Static;
        }
    }

    private void Rejuvenation()
    {

        Collider2D[] colliders = GetComponents<Collider2D>();

        foreach (Collider2D collider in colliders)
        {
            if (collider != colliders[3])
            {
                collider.enabled = true;
            }
        }

        colliders[4].enabled= false; 
        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
        }

        if(P1Name.text== "Larry")
        {
            animator.SetTrigger("animationOver");
        }

        if (P1Name.text == "Angel")
        {
            
            // Start the cooldown timer
            onCooldown = true;
            cdTimer = 45f;
            
            StartCoroutine(AbilityCooldown(45f));
        }
    }

    IEnumerator FullGerosDeactivateAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        FullGerosActive = false;
        moveSpeed = OGMoveSpeed;

        
        // Start the cooldown timer
        cdTimer = 45f;
        onCooldown = true;

        
        StartCoroutine(AbilityCooldown(45f));
    }
    IEnumerator AbilityCooldown(float duration)
    {
        cdbarimage.sprite = ogSprite;  //change the bar appearance to normal
        while (cdTimer > 0)
        {
            yield return new WaitForSeconds(1f);
            cdTimer -= 1f;
            UpdateCooldownSlider(duration); // Update the cooldown slider every second
        }
        Debug.Log("opa");
        // Reset the cooldown flag
        
        onCooldown = false;
        cdTimer = 0f;
    }

    void UpdateCooldownSlider(float duration)
    {
        float progress = Mathf.Clamp01(1f-cdTimer /duration);
        cooldownSlider.value = progress;
    }

    IEnumerator DashAnimation()
    {
        dashin = true;
        // Store the original gravity scale
        float ogGravityScale = rb.gravityScale;

        // Disable gravity while dashing
        rb.gravityScale = 0f;

        // Store the current velocity
        Vector2 currentVelocity = rb.velocity;

        // Determine the dash direction based on the input
        float moveDirection = Input.GetKey(KeyCode.A) ? -1f : 1f; ;

        Collider2D[] colliders = GetComponents<Collider2D>();
        foreach (Collider2D collider in colliders)
        {
            collider.enabled = false;
        }
        colliders[3].enabled = true;
        colliders[4].enabled = true;


        audiomngr.PlaySFX(audiomngr.dash, 1);
        // Calculate the dash velocity
        Vector2 dashVelocity = new Vector2(moveDirection * dashingPower, currentVelocity.y);

        // Apply the dash velocity
        rb.velocity = dashVelocity;

        // Trigger the dash animation
        animator.SetTrigger("Dash");

        

        // Wait for the dash duration
        yield return new WaitForSeconds(dashingTime);

        // Reset the velocity after the dash
        rb.velocity = currentVelocity;

        // Reset the gravity scale
        rb.gravityScale = ogGravityScale;

        dashin = false;

        foreach (Collider2D collider in colliders)
        {
            if (collider != colliders[3])
            {
              collider.enabled = true;
            }
        }
        colliders[3].enabled = false;
        colliders[4].enabled = false;

        dashhit=false;

        // Start the cooldown timer
        cdTimer = 5f;
        onCooldown = true;

        StartCoroutine(AbilityCooldown(5f));

        Debug.Log(grounds);

    }

    public void CounterOff()
    {
        counterOn = false;
    }
}
