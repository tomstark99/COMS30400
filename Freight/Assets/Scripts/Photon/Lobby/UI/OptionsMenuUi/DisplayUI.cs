using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class DisplayUI : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject text;

    public GameObject GameplayTab;
    public GameObject ControlsTab;
    public GameObject AudioTab;
    public GameObject DisplayTab;
    // Start is called before the first frame update
    public void OnMouseOver() {
        transform.parent.parent.GetComponent<AudioSource>().Play();
        TextMeshProUGUI TextMeshPros = text.GetComponent<TextMeshProUGUI>();
        //If your mouse hovers over the GameObject with the script attached, output this message
        //Debug.Log("Mouse is over GameObject.");
        transform.GetComponent<Image>().enabled = true;
       // Debug.Log(TextMeshPros);
        TextMeshPros.color  = new Color32(0, 0, 0, 255);
    }

    // Update is called once per frame
    public void OnMouseExit() {
        //The mouse is no longer hovering over the GameObject so output this message each frame
        TextMeshProUGUI TextMeshPros = text.GetComponent<TextMeshProUGUI>();
        

        
        
        TextMeshPros.color = new Color32(151, 158, 169, 255);
        transform.GetComponent<Image>().enabled = false;
    }

    public void OnMouseClick() 
    {
        DisplayTab.SetActive(!DisplayTab.activeSelf);
        GameplayTab.SetActive(false);
        ControlsTab.SetActive(false);
        AudioTab.SetActive(false);
    }
}
