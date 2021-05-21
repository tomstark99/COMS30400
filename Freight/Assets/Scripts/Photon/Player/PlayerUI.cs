using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Photon.Pun;

//Attached on the Player prefab. Activates and deactivates the menu + sets the mouse sensitvity
public class PlayerUI : MonoBehaviourPun
{

    public Slider mouseSensibilitySlider;
    public MouseLookPhoton mouseLook;
    public PlayerMovementPhoton playerMovement;

    public GameObject Menu;
    public GameObject BasicMenu;
    public GameObject OptionInGame;

    private bool menuOpened = false;

    // Get the mouse sensitivity from cache
    private void Start()
    {
        if (PlayerPrefs.HasKey("MouseSensibility"))
            mouseSensibilitySlider.value = PlayerPrefs.GetFloat("MouseSensibility");
        else
        //if no mouse sensitivy options have been chosen set it to 100
            mouseSensibilitySlider.value = 100f;
    }

    //Set mouse sensibilty in cache 
    public void SetMouseSensibility()
    {
        mouseLook.mouseSensitivity = mouseSensibilitySlider.value;
        PlayerPrefs.SetFloat("MouseSensibility", mouseLook.mouseSensitivity);
        PlayerPrefs.Save();
    }

    //close or open the menu
    public void closeorOpenMenu() 
    {
            menuOpened = !menuOpened; 
  
            //ConfigCursor();
            MenuActive(menuOpened);
    }

    private void Update()
    {
        //if esc is pressed close or open the mneu
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            closeorOpenMenu();
        }
    }

    public void OpenOptionsInGame()
    {
        OptionInGame.SetActive(true);
        BasicMenu.SetActive(false);
    }

    public void CloseOptionsInGame()
    {
        BasicMenu.SetActive(true);
        OptionInGame.SetActive(false);
    }

    public void ApplyChanges()
    {
        SetMouseSensibility();
        CloseOptionsInGame();
    }

    //if menu activate is called, assign the value to the script that need it and cofig cursor
    public void MenuActive(bool value) {
        menuOpened = value;
        mouseLook.onMenu = menuOpened;
        playerMovement.onMenu = menuOpened;
        OptionInGame.SetActive(menuOpened);
        ConfigCursor();
    }


    //activate and deactivate the cursor
    private void ConfigCursor()
    {
        if (menuOpened)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    
}
