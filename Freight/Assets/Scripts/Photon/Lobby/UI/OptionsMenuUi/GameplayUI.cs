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

    public Slider ProximityVoiceChatSlider;
    public GameObject SaveSettings;

    public Slider GraphicsSlider;

    public Slider RenderDistance;
    private void Start()
    {
        Debug.Log("game settings is haarad" + PlayerPrefs.GetFloat("GameGraphics"));
        if (PlayerPrefs.HasKey("MouseSensibility"))
            mouseSensibilitySlider.value = PlayerPrefs.GetFloat("MouseSensibility");
        else
            mouseSensibilitySlider.value = 100f;

        if (PlayerPrefs.HasKey("ProximityVoiceChat"))
            ProximityVoiceChatSlider.value = PlayerPrefs.GetFloat("ProximityVoiceChat");
        else
            ProximityVoiceChatSlider.value = 50f;

        if (PlayerPrefs.HasKey("GameGraphics")) 
            GraphicsSlider.value = PlayerPrefs.GetFloat("GameGraphics");
        else
            GraphicsSlider.value = 0;

        if (PlayerPrefs.HasKey("RenderDistance")) 
            RenderDistance.value = PlayerPrefs.GetFloat("RenderDistance");
        else
            RenderDistance.value = 100;
        
    }
    // Start is called before the first frame update
    public void OnMouseOver() {
        transform.parent.parent.GetComponent<AudioSource>().Play();
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
        PlayerPrefs.SetFloat("ProximityVoiceChat", ProximityVoiceChatSlider.value);
       
    }

    public void OnMouseClick() {
        GameplayTab.SetActive(!GameplayTab.activeSelf);
        ControlsTab.SetActive(false);
        AudioTab.SetActive(false);
        DisplayTab.SetActive(false);
    }
    
    public void ChangeGameGraphics() {
        
        if(GraphicsSlider.value >=0 && GraphicsSlider.value<=6)
            QualitySettings.SetQualityLevel((int)GraphicsSlider.value, true);
        PlayerPrefs.SetFloat("GameGraphics", GraphicsSlider.value);
        PlayerPrefs.Save();
    }

    public void ChangeRenderDistance() {
        PlayerPrefs.SetFloat("RenderDistance", RenderDistance.value);
        PlayerPrefs.Save();
        
    }
  
}
