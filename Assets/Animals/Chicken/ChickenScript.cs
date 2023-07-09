using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChickenScript : MonoBehaviour
{
    [SerializeField] private int maxHP;
    [SerializeField] private float xpDropped;
    private int currentHP;
    [SerializeField] private float attackCooldown;
    private float attackTimer;
    [SerializeField] private float runSpeed;
    [SerializeField] private float circleDistance;
    private GameObject player;
    private Animator anim;
    private SpriteRenderer sprRender;
    [SerializeField] private GameObject egg;
    private bool isLayingEgg;

    [Header("Healthbar")]
    [SerializeField] private float healthBarSize;
    [SerializeField] private float healthBarYOffset;
    private HealthbarScript healthBar;

    [SerializeField] private AudioSource chickenAudioSource;
    [SerializeField] private AudioClip chickenDeathSound;
    [SerializeField] private AudioClip chickenEggLaySound;

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
        //Face left if moving left, right if moving right
        if (transform.position.x > player.transform.position.x) {
            sprRender.flipX = true;
        }
        else if (transform.position.x < player.transform.position.x) {
            sprRender.flipX = false;
        }

        //Lay eggs
        if (attackTimer > attackCooldown) {
            isLayingEgg = true;
            anim.SetTrigger("Attack");
            attackTimer = 0;
        }
        attackTimer += Time.deltaTime;

        //Move towards player if further than circleDistance, back up if closer than circleDistance
        if (!isLayingEgg) {
            float dist = Vector3.Distance(transform.position, player.transform.position);
            if (dist > circleDistance) {
                transform.position = Vector3.MoveTowards(transform.position, player.transform.position, runSpeed * Time.deltaTime);
            }
            else {
                transform.position = Vector3.MoveTowards(transform.position, player.transform.position, -runSpeed * Time.deltaTime);
            }
        }
        
        //Update health bar
        currentHP = gameObject.GetComponent<EnemyDamageScript>().currentHP; //Use EnemyDamageScript to find currentHP
        healthBar.currentHP = currentHP;

        //Die if health low
        if (currentHP <= 0) {
            chickenAudioSource.volume = 0.2f;
            chickenAudioSource.PlayOneShot(chickenDeathSound);

            healthBar.currentHP = 0;
            PlayerScript playerScript = player.GetComponent<PlayerScript>();
            if (playerScript.currentXP >= playerScript.maxXP) {
                playerScript.transformInto = gameObject;
                playerScript.currentXP = 0;
            }
            else {
                playerScript.currentXP += xpDropped;
                Destroy(gameObject);
            }
        }
    }

    void chickenAttack() {
        chickenAudioSource.clip = chickenEggLaySound;
        chickenAudioSource.volume = 0.2f;
        chickenAudioSource.Play();

        Vector3 shootDirection = player.transform.position - transform.position; //Determine direction to shoot
        shootDirection.z = 0;
        Quaternion randomRotation = Quaternion.Euler(0, 0, Random.Range(0.0f, 360.0f));
        GameObject eggShot = Instantiate(egg, transform.position, randomRotation); //Spawn bullet with random rotation
        eggShot.GetComponent<EggScript>().dir = shootDirection.normalized;
        eggShot.GetComponent<EggScript>().shotBy = "Enemy";
        isLayingEgg = false;
    }

    // void OnDrawGizmos()
    // {
    //     Gizmos.color = Color.red;
    //     Gizmos.DrawWireSphere(transform.position, circleDistance);
    // }
}
