using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class NewBehaviourScript : MonoBehaviourPun
{
    private Transform player;

    void Start() {
        player = transform.parent;
    }
    // Start is called before the first frame update
    void LateUpdate() {
        Vector3 newPos = player.position;
        newPos.y = transform.position.y;
        transform.position = newPos;
        transform.rotation = Quaternion.Euler(90f, player.eulerAngles.y, 0.0f);
    }
}
