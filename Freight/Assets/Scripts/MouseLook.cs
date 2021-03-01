using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class MouseLook : NetworkBehaviour
{

    private float mouseSensitivity = 150f;

    [Header("Camera")]
    [SerializeField] private Transform playerBody;
    [SerializeField] private Camera virtualCamera;

    float xRotation = 0f;

    private Transform cameraTransform;

    public override void OnStartAuthority()
    {
        virtualCamera.gameObject.SetActive(true);

        cameraTransform = virtualCamera.GetComponent<Transform>();

        enabled = true;
    }

    public override void OnStartClient()
    {
        if (!hasAuthority) return;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    [Client]
    void Update()
    {
        if (!hasAuthority) return;

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        playerBody.Rotate(Vector3.up * mouseX);
    }
}
