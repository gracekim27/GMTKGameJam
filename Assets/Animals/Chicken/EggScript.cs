using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EggScript : MonoBehaviour
{
    [SerializeField] private float moveSpeedPlayer;
    [SerializeField] private float moveSpeedEnemy;
    [SerializeField] private float explodeTime;
    [SerializeField] private GameObject explodeCircle;
    private float moveSpeed;
    public Vector3 dir;
    public string shotBy;
    private float timer;

    /**
    shotBy could be:
        Player
        Enemy

    The value of shotBy changes the behavior of the acorn
    */

    // Start is called before the first frame update
    void Start()
    {
        if (shotBy == "Player") { 
            moveSpeed = moveSpeedPlayer;
        }
        else {
            moveSpeed = moveSpeedEnemy;
        }
        timer = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += dir * moveSpeed * Time.deltaTime;

        //Explode after explodeTime seconds
        timer += Time.deltaTime;
        if (timer >= explodeTime) {
            GameObject explosion = Instantiate(explodeCircle, transform.position, Quaternion.identity);
            explosion.GetComponent<EggExplodeCircleScript>().shotBy = shotBy;
            Destroy(gameObject);
        }

        //Rotate
        transform.Rotate (0,0,50*Time.deltaTime);
    }

    //Destroy itself after going off screen
    void OnBecameInvisible() {
        Destroy(gameObject);
    }

    void OnCollisionEnter2D(Collision2D other) {
        if (other.gameObject.CompareTag("Hitbox")) {
            Destroy(gameObject);
        }
    }
}
