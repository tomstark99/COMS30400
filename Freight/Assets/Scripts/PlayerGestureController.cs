using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class PlayerGestureController : NetworkBehaviour
{
    public CharacterController character;
    public Transform characterTransform;
    private bool rotated = false;
    private Transform oldRotation;
    
    [ClientCallback]
    void Start()
    {
        if (!hasAuthority)
        {
            return;
        }
        character = gameObject.GetComponent<CharacterController>();
        characterTransform = gameObject.transform;
    }

    [ClientCallback]
    void Update()
    {
        if (!hasAuthority)
        {
            return;
        }
        
        #if !UNITY_WEBGL || UNITY_EDITOR
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
            characterTransform.Rotate(0, 0, 90, relativeTo:Space.Self);
        }
        else if (Input.GetKeyUp(KeyCode.Q))
        {
            characterTransform.rotation = Quaternion.FromToRotation(characterTransform.up,Vector3.up) * characterTransform.rotation;
        }

        
        if (Input.GetKeyDown(KeyCode.E))
        {
            characterTransform.Rotate(0, 0, -90, relativeTo:Space.Self);
        }
        else if (Input.GetKeyUp(KeyCode.E))
        {
            characterTransform.rotation = Quaternion.FromToRotation(characterTransform.up,Vector3.up) * characterTransform.rotation;
        } 
        #elif UNITY_WEBGL
        if (PoseParser.GETGestureAsString().CompareTo("C") == 0)
        {
            character.height = 1.0f;
        }
        else if (PoseParser.GETGestureAsString().CompareTo("C") != 0)
        {
            character.height = 1.8f;
        }
        
        if (PoseParser.GETGestureAsString().CompareTo("W") == 0 && !rotated)
        {   
            rotated = true;
            characterTransform.Rotate(0, 0, 90, relativeTo:Space.Self);
        } 
        else if (PoseParser.GETGestureAsString().CompareTo("Q") == 0 && !rotated)
        {   
            rotated = true;
            characterTransform.Rotate(0, 0, -90, relativeTo:Space.Self);
        }
        else if (PoseParser.GETGestureAsString().CompareTo("W") != 0 && PoseParser.GETGestureAsString().CompareTo("Q") != 0)
        {
            rotated = false;
            characterTransform.rotation = Quaternion.FromToRotation(characterTransform.up, Vector3.up) *
                                          characterTransform.rotation;
        }
#endif
        
        
        
    }
}
