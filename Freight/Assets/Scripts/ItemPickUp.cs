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


    public GameObject equippedWeaponPrefab;
    SphereCollider sphere;
    private GameObject equippedWeaponGo;
    [SyncVar(hook = nameof(onChangeItem))]
    private EquippedItem equippedItem = EquippedItem.nothing;
    void Start()
    {
        rightHand = transform.Find("RightHand").gameObject;
    }
    void Update()
    {
        if (!isLocalPlayer)
            return;

        if (rightHand.transform.childCount == 0)
        {
            return;
        }

       
        if (Input.GetMouseButtonDown(0))
        {
            this.ThrowItem();
        }
    }
    private void onChangeItem(EquippedItem oldEquippedItem, EquippedItem newEquippedItem)
    {
        
         StartCoroutine(ChangeEquipment(newEquippedItem));

    }

    IEnumerator ChangeEquipment(EquippedItem newEquippedItem)
    {
        while (rightHand.transform.childCount > 0)
        {
            Destroy(rightHand.transform.GetChild(0).gameObject);
            yield return null;
        }

        switch (newEquippedItem)
        {
            case EquippedItem.rock:
                Instantiate(equippedWeaponPrefab, rightHand.transform);
                break;
          
        }
    }
    [Command]
    void ThrowItem()
    {
        Debug.Log("hand child " + rightHand.transform.GetChild(0));

        GameObject parent = rightHand.transform.parent.gameObject;
        Debug.Log(parent);

        GameObject cube = parent.transform.Find("Cube").gameObject;
        Debug.Log(cube);

        GameObject camera = cube.transform.Find("Camera").gameObject;
        Debug.Log(camera);

        GameObject rockToBeDestroyed = rightHand.transform.GetChild(0).gameObject;

        Destroy(rockToBeDestroyed.gameObject);
        equippedItem = EquippedItem.nothing;
       // Debug.Log(GameObject.Find("RockWithNetworkIdentity"));
        GameObject rockGo = Instantiate(equippedWeaponPrefab, rightHand.transform.position, rightHand.transform.rotation);
        //Rigidbody gameObjectsRigidBody = rockGo.AddComponent<Rigidbody>(); // Add the rigidbody.
        rockGo.GetComponent<Rigidbody>().isKinematic = false;
        

        rockGo.GetComponent<Rigidbody>().AddForce(camera.transform.forward * 1000 );
        NetworkServer.Spawn(rockGo);
        

    }
    
    [Command]
    public void PickUpItem(EquippedItem equipped)
    {
        Debug.Log("PickUp_weapon = " + equipped);
        equippedItem = equipped;
        Debug.Log("PickUp_weapon = " + equippedItem);

    }

}
