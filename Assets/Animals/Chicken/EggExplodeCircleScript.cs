using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EggExplodeCircleScript : MonoBehaviour
{
    [SerializeField] private float lifetime;
    public string shotBy;
    /**
    shotBy can take the following values:
        Player
        Enemy
     */

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
