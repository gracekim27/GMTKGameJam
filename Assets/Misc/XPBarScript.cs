using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class XPBarScript : MonoBehaviour
{
    [SerializeField] private float colorSpeed;
    [SerializeField] private SpriteRenderer sprRender;
    [SerializeField] private PlayerScript playerScript;
    [SerializeField] private TextMeshProUGUI displayText;
    [SerializeField] private float textBobPeriod;
    [SerializeField] private float textBobAmplitude;
    private Vector3 startPos;

    // Start is called before the first frame update
    void Start()
    {
        startPos = new Vector3(-0.1f,1.5f,0);
    }

    // Update is called once per frame
    void Update()
    {
        //Change x scale based on XP
        Vector2 localScale = transform.localScale;
        localScale.x = Mathf.Min(playerScript.currentXP / playerScript.maxXP, 1);
        transform.localScale = localScale;

        textBob();

        //RGB cycle, brighter if ready to transform
        if (playerScript.currentXP >= playerScript.maxXP) {
            displayText.enabled = true;
            sprRender.color = Color.HSVToRGB(Mathf.PingPong(Time.time * colorSpeed, 1), 0.2f, 1f);
        }
        else {
            displayText.enabled = false;
            sprRender.color = Color.HSVToRGB(Mathf.PingPong(Time.time * colorSpeed, 1), 0.2f, 0.6f);
        }
    }

    void textBob() {
        float theta = Time.timeSinceLevelLoad / textBobPeriod;
        float distance = textBobAmplitude * Mathf.Sin(theta);
        displayText.transform.position = startPos + Vector3.up * distance;
    }
}
