using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLightUI : MonoBehaviour
{
    private float timeToDisable;
    private bool isOn;
    
    // Start is called before the first frame update
    void Awake()
    {
        isOn = false;
        timeToDisable = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(timeToDisable);
        if (isOn)
        {
            timeToDisable += Time.deltaTime;
            if (timeToDisable > 3f)
            {
                isOn = false;
                gameObject.SetActive(false);
            }
        }
    }

    public void LightUITimer()
    {
        timeToDisable = 0f;
        isOn = true;
    }
}
