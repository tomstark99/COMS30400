using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
