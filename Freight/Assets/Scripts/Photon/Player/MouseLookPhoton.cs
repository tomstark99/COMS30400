using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MouseLookPhoton : MonoBehaviourPun
{
    public float mouseSensitivity;

    [Header("Camera")]
    [SerializeField] private Transform playerBody;
    [SerializeField] private Camera virtualCamera;

    float xRotation = 0f;
    float yRotation = 0f;

    private Transform cameraTransform;
    private Quaternion oldCameraRot;
    private bool freeCam;

    public bool onMenu;
    // Start is called before the first frame update
    void Start()
    {
        if (photonView.IsMine)
        {
            cameraTransform = virtualCamera.GetComponent<Transform>();
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
              if (PlayerPrefs.HasKey("MouseSensibility"))
                mouseSensitivity = PlayerPrefs.GetFloat("MouseSensibility");
              else
            mouseSensitivity = 100f;
        }
       
    }

    // Update is called once per frame
    void Update()
    {
        if (!photonView.IsMine || onMenu) return;

        if (Input.GetKeyDown(KeyCode.LeftAlt))
        {
            oldCameraRot = cameraTransform.localRotation;
            freeCam = true;
        }
        if (Input.GetKeyUp(KeyCode.LeftAlt))
        {
            cameraTransform.localRotation = oldCameraRot;
            yRotation = 0f;
            freeCam = false;
        }

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 70f);

        if(onMenu)
            return;

        if (!freeCam)
        {
            cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
            playerBody.Rotate(Vector3.up * mouseX);
        }
        else
        {
            yRotation += mouseX;
            yRotation = Mathf.Clamp(yRotation, -150f, 130f);
            cameraTransform.localRotation = Quaternion.Euler(xRotation, yRotation, 0f);
        }
            
    }
}
