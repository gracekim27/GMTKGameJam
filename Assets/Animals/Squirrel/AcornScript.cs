using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AcornScript : MonoBehaviour
{
    [SerializeField] private float moveSpeedPlayer;
    [SerializeField] private float moveSpeedEnemy;
    private float moveSpeed;
    public Vector3 dir;
    public string shotBy;
    /**
    shotBy could be:
        Player
        Enemy

    The value of shotBy changes the behavior of the acorn
    */

    // Start is called before the first frame update
    void Start()
    {
        dir.Normalize();

        if (shotBy == "Player") { 
            moveSpeed = moveSpeedPlayer;
        }
        else {
            moveSpeed = moveSpeedEnemy;
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += dir * moveSpeed * Time.deltaTime;
    }

    //Destroy itself after going off screen
    void OnBecameInvisible() {
        Destroy(gameObject);
    }
}