using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HippoScript : MonoBehaviour
{
    [SerializeField] private int maxHP;
    [SerializeField] private float xpDropped;
    private int currentHP;
    [SerializeField] private float attackCooldown;
    private float attackTimer;
    [SerializeField] private float runSpeed;
    private GameObject player;
    private Animator anim;
    private SpriteRenderer sprRender;
    [SerializeField] private GameObject hippoAttackCircle;
    [SerializeField] private float hippoAttackRadius;

    [Header("Healthbar")]
    [SerializeField] private float healthBarSize;
    [SerializeField] private float healthBarYOffset;
    private HealthbarScript healthBar;

    [SerializeField] private AudioSource hippoAudioSource;
    [SerializeField] private AudioClip hippoDeathSound;

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

            //Attack
            if (attackTimer > attackCooldown) {
                anim.SetTrigger("Attack");
                attackTimer = 0;
            }
            attackTimer += Time.deltaTime;

            //Move towards player
            transform.position = Vector3.MoveTowards(transform.position, player.transform.position, runSpeed * Time.deltaTime);

            //Update health bar
            currentHP = gameObject.GetComponent<EnemyDamageScript>().currentHP; //Use EnemyDamageScript to find currentHP
            healthBar.currentHP = currentHP;

            //Die if health low
            if (currentHP <= 0) {
                hippoAudioSource.volume = 0.4f;
                hippoAudioSource.PlayOneShot(hippoDeathSound);

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

    void hippoAttack() {
        GameObject hippoAttack = Instantiate(hippoAttackCircle, transform.position, Quaternion.identity); //Create a circle collider around the attack
        CircleCollider2D hippoAttackCollider = hippoAttack.GetComponent<CircleCollider2D>(); //Set the position and radius of the attack collider to preset values

        hippoAttackCollider.radius = hippoAttackRadius;
        hippoAttack.GetComponent<HippoAttackCircleScript>().shotBy = "Enemy";
    }

    void Die() {
        Destroy(gameObject);
    }
}
