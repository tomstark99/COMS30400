using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{

    public Slider mouseSensibilitySlider;
    public MouseLookPhoton mouseLook;
    public PlayerMovementPhoton playerMovement;

    public GameObject Menu;
    public GameObject BasicMenu;
    public GameObject OptionInGame;

    private bool menuOpened = false;

    private void Start()
    {
       /* if (PlayerPrefs.HasKey("MouseSensibility"))
            mouseSensibilitySlider.value = PlayerPrefs.GetFloat("MouseSensibility");
        else*/
           
            Debug.Log(mouseSensibilitySlider);
            mouseSensibilitySlider.value = 100f;
    }

    public void SetMouseSensibility()
    {
        mouseLook.mouseSensitivity = mouseSensibilitySlider.value;
        PlayerPrefs.SetFloat("MouseSensibility", mouseLook.mouseSensitivity);
        PlayerPrefs.Save();
    }

    private void Update()
    {
        //Debug.Log(mouseSensibilitySlider.value);

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            menuOpened = !menuOpened;   
            ConfigCursor();
            MenuActive(menuOpened);
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

    public void MenuActive(bool value) {
        menuOpened = value;
        mouseLook.onMenu = menuOpened;
        playerMovement.onMenu = menuOpened;
        Menu.SetActive(menuOpened);
        ConfigCursor();
    }


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

    /*public void QuitRoom()
    {
        NetworkManager.instance.LeaveRoom();
        SceneManager.LoadScene("Menu");
    }*/
    
}
