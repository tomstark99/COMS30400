using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Cinemachine;
using UnityEngine.UI;

//MouseLookPhoton is attached to the player. It manages the mouse movement in game
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

    public Slider RenderDistanceSlider;
    public bool onMenu;
   
    void Start()
    {
        if (photonView.IsMine)
        {
            cameraTransform = virtualCamera.GetComponent<Transform>();
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            //if player has mouse sensitivity set as an option, set the mouse sensitivity to that
            if (PlayerPrefs.HasKey("MouseSensibility"))
                mouseSensitivity = PlayerPrefs.GetFloat("MouseSensibility");
            else
                mouseSensitivity = 2f;

            //if player has Render Distance set as an option, set the Render Distance to that
            if(PlayerPrefs.HasKey("RenderDistance")) 
                virtualCamera.GetComponent<CinemachineVirtualCamera>().m_Lens.FarClipPlane = PlayerPrefs.GetFloat("RenderDistance");
            else 
                virtualCamera.GetComponent<CinemachineVirtualCamera>().m_Lens.FarClipPlane = 100;
              
        }
       
    }

    public void ChangeRenderDistance() {
        virtualCamera.GetComponent<CinemachineVirtualCamera>().m_Lens.FarClipPlane = RenderDistanceSlider.value;
        PlayerPrefs.SetFloat("RenderDistance", RenderDistanceSlider.value);
        PlayerPrefs.Save();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!photonView.IsMine) return;

        //if escape is pressed, save the cameras position

        if(Input.GetKeyDown(KeyCode.Escape)) 
            oldCameraRot =  cameraTransform.localRotation;

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 70f);

        
        //if not on the menu, rotate the camera acording to the mouse movement
        if(!onMenu) {
                cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
                playerBody.Rotate(Vector3.up * mouseX);
       
        } else {
            //set the camera position to the position before the player pressed the menu button
            cameraTransform.localRotation = oldCameraRot;
        }
        
            
    }
}
