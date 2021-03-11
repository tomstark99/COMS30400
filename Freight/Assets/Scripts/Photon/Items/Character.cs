using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Character : MonoBehaviourPun
{
    public Transform pickUpDestination;
    public Transform pickUpDestinationLocal;
    public PickUpable currentHeldItem;
    public GameObject bulletPrefab;
    
    public bool HasItem()
    {
        return currentHeldItem != null;
    }

    [PunRPC]
    void PickUpRPCLocal(int ItemID)
    {
        PickUpable Item = PhotonView.Find(ItemID).GetComponent<PickUpable>();
        Debug.Log("LOCAL");
        //PhotonView view = Item.GetComponent<PhotonView>();
        //view.TransferOwnership(PhotonNetwork.LocalPlayer);
        // Move to players pickup destination.
        Item.transform.position = pickUpDestinationLocal.position;

        // Set the parent of the object to the pickupDestination so that it moves
        // with the player.
        Item.transform.parent = pickUpDestinationLocal;

        Item.SetItemPickupConditions();
    }

    [PunRPC]
    void PickUpRPC(int ItemID)
    {
        PickUpable Item = PhotonView.Find(ItemID).GetComponent<PickUpable>();
        Debug.Log("drill");
        //PhotonView view = Item.GetComponent<PhotonView>();
        //view.TransferOwnership(PhotonNetwork.LocalPlayer);
        // Move to players pickup destination.
        Item.transform.position = pickUpDestination.position;

        // Set the parent of the object to the pickupDestination so that it moves
        // with the player.
        Item.transform.parent = pickUpDestination;

        Item.SetItemPickupConditions();
    }

    public void PickUp(PickUpable Item) 
    {
        currentHeldItem = Item;

        // An item can only be moved by a player if they are the owner.
        // Therefore, give ownership of the item to the local player before
        // moving it.
        PhotonView view = Item.GetComponent<PhotonView>();
        view.TransferOwnership(PhotonNetwork.LocalPlayer);
        //Item.SetItemPickupConditions();

        // Move to players pickup destination.
       // Item.transform.position = pickUpDestination.position;

        // Set the parent of the object to the pickupDestination so that it moves
        // with the player.
        //Item.transform.parent = pickUpDestination;

        //Item.SetItemPickupConditions();
        photonView.RPC("PickUpRPC", RpcTarget.Others, Item.transform.GetComponent<PhotonView>().ViewID);
        photonView.RPC("PickUpRPCLocal", PhotonNetwork.LocalPlayer, Item.transform.GetComponent<PhotonView>().ViewID);
    }

    [PunRPC]
    void ThrowRPC(int ItemID)
    {
        Throwable Item = PhotonView.Find(ItemID).GetComponent<Throwable>();
        Debug.Log(Item);
        GameObject parent = pickUpDestination.transform.parent.gameObject;

        GameObject cube = parent.transform.GetChild(2).gameObject;

        GameObject camera = cube.transform.GetChild(0).gameObject;

        Item.GetComponent<Rigidbody>().AddForce(camera.transform.forward * 1000);
        Item.transform.parent = GameObject.Find("/Environment/Interactables/Rocks").transform;
    }

    public void Throw(Throwable Item) 
    {
        //GameObject camera = pickUpDestination.transform.parent.gameObject;

        //GameObject parent = pickUpDestination.transform.parent.gameObject;

        //GameObject cube = parent.transform.GetChild(2).gameObject;

        //GameObject camera = cube.transform.GetChild(0).gameObject;

        currentHeldItem = null;
        Item.ResetItemConditions(this);
        //Item.GetComponent<Rigidbody>().AddForce(camera.transform.forward * 1000);
        //Item.transform.parent = GameObject.Find("/Environment/Interactables/Rocks").transform;
        photonView.RPC("ThrowRPC", RpcTarget.All, Item.transform.GetComponent<PhotonView>().ViewID);
    }

    [PunRPC]
    void DropRPC(int ItemID)
    {
        Shootable Item = PhotonView.Find(ItemID).GetComponent<Shootable>();
        Item.transform.Rotate(50, 50, 0);
        Item.transform.parent = GameObject.Find("/Environment/Interactables/Guns").transform;
    }

    public void Drop(PickUpable Item) 
    {
        currentHeldItem = null;
        Item.ResetItemConditions(this);
        //Item.transform.parent = GameObject.Find("/Environment/Interactables/Rocks").transform;
        photonView.RPC("DropRPC", RpcTarget.All, Item.transform.GetComponent<PhotonView>().ViewID);
    }
    [PunRPC]
    void KillGuard(int guardId)
    {
        PhotonView killedGuard = PhotonView.Find(guardId).GetComponent<PhotonView>(); 
        PhotonNetwork.Destroy(killedGuard);
    }
    [PunRPC]
    void CreateBulletLocal()
    {
        //GameObject camera = pickUpDestination.transform.parent.gameObject;

        GameObject parent = pickUpDestination.transform.parent.gameObject;

        GameObject cube = parent.transform.GetChild(2).gameObject;

        GameObject camera = cube.transform.GetChild(0).gameObject;
        Physics.Raycast(camera.transform.position, camera.transform.forward, out RaycastHit hitInfo);
        if(hitInfo.collider != null)
            if(hitInfo.collider.GetComponent<GuardAIPhoton>() != null) {
                Debug.Log("Guard was hit acc");
                photonView.RPC("KillGuard", RpcTarget.MasterClient, hitInfo.collider.GetComponent<PhotonView>().ViewID);
            }
        GameObject bullet = Instantiate(bulletPrefab, pickUpDestinationLocal.transform.GetChild(0).transform.GetChild(1).position, pickUpDestinationLocal.transform.GetChild(0).rotation);

        if (hitInfo.point != new Vector3(0f, 0f, 0f))
        {
            bullet.transform.LookAt(hitInfo.point);

            bullet.GetComponent<Rigidbody>().AddForce(bullet.transform.forward * 5000);
        }
        else
        {
            bullet.GetComponent<Rigidbody>().AddForce(camera.transform.forward * 5000);
        }
        bullet.GetComponent<Rigidbody>().isKinematic = false;
    }

    [PunRPC]
    void CreateBullet()
    {
        //GameObject camera = pickUpDestination.transform.parent.gameObject;

        GameObject parent = pickUpDestination.transform.parent.gameObject;

        GameObject cube = parent.transform.GetChild(2).gameObject;

        GameObject camera = cube.transform.GetChild(0).gameObject;
        Physics.Raycast(camera.transform.position, camera.transform.forward, out RaycastHit hitInfo);
        Debug.Log(hitInfo.point);
        

        Debug.Log(hitInfo.collider);
        GameObject bullet = Instantiate(bulletPrefab, pickUpDestination.transform.GetChild(0).transform.GetChild(1).position, pickUpDestination.transform.GetChild(0).rotation);

        if (hitInfo.point != new Vector3(0f, 0f, 0f))
        {
            bullet.transform.LookAt(hitInfo.point);

            bullet.GetComponent<Rigidbody>().AddForce(bullet.transform.forward * 5000);
        }
        else
        {
            bullet.GetComponent<Rigidbody>().AddForce(camera.transform.forward * 5000);
        }
        bullet.GetComponent<Rigidbody>().isKinematic = false;
    }

    public void Shoot(Shootable Item) 
    {

        //GameObject parent = pickUpDestination.transform.parent.gameObject;

        //GameObject cube = parent.transform.GetChild(2).gameObject;

        //GameObject camera = cube.transform.GetChild(0).gameObject;
        //GameObject bullet = PhotonNetwork.Instantiate("PhotonPrefabs/BulletPrefab", pickUpDestination.position, pickUpDestination.rotation);
        //bullet.transform.position = pickUpDestination.position;
        //bullet.GetComponent<Rigidbody>().AddForce(camera.transform.forward * 1000);

        photonView.RPC("CreateBullet", RpcTarget.Others);
        photonView.RPC("CreateBulletLocal", PhotonNetwork.LocalPlayer);
    }
   
}
