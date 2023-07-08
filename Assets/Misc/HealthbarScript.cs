using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthbarScript : MonoBehaviour
{
    public float healthBarSize;
    public float maxHP;
    public float currentHP;
    public float yPos;
    private Vector3 localScale;
    // Start is called before the first frame update
    void Start()
    {
        localScale = transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        transform.localPosition = new Vector3(0, yPos, 0);
        localScale.x = currentHP/maxHP * healthBarSize;
        transform.localScale = localScale;
    }
}
