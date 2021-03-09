using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Realtime;

public class RoomListing : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI text;

    public void SetRoomInfo(RoomInfo roomInfo)
    {
        text.text = roomInfo.MaxPlayers + ", " + roomInfo.Name;
    }

}
