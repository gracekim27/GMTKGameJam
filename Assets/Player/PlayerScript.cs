using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    Rigidbody2D body;
    Animator anim;
    SpriteRenderer sprRender;
    BoxCollider2D boxCollider;

    private float horizontal;
    private float vertical;
    private float moveLimiter = 0.7f;

    private float runSpeed;
    private float attackTimer;
    [HideInInspector] public int currentHP;

    [HideInInspector] public string currentAnimal;
    private HealthbarScript healthBar;


    [HideInInspector] public GameObject transformInto; //The object the player has just killed and is about to transform into
    [SerializeField] public float maxXP;
    [HideInInspector] public float currentXP;

    [Header("Squirrel")]
    [SerializeField] private int squirrelHP;
    [SerializeField] private float squirrelAttackCooldown;
    [SerializeField] private float squirrelRunSpeed;
    [SerializeField] private RuntimeAnimatorController squirrelAnimController;
    [SerializeField] private GameObject acorn;

    [Header("Snake")]
    [SerializeField] private int snakeHP;
    [SerializeField] private float snakeAttackCooldown;
    [SerializeField] private float snakeRunSpeed;
    [SerializeField] private GameObject snakeAttackCircle;
    [SerializeField] private Vector2 snakeAttackOffset;
    [SerializeField] private float snakeAttackRadius;
    [SerializeField] private RuntimeAnimatorController snakeAnimController;

    [Header("Hippo")]
    [SerializeField] private int hippoHP;
    [SerializeField] private float hippoAttackCooldown;
    [SerializeField] private float hippoRunSpeed;
    [SerializeField] private GameObject hippoAttackCircle;
    [SerializeField] private float hippoAttackRadius;
    [SerializeField] private RuntimeAnimatorController hippoAnimController;

    [Header("Bull")]
    [SerializeField] private int bullHP;
    [SerializeField] private float bullAttackCooldown;
    [SerializeField] private float bullChargeSpeed;
    [SerializeField] private float bullChargeTurn;
    private Vector3 chargeTarget;
    [SerializeField] private RuntimeAnimatorController bullAnimController;

    [Header("Chicken")]
    [SerializeField] private int chickenHP;
    [SerializeField] private float chickenAttackCooldown;
    [SerializeField] private float chickenRunSpeed;
    private bool chickenIsLayingEgg;
    [SerializeField] private RuntimeAnimatorController chickenAnimController;
    [SerializeField] private GameObject egg;

    
    /**
    currentAnimal can be one of the following:
        Squirrel
        Snake
        Hippo
    */

    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sprRender = GetComponent<SpriteRenderer>();
        boxCollider = GetComponent<BoxCollider2D>();
        attackTimer = 0;
        currentXP = 0;
        healthBar = GetComponentInChildren<HealthbarScript>();

        transformInto = null;

        //Start as a Squirrel
        currentAnimal = "Squirrel"; 
        becomeSquirrel();
    }

    // Update is called once per frame
    void Update()
    {
        //Space to switch animals (placeholder)
        if (Input.GetKeyDown(KeyCode.Space)) {
            if (currentAnimal == "Squirrel") {
                currentAnimal = "Hippo";
                becomeHippo();
            }
        }

        //Check what type of animal you are first thing each frame
        if (currentAnimal == "Squirrel") {
            beSquirrel();
        }
        else if (currentAnimal == "Snake") {
            beSnake();
        }
        else if (currentAnimal == "Hippo") {
            beHippo();
        }
        else if (currentAnimal == "Bull") {
            beBull();
        }
        else if (currentAnimal == "Chicken") {
            beChicken();
        }

        //Movement script
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");

        if (currentAnimal == "Bull" && anim.GetBool("isCharging")) {
            horizontal = Mathf.Sign(chargeTarget.x - transform.position.x); //Flip bull around depending on if charging right or left
        }
        if (horizontal == -1) { 
            sprRender.flipX = true; 
        }
        else if (horizontal == 1) {
            sprRender.flipX = false;
        }

        //Increment attack timer every frame (counts how long it's been since last attack)
        attackTimer += Time.deltaTime;

        //Transform into animal after killing it
        if (transformInto is not null) {
            transform.position = transformInto.transform.position;
            if (transformInto.CompareTag("Squirrel")) {
                currentAnimal = "Squirrel";
                becomeSquirrel();
            }
            else if (transformInto.CompareTag("Snake")) {
                currentAnimal = "Snake";
                becomeSnake();
            }
            else if (transformInto.CompareTag("Hippo")) {
                currentAnimal = "Hippo";
                becomeHippo();
            }
            else if (transformInto.CompareTag("Bull")) {
                currentAnimal = "Bull";
                becomeBull();
            }
            else if (transformInto.CompareTag("Chicken")) {
                currentAnimal = "Chicken";
                becomeChicken();
            }
            Destroy(transformInto);
            transformInto = null;
        }

        //Update health bar
        healthBar.currentHP = currentHP;
    }

    //More movement handling
    private void FixedUpdate() 
    {
        if (currentAnimal != "Bull" && !(currentAnimal == "Chicken" && chickenIsLayingEgg))
        {
            if (horizontal != 0 && vertical != 0) {// Check for diagonal movement
                // limit movement speed diagonally, so you move at 70% speed
                horizontal *= moveLimiter;
                vertical *= moveLimiter;
            }
            body.velocity = new Vector2(horizontal * runSpeed, vertical * runSpeed);
        }
    }

    #region Collisions and Damage
    void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.CompareTag("Acorn") && other.GetComponent<AcornScript>().shotBy == "Enemy") {
            Destroy(other.gameObject);
            currentHP--;
        }
        else if (other.gameObject.CompareTag("SnakeAttack") && other.GetComponent<SnakeAttackCircleScript>().shotBy == "Enemy") {
            currentHP--;
        }
        else if (other.gameObject.CompareTag("HippoAttack") && other.GetComponent<HippoAttackCircleScript>().shotBy == "Enemy") {
            currentHP -= 2; //Hippo deals 2x damage
        }
    }

    void OnCollisionEnter2D(Collision2D other) {
        if (other.gameObject.CompareTag("Bull")) {
            currentHP--;
        }
    }
    #endregion

    #region Squirrel Code
    void becomeSquirrel() //This runs only on the first frame of being a squirrel
    {
        //Update stats
        runSpeed = squirrelRunSpeed;
        currentHP = squirrelHP;

        //Update animation
        anim.runtimeAnimatorController = squirrelAnimController as RuntimeAnimatorController;
        
        //Update health bar
        healthBar.healthBarSize = 1f;
        healthBar.maxHP = squirrelHP;
        healthBar.yPos = 0.15f;

        //Update hitbox size
        boxCollider.size = new Vector2(0.3f, 0.25f);
    }
    void beSquirrel() //This runs every frame the player is a squirrel
    {
        //Attack
        if (Input.GetMouseButtonDown(0) && attackTimer > squirrelAttackCooldown) {
            Vector3 shootDirection = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position; //Determine direction to shoot
            shootDirection.z = 0;
            Quaternion randomRotation = Quaternion.Euler(0, 0, Random.Range(0.0f, 360.0f));
            GameObject acornShot = Instantiate(acorn, transform.position, randomRotation); //Spawn bullet with random rotation
            acornShot.GetComponent<AcornScript>().dir = shootDirection;
            acornShot.GetComponent<AcornScript>().shotBy = "Player";

            //Reset attackTimer after attacking
            attackTimer = 0;
        }
    }
    #endregion

    #region Snake Code
    void becomeSnake() //This runs only on the first frame of being a snake
    {
        //Update stats
        runSpeed = snakeRunSpeed;
        currentHP = snakeHP;

        //Update animation
        anim.runtimeAnimatorController = snakeAnimController as RuntimeAnimatorController;
        
        //Update health bar
        healthBar.healthBarSize = 2f;
        healthBar.maxHP = snakeHP;
        healthBar.yPos = 0.1f;

        //Update hitbox size
        boxCollider.size = new Vector2(0.6f, 0.2f);
    }
    void beSnake() //This runs every frame the player is a snake
    {
        if (Input.GetMouseButtonDown(0) && attackTimer > snakeAttackCooldown) {
            anim.SetTrigger("Attack");
            attackTimer = 0;
        }
    }
    void snakeAttack() {
        Vector3 attackCirclePos;

        if (!sprRender.flipX) { //Attack right
            attackCirclePos = new Vector3(transform.position.x+snakeAttackOffset.x, transform.position.y+snakeAttackOffset.y, transform.position.z);
            snakeAttackCircle.transform.position = attackCirclePos;
        }
        else { //Attack left
            attackCirclePos = new Vector3(transform.position.x-snakeAttackOffset.x, transform.position.y+snakeAttackOffset.y, transform.position.z);
            snakeAttackCircle.transform.position = attackCirclePos;
        }

        GameObject snakeAttack = Instantiate(snakeAttackCircle, attackCirclePos, Quaternion.identity); //Create a circle collider around the attack

        snakeAttack.GetComponent<SnakeAttackCircleScript>().shotBy = "Player";
    }
    #endregion
    
    #region Hippo Code
    void becomeHippo() {
        //Update stats
        runSpeed = hippoRunSpeed;
        currentHP = hippoHP;

        //Update animation
        anim.runtimeAnimatorController = hippoAnimController as RuntimeAnimatorController;
        
        //Update health bar
        healthBar.healthBarSize = 3f;
        healthBar.maxHP = hippoHP;
        healthBar.yPos = 0.2f;

        //Update hitbox size
        boxCollider.size = new Vector2(0.4f, 0.3f);
    }
    void beHippo() {
        if (Input.GetMouseButtonDown(0) && attackTimer > hippoAttackCooldown) {
            anim.SetTrigger("Attack");
            attackTimer = 0;
        }
    }

    void hippoAttack() {
        GameObject hippoAttack = Instantiate(hippoAttackCircle, transform.position, Quaternion.identity); //Create a circle collider around the attack
        CircleCollider2D hippoAttackCollider = hippoAttack.GetComponent<CircleCollider2D>(); //Set the position and radius of the attack collider to preset values

        hippoAttackCollider.radius = hippoAttackRadius;
    }
    #endregion
    
    #region Bull Code
    void becomeBull() {
        //Update stats
        runSpeed = 0;
        currentHP = bullHP;

        //Update animation
        anim.runtimeAnimatorController = bullAnimController as RuntimeAnimatorController;

        //Bull transformation can attack immediately
        attackTimer = bullAttackCooldown;
        
        //Update health bar
        healthBar.healthBarSize = 2.5f;
        healthBar.maxHP = bullHP;
        healthBar.yPos = 0.2f;

        //Get rid of momentum
        body.velocity = new Vector2(0,0);

        //Update hitbox size
        boxCollider.size = new Vector2(0.4f, 0.3f);
    }

    void beBull() {
        //Charge up attack on click
        if (Input.GetMouseButtonDown(0) && attackTimer > bullAttackCooldown) {
            attackTimer = 0;
            chargeTarget = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            chargeTarget.z = 0;
            anim.SetTrigger("Charge");
        }

        //If attack charged, charge towards the target
        if (anim.GetBool("isCharging")) {
            //You can slightly control direction during the charge by moving your mouse
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            chargeTarget = Vector3.MoveTowards(chargeTarget, mousePos, bullChargeTurn * Time.deltaTime);
            chargeTarget.z = 0;

            transform.position = Vector3.MoveTowards(transform.position, chargeTarget, bullChargeSpeed * Time.deltaTime);

            //If arrived at the target position, stop charging
            if (Vector3.Distance(transform.position, chargeTarget) < 0.1f) {
                anim.SetBool("isCharging", false);
            }

            //If charging too long, stop charging
            if (attackTimer > bullAttackCooldown) {
                anim.SetBool("isCharging", false);
            }
        }
    }

    void bullAttack() {
        anim.SetTrigger("Attack");
        anim.SetBool("isCharging", true);
    }
    #endregion

    #region Chicken Code
    void becomeChicken() //This runs only on the first frame of being a snake
    {
        chickenIsLayingEgg = false;

        //Update stats
        runSpeed = chickenRunSpeed;
        currentHP = chickenHP;

        //Update animation
        anim.runtimeAnimatorController = chickenAnimController as RuntimeAnimatorController;
        
        //Update health bar
        healthBar.healthBarSize = 1f;
        healthBar.maxHP = chickenHP;
        healthBar.yPos = 0.1f;

        //Update hitbox size
        boxCollider.size = new Vector2(0.2f, 0.2f);
    }
    void beChicken() {
        //Lay eggs
        if (attackTimer > chickenAttackCooldown && Input.GetMouseButtonDown(0)) {
            chickenIsLayingEgg = true;
            body.velocity = new Vector2(0,0); //Get rid of momentum
            anim.SetTrigger("Attack");
            attackTimer = 0;
        }
    }
    void chickenAttack() {
        Vector3 shootDirection = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position; //Determine direction to shoot
        shootDirection.z = 0;
        Quaternion randomRotation = Quaternion.Euler(0, 0, Random.Range(0.0f, 360.0f));
        GameObject eggShot = Instantiate(egg, transform.position, randomRotation); //Spawn bullet with random rotation
        eggShot.GetComponent<EggScript>().dir = shootDirection.normalized;
        eggShot.GetComponent<EggScript>().shotBy = "Player";

        chickenIsLayingEgg = false;
    }
    #endregion
}
