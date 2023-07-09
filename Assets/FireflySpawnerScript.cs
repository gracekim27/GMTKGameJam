using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireflySpawnerScript : MonoBehaviour
{
    [SerializeField] private float spawnCooldown;
    private float spawnTimer;
    [SerializeField] private GameObject firefly;

    // Start is called before the first frame update
    void Start()
    {
        spawnTimer = 0;
    }

    // Update is called once per frame
    void Update()
    {
        spawnTimer += Time.deltaTime;
        if (spawnTimer >= spawnCooldown + Random.Range(-0.5f, 0.5f)) {
            Instantiate(firefly, spawnXY(), Quaternion.identity);
            spawnTimer = 0;
        }
    }

    public Vector3 spawnXY() { 
        float xSpawn = Random.Range(-3.2f, 3.2f);
        float ySpawn = Random.Range(-1.8f, 1.8f);
        return new Vector3(xSpawn, ySpawn, 0);
    }
}
