using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerColliderScript : MonoBehaviour
{
    private BoxCollider2D parentCollider;
    public string currentAnimal;
    [SerializeField] private BoxCollider2D childCollider;
    [SerializeField] private GameObject parent;
    // Start is called before the first frame update
    void Start()
    {
        parentCollider = parent.GetComponentInParent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()   
    {
        Vector2 bullChargeSize = new Vector2(parentCollider.size.x+0.1f, parentCollider.size.y+0.1f);
        childCollider.size = bullChargeSize;
        currentAnimal = parent.GetComponent<PlayerScript>().currentAnimal;
    }
}
