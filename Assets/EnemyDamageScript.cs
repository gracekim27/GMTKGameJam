using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDamageScript : MonoBehaviour
{
    public int currentHP;

    void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.CompareTag("Acorn") && other.GetComponent<AcornScript>().shotBy == "Player") {
            Destroy(other.gameObject);
            currentHP--;
        }
        else if (other.gameObject.CompareTag("SnakeAttack") && other.GetComponent<SnakeAttackCircleScript>().shotBy == "Player") {
            currentHP--;
        }
        else if (other.gameObject.CompareTag("HippoAttack") && other.GetComponent<HippoAttackCircleScript>().shotBy == "Player") {
            currentHP--;
        }
    }
}
