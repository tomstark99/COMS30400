using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class GameplayUI : MonoBehaviour
{
    public GameObject text;

    public GameObject GameplayTab;
    public GameObject ControlsTab;
    public GameObject AudioTab;
    public GameObject DisplayTab;
    public Slider mouseSensibilitySlider;

    private void Start()
    {
        if (PlayerPrefs.HasKey("MouseSensibility"))
            mouseSensibilitySlider.value = PlayerPrefs.GetFloat("MouseSensibility");
        else
            mouseSensibilitySlider.value = 100f;
    }
    // Start is called before the first frame update
    public void OnMouseOver() {
        TextMeshProUGUI TextMeshPros = text.GetComponent<TextMeshProUGUI>();
        //If your mouse hovers over the GameObject with the script attached, output this message
        Debug.Log("Mouse is over GameObject.");
        transform.GetComponent<Image>().enabled = true;
        Debug.Log(TextMeshPros);
        TextMeshPros.color  = new Color32(0, 0, 0, 255);
    }

    // Update is called once per frame
    public void OnMouseExit() {
        //The mouse is no longer hovering over the GameObject so output this message each frame
        TextMeshProUGUI TextMeshPros = text.GetComponent<TextMeshProUGUI>();
        Debug.Log(TextMeshPros);
        

        
        
        TextMeshPros.color = new Color32(151, 158, 169, 255);
        transform.GetComponent<Image>().enabled = false;
    }

    public void SetMouseSensibility()
    {
        //needs changing
        Debug.Log(mouseSensibilitySlider.value);
        PlayerPrefs.SetFloat("MouseSensibility", mouseSensibilitySlider.value);
        PlayerPrefs.Save();
    }

    public void OnMouseClick() {
        GameplayTab.SetActive(!GameplayTab.activeSelf);
        ControlsTab.SetActive(false);
        AudioTab.SetActive(false);
        DisplayTab.SetActive(false);
    }
}
