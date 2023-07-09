using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquirrelScript : MonoBehaviour
{
    [SerializeField] private int maxHP;
    [SerializeField] private float xpDropped;
    private int currentHP;
    [SerializeField] private float attackCooldown;
    private float attackTimer;
    [SerializeField] private float runSpeed;
    [SerializeField] private float circleDistance; //How far the squirrel enemy will stay away from the player
    private GameObject player;
    private Animator anim;
    private SpriteRenderer sprRender;
    [SerializeField] private GameObject acorn;
    [SerializeField] private GameObject thisObject;
    [SerializeField] private AudioSource squirrelAudioSource;
    [SerializeField] private AudioClip squirrelDeathSound;

    [Header("Healthbar")]
    [SerializeField] private float healthBarSize;
    [SerializeField] private float healthBarYOffset;
    private HealthbarScript healthBar;

    // Start is called before the first frame update
    void Start()
    {        
        anim = gameObject.GetComponent<Animator>();
        sprRender = gameObject.GetComponent<SpriteRenderer>();
        player = GameObject.FindGameObjectWithTag("Player");
        gameObject.GetComponent<EnemyDamageScript>().currentHP = maxHP;

        //Initialize health bar
        healthBar = GetComponentInChildren<HealthbarScript>();
        healthBar.healthBarSize = healthBarSize;
        healthBar.maxHP = maxHP;
        healthBar.yPos = healthBarYOffset;

        Physics2D.IgnoreCollision(gameObject.GetComponent<Collider2D>(), player.GetComponent<Collider2D>());
    }

    // Update is called once per frame
    void Update()
    {
        if (!anim.GetBool("IsDying")) {
            //Face left if moving left, right if moving right
            if (transform.position.x > player.transform.position.x) {
                sprRender.flipX = true;
            }
            else if (transform.position.x < player.transform.position.x) {
                sprRender.flipX = false;
            }

            //Throw acorns
            if (attackTimer > attackCooldown) {
                Vector3 shootDirection = player.transform.position - transform.position; //Determine direction to shoot
                shootDirection.z = 0;
                Quaternion randomRotation = Quaternion.Euler(0, 0, Random.Range(0.0f, 360.0f));
                GameObject acornShot = Instantiate(acorn, transform.position, randomRotation); //Spawn bullet with random rotation
                acornShot.GetComponent<AcornScript>().dir = shootDirection;
                acornShot.GetComponent<AcornScript>().shotBy = "Enemy";

                attackTimer = 0;
            }
            attackTimer += Time.deltaTime;

            //Move towards player if further than circleDistance, back up if closer than circleDistance
            float dist = Vector3.Distance(transform.position, player.transform.position);
            if (dist > circleDistance) {
                transform.position = Vector3.MoveTowards(transform.position, player.transform.position, runSpeed * Time.deltaTime);
            }
            else {
                transform.position = Vector3.MoveTowards(transform.position, player.transform.position, -runSpeed * Time.deltaTime);
            }

            //Update health bar
            currentHP = gameObject.GetComponent<EnemyDamageScript>().currentHP; //Use EnemyDamageScript to find currentHP
            healthBar.currentHP = currentHP;

            //Die if health low
            if (currentHP <= 0) {
                squirrelAudioSource.volume = 0.1f;
                squirrelAudioSource.PlayOneShot(squirrelDeathSound);
                healthBar.currentHP = 0;
                PlayerScript playerScript = player.GetComponent<PlayerScript>();
                if (playerScript.currentXP >= playerScript.maxXP) {
                    playerScript.transformInto = gameObject;
                    playerScript.currentXP = 0;
                }
                else {
                    anim.SetBool("IsDying", true);
                    gameObject.GetComponent<Rigidbody2D>().simulated = false;
                    playerScript.currentXP += xpDropped;
                }
            }
        }
    }

    void Die() {
        Destroy(gameObject);
    }

    // void OnDrawGizmos()
    // {
    //     Gizmos.color = Color.red;
    //     Gizmos.DrawWireSphere(transform.position, circleDistance);
    // }
}
