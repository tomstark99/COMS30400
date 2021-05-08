using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class SaveSettings : MonoBehaviour
{
    // Start is called before the first frame update
    public void onMouseOver() {
        gameObject.GetComponent<RectTransform>().localScale = new Vector2(0.9f, 0.9f);
        TextMeshProUGUI TextMeshPros = transform.GetComponent<TextMeshProUGUI>();
        
    }

    public void onMouseExit() {
        gameObject.GetComponent<RectTransform>().localScale = new Vector2(0.8f, 0.8f);
        TextMeshProUGUI TextMeshPros = transform.GetComponent<TextMeshProUGUI>();
       
    }
}
