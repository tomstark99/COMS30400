using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerMovementPhoton : MonoBehaviour
{
    private PhotonView PV;
    void Start()
    {
        PV = GetComponent<PhotonView>();
    }

    void Update()
    {
        if (PV.IsMine)
            Movement();

        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();
    }

    void Movement()
    {
        float yMov = Input.GetAxis("Vertical");
        float xMov = Input.GetAxis("Horizontal");
        transform.position += new Vector3(xMov / 7.5f, yMov / 7.5f, 0);
    }
}
