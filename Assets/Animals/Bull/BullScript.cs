using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BullScript : MonoBehaviour
{
    [SerializeField] private int maxHP;
    [SerializeField] private float xpDropped;
    private int currentHP;
    [SerializeField] private float attackCooldown;
    private float attackTimer;
    [SerializeField] private float chargeSpeed;
    [SerializeField] private float chargeDistance;
    private Vector3 chargeTarget;
    private GameObject player;
    private Animator anim;
    private SpriteRenderer sprRender;

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
        attackTimer = attackCooldown - 0.5f; //bulls charge almost immediately upon spawning

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
            if (!anim.GetBool("isCharging")) {
                if (transform.position.x > player.transform.position.x) {
                    sprRender.flipX = true;
                }
                else if (transform.position.x < player.transform.position.x) {
                    sprRender.flipX = false;
                }
            }

            //Charge up attack
            if (attackTimer > attackCooldown) {
                anim.SetTrigger("Charge");
                attackTimer = 0;
            }
            attackTimer += Time.deltaTime;

            //If attack charged, charge towards the player
            if (anim.GetBool("isCharging")) {
                transform.position = Vector3.MoveTowards(transform.position, chargeTarget, chargeSpeed * Time.deltaTime);

                //If arrived at the target position, stop charging
                if (Vector3.Distance(transform.position, chargeTarget) < 0.1f) {
                    anim.SetBool("isCharging", false);
                }

                //If charging too long, stop charging
                if (attackTimer > attackCooldown) {
                    anim.SetBool("isCharging", false);
                }
            }

            //Update health bar
            currentHP = gameObject.GetComponent<EnemyDamageScript>().currentHP; //Use EnemyDamageScript to find currentHP
            healthBar.currentHP = currentHP;

            //Die if health low
            if (currentHP <= 0) {
                healthBar.currentHP = 0;
                anim.SetBool("isCharging", false);
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

    void bullAttack() {
        anim.SetTrigger("Attack");
        Vector3 directionToPlayer = player.transform.position - transform.position;
        chargeTarget = directionToPlayer.normalized * chargeDistance * Random.Range(0.75f,1.25f); //Add some random variation to charge distance
        anim.SetBool("isCharging", true);
    }

    void Die() {
        Destroy(gameObject);
    }
}
