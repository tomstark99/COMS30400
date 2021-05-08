 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ControlsUI : MonoBehaviour
{
    public GameObject text;

    public GameObject ControlsUiTab;
    public GameObject GameplayTab;
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
        //Debug.Log(TextMeshPros);
        
        TextMeshPros.color = new Color32(151, 158, 169, 255);
        transform.GetComponent<Image>().enabled = false;
    }

    public void OnMouseClick() {
        ControlsUiTab.SetActive(!ControlsUiTab.activeSelf);
        GameplayTab.SetActive(false);
        AudioTab.SetActive(false);
        DisplayTab.SetActive(false);
    }
}
