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

    private string currentAnimal;
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

        //Movement script
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");
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
            Destroy(transformInto);
            transformInto = null;
        }

        //Update health bar
        healthBar.currentHP = currentHP;
    }

    //More movement handling
    private void FixedUpdate() 
    {
        if (horizontal != 0 && vertical != 0) {// Check for diagonal movement
            // limit movement speed diagonally, so you move at 70% speed
            horizontal *= moveLimiter;
            vertical *= moveLimiter;
        }
        body.velocity = new Vector2(horizontal * runSpeed, vertical * runSpeed);
    }

    //Collisions with enemies/bullets
    void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.CompareTag("Acorn") && other.GetComponent<AcornScript>().shotBy == "Enemy") {
            Destroy(other.gameObject);
            currentHP--;
        }
        else if (other.gameObject.CompareTag("SnakeAttack") && other.GetComponent<SnakeAttackCircleScript>().shotBy == "Enemy") {
            currentHP--;
        }
        else if (other.gameObject.CompareTag("HippoAttack") && other.GetComponent<HippoAttackCircleScript>().shotBy == "Enemy") {
            currentHP -= 2; //Hippo
        }
    }

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
        GameObject snakeAttack = Instantiate(snakeAttackCircle, transform.position, Quaternion.identity); //Create a circle collider around the attack
        CircleCollider2D snakeAttackCollider = snakeAttack.GetComponent<CircleCollider2D>(); //Set the position and radius of the attack collider to preset values

        if (!sprRender.flipX) { //Attack right
            snakeAttackCollider.offset = snakeAttackOffset; 
        }
        else { //Attack left
            snakeAttackOffset.x = -snakeAttackOffset.x;
            snakeAttackCollider.offset = snakeAttackOffset;
            snakeAttackOffset.x = -snakeAttackOffset.x;
        }
        snakeAttackCollider.radius = snakeAttackRadius;
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
    
}
