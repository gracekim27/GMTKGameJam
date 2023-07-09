using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeScript : MonoBehaviour
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
    [SerializeField] private GameObject snakeAttackCircle;
    [SerializeField] private Vector2 snakeAttackOffset;
    [SerializeField] private float snakeAttackRadius;

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

        snakeAttack.GetComponent<SnakeAttackCircleScript>().shotBy = "Enemy";
    }

    void Die() {
        Destroy(gameObject);
    }
}
