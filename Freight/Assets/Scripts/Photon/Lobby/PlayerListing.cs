using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Realtime;

public class PlayerListing : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI text;

    public Photon.Realtime.Player Player { get; private set; }

    public void SetPlayerInfo(Photon.Realtime.Player tmpPlayer)
    {
        Player = tmpPlayer;
        text.text = Player.NickName;
    }

}
