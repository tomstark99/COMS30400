using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spotlights : MonoBehaviour
{
    
    // Start is called before the first frame update
    void Start()
    {
        if (!GameObject.Find("GameSettings").GetComponent<GameSettings>().SpotlightsActive)
        {
            gameObject.SetActive(false);
        }
    }

}
