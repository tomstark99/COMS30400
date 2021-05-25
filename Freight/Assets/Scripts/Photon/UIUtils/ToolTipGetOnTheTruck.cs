using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//this script is attached to an empty gameobject near the truck. When the player hits the collider next to the truck, a toolTip telling
//the players to jump on the truck will appear. The tooltip follows the players camera direction, so it always faces the player
public class ToolTipGetOnTheTruck : MonoBehaviour
{
    public GameObject toolTipJumpInTheTruck;
    bool tooltip = true;
    // Start is called before the first frame update
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player") 
            {
                if(tooltip == true)
                {
                    Quaternion objRot = transform.rotation;
                    GameObject playerTooltip = Instantiate(toolTipJumpInTheTruck, new Vector3(transform.parent.position.x, transform.parent.position.y + 5, transform.parent.position.z), Quaternion.Euler(objRot.eulerAngles));
                    playerTooltip.GetComponent<Tooltip>().Player = other.gameObject;
                    tooltip = false;
                }
            }
        
    }
   
}
