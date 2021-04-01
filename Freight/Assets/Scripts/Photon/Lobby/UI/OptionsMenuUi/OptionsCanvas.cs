using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class OptionsCanvas : MonoBehaviour
{
    [SerializeField]
    private BackButtonMenu backButtonMenu;

    private RoomsCanvases roomsCanvases;

    public void Initialise(RoomsCanvases canvases)
    {
        roomsCanvases = canvases;
        backButtonMenu.Initialise(canvases);
    }

    // shows current room canvas
    public void Show()
    {
        gameObject.SetActive(true);
    }

    // hides current room canvas
    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
