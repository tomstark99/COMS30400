using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class Item : NetworkBehaviour
{
    public GameObject weaponPrefab;
    private GameObject childrenWeapon;

    void Start()
    {
        childrenWeapon = Instantiate(weaponPrefab, transform.position, Quaternion.identity);
        childrenWeapon.transform.parent = transform;
    }
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.tag == "Player" || collision.tag == "Player(Clone)")
 
        {
           
            Debug.Log("Triggered with: " + collision);
            Component[] components = collision.GetComponents(typeof(Component));
            foreach (Component component in components)
            {
                Debug.Log(component.ToString());
            }
            Debug.Log(collision.GetComponent<ItemPickUp>());
           
            ItemPickUp script = collision.GetComponent<ItemPickUp>();
            Debug.Log(script);
            script.PickUpWeapon(EquippedWeapon.ak47);
            Destroy(transform.gameObject);
        }
    }


}