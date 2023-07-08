using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerScript : MonoBehaviour
{
    [SerializeField] private GameObject squirrel;
    [SerializeField] private GameObject snake;
    [SerializeField] private GameObject hippo;
    [SerializeField] private float spawnCooldown;
    private float spawnTimer;
    public int waveNumber;
    
    // Start is called before the first frame update
    void Start()
    {
        spawnTimer = 0;
        waveNumber = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (spawnTimer < spawnCooldown) {
            spawnTimer += Time.deltaTime;
        }
        else {
            for (int i = 0; i < waveNumber/3; i++) { // Spawn waveNumber/3 number of animals
                Instantiate(spawnRandomAnimal(), spawnXY(), transform.rotation);
            }

            spawnTimer = 0;
            waveNumber++;
        }
    }

    public Vector3 spawnXY() { 
        float xSpawn = 0f;
        float ySpawn = 0f;
        int spawnWall = Random.Range(1,5); // Randomly select a wall to spawn each animal from
        switch (spawnWall) {
            case 1: //top
                xSpawn = Random.Range(-3.2f, 3.2f);
                ySpawn = 1.8f;
                break;
            case 2: //right
                xSpawn = 3.2f;
                ySpawn = Random.Range(-1.8f, 1.8f);
                break;
            case 3: //bottom
                xSpawn = Random.Range(-3.2f, 3.2f);
                ySpawn = -1.8f;
                break;
            case 4: //left
                xSpawn = -3.2f;
                ySpawn = Random.Range(-1.8f, 1.8f);
                break;
        }
        return new Vector3(xSpawn, ySpawn, 0);
    }

    public GameObject spawnRandomAnimal() {
        int randomAnimal = Random.Range(1,2);
        GameObject spawnAnimal;
        switch (randomAnimal) {
            case 1:
                spawnAnimal = squirrel;
                break;
            case 2:
                spawnAnimal = snake;
                break;
            case 3:
                spawnAnimal = hippo;
                break;
            default:
                spawnAnimal = squirrel;
                break;
        }
        return spawnAnimal;
    }
}
