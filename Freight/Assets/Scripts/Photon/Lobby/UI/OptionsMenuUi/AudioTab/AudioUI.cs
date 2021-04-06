using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AudioUI : MonoBehaviour
{
   public GameObject text;

   public Slider MasterAudioSlider;

   public GameObject audioUIMenu;
   public GameObject GameplayTab;
   public GameObject ControlsTab;
   public GameObject DisplayTab;

   public void Start() {
       if (PlayerPrefs.HasKey("MasterAudio"))
            MasterAudioSlider.value = PlayerPrefs.GetFloat("MasterAudio");
        else
            MasterAudioSlider.value = 1f;
        Debug.Log("audio Listener volume is " + AudioListener.volume);
        Debug.Log("Master audio slider is " + MasterAudioSlider.value);
        AudioListener.volume = MasterAudioSlider.value;

        Debug.Log("acc tho allow still");
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

    public void AdjustVolume () {
     
     AudioListener.volume = MasterAudioSlider.value;
     Debug.Log(AudioListener.volume);
 }

    public void SaveSettings () {
        Debug.Log("Saved Audio Settings");
        AudioListener.volume = MasterAudioSlider.value;
        PlayerPrefs.SetFloat("MasterAudio", AudioListener.volume);
        PlayerPrefs.Save();
    }

    public void OnMouseClick() {
        audioUIMenu.SetActive(!audioUIMenu.activeSelf);
        GameplayTab.SetActive(false);
        ControlsTab.SetActive(false);
        DisplayTab.SetActive(false);
    }
}
