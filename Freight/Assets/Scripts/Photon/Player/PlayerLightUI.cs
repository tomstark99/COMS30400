using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    This class disables the text that says lights have been either turned on or off after a certain amount of time
*/
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
        // if the text is active, increase time and once it has hit 3 seconds, disable the text
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
