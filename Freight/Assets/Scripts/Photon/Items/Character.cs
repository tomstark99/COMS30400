using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Cinemachine;
public class Character : MonoBehaviourPunCallbacks, IPunOwnershipCallbacks
{
    public Transform pickUpDestination;
    public Transform pickUpDestinationLocal;
    public Transform dragDestination;
    public Transform grabDestination;
    public PickUpable currentHeldItem;
    public GameObject bulletPrefab;

    public MoveCrosshair crosshair;

    public GameObject camera;

    private bool holdingTheBag;

    private bool bagDroppedOff;

    public GameObject backPackObject;

    private GameObject ObjectToSeeTheLights;

    public GameObject actualCamera;

    public LayerMask gunLayerMask;

    public bool HoldingTheBag
    {
        get { return holdingTheBag; }
    }

    public bool BagDroppedOff
    {
        get { return bagDroppedOff; }
    }

    public bool HasItem()
    {
        return currentHeldItem != null;
    }

    void Start()
    {
        ObjectToSeeTheLights = GameObject.Find("CameraToSeeTheLights");
        if (ObjectToSeeTheLights)
            ObjectToSeeTheLights.SetActive(false);

        Debug.Log(ObjectToSeeTheLights);
    }

    [PunRPC]
    void PickUpRPCLocal(int ItemID)
    {
        PickUpable Item = PhotonView.Find(ItemID).GetComponent<PickUpable>();
        if (!PlayerPrefs.HasKey("TheCompletePicture"))
        {
            if (Item.GetComponent<Unachievable>() == null)
                GetComponent<Achievements>()?.TheCompletePictureCompleted();
        }

        Debug.Log("LOCAL");
        //PhotonView view = Item.GetComponent<PhotonView>();
        //view.TransferOwnership(PhotonNetwork.LocalPlayer);
        // Move to players pickup destination.
        Item.transform.position = pickUpDestinationLocal.position;
        if (Item.GetComponent<Shootable>() != null)
            Item.transform.Find("Canvas").gameObject.SetActive(true);
        // Set the parent of the object to the pickupDestination so that it moves
        // with the player.
        Item.transform.parent = pickUpDestinationLocal;
        Item.transform.Rotate(0, 90, 0);

        Item.ItemPickedUp();
    }

    [PunRPC]
    void PickUpRPC(int ItemID)
    {
        PickUpable Item = PhotonView.Find(ItemID).GetComponent<PickUpable>();
        Debug.Log("drill");
        //PhotonView view = Item.GetComponent<PhotonView>();
        //view.TransferOwnership(PhotonNetwork.LocalPlayer);
        // Move to players pickup destination.
        //Item.transform.position = pickUpDestination.position;

        // Set the parent of the object to the pickupDestination so that it moves
        // with the player.
        if (Item.tag == "Gun")
        {
            GetComponent<IkBehaviour>().ikActive = true;
            GetComponent<IkBehaviour>().handObj = Item.transform.GetChild(18);
        }
        else if (Item.tag == "Rock")
        {
            GetComponent<IkBehaviour>().ikActive = true;
            GetComponent<IkBehaviour>().handObj = Item.transform.GetChild(0).transform.GetChild(2);
        }
        Item.transform.parent = pickUpDestination;
        Item.transform.Rotate(0, 90, 0);

        Item.ItemPickedUp();
    }

    public void OnOwnershipRequest(PhotonView targetView, Photon.Realtime.Player previousOwner)
    {

    }

    public void OnOwnershipTransfered(PhotonView targetView, Photon.Realtime.Player previousOwner)
    {
        Debug.Log("Is this mine? " + targetView.IsMine);
        Debug.Log("Current item = " + currentHeldItem);
        if (!targetView.IsMine)
        {
            return;
        }

        if (currentHeldItem != null)
        {
            if (currentHeldItem.tag == "Gun")
            {
                currentHeldItem.transform.GetChild(17).GetChild(0).gameObject.SetActive(true);
                GetComponent<IkBehaviour>().ikActive = true;
                GetComponent<IkBehaviour>().handObj = currentHeldItem.transform.GetChild(18);
            }
            else if (currentHeldItem.tag == "Rock")
            {
                GetComponent<IkBehaviour>().ikActive = true;
                GetComponent<IkBehaviour>().handObj = currentHeldItem.transform.GetChild(0).transform.GetChild(2);
                actualCamera.transform.GetChild(0).gameObject.SetActive(true);
            }

        }
        else
        {
            return;
        }

        photonView.RPC("PickUpRPC", RpcTarget.Others, currentHeldItem.transform.GetComponent<PhotonView>().ViewID);
        photonView.RPC("PickUpRPCLocal", PhotonNetwork.LocalPlayer, currentHeldItem.transform.GetComponent<PhotonView>().ViewID);
    }

    public void PickUp(PickUpable Item)
    {
        currentHeldItem = Item;

        PhotonView view = Item.GetComponent<PhotonView>();
        if (!view.IsMine)
            view.TransferOwnership(PhotonNetwork.LocalPlayer);
        else
        {
            if (Item.tag == "Gun")
            {
                Item.transform.GetChild(17).GetChild(0).gameObject.SetActive(true);
                GetComponent<IkBehaviour>().ikActive = true;
                GetComponent<IkBehaviour>().handObj = Item.transform.GetChild(18);
            }
            else if (Item.tag == "Rock")
            {
                GetComponent<IkBehaviour>().ikActive = true;
                GetComponent<IkBehaviour>().handObj = Item.transform.GetChild(0).transform.GetChild(2);
                actualCamera.transform.GetChild(0).gameObject.SetActive(true);
            }

            photonView.RPC("PickUpRPC", RpcTarget.Others, Item.transform.GetComponent<PhotonView>().ViewID);
            photonView.RPC("PickUpRPCLocal", PhotonNetwork.LocalPlayer, Item.transform.GetComponent<PhotonView>().ViewID);
        }


    }

    [PunRPC]
    void ThrowRPC(int ItemID)
    {
        Throwable Item = PhotonView.Find(ItemID).GetComponent<Throwable>();
        GetComponent<IkBehaviour>().ikActive = false;
        // Gesture aim
        // Debug.Log("CAMERA POSITION"+ (camera.transform.forward + (camera.transform.rotation * MoveCrosshair.GETCrosshairOffsetFromCentre())));
        Item.GetComponent<Rigidbody>().AddForce((actualCamera.transform.forward + (actualCamera.transform.rotation * crosshair.GETCrosshairOffsetFromCentre()) * 0.002f).normalized * 1000);
        Item.transform.parent = GameObject.Find("/Environment/Interactables/Rocks").transform;
    }

    public void Throw(Throwable Item)
    {
        GetComponent<IkBehaviour>().ikActive = false;
        currentHeldItem = null;
        Item.ItemDropped(this);
        actualCamera.transform.GetChild(0).gameObject.SetActive(false);
        photonView.RPC("ThrowRPC", RpcTarget.All, Item.transform.GetComponent<PhotonView>().ViewID);
    }

    [PunRPC]
    void DropRPC(int ItemID)
    {
        PickUpable Item = PhotonView.Find(ItemID).GetComponent<PickUpable>();
        Item.transform.Rotate(50, 50, 0);
        GetComponent<IkBehaviour>().ikActive = false;
        if (Item.GetComponent<Shootable>() != null)
        {
            Item.transform.GetChild(17).GetChild(0).gameObject.SetActive(false);
            Item.transform.parent = GameObject.Find("/Environment/Interactables/Guns").transform;
            Item.transform.Find("Canvas").gameObject.SetActive(false);
        }
        else
        {
            Item.transform.parent = GameObject.Find("/Environment/Interactables/DeadGuards").transform;
        }

    }

    public void Drop(PickUpable Item)
    {
        currentHeldItem = null;
        Item.ItemDropped(this);
        GetComponent<IkBehaviour>().ikActive = false;
        if (Item.tag == "Gun")
        {
            Item.transform.GetChild(17).GetChild(0).gameObject.SetActive(false);
            GetComponent<IkBehaviour>().ikActive = false;
        }
        else actualCamera.transform.GetChild(0).gameObject.SetActive(false);
        gameObject.transform.GetComponent<PlayerMovementPhoton>().Speed = 4f;
        //Item.transform.parent = GameObject.Find("/Environment/Interactables/Rocks").transform;
        photonView.RPC("DropRPC", RpcTarget.All, Item.transform.GetComponent<PhotonView>().ViewID);
    }

    [PunRPC]
    void KillGuard(int guardId)
    {
        if (!PlayerPrefs.HasKey("Roadman"))
        {
            GetComponent<Achievements>()?.Roadman();
        }

        // get the guard's photon view
        PhotonView killedGuard = PhotonView.Find(guardId)?.GetComponent<PhotonView>();
        Debug.Log(killedGuard);
        if (killedGuard != null)
        {
            GuardAIPhoton killedGuardObject = PhotonView.Find(guardId).GetComponent<GuardAIPhoton>();
            killedGuardObject.CheckMusicOnGuardDeath();
            Vector3 guardPos = killedGuard.transform.position;
            Quaternion guardRot = killedGuard.transform.rotation;
            guardPos.y += 0.5f;

            // remove the guard 
            PhotonNetwork.Destroy(killedGuard);
            // create a dead body that will be draggable (allow new guard model)
            GameObject deadGuard = PhotonNetwork.Instantiate("PhotonPrefabs/dead_guard", guardPos, guardRot);
        }


    }
    IEnumerator GunRecoil()
    {
        /*Debug.Log("omars haaard");
        pickUpDestinationLocal.transform.GetChild(0).Rotate(0.0f, 0.0f, 1.0f, Space.Self);
        Debug.Log("initianl rotation" + pickUpDestinationLocal.transform.GetChild(0).transform.eulerAngles.z);
        while(pickUpDestinationLocal.transform.GetChild(0).transform.eulerAngles.z > 340) 
        {
             Debug.Log("the while loop is haaard" + pickUpDestinationLocal.transform.GetChild(0).transform.localRotation.eulerAngles.z);
            pickUpDestinationLocal.transform.GetChild(0).Rotate(0.0f, 0.0f, 1.0f, Space.Self);
            yield return null;
        }

        while(pickUpDestinationLocal.transform.GetChild(0).transform.eulerAngles.z <359) 
        {
             Debug.Log("the while loop is haaard" + pickUpDestinationLocal.transform.GetChild(0).transform.localRotation.eulerAngles.z);
            pickUpDestinationLocal.transform.GetChild(0).Rotate(0.0f, 0.0f, -1.0f, Space.Self);
            yield return null;
        }
        pickUpDestinationLocal.transform.GetChild(0).Rotate(0.0f, 0.0f, -1.0f, Space.Self);*/
        while (pickUpDestinationLocal.transform.localPosition.z > 0.2f)
        {
            pickUpDestinationLocal.transform.localPosition = new Vector3(pickUpDestinationLocal.transform.localPosition.x, pickUpDestinationLocal.transform.localPosition.y, pickUpDestinationLocal.transform.localPosition.z - 0.01f);
            yield return null;
        }

        while (pickUpDestinationLocal.transform.localPosition.z < 0.26f)
        {
            pickUpDestinationLocal.transform.localPosition = new Vector3(pickUpDestinationLocal.transform.localPosition.x, pickUpDestinationLocal.transform.localPosition.y, pickUpDestinationLocal.transform.localPosition.z + 0.01f);
            yield return null;
        }
        pickUpDestinationLocal.transform.localPosition = new Vector3(pickUpDestinationLocal.transform.localPosition.x, pickUpDestinationLocal.transform.localPosition.y, 0.26f);
        yield break;
    }

    [PunRPC]
    void CreateBulletLocal()
    {
        StartCoroutine("GunRecoil");
        Debug.Log("don t think coroutine startedt stillll");
        // shoots out a raycast to see what the bullet hits

        Physics.Raycast(actualCamera.transform.position, actualCamera.transform.forward, out RaycastHit hitInfo);

        if (!PlayerPrefs.HasKey("LetTheHuntBegin"))
        {
            if (pickUpDestinationLocal.transform.GetChild(0).GetComponent<Unachievable>() == null)
                GetComponent<Achievements>()?.LetTheHuntBeginCompleted();
        }

        // if bullet collides with guard, tell masterclient to kill guard
        Debug.Log(hitInfo.collider);

        if (hitInfo.collider != null)
            if (hitInfo.collider.GetComponent<GuardAIPhoton>() != null)
            {
                Debug.Log("Guard was hit acc");
                photonView.RPC("KillGuard", RpcTarget.MasterClient, hitInfo.collider.GetComponent<PhotonView>().ViewID);
            }
        // instantiate the bullet locally
        GameObject bullet = Instantiate(bulletPrefab, pickUpDestinationLocal.transform.GetChild(0).transform.GetChild(14).position, pickUpDestinationLocal.transform.GetChild(0).rotation);
        pickUpDestinationLocal.transform.GetChild(0).GetComponent<Gun>().GunShot();

        // if it hits something, have the bullet point at that thing and add a force based on bullet forward facing transform
        // this is so the bullet goes towards crosshair
        if (hitInfo.point != new Vector3(0f, 0f, 0f))
        {
            bullet.transform.LookAt(hitInfo.point);

            bullet.GetComponent<Rigidbody>().AddForce(bullet.transform.forward * 1400);
        }
        // in case it doesn't hit anything, just add force based on camera transform
        else
        {
            bullet.GetComponent<Rigidbody>().AddForce(actualCamera.transform.forward * 1400);
        }
        // so bullet moves
        bullet.GetComponent<Rigidbody>().isKinematic = false;
    }

    [PunRPC]
    void CreateBullet()
    {

        // shoots out a raycast to see what the bullet hits
        Physics.Raycast(actualCamera.transform.position, actualCamera.transform.forward, out RaycastHit hitInfo);

        // instantiate the bullet locally
        GameObject bullet = Instantiate(bulletPrefab, pickUpDestination.transform.GetChild(1).transform.GetChild(14).position, pickUpDestination.transform.GetChild(1).rotation);
        //pickUpDestination.transform.GetChild(0).GetComponent<Gun>().GunShot();

        // if it hits something, have the bullet point at that thing and add a force based on bullet forward facing transform
        // this is so the bullet goes towards crosshair
        if (hitInfo.point != new Vector3(0f, 0f, 0f))
        {
            bullet.transform.LookAt(hitInfo.point);

            bullet.GetComponent<Rigidbody>().AddForce(bullet.transform.forward * 1400);
        }
        // in case it doesn't hit anything, just add force based on camera transform
        else
        {
            bullet.GetComponent<Rigidbody>().AddForce(actualCamera.transform.forward * 1400);
        }
        // so bullet moves
        bullet.GetComponent<Rigidbody>().isKinematic = false;
    }

    public void Shoot(Shootable Item)
    {
        if (!gameObject.GetComponent<PlayerMovementPhoton>().onMenu)
        {
            // send an RPC to shoot a bullet on the local client and on all other clients
            // this is done because the local player has to hold the gun in a game object that is the child of the camera
            if (pickUpDestinationLocal.transform.GetChild(0).GetComponent<Gun>().Ammo > 0)
            {
                photonView.RPC("CreateBullet", RpcTarget.Others);
                photonView.RPC("CreateBulletLocal", PhotonNetwork.LocalPlayer);
            }
            else
            {
                pickUpDestinationLocal.transform.GetChild(0).GetComponent<Gun>().EmptyGunShot();
            }
        }

    }

    IEnumerator DoorOpeningSmall(int breakableID)
    {
        PhotonView breakable = PhotonView.Find(breakableID).GetComponent<PhotonView>();
        while (breakable.transform.localPosition.y < 3.1f && breakable.transform.localScale.y > 0.34f)
        {
            breakable.transform.localPosition = new Vector3(breakable.transform.localPosition.x, breakable.transform.localPosition.y + 0.1f, breakable.transform.localPosition.z);
            breakable.transform.localScale = new Vector3(breakable.transform.localScale.x, breakable.transform.localScale.y - 0.18f, breakable.transform.localScale.z);
            yield return null;
        }

        while (breakable.transform.localPosition.y < 3.1f)
        {
            breakable.transform.localPosition = new Vector3(breakable.transform.localPosition.x, breakable.transform.localPosition.y + 0.1f, breakable.transform.localPosition.z);
            yield return null;
        }

        while (breakable.transform.localScale.y > 0.34f)
        {
            breakable.transform.localScale = new Vector3(breakable.transform.localScale.x, breakable.transform.localScale.y - 0.18f, breakable.transform.localScale.z);
            yield return null;
        }

        breakable.GetComponent<BoxCollider>().size = new Vector3(breakable.GetComponent<BoxCollider>().size.x, 30, breakable.GetComponent<BoxCollider>().size.y);
        breakable.transform.GetChild(2).GetComponent<BoxCollider>().size = new Vector3(breakable.GetComponent<BoxCollider>().size.x, 200, breakable.GetComponent<BoxCollider>().size.y);
        yield break;
    }

    IEnumerator DoorOpeningBig(int breakableID)
    {
        PhotonView breakable = PhotonView.Find(breakableID).GetComponent<PhotonView>();
        while (breakable.transform.localPosition.y < 3.16f && breakable.transform.localScale.y > 0.15f)
        {
            breakable.transform.localPosition = new Vector3(breakable.transform.localPosition.x, breakable.transform.localPosition.y + 0.09f, breakable.transform.localPosition.z);
            breakable.transform.localScale = new Vector3(breakable.transform.localScale.x, breakable.transform.localScale.y - 0.05f, breakable.transform.localScale.z);
            yield return null;
        }
        Debug.Log("it got here calm local Scale 2");
        while (breakable.transform.localScale.y > 0.15f)
        {
            breakable.transform.localScale = new Vector3(breakable.transform.localScale.x, breakable.transform.localScale.y - 0.05f, breakable.transform.localScale.z);
            yield return null;
        }

        while (breakable.transform.localPosition.y < 3.16f)
        {
            Debug.Log("it got here calm local Scale 2");
            breakable.transform.localPosition = new Vector3(breakable.transform.localPosition.x, breakable.transform.localPosition.y + 0.09f, breakable.transform.localPosition.z);
            yield return null;
        }

        breakable.GetComponent<BoxCollider>().size = new Vector3(breakable.GetComponent<BoxCollider>().size.x, 50, breakable.GetComponent<BoxCollider>().size.y);
        breakable.transform.GetChild(3).GetComponent<BoxCollider>().size = new Vector3(breakable.GetComponent<BoxCollider>().size.x, 40, breakable.GetComponent<BoxCollider>().size.y);
        yield break;
    }


    [PunRPC]
    void DestroyBreakable(int breakableID)
    {
        PhotonView breakable = PhotonView.Find(breakableID).GetComponent<PhotonView>();

        if (breakable.tag == "BrokenFence")
        {
            Vector3 spawnPosition = breakable.transform.position;
            Quaternion rotation = breakable.transform.gameObject.transform.rotation;
            PhotonNetwork.Destroy(breakable.transform.gameObject);
            PhotonNetwork.Instantiate("PhotonPrefabs/fence_simple_broken_open Variant 1", spawnPosition, rotation);
        }


    }
    public void Break(Breakable Item)
    {
        photonView.RPC(nameof(DestroyBreakable), RpcTarget.MasterClient, Item.transform.GetComponent<PhotonView>().ViewID);
    }

    public void Open(Openable Item)
    {
        photonView.RPC(nameof(OpenRPC), RpcTarget.All, Item.transform.GetComponent<PhotonView>().ViewID);
    }

    [PunRPC]
    public void OpenRPC(int openableID)
    {
        PhotonView openable = PhotonView.Find(openableID).GetComponent<PhotonView>();

        if (openable.tag == "Door")
        {
            StartCoroutine(DoorOpeningSmall(openable.transform.GetComponent<PhotonView>().ViewID));
        }
        else
        if (openable.tag == "DoorBig")
        {
            StartCoroutine(DoorOpeningBig(openable.transform.GetComponent<PhotonView>().ViewID));
        }
    }

    IEnumerator DoorClosingSmall(int breakableID)
    {
        PhotonView breakable = PhotonView.Find(breakableID).GetComponent<PhotonView>();
        while (breakable.transform.localPosition.y > 0.91f && breakable.transform.localScale.y < 5.1f)
        {
            breakable.transform.localPosition = new Vector3(breakable.transform.localPosition.x, breakable.transform.localPosition.y - 0.1f, breakable.transform.localPosition.z);
            breakable.transform.localScale = new Vector3(breakable.transform.localScale.x, breakable.transform.localScale.y + 0.18f, breakable.transform.localScale.z);
            yield return null;
        }
        Debug.Log("Closing door got here position");
        while (breakable.transform.localPosition.y > 0.91f)
        {
            breakable.transform.localPosition = new Vector3(breakable.transform.localPosition.x, breakable.transform.localPosition.y - 0.1f, breakable.transform.localPosition.z);
            yield return null;
        }
        Debug.Log("Scale");
        while (breakable.transform.localScale.y < 5.1f)
        {
            breakable.transform.localScale = new Vector3(breakable.transform.localScale.x, breakable.transform.localScale.y + 0.18f, breakable.transform.localScale.z);
            yield return null;
        }
        breakable.GetComponent<BoxCollider>().size = new Vector3(breakable.GetComponent<BoxCollider>().size.x, 1, breakable.GetComponent<BoxCollider>().size.y);
        breakable.transform.GetChild(2).GetComponent<BoxCollider>().size = new Vector3(breakable.GetComponent<BoxCollider>().size.x, 20, breakable.GetComponent<BoxCollider>().size.y);
        yield break;
    }

    IEnumerator DoorClosingBig(int breakableID)
    {
        PhotonView breakable = PhotonView.Find(breakableID).GetComponent<PhotonView>();
        while (breakable.transform.localPosition.y > 1.09f && breakable.transform.localScale.y < 1.0f)
        {
            breakable.transform.localPosition = new Vector3(breakable.transform.localPosition.x, breakable.transform.localPosition.y - 0.09f, breakable.transform.localPosition.z);
            breakable.transform.localScale = new Vector3(breakable.transform.localScale.x, breakable.transform.localScale.y + 0.05f, breakable.transform.localScale.z);
            yield return null;
        }

        while (breakable.transform.localScale.y < 1.0f)
        {
            breakable.transform.localScale = new Vector3(breakable.transform.localScale.x, breakable.transform.localScale.y + 0.05f, breakable.transform.localScale.z);
            yield return null;
        }

        while (breakable.transform.localPosition.y > 1.09f)
        {
            breakable.transform.localPosition = new Vector3(breakable.transform.localPosition.x, breakable.transform.localPosition.y - 0.09f, breakable.transform.localPosition.z);
            yield return null;
        }

        breakable.GetComponent<BoxCollider>().size = new Vector3(breakable.GetComponent<BoxCollider>().size.x, 6, breakable.GetComponent<BoxCollider>().size.y);
        breakable.transform.GetChild(3).GetComponent<BoxCollider>().size = new Vector3(breakable.GetComponent<BoxCollider>().size.x, 20, breakable.GetComponent<BoxCollider>().size.y);
        yield break;
    }
    public void Close(Openable Item)
    {
        photonView.RPC(nameof(CloseRPC), RpcTarget.All, Item.transform.GetComponent<PhotonView>().ViewID);
    }

    [PunRPC]
    public void CloseRPC(int openableID)
    {
        PhotonView openable = PhotonView.Find(openableID).GetComponent<PhotonView>();
        if (openable.tag == "Door")
        {
            StartCoroutine(DoorClosingSmall(openable.transform.GetComponent<PhotonView>().ViewID));
        }
        else
        if (openable.tag == "DoorBig")
        {
            StartCoroutine(DoorClosingBig(openable.transform.GetComponent<PhotonView>().ViewID));
        }
    }


    [PunRPC]
    void DragRPC(int ItemID)
    {
        Draggable Item = PhotonView.Find(ItemID).GetComponent<Draggable>();
        Item.transform.position = dragDestination.position;
        Item.transform.parent = dragDestination;

        Item.ItemPickedUp();
        //Item.transform.Rotate(90, 0, 0);

    }

    public void Drag(Draggable Item)
    {
        currentHeldItem = Item;

        PhotonView view = Item.GetComponent<PhotonView>();
        view.TransferOwnership(PhotonNetwork.LocalPlayer);
        gameObject.transform.GetComponent<PlayerMovementPhoton>().Speed = 3f;
        photonView.RPC("DragRPC", RpcTarget.All, Item.transform.GetComponent<PhotonView>().ViewID);
    }

    [PunRPC]
    void DestroyBackpack(int backPackId)
    {
        PhotonNetwork.Destroy(PhotonView.Find(backPackId));

    }
    [PunRPC]
    void ActivateBackPack()
    {
        backPackObject.SetActive(true);
        backPackObject.transform.GetChild(0).gameObject.SetActive(false);
    }

    public void Grab(Grabbable Item)
    {
        holdingTheBag = true;

        Debug.Log(backPackObject);
        photonView.RPC(nameof(DestroyBackpack), RpcTarget.MasterClient, Item.transform.GetComponent<PhotonView>().ViewID);
        photonView.RPC(nameof(ActivateBackPack), RpcTarget.All);

    }

    [PunRPC]
    void TurnOffLight(int lightID, int itemID)
    {
        GameObject light = PhotonView.Find(lightID).gameObject;
        GameObject laptop = PhotonView.Find(itemID).gameObject;
        light.GetComponent<RotateLight>().ToggleLights();

        GameObject lightsOff = laptop.transform.GetChild(0).GetChild(1).gameObject;
        GameObject lightsOn = laptop.transform.GetChild(0).GetChild(0).gameObject;
        if (light.GetComponent<RotateLight>().lightsTurnedOff)
        {
            lightsOff.SetActive(true);
            lightsOff.GetComponent<PlayerLightUI>().LightUITimer();
            lightsOn.SetActive(false);
        }
        else
        {
            lightsOn.SetActive(true);
            lightsOn.GetComponent<PlayerLightUI>().LightUITimer();
            lightsOff.SetActive(false);
        }
    }

    IEnumerator LightsCoroutine(Switchable Item)
    {

        actualCamera.transform.GetChild(0).gameObject.SetActive(false);
        pickUpDestinationLocal.gameObject.SetActive(false);

        GameObject[] spinningLights = GameObject.FindGameObjectsWithTag("SpinningLight");
        ObjectToSeeTheLights.SetActive(true);
        yield return new WaitForSeconds(3);
        foreach (var light in spinningLights)
        {
            photonView.RPC(nameof(TurnOffLight), RpcTarget.All, light.transform.GetComponent<PhotonView>().ViewID, Item.transform.GetComponent<PhotonView>().ViewID);
        }
        yield return new WaitForSeconds(1);
        ObjectToSeeTheLights.SetActive(false);
        camera.transform.GetChild(1).gameObject.SetActive(true);
        Debug.Log(camera.transform.GetChild(1).gameObject);
        yield return new WaitForSeconds(2);
        camera.transform.GetChild(1).gameObject.SetActive(false);
        //camera.transform.GetChild(0).GetComponent<CinemachineVirtualCamera>().MoveToTopOfPrioritySubqueue();

        if (pickUpDestinationLocal.childCount > 0)
        {
            if (pickUpDestinationLocal.GetChild(0).GetComponent<Throwable>() != null)
                actualCamera.transform.GetChild(0).gameObject.SetActive(true);
        }
        pickUpDestinationLocal.gameObject.SetActive(true);

        yield break;
    }

    public void SwitchOff(Switchable Item)
    {
        if (!PlayerPrefs.HasKey("Hackerman"))
        {
            GetComponent<Achievements>()?.HackermanCompleted();
        }
        StartCoroutine(LightsCoroutine(Item));
    }

    [PunRPC]
    void ActivateDropOffBackpack(int itemID)
    {
        GameObject drop = PhotonView.Find(itemID).gameObject;
        drop.transform.GetChild(0).gameObject.SetActive(true);
    }

    [PunRPC]
    void DeactivateBackpack()
    {
        backPackObject.SetActive(false);
    }

    public void DropOff(Droppable Item)
    {
        bagDroppedOff = true;

        photonView.RPC(nameof(ActivateDropOffBackpack), RpcTarget.All, Item.transform.GetComponent<PhotonView>().ViewID);
        photonView.RPC(nameof(DeactivateBackpack), RpcTarget.All);
    }
}
