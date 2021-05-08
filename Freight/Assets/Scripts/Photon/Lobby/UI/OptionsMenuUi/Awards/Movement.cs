using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class Movement : MonoBehaviour
{
    private TextMeshProUGUI text;
    
    private void Start() {
        text = transform.parent.parent.Find("AchievementDescription").GetComponent<TextMeshProUGUI>();
        
    }
    // Start is called before the first frame update
    public void onMouseClick() {
        text.SetText("<size=100%>                              Baby Steps:\n <size=70%>                        Move your character for the first time");
    }
}
