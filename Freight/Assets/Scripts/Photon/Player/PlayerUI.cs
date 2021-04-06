using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Photon.Pun;

public class PlayerUI : MonoBehaviourPun
{

    public Slider mouseSensibilitySlider;
    public MouseLookPhoton mouseLook;
    public PlayerMovementPhoton playerMovement;

    public GameObject Menu;
    public GameObject BasicMenu;
    public GameObject OptionInGame;

    private bool menuOpened = false;

    // https://titanwolf.org/Network/Articles/Article?AID=5698ab7c-fa2c-4dd3-997e-0512d22a64ba#gsc.tab=0
    private void Start()
    {
        if (PlayerPrefs.HasKey("MouseSensibility"))
            mouseSensibilitySlider.value = PlayerPrefs.GetFloat("MouseSensibility");
        else
            mouseSensibilitySlider.value = 100f;
    }

    public void SetMouseSensibility()
    {
        mouseLook.mouseSensitivity = mouseSensibilitySlider.value;
        PlayerPrefs.SetFloat("MouseSensibility", mouseLook.mouseSensitivity);
        PlayerPrefs.Save();
    }

    public void closeorOpenMenu() {
            menuOpened = !menuOpened; 
            Debug.Log("menu is openend:" + menuOpened);
  
            //ConfigCursor();
            MenuActive(menuOpened);
    }

    private void Update()
    {
        //Debug.Log(mouseSensibilitySlider.value);
        //if (!gameObject.transform.parent.GetComponent<PhotonView>().IsMine)
        //{
        //    return;
        //}
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("esc");
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

    public void MenuActive(bool value) {
        menuOpened = value;
        mouseLook.onMenu = menuOpened;
        playerMovement.onMenu = menuOpened;
        OptionInGame.SetActive(menuOpened);
        ConfigCursor();
    }


    private void ConfigCursor()
    {
        if (menuOpened)
        {
            Debug.Log("menu is opened here");

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Debug.Log("allow");
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
