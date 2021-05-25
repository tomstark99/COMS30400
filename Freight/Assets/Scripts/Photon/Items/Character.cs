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

        //Debug.Log(ObjectToSeeTheLights);
    }

    // RPC sent to the local player telling them to pickup an object
    [PunRPC]
    void PickUpRPCLocal(int itemID)
    {
        // get component by using the item's photon view
        PickUpable item = PhotonView.Find(itemID).GetComponent<PickUpable>();
        
        // checks if the player has achievement
        if (!PlayerPrefs.HasKey("TheCompletePicture"))
        {
            // the Unachievable class was placed on the tutorial gun and rock outside the station so players could not get the achievement for picking up those objects
            if (item.GetComponent<Unachievable>() == null)
                GetComponent<Achievements>()?.TheCompletePictureCompleted();
        }

        // we make the item's position be the same as the pickup destination on the local player
        item.transform.position = pickUpDestinationLocal.position;
        // if the item is a shootable, we set the canvas to active as we want to see the ammo count
        if (item.GetComponent<Shootable>() != null)
            item.transform.Find("Canvas").gameObject.SetActive(true);
        // set item to be a child of the pickup destination
        item.transform.parent = pickUpDestinationLocal;
        item.transform.Rotate(0, 90, 0);

        item.ItemPickedUp();
    }

    [PunRPC]
    void PickUpRPC(int itemID)
    {
        PickUpable item = PhotonView.Find(itemID).GetComponent<PickUpable>();

        // check if the item is a gun or a rock
        // we set ikActive to true to let the player look as if they are holding the item
        // we set the handObj to be an empty game object on the item which indicates where the player should appear to be holding the item
        if (item.tag == "Gun")
        {
            GetComponent<IkBehaviour>().ikActive = true;
            GetComponent<IkBehaviour>().handObj = item.transform.GetChild(18);
        }
        else if (item.tag == "Rock")
        {
            GetComponent<IkBehaviour>().ikActive = true;
            GetComponent<IkBehaviour>().handObj = item.transform.GetChild(0).transform.GetChild(2);
        }
        // set item to be a child of the pickup destination
        item.transform.parent = pickUpDestination;
        item.transform.Rotate(0, 90, 0);

        item.ItemPickedUp();
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

        // this does the same logic as the pickup function however it is only called when the player is picking up an object that the player was not an owner of
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

    public void PickUp(PickUpable item)
    {
        currentHeldItem = item;

        PhotonView view = item.GetComponent<PhotonView>();
        // transfer ownership to the player if they were not already the owner
        if (!view.IsMine)
            view.TransferOwnership(PhotonNetwork.LocalPlayer);
        else
        {
            // check if the item is a gun or a rock
            // we set ikActive to true to let the player look as if they are holding the item
            // we set the handObj to be an empty game object on the item which indicates where the player should appear to be holding the item
            if (item.tag == "Gun")
            {
                item.transform.GetChild(17).GetChild(0).gameObject.SetActive(true);
                GetComponent<IkBehaviour>().ikActive = true;
                GetComponent<IkBehaviour>().handObj = item.transform.GetChild(18);
            }
            else if (item.tag == "Rock")
            {
                GetComponent<IkBehaviour>().ikActive = true;
                GetComponent<IkBehaviour>().handObj = item.transform.GetChild(0).transform.GetChild(2);
                // sets a fake rock to active so the player can see a rock on their screen to let them know they are holding a rock
                actualCamera.transform.GetChild(0).gameObject.SetActive(true);
            }

            photonView.RPC("PickUpRPC", RpcTarget.Others, item.transform.GetComponent<PhotonView>().ViewID);
            photonView.RPC("PickUpRPCLocal", PhotonNetwork.LocalPlayer, item.transform.GetComponent<PhotonView>().ViewID);
        }


    }

    [PunRPC]
    void ThrowRPC(int itemID)
    {
        Throwable item = PhotonView.Find(itemID).GetComponent<Throwable>();
        // sets the Inverse Kinematics to false so the player's hand can be reset to normal
        GetComponent<IkBehaviour>().ikActive = false;

        // Gesture aim
        item.GetComponent<Rigidbody>().AddForce((actualCamera.transform.forward + (actualCamera.transform.rotation * crosshair.GETCrosshairOffsetFromCentre()) * 0.002f).normalized * 1000);
        item.transform.parent = GameObject.Find("/Environment/Interactables/Rocks").transform;
    }

    public void Throw(Throwable item)
    {
        // sets the Inverse Kinematics to false so the player's hand can be reset to normal
        GetComponent<IkBehaviour>().ikActive = false;
        // player no longer holding any item
        currentHeldItem = null;
        // resets the item's conditions as it is no longer picked up
        item.ItemDropped(this);
        // disables fake rock
        actualCamera.transform.GetChild(0).gameObject.SetActive(false);
        photonView.RPC("ThrowRPC", RpcTarget.All, item.transform.GetComponent<PhotonView>().ViewID);
    }

    [PunRPC]
    void DropRPC(int itemID)
    {
        PickUpable item = PhotonView.Find(itemID).GetComponent<PickUpable>();
        item.transform.Rotate(50, 50, 0);
        // sets the Inverse Kinematics to false so the player's hand can be reset to normal
        GetComponent<IkBehaviour>().ikActive = false;
        if (item.GetComponent<Shootable>() != null)
        {
            item.transform.GetChild(17).GetChild(0).gameObject.SetActive(false);
            item.transform.parent = GameObject.Find("/Environment/Interactables/Guns").transform;
            item.transform.Find("Canvas").gameObject.SetActive(false);
        }
        else
        {
            item.transform.parent = GameObject.Find("/Environment/Interactables/DeadGuards").transform;
        }

    }

    public void Drop(PickUpable item)
    {
        currentHeldItem = null;
        // resets the item's conditions as it is no longer picked up
        item.ItemDropped(this);
        // sets the Inverse Kinematics to false so the player's hand can be reset to normal
        GetComponent<IkBehaviour>().ikActive = false;
        // if item is a gun, disable the canvas
        if (item.tag == "Gun")
        {
            item.transform.GetChild(17).GetChild(0).gameObject.SetActive(false);
        }
        // otherwise disable fake rock
        else actualCamera.transform.GetChild(0).gameObject.SetActive(false);

        gameObject.transform.GetComponent<PlayerMovementPhoton>().Speed = 4f;

        photonView.RPC("DropRPC", RpcTarget.All, item.transform.GetComponent<PhotonView>().ViewID);
    }

    // RPC called when you shoot a guard and the raycast collides with a guard
    [PunRPC]
    void KillGuard(int guardId)
    {
        // if achievement not unlocked
        if (!PlayerPrefs.HasKey("Roadman"))
        {
            GetComponent<Achievements>()?.Roadman();
        }

        // get the guard's photon view
        PhotonView killedGuard = PhotonView.Find(guardId)?.GetComponent<PhotonView>();
        
        // this check is added due to the laggy nature of RPCs, if the master client had high ping and the other client shot at the guard several times before they died, multiple dead bodies would
        // appear as the guard dying would have a slight delay
        if (killedGuard != null)
        {
            GuardAIPhoton killedGuardObject = PhotonView.Find(guardId).GetComponent<GuardAIPhoton>();
            // checks if the music should be reset to normal
            killedGuardObject.CheckMusicOnGuardDeath();
            // gets the position of the guard before they died so they can be spawned in the same position
            Vector3 guardPos = killedGuard.transform.position;
            Quaternion guardRot = killedGuard.transform.rotation;
            // spawns dead guard slightly above so they don't clip through the floor
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
        */

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

    // RPC call for the local player to check if they hit a guard when shooting the gun
    [PunRPC]
    void CreateBulletLocal()
    {
        // coroutine for gun recoil
        StartCoroutine(GunRecoil());

        // shoots out a raycast to see what the bullet hits
        Physics.Raycast(actualCamera.transform.position, actualCamera.transform.forward, out RaycastHit hitInfo);

        // checks for achievement
        if (!PlayerPrefs.HasKey("LetTheHuntBegin"))
        {
            // the Unachievable class was placed on the tutorial gun outside the station so players could not get the achievement for shooting this gun
            if (pickUpDestinationLocal.transform.GetChild(0).GetComponent<Unachievable>() == null)
                GetComponent<Achievements>()?.LetTheHuntBeginCompleted();
        }

        // if bullet collides with guard, tell masterclient to kill guard
        if (hitInfo.collider != null)
            if (hitInfo.collider.GetComponent<GuardAIPhoton>() != null)
            {
                Debug.Log("Guard was hit acc");
                // send RPC to master client to destroy guard object as only master client can destroy and spawn objects in the game
                photonView.RPC("KillGuard", RpcTarget.MasterClient, hitInfo.collider.GetComponent<PhotonView>().ViewID);
            }

        // instantiate the bullet locally
        GameObject bullet = Instantiate(bulletPrefab, pickUpDestinationLocal.transform.GetChild(0).transform.GetChild(14).position, pickUpDestinationLocal.transform.GetChild(0).rotation);
        // play the gun shot sound
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

    // RPC call for other player to create bullet
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

    public void Shoot(Shootable item)
    {
        // can only shoot if you are not on the menu
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

    // animation for small doors opening and going up
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

    // animation for big doors opening and going up
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

    // RPC to destroy the fence and place the broken fence prefab in its place
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
    // sends RPC to master client to break fence
    public void Break(Breakable item)
    {
        photonView.RPC(nameof(DestroyBreakable), RpcTarget.MasterClient, item.transform.GetComponent<PhotonView>().ViewID);
    }

    // sends RPC to all clients to open door and do the animation
    public void Open(Openable item)
    {
        photonView.RPC(nameof(OpenRPC), RpcTarget.All, item.transform.GetComponent<PhotonView>().ViewID);
    }

    // RPC call that opens the door 
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

    // animation for doors closing and going down
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

    // animation for big doors closing and going down
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

    // sends RPC to all clients to close door and do the animation
    public void Close(Openable item)
    {
        photonView.RPC(nameof(CloseRPC), RpcTarget.All, item.transform.GetComponent<PhotonView>().ViewID);
    }

    // RPC call that closes the door 
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

    // RPC call that allows player to drag guard
    [PunRPC]
    void DragRPC(int itemID)
    {
        Draggable item = PhotonView.Find(itemID).GetComponent<Draggable>();
        item.transform.position = dragDestination.position;
        item.transform.parent = dragDestination;

        item.ItemPickedUp();
        //Item.transform.Rotate(90, 0, 0);

    }

    public void Drag(Draggable item)
    {
        currentHeldItem = item;

        PhotonView view = item.GetComponent<PhotonView>();
        view.TransferOwnership(PhotonNetwork.LocalPlayer);
        gameObject.transform.GetComponent<PlayerMovementPhoton>().Speed = 3f;
        photonView.RPC("DragRPC", RpcTarget.All, item.transform.GetComponent<PhotonView>().ViewID);
    }

    // RPC call that destroys the backpack available to be picked up
    [PunRPC]
    void DestroyBackpack(int backPackId)
    {
        PhotonNetwork.Destroy(PhotonView.Find(backPackId));

    }
    
    // RPC call that activates the backpack on the player's back
    [PunRPC]
    void ActivateBackPack()
    {
        backPackObject.SetActive(true);
        backPackObject.transform.GetChild(0).gameObject.SetActive(false);
    }

    // sends RPCs to destroy the backpack and place it on the player's back
    public void Grab(Grabbable item)
    {
        holdingTheBag = true;

        Debug.Log(backPackObject);
        photonView.RPC(nameof(DestroyBackpack), RpcTarget.MasterClient, item.transform.GetComponent<PhotonView>().ViewID);
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

    IEnumerator LightsCoroutine(Switchable item)
    {

        actualCamera.transform.GetChild(0).gameObject.SetActive(false);
        pickUpDestinationLocal.gameObject.SetActive(false);

        GameObject[] spinningLights = GameObject.FindGameObjectsWithTag("SpinningLight");
        ObjectToSeeTheLights.SetActive(true);
        yield return new WaitForSeconds(3);
        foreach (var light in spinningLights)
        {
            photonView.RPC(nameof(TurnOffLight), RpcTarget.All, light.transform.GetComponent<PhotonView>().ViewID, item.transform.GetComponent<PhotonView>().ViewID);
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

    // used to turn off lights
    public void SwitchOff(Switchable item)
    {
        if (!PlayerPrefs.HasKey("Hackerman"))
        {
            GetComponent<Achievements>()?.HackermanCompleted();
        }
        StartCoroutine(LightsCoroutine(item));
    }

    [PunRPC]
    void ActivateDropOffBackpack(int itemID)
    {
        GameObject drop = PhotonView.Find(itemID).gameObject;
        drop.transform.GetChild(0).gameObject.SetActive(true);
    }

    // RPC in second level to disable the backpack on player's back
    [PunRPC]
    void DeactivateBackpack()
    {
        backPackObject.SetActive(false);
    }

    // used in second level to drop off the backpack on the dropoff spot
    public void DropOff(Droppable item)
    {
        bagDroppedOff = true;

        photonView.RPC(nameof(ActivateDropOffBackpack), RpcTarget.All, item.transform.GetComponent<PhotonView>().ViewID);
        photonView.RPC(nameof(DeactivateBackpack), RpcTarget.All);
    }
}
