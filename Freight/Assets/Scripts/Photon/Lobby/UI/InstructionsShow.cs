using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstructionsShow : MonoBehaviour
{
    public GameObject targetPosition;
    public GameObject initialPosition;

    
    public void onClickInstructionsShow() {
        transform.position = targetPosition.transform.position;
    }

    public void onClickInstructionsHide() {
         transform.gameObject.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
    }
}
