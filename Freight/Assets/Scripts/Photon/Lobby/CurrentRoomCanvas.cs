using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class CurrentRoomCanvas : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI roomText;

    public string roomName;

    private RoomsCanvases roomsCanvases;

    public void Initialise(RoomsCanvases canvases)
    {
        roomsCanvases = canvases;
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void SetRoomName(string name)
    {
        roomText.text = "Room: " + name;
        roomName = name;
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    public void StartGame()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.AutomaticallySyncScene = true;
            Debug.Log("Starting Game");
            PhotonNetwork.LoadLevel(1);
        }
    }
}
