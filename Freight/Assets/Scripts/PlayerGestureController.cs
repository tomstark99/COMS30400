using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGestureController : MonoBehaviour
{
    public CharacterController character;
    public Transform characterTransform;
    
    void Start()
    {
        character = gameObject.GetComponent<CharacterController>();
        characterTransform = gameObject.transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            character.height = 1.0f;
        }
        else if (Input.GetKeyUp(KeyCode.C))
        {
            character.height = 1.8f;
        }
        
        if (Input.GetKeyDown(KeyCode.Q))
        {
            characterTransform.rotation =  Quaternion.Euler(0, 0, 90);
        }
        else if (Input.GetKeyUp(KeyCode.Q))
        {
            characterTransform.rotation =  Quaternion.Euler(0, 0, 0);
        }
        
        if (Input.GetKeyDown(KeyCode.E))
        {
            characterTransform.rotation =  Quaternion.Euler(0, 0, -90);
        }
        else if (Input.GetKeyUp(KeyCode.E))
        {
            characterTransform.rotation =  Quaternion.Euler(0, 0, 0);
        }
        
    }
}
