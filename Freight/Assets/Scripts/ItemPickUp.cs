using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public enum EquippedItem : int
{
    nothing,
    rock
}
public class ItemPickUp : NetworkBehaviour
{
    //right hand of the player -- item gonna be a child of the hand
    private GameObject rightHand;

    //rock prefab
    public GameObject rockPrefab;

    //sync the item on the server to all clients
    [SyncVar(hook = nameof(onChangeItem))]
    private EquippedItem equippedItem = EquippedItem.nothing;

    //find the right hand object
    void Start()
    {
        rightHand = transform.Find("RightHand").gameObject;
    }

    
    void Update()
    {
        if (!isLocalPlayer)
            return;

        //if the hand doesnt have anything attached to it return
        if (rightHand.transform.childCount == 0)
        {
            return;
        }

       //if click throw the item
        if (Input.GetMouseButtonDown(0))
        {
            this.ThrowItem();
        }
    }


    private void onChangeItem(EquippedItem oldEquippedItem, EquippedItem newEquippedItem)
    {
        
         StartCoroutine(ChangeEquipment(newEquippedItem));

    }

    //change item
    IEnumerator ChangeEquipment(EquippedItem newEquippedItem)
    {

        //destroy anything the hand is holding
        while (rightHand.transform.childCount > 0)
        {
            Destroy(rightHand.transform.GetChild(0).gameObject);
            yield return null;
        }

        //attach item to hand
        switch (newEquippedItem)
        {
            case EquippedItem.rock:
                Instantiate(rockPrefab, rightHand.transform);
                break;
          
        }
    }


    [Command]
    void ThrowItem()
    {
        //get the camera game object
        GameObject parent = rightHand.transform.parent.gameObject;

        GameObject cube = parent.transform.Find("Cube").gameObject;

        GameObject camera = cube.transform.Find("Camera").gameObject;

        //get the rock object
        GameObject rockToBeDestroyed = rightHand.transform.GetChild(0).gameObject;

        //destroy the rock
        Destroy(rockToBeDestroyed.gameObject);

        //deequip rock
        equippedItem = EquippedItem.nothing;

        //create a new rock
        GameObject rockGo = Instantiate(rockPrefab, rightHand.transform.position, rightHand.transform.rotation);

        // is kinematic = false to rock
        rockGo.GetComponent<Rigidbody>().isKinematic = false;
        
        //add force to the rock so it shooots
        rockGo.GetComponent<Rigidbody>().AddForce(camera.transform.forward * 1000 );

        //spawn the rock on the server and on the clients respectively
        NetworkServer.Spawn(rockGo);
        

    }
    
    //command to server to change the item to the equipped parameter
    [Command]
    public void PickUpItem(EquippedItem equipped)
    {
        equippedItem = equipped;
    }

}
