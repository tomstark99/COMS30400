using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

//this script calculates the player's client fps and displays an LOW CLIENT FPS icon if the fps is less then 25
public class FPScounter : MonoBehaviour
{
    // Start is called before the first frame update
   public GameObject lowClientFps;
    public int avgFrameRate;
    public Text display_Text;
    public TextMeshProUGUI fpsCounter;
    
    public void Start() {
        StartCoroutine (onCoroutine()); 
    }
    public void Update ()
    {
        float current = 0;
        current =current = (int)(1f / Time.unscaledDeltaTime);
        avgFrameRate = (int)current;
        
    }

     IEnumerator onCoroutine()
     {
         
         while(true) 
         { 
             if(fpsCounter.gameObject.activeSelf == true)
                fpsCounter.text = avgFrameRate.ToString() + " FPS";
             if(avgFrameRate < 25)
                lowClientFps.SetActive(true);
                else
                lowClientFps.SetActive(false);

             yield return new WaitForSeconds(0.5f);
         }
     }
}

