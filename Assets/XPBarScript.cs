using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XPBarScript : MonoBehaviour
{
    [SerializeField] private float colorSpeed;
    [SerializeField] private SpriteRenderer sprRender;
    [SerializeField] private PlayerScript playerScript;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        //Change x scale based on XP
        Vector2 localScale = transform.localScale;
        localScale.x = Mathf.Min(playerScript.currentXP / playerScript.maxXP, 1);
        transform.localScale = localScale;

        //RGB cycle, brighter if ready to transform
        if (playerScript.currentXP >= playerScript.maxXP) {
            sprRender.color = Color.HSVToRGB(Mathf.PingPong(Time.time * colorSpeed, 1), 0.2f, 1f);
        }
        else {
            sprRender.color = Color.HSVToRGB(Mathf.PingPong(Time.time * colorSpeed, 1), 0.2f, 0.6f);
        }
    }
}
