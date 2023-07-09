using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireflyScript : MonoBehaviour
{
    [SerializeField] private float lifespan;
    private Vector3 nextDirection;
    [SerializeField] private float wanderSpeed;
    [SerializeField] private float wanderTime;
    [SerializeField] private float wanderRange;
    private float wanderTimer;
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, lifespan);
    }

    // Update is called once per frame
    void Update()
    {
        wanderTimer += Time.deltaTime;
        if (wanderTimer >= wanderTime) {
            wanderTimer = 0;
            nextDirection = RandomWanderTarget();
        }
        transform.position = Vector3.Lerp(transform.position, nextDirection, wanderSpeed * Time.deltaTime);
    }

    void OnBecameInvisible() {
        Destroy(gameObject);
    }

    Vector3 RandomWanderTarget() {
        float x = Random.Range(transform.position.x - wanderRange, transform.position.x + wanderRange);
        float y = Random.Range(transform.position.y - wanderRange, transform.position.y + wanderRange);

        return new Vector3 (x, y, 0);
    }
}
