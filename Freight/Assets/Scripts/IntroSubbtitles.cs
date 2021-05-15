using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class IntroSubbtitles : MonoBehaviour
{
    public TextMeshProUGUI subb;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Subbtitles());
    }

    IEnumerator Subbtitles() 
    {
        subb.text = "The year is 2057. You are part of an anti-government group who steal and sell private intelligence";
        yield return new WaitForSeconds(5.1f);
        subb.text = "Your mission is to infiltrate a military train station and steal backpacks containing government secrets.";
        yield return new WaitForSeconds(4.8f);
        subb.text = "The backpacks are placed in some of the buildings, we’re not sure where yet.";
        yield return new WaitForSeconds(3.3f);
        subb.text = "That’s for you to find out.";
        yield return new WaitForSeconds(1.85f);
        subb.text = "You can find rocks around the station, throw them near the guards to distract them from their path.";
        yield return new WaitForSeconds(4.4f);
        subb.text = " If you don’t mind getting your hands dirty, you can find a gun, execute the guards and hide their bodies so you don’t get caught.";
        yield return new WaitForSeconds(6.4f);
        subb.text = "Watch out for the spotlights though, if you step in them you’ll set off the alarm and the guards will know your location.";
        yield return new WaitForSeconds(5.45f);
        subb.text = "There should be a laptop somewhere that turns the spotlights off, should make your job a whole lot easier.";
        yield return new WaitForSeconds(4.8f);
        subb.text = "There will be a train leaving the station very soon, we will update you with its time to leave. Good luck out there…";
        yield return new WaitForSeconds(5.5f);
        subb.text = "";
        yield break;
    }
    // Update is called once per frame
}
