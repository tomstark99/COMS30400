using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Hacker : MonoBehaviour
{
    private TextMeshProUGUI text1;
    
    private void Start() {
        text1 = transform.parent.parent.Find("AchievementDescription").GetComponent<TextMeshProUGUI>();
        
    }
    // Start is called before the first frame update
    public void onMouseClick() {
        text1.SetText("<size=100%>                               Hackerman:\n <size=70%>                  Hack the laptop to deactivate the spotlights");
    }
}
