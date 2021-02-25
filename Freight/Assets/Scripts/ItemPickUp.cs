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
    //the parent of the objects
    public GameObject sceneObjectPrefab;

    //right hand of the player -- item gonna be a child of the hand
    public GameObject rightHand;

    //rock prefab
    public GameObject rockPrefab;

    //sync the item on the server to all clients
    [SyncVar(hook = nameof(onChangeItem))]
    public EquippedItem equippedItem;

 
   
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

    void Update()
    {
        if (!isLocalPlayer)  return;

        //if click throw the item
        if (Input.GetKeyDown(KeyCode.X) && equippedItem != EquippedItem.nothing)
        {
            GameObject parent = rightHand.transform.parent.transform.parent.gameObject;

            GameObject cube = parent.transform.GetChild(2).gameObject;

            GameObject camera = cube.transform.GetChild(0).gameObject;

            ThrowItem(camera.transform.forward);
        }
    }

    
    [Command]
    void ThrowItem(Vector3 cameraVector)
    {

        //create a new rock
        GameObject newSceneObject = Instantiate(sceneObjectPrefab, rightHand.transform.position, rightHand.transform.rotation);

        // is kinematic = false to rock
        newSceneObject.GetComponent<Rigidbody>().isKinematic = false;

        SceneObject sceneObject = newSceneObject.GetComponent<SceneObject>();
        Debug.Log(sceneObject);
        sceneObject.SetEquippedItem(equippedItem);

        sceneObject.equippedItem = equippedItem;

        Debug.Log("when i dropped the object" + equippedItem);
        equippedItem = EquippedItem.nothing;


        //add force to the rock so it shooots
        newSceneObject.GetComponent<Rigidbody>().AddForce(cameraVector * 1000);

        //spawn the rock on the server and on the clients respectively
        NetworkServer.Spawn(newSceneObject);
        //newSceneObject.GetComponent<Rigidbody>().AddForce(camera.transform.forward * 1000);


    }
    
    //command to server to change the item to the equipped parameter
    [Command]
    public void CmdPickupItem(GameObject sceneObject)
    {
        equippedItem = sceneObject.GetComponent<SceneObject>().equippedItem;
        Debug.Log(equippedItem);
        NetworkServer.Destroy(sceneObject);
    }

}
